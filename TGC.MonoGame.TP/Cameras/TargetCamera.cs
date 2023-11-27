using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Prefab;

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
        
        // Camera shake
        private bool _isShaking;
        private float _shakeIntensity;
        private float _shakeDuration;
        private float _elapsedShakeTime;
        private Vector3 _originalCameraPosition;
        
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
        public void BuildView()
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

        public void Shake(float shakeIntensity, float shakeDuration)
        {
            _isShaking = true;
            _shakeIntensity = shakeIntensity;
            _shakeDuration = shakeDuration;
            _elapsedShakeTime = 0f;
        }
        
        public void Update(Vector3 playerPosition, float yaw, MouseState mouseState, GameTime gameTime, float playerSpeed, GraphicsDevice graphicsDevice)
        {
            UpdateFollowRadius(mouseState);

            AdjustFov(playerSpeed, graphicsDevice);

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
            
            ApplyCameraShake(gameTime);

            TargetPosition = playerPosition;
            BuildView();
        }

        private void AdjustFov(float playerSpeed, GraphicsDevice graphicsDevice)
        {
            const float initialFoV = MathHelper.PiOver4;
            const float maxFoV = MathHelper.PiOver2;

            var adjustmentFactor = MathHelper.Clamp(playerSpeed / 600f, 0.0f, 1.0f);

            var adjustedFoV = MathHelper.Lerp(initialFoV, maxFoV, adjustmentFactor);

            Projection = Matrix.CreatePerspectiveFieldOfView(adjustedFoV, graphicsDevice.Viewport.AspectRatio,
                TGCGame.CameraNearPlaneDistance, TGCGame.CameraFarPlaneDistance);
        }

        private static float? CameraCollided(Vector3 cameraPosition, Vector3 playerPosition)
        {
            var difference = playerPosition - cameraPosition;
            var distanceToPlayer = Vector3.Distance(playerPosition, cameraPosition);
            var normalizedDifference = difference / distanceToPlayer;
            var cameraToPlayerRay = new Ray(cameraPosition, normalizedDifference);
            
            foreach (var prefab in PrefabManager.Prefabs)
            {
                var distance = prefab.Intersects(cameraToPlayerRay);
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
        
        private void ApplyCameraShake(GameTime gameTime)
        {
            if (_isShaking)
            {
                _elapsedShakeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_elapsedShakeTime >= _shakeDuration)
                {
                    _isShaking = false;
                }
                else
                {
                    var shakeFactor = 1.0f - _elapsedShakeTime / _shakeDuration;

                    var offsetX = MathF.Sin(_elapsedShakeTime * 45f) * _shakeIntensity * shakeFactor;
                    var offsetY = MathF.Cos(_elapsedShakeTime * 25f) * _shakeIntensity * shakeFactor;
                    var offsetZ = MathF.Sin(_elapsedShakeTime * 45f) * _shakeIntensity * shakeFactor;

                    Position = new Vector3(Position.X + offsetX, Position.Y + offsetY, Position.Z + offsetZ);
                }
            }
        }
    }
}