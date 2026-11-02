using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Modules.HudModule.Runtime.Scripts.TastyLabel
{
    /// <summary>
    /// Shows "Tasty!" under an animal that has just eaten, following it for a while.
    /// </summary>
    public class TastyLabelsController : IDisposable
    {
        private readonly Camera _camera;
        private readonly Canvas _canvas;
        private readonly HudConfig _config;
        private readonly ObjectPool<TastyLabelSerializableComponents> _labelsPool;
        private readonly CancellationTokenSource _disposeTokenSource = new CancellationTokenSource();

        public TastyLabelsController(Camera camera, Canvas canvas, HudConfig config,
            TastyLabelSerializableComponents labelPrefab, RectTransform labelsRoot)
        {
            _camera = camera;
            _canvas = canvas;
            _config = config;

            _labelsPool = new ObjectPool<TastyLabelSerializableComponents>(
                createFunc: () => Object.Instantiate(labelPrefab, labelsRoot),
                actionOnGet: label => label.gameObject.SetActive(true),
                actionOnRelease: label => label.gameObject.SetActive(false),
                actionOnDestroy: label => Object.Destroy(label.gameObject));
        }

        public void Dispose()
        {
            _disposeTokenSource.Cancel();
            _disposeTokenSource.Dispose();
            _labelsPool.Dispose();
        }

        public void Show(AnimalController eater)
        {
            ShowAsync(eater, _disposeTokenSource.Token).Forget();
        }

        private async UniTaskVoid ShowAsync(AnimalController eater, CancellationToken cancellationToken)
        {
            var label = _labelsPool.Get();
            var eaterTransform = eater.SerializableComponents.transform;
            var elapsedTime = 0f;

            while (elapsedTime < _config.TastyLabelDuration && !eater.Model.HealthModel.IsDead)
            {
                label.RectTransform.position = (Vector2)_camera.WorldToScreenPoint(eaterTransform.position) +
                                               _config.TastyLabelScreenOffset * _canvas.scaleFactor;

                if (await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken).SuppressCancellationThrow())
                    return;

                elapsedTime += Time.deltaTime;
            }

            _labelsPool.Release(label);
        }
    }
}
