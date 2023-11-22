using System;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP;

public class MainMenuCamera
{
    private const float MenuRotationSpeed = 0.01f;

    private float _rotationAngle = 0f;
    private const float CameraFollowRadius = 350f;

    private TargetCamera TargetCamera { get; set; }

    public MainMenuCamera(TargetCamera targetCamera)
    {
        TargetCamera = targetCamera;
    }

    public void Update(Vector3 targetPosition)
    {
        _rotationAngle += MenuRotationSpeed;

        // Calcula la posición orbital
        var orbitalPosition = new Vector3(
            targetPosition.X + (float)Math.Sin(_rotationAngle) * CameraFollowRadius,
            targetPosition.Y,
            targetPosition.Z + (float)Math.Cos(_rotationAngle) * CameraFollowRadius
        );

        // Actualiza la posición de la cámara principal
        TargetCamera.Position = orbitalPosition + new Vector3(0, 0f, -50f); // Ajusta según sea necesario
        TargetCamera.BuildView();
    }
}