using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Collectible.Coins;
using TGC.MonoGame.TP.Collectible.PowerUps;

namespace TGC.MonoGame.TP.Collectible;

public static class CollectibleManager
{
    public static readonly List<Collectible> Collectibles = new();
    
    public static void CreateCollectible<T>(float x, float y, float z) where T : Collectible
    {
        var position = new Vector3(x, y, z);
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
        
        var collectibleSounds = new Dictionary<Type, SoundEffect>
        {
            { typeof(LowGravity), content.Load<SoundEffect>(TGCGame.ContentFolderSounds + "gravity_change") },
            { typeof(SpeedUp), content.Load<SoundEffect>(TGCGame.ContentFolderSounds + "speed_up") },
            { typeof(Coin), content.Load<SoundEffect>(TGCGame.ContentFolderSounds + "coin") }
        };
        
        var powerUpEffect = content.Load<Effect>(TGCGame.ContentFolderEffects + "PowerUpShader");
        var basicShader = content.Load<Effect>(TGCGame.ContentFolderEffects + "BasicShader");

        foreach (var collectible in Collectibles)
        {
            if (!collectibleModels.TryGetValue(collectible.GetType(), out var model)) continue;
            collectible.Model = model;
            collectible.Shader = collectible is Coin ? basicShader : powerUpEffect;
            TGCGame.loadEffectOnMesh(collectible.Model, collectible.Shader);
            if (collectibleSounds.TryGetValue(collectible.GetType(), out var sound))
            {
                collectible.Sound = sound;
            }
        }
    }
}