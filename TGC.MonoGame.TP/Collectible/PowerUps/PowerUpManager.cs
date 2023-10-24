using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Collectible.PowerUps;

public static class PowerUpManager
{
    public static readonly List<PowerUp> PowerUps = new();
    
    public static void CreatePowerUp<T>(Vector3 position) where T : PowerUp
    {
        const float scale = 0.5f;
        PowerUps.Add((T)Activator.CreateInstance(typeof(T), position, scale));
    }
    
    public static void LoadPowerUps(ContentManager content)
    {
        var powerUpModels = new Dictionary<Type, Model>
        {
            { typeof(LowGravity), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/Gold_Star") },
            { typeof(SpeedUp), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/speed_power") },
        };
        
        var powerUpEffect = content.Load<Effect>(TGCGame.ContentFolderEffects + "PowerUpShader");

        foreach (var powerUp in PowerUps)
        {
            if (!powerUpModels.TryGetValue(powerUp.GetType(), out var model)) continue;
            powerUp.Model = model;
            powerUp.Shader = powerUpEffect;
            TGCGame.loadEffectOnMesh(powerUp.Model, powerUp.Shader);
        }
    }
}