using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Audio;
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
        
        var powerUpEffect = content.Load<Effect>(TGCGame.ContentFolderEffects + "PowerUpShader");
        var basicShader = content.Load<Effect>(TGCGame.ContentFolderEffects + "BasicShader");

        foreach (var collectible in Collectibles)
        {
            AssignModelAndShader(collectible, collectibleModels, basicShader, powerUpEffect);
            AssignSound(collectible, AudioManager.CollectibleSounds);
        }
    }
    
    private static void AssignModelAndShader(Collectible collectible, Dictionary<Type, Model> collectibleModels, Effect basicShader, Effect powerUpEffect)
    {
        if (!collectibleModels.TryGetValue(collectible.GetType(), out var model)) return;
        collectible.Model = model;
        collectible.Shader = collectible is Coin ? basicShader : powerUpEffect;
        TGCGame.loadEffectOnMesh(collectible.Model, collectible.Shader);
    }

    private static void AssignSound(Collectible collectible, Dictionary<Type, SoundEffect> collectibleSounds)
    {
        if (!collectibleSounds.TryGetValue(collectible.GetType(), out var sound)) return;
        collectible.Sound = sound;
    }
}