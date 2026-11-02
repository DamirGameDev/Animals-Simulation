using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.SharedModule.Runtime.Scripts.Tools
{
    public static class AsyncTools
    {
        public static async UniTask WaitWhileWithoutSkippingFrame(Func<bool> predicate,
            CancellationToken cancellationToken = default)
        {
            if (!predicate.Invoke())
                return;

            await UniTask.WaitWhile(predicate, cancellationToken: cancellationToken);
        }
    }
}
