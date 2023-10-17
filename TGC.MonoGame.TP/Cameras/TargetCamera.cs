using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Platform;

namespace TGC.MonoGame.TP.Cameras
{
    /// <summary>
    ///     Camera looking at a particular point, assumes the up vector is in y.
    /// </summary>
    public class TargetCamera : Camera
    {
        private const float CameraFollowRadius = 60f;
        private const float CameraUpDistance = 15f;
        
        /// <summary>
        ///     The direction that is "up" from the camera's point of view.
        /// </summary>
        public readonly Vector3 DefaultWorldUpVector = Vector3.Up;

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
        public Vector3 TargetPosition { get; set; }

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
            RightDirection = Vector3.Normalize(Vector3.Cross(DefaultWorldUpVector, FrontDirection));
            UpDirection = Vector3.Cross(FrontDirection, RightDirection);
            View = Matrix.CreateLookAt(Position, Position + FrontDirection, UpDirection);
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            // This camera has no movement, once initialized with position and lookAt it is no longer updated automatically.
        }
        
        public void Update(Vector3 playerPosition, float yaw)
        {
            var playerBackDirection = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(yaw));
            
            var orbitalPosition = playerBackDirection * CameraFollowRadius;
            
            var upDistance = Vector3.Up * CameraUpDistance;
            
            var newCameraPosition = playerPosition + orbitalPosition + upDistance;

            var collisionDistance = CameraCollided(newCameraPosition, playerPosition);

            if (collisionDistance.HasValue)
            {
                var clampedDistance =
                    MathHelper.Clamp(CameraFollowRadius - collisionDistance.Value, 0.1f, CameraFollowRadius);
                
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
            
            foreach (var t in Prefab.PlatformAabb)
            {
                var distance = cameraToPlayerRay.Intersects(t);
                if (distance < distanceToPlayer)
                    return distance;
            }
            return null;
        }
    }
}