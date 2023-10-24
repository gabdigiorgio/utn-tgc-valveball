﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Collectible;
using TGC.MonoGame.TP.Collectible.PowerUps;
using TGC.MonoGame.TP.Collisions;
using TGC.MonoGame.TP.Geometries;
using TGC.MonoGame.TP.Platform;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }
    
        // Graphics
        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        
        // Skybox
        private SkyBox SkyBox { get; set; }
        
        // Camera
        private Camera Camera { get; set; }
        private TargetCamera TargetCamera { get; set; }
        
        // Scene
        private Matrix SphereWorld { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        
        // Geometries
        private BoxPrimitive BoxPrimitive { get; set; }
        
        // Sphere position & rotation
        public static readonly Vector3 InitialSpherePosition = new(300f, 10f, 0f);
        public const float InitialSphereYaw = 1.57f;
        private readonly Matrix _sphereScale = Matrix.CreateScale(5f);
        private const float SphereRadius = 5f;

        // Effects
        private Effect BlinnPhongEffect { get; set; }

        // Models
        private Model SphereModel { get; set; }
        private static Player Player { get; set; }
        
        // Colliders
        private Gizmos.Gizmos Gizmos { get; set; }


        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.
            
            // Configuro las dimensiones de la pantalla.
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();
            
            // Camera
            var size = GraphicsDevice.Viewport.Bounds.Size;
            size.X /= 2;
            size.Y /= 2;
            //Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 40, 200), size);
            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);
            
            // Configuramos nuestras matrices de la escena.
            SphereWorld = Matrix.Identity;
            View = Matrix.CreateLookAt(Vector3.UnitZ * 150, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);
            
            // Player
            Player = new Player(_sphereScale, InitialSpherePosition, new BoundingSphere(InitialSpherePosition, SphereRadius), InitialSphereYaw);
            
            // Gizmos
            Gizmos = new Gizmos.Gizmos
            {
                Enabled = true
            };
            
            // Collectibles
            CollectibleManager.CreateCollectible<LowGravity>(new Vector3(150f, 5f, 0f));
            CollectibleManager.CreateCollectible<LowGravity>(new Vector3(-450f, 5f, 0f));
            CollectibleManager.CreateCollectible<SpeedUp>(new Vector3(150f, 10f, -200f));
            CollectibleManager.CreateCollectible<SpeedUp>(new Vector3(150f, 10f, 200f));
            CollectibleManager.CreateCollectible<SpeedUp>(new Vector3(-450f,10f, -200f));
            CollectibleManager.CreateCollectible<SpeedUp>(new Vector3(-450f,10f, 200f));
            
            CollectibleManager.CreateCollectible<Coin>(new Vector3(300f, 15f, 100f));
            
            // Map
            Prefab.CreateSquareCircuit(Vector3.Zero);
            Prefab.CreateSquareCircuit(new Vector3(-600, 0f, 0f));
            Prefab.CreateBridge();
            Prefab.CreateSwitchbackRamp();
            
            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Diffuse
            var platformGreenDiffuse = Content.Load<Texture2D>(ContentFolderTextures + "platform_green_diffuse");
            var platformOrangeDiffuse = Content.Load<Texture2D>(ContentFolderTextures + "platform_orange_diffuse");
            var marbleDiffuse = Content.Load<Texture2D>(ContentFolderTextures + "marble_black_diffuse");
            var rubberDiffuse = Content.Load<Texture2D>(ContentFolderTextures + "rubber_diffuse");
            var metalDiffuse = Content.Load<Texture2D>(ContentFolderTextures + "metal_diffuse");
            
            // Normals
            var platformSquareNormal = Content.Load<Texture2D>(ContentFolderTextures + "platform_square_normal");
            var platformNormal = Content.Load<Texture2D>(ContentFolderTextures + "platform_normal");
            var plainNormal = Content.Load<Texture2D>(ContentFolderTextures + "plain_normal");
            var rubberNormal = Content.Load<Texture2D>(ContentFolderTextures + "rubber_normal");
            var metalNormal = Content.Load<Texture2D>(ContentFolderTextures + "metal_normal");
            
            // Materials
            Material.Platform.LoadTexture(platformGreenDiffuse, platformSquareNormal);
            Material.MovingPlatform.LoadTexture(platformOrangeDiffuse, platformNormal);
            Material.Marble.LoadTexture(marbleDiffuse, plainNormal);
            Material.Rubber.LoadTexture(rubberDiffuse, rubberNormal);
            Material.Metal.LoadTexture(metalDiffuse, metalNormal);
            
            // Platform
            BoxPrimitive = new BoxPrimitive(GraphicsDevice, Vector3.One, platformGreenDiffuse);
            
            // Collectibles
            CollectibleManager.LoadCollectibles(Content);
            
            // Sphere
            SphereModel = Content.Load<Model>(ContentFolder3D + "geometries/sphere");
            BlinnPhongEffect = Content.Load<Effect>(ContentFolderEffects + "BlinnPhongTypes");
            loadEffectOnMesh(SphereModel, BlinnPhongEffect);
            SphereWorld = _sphereScale * Matrix.CreateTranslation(InitialSpherePosition);
            
            // SkyBox
            var skyBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skyboxes/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 1000f);
            
            // Gizmos
            Gizmos.LoadContent(GraphicsDevice, Content);

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var time = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            SphereWorld = Player.Update(time, keyboardState);

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            TargetCamera.Update(Player.SpherePosition, Player.Yaw, mouseState);
            
            SetLightPosition(new Vector3(150f, 750f, 0f));

            Prefab.UpdateMovingPlatforms();

            UpdatePowerUps(gameTime);

            Gizmos.UpdateViewProjection(TargetCamera.View, TargetCamera.Projection);

            base.Update(gameTime);
        }

        private void SetLightPosition(Vector3 lightPosition)
        {
            BlinnPhongEffect.Parameters["lightPosition"].SetValue(lightPosition);
            BlinnPhongEffect.Parameters["eyePosition"].SetValue(TargetCamera.Position);
        }

        private static void UpdatePowerUps(GameTime gameTime)
        {
            foreach (var powerUp in CollectibleManager.Collectibles)
            {
                powerUp.Update(gameTime, Player);
            }
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            DrawPlatforms(BlinnPhongEffect, Material.Platform);
            
            DrawRamps(BlinnPhongEffect, Material.Platform); 

            DrawMovingPlatforms(BlinnPhongEffect, Material.MovingPlatform);

            DrawTexturedModel(SphereWorld, SphereModel, BlinnPhongEffect, Player.CurrentSphereMaterial.Material);

            DrawCollectibles(CollectibleManager.Collectibles, gameTime);
            
            DrawGizmos();
            Gizmos.Draw();
            
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Graphics.GraphicsDevice.RasterizerState = rasterizerState;
            
            SkyBox.Draw(TargetCamera.View, TargetCamera.Projection, new Vector3(0f,0f,0f));
            GraphicsDevice.RasterizerState = originalRasterizerState;
            
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }
        
        private void DrawTexturedModel(Matrix worldMatrix, Model model, Effect effect, Material material){
            SetBlinnPhongParameters(effect, material, Vector2.One * 5f, worldMatrix, TargetCamera);
            foreach (var mesh in model.Meshes)
            {   
                mesh.Draw();
            }
        }

        private void DrawPlatforms(Effect effect, Material material)
        {
            foreach (var platformWorld in Prefab.PlatformMatrices)
            {
                SetBlinnPhongParameters(effect, material, Vector2.One * 3f, platformWorld, TargetCamera);
                BoxPrimitive.Draw(effect);
            }
        }

        private void DrawRamps(Effect effect, Material material)
        {
            foreach (var rampWorld in Prefab.RampMatrices)
            {
                SetBlinnPhongParameters(effect, material,Vector2.One * 2f, rampWorld, TargetCamera);
                BoxPrimitive.Draw(effect);
            }
        }
        
        private void DrawMovingPlatforms(Effect effect, Material material)
        {
            foreach (var movingPlatform in Prefab.MovingPlatforms)
            {
                var movingPlatformWorld = movingPlatform.World;
                SetBlinnPhongParameters(effect, material, Vector2.One * 3f, movingPlatformWorld, TargetCamera);
                BoxPrimitive.Draw(effect);
            }
        }
        
        private static void SetBlinnPhongParameters(Effect effect, Material material, Vector2 tiling, Matrix worldMatrix, 
            Camera camera)
        {
            effect.CurrentTechnique = effect.Techniques["NormalMapping"];
            effect.Parameters["World"].SetValue(worldMatrix);
            effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
            effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * camera.View * camera.Projection);
            effect.Parameters["ModelTexture"].SetValue(material.Diffuse);
            effect.Parameters["NormalTexture"].SetValue(material.Normal);
            effect.Parameters["Tiling"].SetValue(tiling);
            effect.Parameters["ambientColor"].SetValue(material.AmbientColor);
            effect.Parameters["diffuseColor"].SetValue(material.DiffuseColor);
            effect.Parameters["specularColor"].SetValue(material.SpecularColor);
            effect.Parameters["KAmbient"].SetValue(material.KAmbient);
            effect.Parameters["KDiffuse"].SetValue(material.KDiffuse);
            effect.Parameters["KSpecular"].SetValue(material.KSpecular);
            effect.Parameters["shininess"].SetValue(material.Shininess);
        }

        private void DrawGizmos()
        {
            foreach (var boundingBox in Prefab.PlatformAabb)
            {
                var center = BoundingVolumesExtensions.GetCenter(boundingBox);
                var extents = BoundingVolumesExtensions.GetExtents(boundingBox);
                Gizmos.DrawCube(center, extents * 2f, Color.Red);
            }

            foreach (var orientedBoundingBox in Prefab.RampObb)
            {
                var orientedBoundingBoxWorld = Matrix.CreateScale(orientedBoundingBox.Extents * 2f)
                                               * orientedBoundingBox.Orientation *
                                               Matrix.CreateTranslation(orientedBoundingBox.Center);
                Gizmos.DrawCube(orientedBoundingBoxWorld, Color.Red);
            }

            foreach (var movingPlatform in Prefab.MovingPlatforms)
            {
                var movingBoundingBox = movingPlatform.MovingBoundingBox;
                var center = BoundingVolumesExtensions.GetCenter(movingBoundingBox);
                var extents = BoundingVolumesExtensions.GetExtents(movingBoundingBox);
                Gizmos.DrawCube(center, extents * 2f, Color.GreenYellow);
            }
            
            Gizmos.DrawSphere(Player.BoundingSphere.Center, Player.BoundingSphere.Radius * Vector3.One, Color.Yellow);
        }

        private void DrawCollectibles(List<Collectible.Collectible> collectibles, GameTime gameTime)
        {
            foreach (var collectible in collectibles)
            {
                if (!collectible.ShouldDraw) continue;
                DrawModel(collectible.World, collectible.Model, collectible.Shader, gameTime);
                var center = BoundingVolumesExtensions.GetCenter(collectible.BoundingBox);
                var extents = BoundingVolumesExtensions.GetExtents(collectible.BoundingBox);
                Gizmos.DrawCube(center, extents * 2f, Color.Red);
            }
        }
        
        private void DrawModel(Matrix world, Model model, Effect effect, GameTime gameTime){
            effect.Parameters["View"].SetValue(TargetCamera.View);
            effect.Parameters["Projection"].SetValue(TargetCamera.Projection);
            effect.Parameters["DiffuseColor"]?.SetValue(Color.Yellow.ToVector3());
            effect.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);

            foreach (var mesh in model.Meshes)
            {
                var meshMatrix = mesh.ParentBone.Transform;
                effect.Parameters["World"].SetValue(meshMatrix * world);
                mesh.Draw();
            }
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }

        public static void loadEffectOnMesh(Model model,Effect effect)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect;
                }
            }
        }
    }
}