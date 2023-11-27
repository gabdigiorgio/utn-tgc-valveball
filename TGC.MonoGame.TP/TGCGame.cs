using System;
using System.Collections.Generic;
using System.Linq;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TGC.MonoGame.TP.Audio;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Collectible;
using TGC.MonoGame.TP.Geometries;
using TGC.MonoGame.TP.Material;
using TGC.MonoGame.TP.Menu;
using TGC.MonoGame.TP.Player;
using TGC.MonoGame.TP.Prefab;
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
        private SpriteFont _font;
        
        // GUI
        private bool _isMenuOpen;
        private MenuState _menuState = MenuState.Resume;
        private TimeSpan _gameTimer = TimeSpan.Zero;
        
        // Skybox
        private SkyBox SkyBox { get; set; }
        
        // Camera
        private Camera Camera { get; set; }
        public static TargetCamera TargetCamera { get; set; }
        private MainMenuCamera MainMenuCamera { get; set; }
        private BoundingFrustum BoundingFrustum { get; set; }
        private Vector3 MainMenuCameraTarget { get; } =  new(0f, 200f, 0f);
        private bool _inMainMenu;
        public static float CameraFarPlaneDistance { get; set; } = 10000f;
        public static float CameraNearPlaneDistance { get; set; } = 1f;
        
        // Light
        private TargetCamera TargetLightCamera { get; set; }
        private Vector3 LightPosition { get; } = new(300f, 250f, 390f);
        private float LightCameraFarPlaneDistance { get; set; } = 3000f;
        private float LightCameraNearPlaneDistance { get; set; } = 5f;
        
        // ShadowMap
        private RenderTarget2D ShadowMapRenderTarget { get; set; }
        private const int ShadowmapSize = 4096;

        // Scene
        private Matrix SphereWorld { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        
        // Geometries
        private BoxPrimitive BoxPrimitive { get; set; }
        public static CylinderPrimitive CylinderPrimitive { get; private set; }
        
        // Sphere position & rotation
        public static readonly Vector3 InitialSpherePosition = new(1100, 250f, 0f);//new(100, 690f, 0f);
        public const float InitialSphereYaw = 1.57f;
        private readonly Matrix _sphereScale = Matrix.CreateScale(5f);
        private const float SphereRadius = 5f;
        private static Player.Player Player { get; set; }

        // Effects
        private Effect BlinnPhongShadows { get; set; }
        
        // EnvMap
        private RenderTargetCube EnvironmentMapRenderTarget { get; set; }
        private const int EnvironmentmapSize = 100;
        private StaticCamera CubeMapCamera { get; set; }

        // Models
        private Model SphereModel { get; set; }
        
        // Colliders
        public static Gizmos.Gizmos Gizmos { get; private set; }


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
            
            // Light
            TargetLightCamera = new TargetCamera(1f, LightPosition, Vector3.Zero);
            TargetLightCamera.BuildProjection(1f, LightCameraNearPlaneDistance, LightCameraFarPlaneDistance, 
                MathHelper.PiOver2);
            
            // Configuramos nuestras matrices de la escena.
            SphereWorld = Matrix.Identity;
            View = Matrix.CreateLookAt(Vector3.UnitZ * 150, Vector3.Zero, Vector3.Up);
            
            TargetCamera.Projection = 
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 
                    CameraNearPlaneDistance, CameraFarPlaneDistance);
            
            BoundingFrustum = new BoundingFrustum(TargetCamera.View * TargetCamera.Projection);
            
            // Player
            Player = new Player.Player(_sphereScale, InitialSpherePosition, new BoundingSphere(InitialSpherePosition, SphereRadius), InitialSphereYaw);
            
            // EnvMap camera
            CubeMapCamera = new StaticCamera(1f, Player.SpherePosition, Vector3.UnitX, Vector3.Up);
            CubeMapCamera.BuildProjection(1f, 1f, 3000f, MathHelper.PiOver2);
            
            // MainMenu
            _inMainMenu = true;
            MainMenuCamera = new MainMenuCamera(TargetCamera);
            
            // Gizmos
            Gizmos = new Gizmos.Gizmos
            {
                Enabled = false
            };
            
            // Collectibles
            CollectibleManager.CreatePowerUpsSquareCircuit(0, 0, 0);
            CollectibleManager.CreatePowerUpsSquareCircuit(-600, 0, 0);
            CollectibleManager.CreateCoinsSquareCircuit(0, 0, 0);
            CollectibleManager.CreateCoinsSquareCircuit(-600, 0, 0);
            
            // Checkpoints
            CollectibleManager.CreateCheckpoints();

            // Map
            PrefabManager.CreateInitialCircuit(new Vector3(400f, 100f, 0f));
            PrefabManager.CreateSquareCircuit(Vector3.Zero);
            PrefabManager.CreateSquareCircuit(new Vector3(-600, 0f, 0f));
            PrefabManager.CreateBridge();
            PrefabManager.CreateSwitchbackRamp();
            
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
            _font = Content.Load<SpriteFont>(ContentFolderSpriteFonts + "CascadiaCode/CascadiaCodePL");
            
            AudioManager.LoadSounds(Content);
            AudioManager.PlayBackgroundMusic(0.1f, true);
            
            Material.Material.LoadMaterials(Content);
            
            // Platform
            BoxPrimitive = new BoxPrimitive(GraphicsDevice, Vector3.One, Material.Material.Default.Diffuse);
            
            // Checkpoint
            CylinderPrimitive = new CylinderPrimitive(GraphicsDevice, 10f, 20f);
            
            CollectibleManager.LoadCollectibles(Content);
            
            LoadSphere();

            LoadSkyBox();

            LoadShadowMap();
            
            Gizmos.LoadContent(GraphicsDevice, Content);

            base.LoadContent();
        }

        private void LoadShadowMap()
        {
            BlinnPhongShadows = Content.Load<Effect>(ContentFolderEffects + "BlinnPhongShadows");
            CreateShadowMapRenderTarget();
            CreateEnvironmentMapRenderTarget();
        }

        private void CreateShadowMapRenderTarget()
        {
            ShadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, ShadowmapSize, ShadowmapSize, false,
                SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
        }
        
        private void CreateEnvironmentMapRenderTarget()
        {
            EnvironmentMapRenderTarget = new RenderTargetCube(GraphicsDevice, EnvironmentmapSize, false,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            GraphicsDevice.BlendState = BlendState.Opaque;
        }

        private void LoadSkyBox()
        {
            var skyBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skyboxes/day_skybox_02");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 2000);
        }

        private void LoadSphere()
        {
            SphereModel = Content.Load<Model>(ContentFolder3D + "geometries/sphere");
            SphereWorld = _sphereScale * Matrix.CreateTranslation(InitialSpherePosition);
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
            
            UpdateMenuSelection(keyboardState);

            if (!_isMenuOpen)
            {
                if (_inMainMenu)
                {
                    MainMenuCamera.Update(MainMenuCameraTarget);

                    if (keyboardState.IsKeyDown(Keys.Enter) || keyboardState.IsKeyDown(Keys.Space))
                    {
                        _inMainMenu = false;
                    }
                }
                else
                {
                    if (keyboardState.IsKeyDown(Keys.Escape) && !_isMenuOpen)
                    {
                        AudioManager.PauseBackgroundMusic();
                        AudioManager.OpenMenuSound.Play();
                        _isMenuOpen = true;
                    }
                    SphereWorld = Player.Update(time, keyboardState);
                    TargetCamera.Update(Player.SpherePosition, Player.Yaw, mouseState, gameTime, Player.Speed, GraphicsDevice);
                    _gameTimer += gameTime.ElapsedGameTime;
                }
                
                TargetLightCamera.Position = LightPosition;
                TargetLightCamera.BuildView();
                
                // Update the view projection matrix of the bounding frustum
                BoundingFrustum.Matrix = TargetCamera.View * TargetCamera.Projection;

                CubeMapCamera.Position = Player.SpherePosition;

                PrefabManager.UpdatePrefabs(gameTime, Player);

                UpdateCollectibles(gameTime);

                Gizmos.UpdateViewProjection(TargetCamera.View, TargetCamera.Projection);
                
                AudioManager.ResumeBackgroundMusic();

                HandleGizmos(keyboardState);
            }
            base.Update(gameTime);
        }

        private bool _wasKeyPressed;

        private void UpdateMenuSelection(KeyboardState keyboardState)
        {
            if (!_isMenuOpen) return;

            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                if (!_wasKeyPressed && _menuState > MenuState.Resume)
                {
                    _menuState--;
                    _wasKeyPressed = true;
                    AudioManager.SelectMenuSound.Play();
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                if (!_wasKeyPressed && _menuState < MenuState.Exit)
                {
                    _menuState++;
                    _wasKeyPressed = true;
                    AudioManager.SelectMenuSound.Play();
                }
            }
            else
            {
                _wasKeyPressed = false;
            }

            if (!keyboardState.IsKeyDown(Keys.Enter) && !keyboardState.IsKeyDown(Keys.Space)) return;
            AudioManager.ClickMenuSound.Play();
            HandleMenuSelection();
        }

        private void HandleMenuSelection()
        {
            switch (_menuState)
            {
                case MenuState.Resume:
                    _isMenuOpen = false;
                    break;
                case MenuState.StopMusic:
                    MediaPlayer.Stop();
                    break;
                case MenuState.Exit:
                    Exit();
                    break;
            }
        }

        private static void UpdateCollectibles(GameTime gameTime)
        {
            foreach (var collectible in CollectibleManager.Collectibles)
            {
                collectible.Update(gameTime, Player);
            }
        }

        private void HandleGizmos(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.G) && !Gizmos.Enabled)
            {
                Gizmos.Enabled = true;
            }
            else if (keyboardState.IsKeyUp(Keys.G) && Gizmos.Enabled)
            {
                Gizmos.Enabled = false;
            }
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            DrawWithShadows();
            
            DrawGizmos();
            
            Gizmos.Draw();
            
            DrawSkybox(TargetCamera);
            
            DrawCollectibles(CollectibleManager.Collectibles, gameTime);

            const int menuHeight = 60;
            var center = GraphicsDevice.Viewport.Bounds.Center.ToVector2();
            if (_isMenuOpen)
            {
                DrawMenu(center, menuHeight);
            }

            if (_inMainMenu)
            {
                DrawMainMenu();
            }
            else
            {
                DrawGui();  
            }

            base.Draw(gameTime);
        }

        private void DrawWithShadows()
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            for (var face = CubeMapFace.PositiveX; face <= CubeMapFace.NegativeZ; face++)
            { 
                GraphicsDevice.SetRenderTarget(EnvironmentMapRenderTarget, face);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
                SetCubemapCameraForOrientation(face);
                CubeMapCamera.BuildView();
                DrawSkybox(CubeMapCamera);
                DrawPrefabs(PrefabManager.Prefabs, CubeMapCamera);
            }
            
            #region Pass 1
            
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.SetRenderTarget(ShadowMapRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);

            BlinnPhongShadows.CurrentTechnique = BlinnPhongShadows.Techniques["DepthPass"];
            
            DrawModelShadows(SphereWorld, SphereModel);

            DrawPrefabs(PrefabManager.Prefabs, TargetLightCamera);

            #endregion

            #region Pass 2
            
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

            BlinnPhongShadows.CurrentTechnique = BlinnPhongShadows.Techniques["DrawBlinnPhongShadowed"];
            SetShadowParameters();
            
            DrawPrefabs(PrefabManager.Prefabs, BlinnPhongShadows);
            
            DrawModel(SphereWorld, BlinnPhongShadows, SphereModel, Player.CurrentSphereMaterial.Material);
            
            #endregion
            
        }
        
        private void DrawSkybox(Camera camera)
        {
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Graphics.GraphicsDevice.RasterizerState = rasterizerState;

            SkyBox.Draw(camera.View, camera.Projection, camera.Position);
            GraphicsDevice.RasterizerState = originalRasterizerState;
        }
        
        private void DrawMainMenu()
        {
            SpriteBatch.Begin();

            const string titleText = "ValveBall";
            var titleSize = _font.MeasureString(titleText);
            const float titleScale = 1.5f;
            
            var titlePosition = new Vector2((GraphicsDevice.Viewport.Width - titleSize.X * titleScale) / 2,
                (GraphicsDevice.Viewport.Height - titleSize.Y * titleScale) / 2 - 50);

            SpriteBatch.DrawString(_font, titleText, titlePosition + new Vector2(2, 2), Color.Black, 0f, Vector2.Zero,
                titleScale, SpriteEffects.None, 0f);
            
            SpriteBatch.DrawString(_font, titleText, titlePosition, Color.IndianRed, 0f, Vector2.Zero,
                titleScale, SpriteEffects.None, 0f);

            const string pressStartText = "<Press Start>";
            var pressStartSize = _font.MeasureString(pressStartText);
            
            var pressStartPosition = new Vector2((GraphicsDevice.Viewport.Width - pressStartSize.X) / 2,
                (GraphicsDevice.Viewport.Height - pressStartSize.Y) / 2);
            
            SpriteBatch.DrawString(_font, pressStartText, pressStartPosition, Color.White);

            SpriteBatch.End();
        }


        
        private void DrawMenu(Vector2 center, int menuHeight)
        {
            SpriteBatch.Begin();
            
            var position = center - new Vector2(30, menuHeight / 2f);
            SpriteBatch.DrawString(_font, "Resume", position, _menuState == MenuState.Resume ? Color.Yellow : Color.White);
            position.Y += 30;
            
            SpriteBatch.DrawString(_font, "Stop Music", position, _menuState == MenuState.StopMusic ? Color.Yellow : Color.White);
            position.Y += 30;
            
            SpriteBatch.DrawString(_font, "Exit", position, _menuState == MenuState.Exit ? Color.Yellow : Color.White);
            
            SpriteBatch.End();
        }
        
        private void DrawGui()
        {
            SpriteBatch.Begin();

            var timerText = _gameTimer.ToString(@"mm\:ss");
            var timerSize = _font.MeasureString(timerText);
            var timerPosition = new Vector2((GraphicsDevice.Viewport.Width - timerSize.X) / 2, 10);
            SpriteBatch.DrawString(_font, timerText, timerPosition, Color.White);

            SpriteBatch.DrawString(_font, "Score:" + Player.Score, new Vector2(10, 10), Color.White);

            var sphereNames = new List<string> { "MarbleSphere", "RubberSphere", "MetalSphere" };
            for (var i = 0; i < sphereNames.Count; i++)
            {
                var spherePosition = new Vector2(GraphicsDevice.Viewport.Width - 220, 10 + i * 20);
                SpriteBatch.DrawString(_font, (i + 1) + ": " + sphereNames[i], spherePosition, Color.White);
            }

            const string zoomMessage = "Zoom: Use mouse wheel";
            var zoomMessageSize = _font.MeasureString(zoomMessage);
            var zoomMessagePosition = new Vector2(GraphicsDevice.Viewport.Width - zoomMessageSize.X - 10,
                GraphicsDevice.Viewport.Height - zoomMessageSize.Y - 10);
            SpriteBatch.DrawString(_font, zoomMessage, zoomMessagePosition, Color.White);
            
            const string restartMessage = "Press R to restart from last checkpoint";
            var restartMessageSize = _font.MeasureString(restartMessage);
            var restartMessagePosition = new Vector2((GraphicsDevice.Viewport.Width - restartMessageSize.X) / 2,
                GraphicsDevice.Viewport.Height - restartMessageSize.Y - 30);
            SpriteBatch.DrawString(_font, restartMessage, restartMessagePosition, Color.White);

            SpriteBatch.End();
        }

        private void DrawModel(Matrix worldMatrix, Effect effect, Model model, Material.Material material){
            
            if (Player.CurrentSphereMaterial == SphereMaterial.SphereMetal)
            {
                SetEnvMapParameters();
            }
            SetEffectParameters(effect, material, material.Tiling, worldMatrix, TargetCamera);
            foreach (var mesh in model.Meshes)
            {   
                mesh.Draw();
            }
        }
        
        private void DrawModelShadows(Matrix worldMatrix, Model model)
        {
            var modelMeshesBaseTransforms = new Matrix[SphereModel.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);
            foreach (var modelMesh in SphereModel.Meshes)
            {
                foreach (var part in modelMesh.MeshParts)
                    part.Effect = BlinnPhongShadows;

                BlinnPhongShadows.Parameters["WorldViewProjection"]
                    .SetValue(worldMatrix * TargetLightCamera.View * TargetLightCamera.Projection);
                modelMesh.Draw();
            }
        }

        private void DrawPrefabs(IEnumerable<Prefab.Prefab> prefabs, Effect effect)
        {
            foreach (var prefab in prefabs.Where(prefab => prefab.Intersects(BoundingFrustum)))
            {
                SetEffectParameters(effect, prefab.Material, prefab.Tiling, prefab.World, TargetCamera);
                BoxPrimitive.Draw(effect);
            }
        }

        private void DrawPrefabs(IEnumerable<Prefab.Prefab> prefabs, Camera camera)
        {
            foreach (var prefabWorld in from prefab in prefabs where prefab.Intersects(BoundingFrustum) select prefab.World)
            {
                BlinnPhongShadows.Parameters["WorldViewProjection"].SetValue(prefabWorld * camera.View * camera.Projection);
                BoxPrimitive.Draw(BlinnPhongShadows);
            }
        }
        
        public static void SetEffectParameters(Effect effect, Material.Material material, Vector2 tiling, Matrix worldMatrix, 
            Camera camera)
        {
            effect.Parameters["eyePosition"].SetValue(camera.Position);
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
        
        private void SetShadowParameters()
        {
            BlinnPhongShadows.Parameters["shadowMap"].SetValue(ShadowMapRenderTarget);
            BlinnPhongShadows.Parameters["lightPosition"].SetValue(LightPosition);
            BlinnPhongShadows.Parameters["shadowMapSize"].SetValue(Vector2.One * ShadowmapSize);
            BlinnPhongShadows.Parameters["LightViewProjection"].SetValue(TargetLightCamera.View * TargetLightCamera.Projection);
        }
        
        private void SetEnvMapParameters()
        {
            BlinnPhongShadows.CurrentTechnique = BlinnPhongShadows.Techniques["EnvironmentMapSphere"];
            BlinnPhongShadows.Parameters["environmentMap"].SetValue(EnvironmentMapRenderTarget);
        }

        private void DrawGizmos()
        {
            foreach (var prefab in PrefabManager.Prefabs)
            {
                prefab.DrawGizmos();
            }
            
            Gizmos.DrawSphere(Player.BoundingSphere.Center, Player.BoundingSphere.Radius * Vector3.One, Color.Yellow);
            
            //Gizmos.DrawFrustum(TargetCamera.View * TargetCamera.Projection, Color.Yellow);
        }

        private void DrawCollectibles(IEnumerable<Collectible.Collectible> collectibles, GameTime gameTime)
        {
            foreach (var collectible in collectibles)
            {
                if (!collectible.ShouldDraw) continue;
                collectible.Draw(gameTime, TargetCamera, GraphicsDevice);
            }
        }
        
        private void DrawModel(Matrix world, Model model, Effect effect, GameTime gameTime){
            effect.Parameters["View"].SetValue(TargetCamera.View);
            effect.Parameters["Projection"].SetValue(TargetCamera.Projection);
            effect.Parameters["DiffuseColor"]?.SetValue(Color.Yellow.ToVector3());
            effect.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);

            foreach (var mesh in model.Meshes)
            {
                var meshMatrix = mesh.ParentBone.Transform;
                effect.Parameters["World"].SetValue(meshMatrix * world);
                mesh.Draw();
            }
        }
        
        private void SetCubemapCameraForOrientation(CubeMapFace face)
        {
            switch (face)
            {
                default:
                case CubeMapFace.PositiveX:
                    CubeMapCamera.FrontDirection = -Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeX:
                    CubeMapCamera.FrontDirection = Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.PositiveY:
                    CubeMapCamera.FrontDirection = Vector3.Down;
                    CubeMapCamera.UpDirection = Vector3.UnitZ;
                    break;

                case CubeMapFace.NegativeY:
                    CubeMapCamera.FrontDirection = Vector3.Up;
                    CubeMapCamera.UpDirection = -Vector3.UnitZ;
                    break;

                case CubeMapFace.PositiveZ:
                    CubeMapCamera.FrontDirection = -Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeZ:
                    CubeMapCamera.FrontDirection = Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;
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