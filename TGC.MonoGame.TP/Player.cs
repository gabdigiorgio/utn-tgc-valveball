using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Platform;

namespace TGC.MonoGame.TP;

public class Player
{
    public Vector3 SpherePosition;
    public float Yaw { get; private set; }
    public float Gravity { private get;  set; } = MaxGravity;
    public int Score { get; private set; }
    private readonly Matrix _sphereScale;
    private float _pitch;
    private float _roll;
    private float _speed;
    private float _pitchSpeed; 
    private float _yawSpeed;
    private float _jumpSpeed;
    private bool _isJumping;
    private bool _onGround;
    private bool _isRollingSoundPlaying = false;
    public BoundingSphere BoundingSphere;

    public Player(Matrix sphereScale, Vector3 spherePosition, BoundingSphere boundingSphere, float yaw)
    {
        _sphereScale = sphereScale;
        SpherePosition = spherePosition;
        BoundingSphere = boundingSphere;
        Yaw = yaw;
    }

    public SphereMaterial CurrentSphereMaterial { get; private set; } = SphereMaterial.SphereRubber;

    private const float PitchMaxSpeed = 15f;
    private const float YawMaxSpeed = 5.8f;
    private const float PitchAcceleration = 5f;
    private const float YawAcceleration = 5f;
    private const float MaxGravity = 175f;

    public Matrix Update(float time, KeyboardState keyboardState)
    {
        ChangeSphereMaterial(keyboardState);
        HandleJumping(keyboardState);
        HandleFalling(time);
        HandleYaw(time, keyboardState);
        var rotationY = Matrix.CreateRotationY(Yaw);
        var forward = rotationY.Forward;
        HandleMovement(time, keyboardState, forward);
        var rotationX = Matrix.CreateRotationX(_pitch);
        var translation = Matrix.CreateTranslation(BoundingSphere.Center);
        RestartPosition(keyboardState);
        return _sphereScale * rotationX * rotationY * translation;
    }

    private void ChangeSphereMaterial(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.D1))
        {
            CurrentSphereMaterial = SphereMaterial.SphereMarble;
        }

        if (keyboardState.IsKeyDown(Keys.D2))
        {
            CurrentSphereMaterial = SphereMaterial.SphereRubber;
        }

        if (keyboardState.IsKeyDown(Keys.D3))
        {
            CurrentSphereMaterial = SphereMaterial.SphereMetal;
        }
    }

    private void RestartPosition(KeyboardState keyboardState)
    {
        if (!(BoundingSphere.Center.Y <= -150f) && !keyboardState.IsKeyDown(Keys.R)) return;
        BoundingSphere.Center = TGCGame.InitialSpherePosition; // TODO: checkpoint
        Yaw = TGCGame.InitialSphereYaw;
        SetSpeedToZero();
    }
    
    public void ResetGravity()
    {
        Gravity = MaxGravity;
    }

    public void IncreaseScore(int value)
    {
        Score += value;
    }

    private void SetSpeedToZero()
    {
        _pitchSpeed = 0;
        _speed = 0;
        _jumpSpeed = 0;
    }

    private void HandleJumping(KeyboardState keyboardState)
    {
        if(keyboardState.IsKeyDown(Keys.Space) && !_isJumping)
        {
            StartJump();
        }
    }

    private void HandleFalling(float time)
    {
        if (_onGround) return;
        var newYPosition = CalculateFallPosition(time);
        BoundingSphere.Center = newYPosition;
    }
    
    private void StartJump()
    {
        TGCGame.JumpSound.Play();
        _isJumping = true;
        _onGround = false;
        _jumpSpeed += CalculateJumpSpeed();
    }

    private void EndJump()
    {
        _isJumping = false;
        _jumpSpeed = 0;
    }
    
    private Vector3 CalculateFallPosition(float time)
    {
        _jumpSpeed -= Gravity * time;
        var newYPosition = BoundingSphere.Center.Y + _jumpSpeed * time;
        return new Vector3(BoundingSphere.Center.X, newYPosition, BoundingSphere.Center.Z);
    }

    private float CalculateJumpSpeed()
    {
        return (float)Math.Sqrt(2 * CurrentSphereMaterial.MaxJumpHeight * Math.Abs(Gravity));
    }

    private void HandleYaw(float time, KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.A))
        {
            AccelerateYaw(YawAcceleration, time);
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            AccelerateYaw(-YawAcceleration, time);
        }
        else
        {
            DecelerateYaw(time);
        }

        AdjustYawSpeed(time);
    }

    private void AdjustYawSpeed(float time)
    {
        _yawSpeed = MathHelper.Clamp(_yawSpeed, -YawMaxSpeed, YawMaxSpeed);
        Yaw += _yawSpeed * time;
    }

    private void AccelerateYaw(float yawAcceleration, float time)
    {
        _yawSpeed += yawAcceleration * time;
    }

    private void DecelerateYaw(float time)
    {
        var yawDecelerationDirection = Math.Sign(_yawSpeed) * -1;
        _yawSpeed += YawAcceleration * time * yawDecelerationDirection;
    }

    private void HandleMovement(float time, KeyboardState keyboardState, Vector3 forward)
    {
        if (keyboardState.IsKeyDown(Keys.W))
        {
            Accelerate(CurrentSphereMaterial.Acceleration, time);
            AcceleratePitch(PitchAcceleration, time);
        }
        else if (keyboardState.IsKeyDown(Keys.S))
        {
            Accelerate(-CurrentSphereMaterial.Acceleration, time);
            AcceleratePitch(-PitchAcceleration, time);
        }
        else
        {
            Decelerate(CurrentSphereMaterial.Acceleration, time);
            DeceleratePitch(time);
        }

        AdjustPitchSpeed(time);
        AdjustSpeed(time, forward);
        SolveCollisions();
        UpdateSpherePosition(BoundingSphere.Center);
    }

    private void UpdateSpherePosition(Vector3 newPosition)
    {
        SpherePosition = newPosition;
    }

    private void AdjustSpeed(float time, Vector3 forward)
    {
        _speed = MathHelper.Clamp(_speed, -CurrentSphereMaterial.MaxSpeed, CurrentSphereMaterial.MaxSpeed);
        BoundingSphere.Center += forward * time * _speed;
    }

    private void AdjustPitchSpeed(float time)
    {
        _pitchSpeed = MathHelper.Clamp(_pitchSpeed, -PitchMaxSpeed, PitchMaxSpeed);
        _pitch += _pitchSpeed * time;
    }

    private void AcceleratePitch(float pitchAcceleration,float time)
    {
        _pitchSpeed -= pitchAcceleration * time;
    }
    
    private void DeceleratePitch(float time)
    {
        var pitchDecelerationDirection = Math.Sign(_pitchSpeed) * -1;
        _pitchSpeed += PitchAcceleration * time * pitchDecelerationDirection;
    }

    private void Accelerate(float acceleration, float time)
    {
        _speed += acceleration * time;
    }

    private void Decelerate(float acceleration, float time)
    {
        var decelerationDirection = Math.Sign(_speed) * -1;
        _speed += acceleration * time * decelerationDirection;
    }

    private void SolveCollisions()
    {
        var sphereCenter = BoundingSphere.Center;
        var radius = BoundingSphere.Radius;
        var collisions = new List<CollisionInfo>();

        _onGround = false;
        
        DetectAabbCollisions(sphereCenter, collisions);
        
        DetectObbCollisions(sphereCenter, collisions);
        
        DetectMovingCollisions(sphereCenter, collisions);



        // Solve first near collisions
        collisions.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        
        foreach (var collision in collisions)
        {
            BoundingSphere.Center = SolveCollisionPosition(BoundingSphere.Center, collision.ClosestPoint, radius, collision.Distance)
                + collision.ColliderMovement;
        }
    }
    
    private void DetectAabbCollisions(Vector3 sphereCenter, List<CollisionInfo> collisions)
    {
        foreach (var collider in Prefab.PlatformAabb)
        {
            if (!collider.Intersects(BoundingSphere)) continue;

            var closestPoint = BoundingVolumesExtensions.ClosestPoint(collider, sphereCenter);
            var distance = Vector3.Distance(closestPoint, sphereCenter);
            collisions.Add(new CollisionInfo(closestPoint, distance));

            if (!(sphereCenter.Y > collider.Max.Y)) continue;
            _onGround = true;
            EndJump();
        }
    }
    
    private void DetectObbCollisions(Vector3 sphereCenter, List<CollisionInfo> collisions)
    {
        foreach (var collider in Prefab.RampObb)
        {
            if (!collider.Intersects(BoundingSphere, out _, out _)) continue;

            var closestPoint = collider.ClosestPoint(sphereCenter);
            var distance = Vector3.Distance(closestPoint, sphereCenter);
            collisions.Add(new CollisionInfo(closestPoint, distance));

            _onGround = true;
            EndJump();
        }
    }

    private void DetectMovingCollisions(Vector3 sphereCenter, List<CollisionInfo> collisions)
    {
        foreach (var movingPlatform in Prefab.MovingPlatforms)
        {
            var collider = movingPlatform.MovingBoundingBox;

            if (!collider.Intersects(BoundingSphere)) continue;

            var closestPoint = BoundingVolumesExtensions.ClosestPoint(collider, sphereCenter);
            var distance = Vector3.Distance(closestPoint, sphereCenter);
            var platformMovement = movingPlatform.Position - movingPlatform.PreviousPosition;
            collisions.Add(new CollisionInfo(closestPoint, distance, platformMovement));

            if (!(sphereCenter.Y > collider.Max.Y)) continue;
            _onGround = true;
            EndJump();
        }

        foreach (var movingObstacle in Prefab.MovingObstacles)
        {
            var collider = movingObstacle.MovingBoundingBox;

            if (!collider.Intersects(BoundingSphere)) continue;

            var closestPoint = BoundingVolumesExtensions.ClosestPoint(collider, sphereCenter);
            var distance = Vector3.Distance(closestPoint, sphereCenter);
            var platformMovement = movingObstacle.Position - movingObstacle.PreviousPosition;
            collisions.Add(new CollisionInfo(closestPoint, distance, platformMovement));

            if (!(sphereCenter.Y > collider.Max.Y)) continue;
            _onGround = true;
            EndJump();
        }
    }

    private static Vector3 SolveCollisionPosition(Vector3 currentPosition, Vector3 closestPoint, float radius, float distance)
    {
        var penetration = radius - distance;
        var direction = Vector3.Normalize(currentPosition - closestPoint);
        return currentPosition + direction * penetration;
    }
}