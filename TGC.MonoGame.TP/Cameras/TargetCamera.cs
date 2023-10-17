using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Platform;

namespace TGC.MonoGame.TP.Cameras
{
    /// <summary>
    ///     Camera looking at a particular point, assumes the up vector is in y.
    /// </summary>
    public class TargetCamera : Camera
    {
        private int _previousScrollValue;
        private bool _mouseWheelChanged;
        private float _cameraFollowRadius = InitialCameraFollowRadius;
        private const float MaxCameraFollowRadius = 100f;
        private const float MinCameraFollowRadius = 30f;
        private const float InitialCameraFollowRadius = 60f;
        private const float RadiusIncrement = 10f;
        private const float CameraUpDistance = 15f;

        
        /// <summary>
        ///     The direction that is "up" from the camera's point of view.
        /// </summary>
        private readonly Vector3 _defaultWorldUpVector = Vector3.Up;

        /// <summary>
        ///     Camera looking at a particular direction, which has the up vector (0,1,0).
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="position">The position of the camera.</param>
        /// <param name="targetPosition">The target towards which the camera is pointing.</param>
        public TargetCamera(float aspectRatio, Vector3 position, Vector3 targetPosition) : base(aspectRatio)
        {
            BuildView(position, targetPosition);
        }

        /// <summary>
        ///     Camera looking at a particular direction, which has the up vector (0,1,0).
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="position">The position of the camera.</param>
        /// <param name="targetPosition">The target towards which the camera is pointing.</param>
        /// <param name="nearPlaneDistance">Distance to the near view plane.</param>
        /// <param name="farPlaneDistance">Distance to the far view plane.</param>
        public TargetCamera(float aspectRatio, Vector3 position, Vector3 targetPosition, float nearPlaneDistance,
            float farPlaneDistance) : base(aspectRatio, nearPlaneDistance, farPlaneDistance)
        {
            BuildView(position, targetPosition);
        }

        /// <summary>
        ///     The target towards which the camera is pointing.
        /// </summary>
        private Vector3 TargetPosition { get; set; }

        /// <summary>
        ///     Build view matrix and update the internal directions.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="targetPosition">The target towards which the camera is pointing.</param>
        private void BuildView(Vector3 position, Vector3 targetPosition)
        {
            Position = position;
            TargetPosition = targetPosition;
            BuildView();
        }

        /// <summary>
        ///     Build view matrix and update the internal directions.
        /// </summary>
        private void BuildView()
        {
            FrontDirection = Vector3.Normalize(TargetPosition - Position);
            RightDirection = Vector3.Normalize(Vector3.Cross(_defaultWorldUpVector, FrontDirection));
            UpDirection = Vector3.Cross(FrontDirection, RightDirection);
            View = Matrix.CreateLookAt(Position, Position + FrontDirection, UpDirection);
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            // This camera has no movement, once initialized with position and lookAt it is no longer updated automatically.
        }
        
        public void Update(Vector3 playerPosition, float yaw, MouseState mouseState)
        {
            UpdateFollowRadius(mouseState);
            var playerBackDirection = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(yaw));
            var orbitalPosition = playerBackDirection * _cameraFollowRadius;
            var upDistance = Vector3.Up * CameraUpDistance;
            var newCameraPosition = playerPosition + orbitalPosition + upDistance;
            var collisionDistance = CameraCollided(newCameraPosition, playerPosition);

            if (collisionDistance.HasValue)
            {
                var clampedDistance =
                    MathHelper.Clamp(_cameraFollowRadius - collisionDistance.Value, 0.1f, _cameraFollowRadius);
                
                var recalculatedPosition = playerBackDirection * clampedDistance;
                
                Position = playerPosition + recalculatedPosition + upDistance;
            }
            else
            {
                Position = newCameraPosition;
            }

            TargetPosition = playerPosition;
            BuildView();
        }
        
        private static float? CameraCollided(Vector3 cameraPosition, Vector3 playerPosition)
        {
            var difference = playerPosition - cameraPosition;
            var distanceToPlayer = Vector3.Distance(playerPosition, cameraPosition);
            var normalizedDifference = difference / distanceToPlayer;
            var cameraToPlayerRay = new Ray(cameraPosition, normalizedDifference);
            
            foreach (var collider in Prefab.PlatformAabb)
            {
                var distance = cameraToPlayerRay.Intersects(collider);
                if (distance < distanceToPlayer)
                    return distance;
            }
            return null;
        }

        private void UpdateFollowRadius(MouseState mouseState)
        {
            ZoomIn(mouseState);
            ZoomOut(mouseState);
            AdjustCameraFollowRadius();
            HandleMouseWheelChange(mouseState);
        }
        
        private void ZoomIn(MouseState mouseState)
        {
            if (mouseState.ScrollWheelValue >= _previousScrollValue) return;
            UpdateCameraFollowRadius(_cameraFollowRadius + RadiusIncrement);
        }
        
        private void ZoomOut(MouseState mouseState)
        {
            if (mouseState.ScrollWheelValue <= _previousScrollValue) return;
            UpdateCameraFollowRadius(_cameraFollowRadius - RadiusIncrement);
        }
        
        private void UpdateCameraFollowRadius(float newRadius)
        {
            _cameraFollowRadius = newRadius;
            _mouseWheelChanged = true;
        }

        private void HandleMouseWheelChange(MouseState mouseState)
        {
            if (!_mouseWheelChanged) return;
            _previousScrollValue = mouseState.ScrollWheelValue;
            _mouseWheelChanged = false;
        }

        private void AdjustCameraFollowRadius()
        {
            _cameraFollowRadius = MathHelper.Clamp(_cameraFollowRadius, MinCameraFollowRadius, MaxCameraFollowRadius);
        }
    }
}