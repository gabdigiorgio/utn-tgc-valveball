﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Audio;
using TGC.MonoGame.TP.Collectible.Checkpoints;
using TGC.MonoGame.TP.Collectible.Coins;
using TGC.MonoGame.TP.Collectible.PowerUps;

namespace TGC.MonoGame.TP.Collectible;

public static class CollectibleManager
{
    public static readonly List<Collectible> Collectibles = new();
    
    public static void LoadCollectibles(ContentManager content)
    {
        var collectibleModels = new Dictionary<Type, Model>
        {
            { typeof(LowGravity), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/Gold_Star") },
            { typeof(SpeedUp), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/speed_power") },
            { typeof(Coin), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/dollar_coin") },
            { typeof(FinalCheckpoint), content.Load<Model>(TGCGame.ContentFolder3D + "collectibles/checkpoint")}
        };
        
        var powerUpShader = content.Load<Effect>(TGCGame.ContentFolderEffects + "PowerUpShader");
        var checkpointShader = content.Load<Effect>(TGCGame.ContentFolderEffects + "AlphaBlending");
        var blinnPhong = content.Load<Effect>(TGCGame.ContentFolderEffects + "BlinnPhongTypes");

        foreach (var collectible in Collectibles)
        {
            AssignModelAndShader(collectible, collectibleModels, blinnPhong, powerUpShader, checkpointShader);
            AssignSound(collectible, AudioManager.CollectibleSounds);
        }
    }
    
    public static void CreateCoinsSquareCircuit(float xOffset, float yOffset, float zOffset)
    {
        // Side
        CreateCollectible<Coin>(300f + xOffset, 13f + yOffset, 50f + zOffset);
        CreateCollectible<Coin>(300f + xOffset, 13f + yOffset, 75f + zOffset);
        CreateCollectible<Coin>(300f + xOffset, 13f + yOffset, 95f + zOffset);
        CreateCollectible<Coin>(300f + xOffset, 13f + yOffset, -50f + zOffset);
        CreateCollectible<Coin>(300f + xOffset, 13f + yOffset, -75f + zOffset);
        CreateCollectible<Coin>(300f + xOffset, 13f + yOffset, -95f + zOffset);
        
        CreateCollectible<Coin>(0f + xOffset, 13f + yOffset, 50f + zOffset);
        CreateCollectible<Coin>(0f + xOffset, 13f + yOffset, 75f + zOffset);
        CreateCollectible<Coin>(0f + xOffset, 13f + yOffset, 95f + zOffset);
        CreateCollectible<Coin>(0f + xOffset, 13f + yOffset, -50f + zOffset);
        CreateCollectible<Coin>(0f + xOffset, 13f + yOffset, -75f + zOffset);
        CreateCollectible<Coin>(0f + xOffset, 13f + yOffset, -95f + zOffset);

        // Parable
        CreateCollectible<Coin>(230f + xOffset, 23f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(210f + xOffset, 28f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(190f + xOffset, 33f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(170f + xOffset, 38f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(150f + xOffset, 38f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(70f + xOffset, 23f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(90f + xOffset, 28f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(110f + xOffset, 33f + yOffset, 0f + zOffset);
        CreateCollectible<Coin>(130f + xOffset, 38f + yOffset, 0f + zOffset);
    }
    
    public static void CreateCoinsSwitchBackRamp()
    {
        float[] heights = { 47f, 163f, 279f, 395f, 511f, 627f };

        foreach (var height in heights)
        {
            CreateCollectible<Coin>(-922f, height, 0f);
            CreateCollectible<Coin>(-922f, height, 46f);
        }
    }
    
    public static void CreateCoinsMaze()
    {
        CreateCollectible<Coin>(-250f, 685, 0f);
        CreateCollectible<Coin>(-250f, 685, 60f);
        CreateCollectible<Coin>(-250f, 685, 120f);
        CreateCollectible<Coin>(-220f, 685, 150f);
        CreateCollectible<Coin>(-150f, 685, 150f);
        CreateCollectible<Coin>(-150f, 685, 180f);
        CreateCollectible<Coin>(-150f, 685, 240f);
        CreateCollectible<Coin>(-150f, 685, 300f);
        CreateCollectible<Coin>(-205f, 685, 345f);
        CreateCollectible<Coin>(-145f, 685, 345f);
        CreateCollectible<Coin>(-85f, 685, 345f);
        CreateCollectible<Coin>(-40, 685, 300f);
        CreateCollectible<Coin>(40f, 685, 300f);
        CreateCollectible<Coin>(40f, 685, 250f);
        CreateCollectible<Coin>(90f, 685, 250f);
        CreateCollectible<Coin>(90f, 685, 200f);
        CreateCollectible<Coin>(150f, 685, 200f);
        CreateCollectible<Coin>(150f, 685, 245f);
        CreateCollectible<Coin>(250f, 685, 200f);
        CreateCollectible<Coin>(250f, 685, 150f);
        CreateCollectible<Coin>(350f, 685, 150f);
        CreateCollectible<Coin>(300f, 685, -50f);
        CreateCollectible<Coin>(250f, 685, -50f);
        CreateCollectible<Coin>(250f, 685, 30f);
        CreateCollectible<Coin>(250f, 685, 100f);
        CreateCollectible<Coin>(150f, 685, 100f);
    }
    
    public static void CreatePowerUpsSwitchBackRamp()
    {
        //CreateCollectible<SpeedUp>(-675f, 110f, 25f);
        CreateCollectible<SpeedUp>(-675f, 226f, 25f);
        //CreateCollectible<SpeedUp>(-675f, 342f, 25f);
        CreateCollectible<SpeedUp>(-675f, 458f, 25f);
        //CreateCollectible<SpeedUp>(-675f, 574f, 25f);
    }
    
    public static void CreatePowerUpsMaze()
    {
        CreateCollectible<LowGravity>(-250f, 680f, -310f);
    }

    public static void CreatePowerUpsSquareCircuit(float xOffset, float yOffset, float zOffset)
    {
        CreateCollectible<LowGravity>(150f + xOffset, 5f + yOffset, 0f + zOffset);
        CreateCollectible<SpeedUp>(150f + xOffset, 10f + yOffset, -200f + zOffset);
        CreateCollectible<SpeedUp>(150f + xOffset, 10f + yOffset, 200f + zOffset);
    }

    public static void CreateCheckpoints()
    {
        CreateCollectible<Checkpoint>(1100, 230.5f, 0f, MathHelper.PiOver2);
        CreateCollectible<Checkpoint>(300f, 8.5f, 0f, MathHelper.PiOver2);
        CreateCollectible<Checkpoint>(-600f, 8.5f, 0f, MathHelper.PiOver2);
        CreateCollectible<Checkpoint>(-625f, 680.5f, 0f, -MathHelper.PiOver2);
        CreateCollectible<FinalCheckpoint>(100f, 678f, 0f);
    }
    
    private static void CreateCollectible<T>(float x, float y, float z) where T : Collectible
    {
        var position = new Vector3(x, y, z);
        Collectibles.Add((T)Activator.CreateInstance(typeof(T), position));
    }
    
    private static void CreateCollectible<T>(float x, float y, float z, float yaw) where T : Collectible
    {
        var position = new Vector3(x, y, z);
        Collectibles.Add((T)Activator.CreateInstance(typeof(T), position, yaw));
    }

    private static void AssignModelAndShader(Collectible collectible, Dictionary<Type, Model> collectibleModels,
        Effect coinShader, Effect powerUpShader,
        Effect checkpointShader)
    {
        collectible.Shader = collectible switch
        {
            Checkpoint => checkpointShader,
            Coin => coinShader,
            _ => powerUpShader
        };
        
        if (!collectibleModels.TryGetValue(collectible.GetType(), out var model)) return;
        collectible.Model = model;
        
        TGCGame.loadEffectOnMesh(collectible.Model, collectible.Shader);
    }


    private static void AssignSound(Collectible collectible, Dictionary<Type, SoundEffect> collectibleSounds)
    {
        if (!collectibleSounds.TryGetValue(collectible.GetType(), out var sound)) return;
        collectible.Sound = sound;
    }
}