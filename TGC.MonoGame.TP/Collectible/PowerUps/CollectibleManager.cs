using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Collectible.PowerUps;

public static class CollectibleManager
{
    public static readonly List<Collectible> Collectibles = new();
    
    public static void CreateCollectible<T>(Vector3 position) where T : Collectible
    {
        const float scale = 0.5f;
        Collectibles.Add((T)Activator.CreateInstance(typeof(T), position));
    }
    
    public static void LoadCollectibles(ContentManager content)
    {
        var collectibleModels = new Dictionary<Type, Model>
        {
            { typeof(LowGravity), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/Gold_Star") },
            { typeof(SpeedUp), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/speed_power") },
            { typeof(Coin), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/coin") }
        };
        
        var collectibleEffect = content.Load<Effect>(TGCGame.ContentFolderEffects + "PowerUpShader");

        foreach (var collectible in Collectibles)
        {
            if (!collectibleModels.TryGetValue(collectible.GetType(), out var model)) continue;
            collectible.Model = model;
            collectible.Shader = collectibleEffect;
            TGCGame.loadEffectOnMesh(collectible.Model, collectible.Shader);
        }
    }
}