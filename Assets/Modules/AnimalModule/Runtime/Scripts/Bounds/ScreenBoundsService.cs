using System;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Bounds
{
    /// <summary>
    /// Screen bounds through the camera viewport, so they stay correct for a tilted perspective camera
    /// and for any window size.
    /// </summary>
    public class ScreenBoundsService : IScreenBounds
    {
        /// <summary>
        /// Cached: the center ray depends only on the camera pose, and every animal outside the screen asks for it each tick.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                var cameraTransform = _camera.transform;

                if (!_hasCachedCenter ||
                    cameraTransform.position != _cachedCameraPosition ||
                    cameraTransform.rotation != _cachedCameraRotation)
                {
                    _cachedCenter = GetGroundPoint(ViewportCenter);
                    _cachedCameraPosition = cameraTransform.position;
                    _cachedCameraRotation = cameraTransform.rotation;
                    _hasCachedCenter = true;
                }

                return _cachedCenter;
            }
        }

        private static readonly Plane GroundPlane = new Plane(Vector3.up, Vector3.zero);
        private static readonly Vector2 ViewportCenter = new Vector2(0.5f, 0.5f);

        private readonly Camera _camera;

        private bool _hasCachedCenter;
        private Vector3 _cachedCenter;
        private Vector3 _cachedCameraPosition;
        private Quaternion _cachedCameraRotation;

        public ScreenBoundsService(Camera camera)
        {
            _camera = camera;
        }

        public bool Contains(Vector3 position, float viewportMargin)
        {
            var viewportPoint = _camera.WorldToViewportPoint(position);
            var min = viewportMargin;
            var max = 1 - viewportMargin;

            return viewportPoint.z > 0 &&
                   viewportPoint.x >= min && viewportPoint.x <= max &&
                   viewportPoint.y >= min && viewportPoint.y <= max;
        }

        public Vector3 GetGroundPoint(Vector2 viewportPoint)
        {
            var ray = _camera.ViewportPointToRay(viewportPoint);

            if (!GroundPlane.Raycast(ray, out var distance))
                throw new InvalidOperationException($"Camera doesn't see the ground at viewport point {viewportPoint}");

            return ray.GetPoint(distance);
        }
    }
}
