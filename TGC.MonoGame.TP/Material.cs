﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP;

public class Material
{
    public Texture2D Texture { get; private set; }
    public Vector3 AmbientColor { get; private set; } = new(1f, 1f, 1f);
    public Vector3 DiffuseColor { get; private set; } = new(1f, 1f, 1f);
    public Vector3 SpecularColor { get; private set; } = new(1f, 1f, 1f);
    
    public float KAmbient { get; private set; }
    public float KDiffuse { get; private set; }
    public float KSpecular { get; private set; }
    public float Shininess { get; private set; }
    
    public Material(float kAmbient, float kDiffuse, float kSpecular, float shininess)
    {
        KAmbient = kAmbient;
        KDiffuse = kDiffuse;
        KSpecular = kSpecular;
        Shininess = shininess;
    }
    
    public static readonly Material Marble = new(0.310f, 0.830f, 1.0f, 29.0f);
    public static readonly Material Rubber = new(0.310f, 0.830f, 1.0f, 29.0f);
    public static readonly Material Metal = new(0.310f, 0.830f, 1.0f, 29.0f);
    public void LoadTexture(Texture2D texture)
    {
        Texture = texture;
    }
}