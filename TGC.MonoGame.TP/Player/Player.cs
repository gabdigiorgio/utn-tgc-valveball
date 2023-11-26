using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Audio;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Prefab;

namespace TGC.MonoGame.TP.Player;

public class Player
{
    public Vector3 SpherePosition;
    public float Yaw { get; private set; }
    public float Gravity { private get;  set; } = MaxGravity;
    public int Score { get; private set; }
    public BoundingSphere BoundingSphere;
    public SphereMaterial CurrentSphereMaterial { get; private set; } = SphereMaterial.SphereMetal;
    
    private readonly Matrix _sphereScale;
    private float _pitch;
    private float _roll;
    public float Speed { get; private set; }
    private float _pitchSpeed; 
    private float _yawSpeed;
    private float _jumpSpeed;
    private bool _isJumping;
    private bool _onGround;
    private SoundEffectInstance _rollingSoundInstance;
    private SoundEffectInstance _bumpSoundInstance;
    private readonly Random _random = new();
    
    private const float PitchMaxSpeed = 15f;
    private const float YawMaxSpeed = 5.8f;
    private const float PitchAcceleration = 5f;
    private const float YawAcceleration = 5f;
    private const float MaxGravity = 175f;
    private Vector3 _restartPosition = TGCGame.InitialSpherePosition;

    public Player(Matrix sphereScale, Vector3 spherePosition, BoundingSphere boundingSphere, float yaw)
    {
        _sphereScale = sphereScale;
        SpherePosition = spherePosition;
        BoundingSphere = boundingSphere;
        Yaw = yaw;
    }

    public Matrix Update(float time, KeyboardState keyboardState)
    {
        ChangeSphereMaterial(keyboardState);
        HandleJumping(keyboardState);
        HandleFalling(time);
        HandleYaw(time, keyboardState);
        var rotationY = Matrix.CreateRotationY(Yaw);
        var forward = rotationY.Forward;
        HandleMovement(time, keyboardState, forward);
        PlayRollingSound();
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
    
    private void PlayRollingSound()
    {
        const float quietThreshold = 0.01f;

        if (ShouldPlayRollingSound(quietThreshold))
        {
            InitializeRollingSoundInstance();
            
            _rollingSoundInstance.Volume = CalculateVolumeSound(_pitchSpeed, PitchMaxSpeed);
            
            _rollingSoundInstance.Pitch = CalculatePitchSound();
        }
        else
        {
            StopRollingSoundInstance();
        }
    }

    private bool ShouldPlayRollingSound(float threshold)
    {
        return Math.Abs(_pitchSpeed) > threshold && _onGround;
    }

    private void InitializeRollingSoundInstance()
    {
        if (_rollingSoundInstance != null) return;
        _rollingSoundInstance = AudioManager.RollingSound.CreateInstance();
        _rollingSoundInstance.IsLooped = true;
        _rollingSoundInstance.Play();
    }

    private float CalculateVolumeSound(float speed, float maxSpeed)
    {
        return MathHelper.Clamp(Math.Abs(speed) / maxSpeed, 0, 1);
    }

    private float CalculatePitchSound()
    {
        const float pitchScaleFactor = 0.1f;
        return MathHelper.Clamp(Math.Abs(_pitchSpeed) * pitchScaleFactor, 0.0f, 1.0f);
    }

    private void StopRollingSoundInstance()
    {
        if (_rollingSoundInstance == null) return;
        _rollingSoundInstance.Stop();
        _rollingSoundInstance.Dispose();
        _rollingSoundInstance = null;
    }

    private void RestartPosition(KeyboardState keyboardState)
    {
        if (!(BoundingSphere.Center.Y <= -150f) && !keyboardState.IsKeyDown(Keys.R)) return;
        BoundingSphere.Center = _restartPosition;
        Yaw = TGCGame.InitialSphereYaw;
        SetSpeedToZero();
    }
    
    public void ChangeRestartPosition(Vector3 newPosition)
    {
        _restartPosition = newPosition;
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
        Speed = 0;
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
    
    public void StartJump()
    {
        AudioManager.JumpSound.Play();
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
        Speed = MathHelper.Clamp(Speed, -CurrentSphereMaterial.MaxSpeed, CurrentSphereMaterial.MaxSpeed);
        BoundingSphere.Center += forward * time * Speed;
    }

    private void AdjustPitchSpeed(float time)
    {
        _pitchSpeed = MathHelper.Clamp(_pitchSpeed, -PitchMaxSpeed, PitchMaxSpeed);
        _pitch += _pitchSpeed * time;
        
        if (Math.Abs(_pitchSpeed) < 0.0001f)
        {
            _pitchSpeed = 0f;
        }
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
        Speed += acceleration * time;
    }

    private void Decelerate(float acceleration, float time)
    {
        var decelerationDirection = Math.Sign(Speed) * -1;
        Speed += acceleration * time * decelerationDirection;
    }

    private void SolveCollisions()
    {
        var sphereCenter = BoundingSphere.Center;
        var radius = BoundingSphere.Radius;
        var collisions = new List<CollisionInfo>();
        var wasOnGround = _onGround;
        var lastJumpSpeed = _jumpSpeed;

        _onGround = false;
        
        DetectPrefabCollisions(sphereCenter, collisions);
        
        foreach (var collision in collisions)
        {
            BoundingSphere.Center = SolveCollisionPosition(BoundingSphere.Center, collision.ClosestPoint, radius, collision.Distance) 
                                    + (collision.ColliderMovement ?? Vector3.Zero);
        }

        PlayBumpSound(wasOnGround, lastJumpSpeed);
    }

    private void PlayBumpSound(bool wasOnGround, float lastJumpSpeed)
    {
        if (!ShouldPlayBumpSound(wasOnGround)) return;
        var randomIndex = _random.Next(AudioManager.BumpSounds.Count);
        _bumpSoundInstance = AudioManager.BumpSounds[randomIndex].CreateInstance();
        _bumpSoundInstance.Volume = CalculateVolumeSound(lastJumpSpeed, MaxGravity);
        _bumpSoundInstance.Play();
    }
    
    private bool ShouldPlayBumpSound(bool wasOnGround)
    {
        return !wasOnGround && _onGround && (_bumpSoundInstance == null || _bumpSoundInstance.State == SoundState.Stopped);
    }

    private void DetectPrefabCollisions(Vector3 sphereCenter, ICollection<CollisionInfo> collisions)
    {
        foreach (var prefab in PrefabManager.Prefabs)
        {
            if (!prefab.Intersects(BoundingSphere)) continue;

            var closestPoint = prefab.ClosestPoint(sphereCenter);
            var distance = Vector3.Distance(closestPoint, sphereCenter);
            var platformMovement = prefab.Position - prefab.PreviousPosition;
            collisions.Add(new CollisionInfo(closestPoint, distance, platformMovement));

            switch (prefab)
            {
                case Platform when !(sphereCenter.Y > prefab.MaxY()):
                    break;
                case Platform:
                case Ramp:
                    HandleGroundCollision();
                    break;
            }
        }
    }

    private static Vector3 SolveCollisionPosition(Vector3 currentPosition, Vector3 closestPoint, float radius, float distance)
    {
        var penetration = radius - distance;
        var direction = Vector3.Normalize(currentPosition - closestPoint);
        return currentPosition + direction * penetration;
    }
    
    private void HandleGroundCollision()
    {
        _onGround = true;
        EndJump();
    }
}