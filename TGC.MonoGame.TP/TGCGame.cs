﻿using System;
using System.Collections.Generic;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Geometries;

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

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private Model Model { get; set; }
        private Effect Effect { get; set; }
        private float Rotation { get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private SpherePrimitive Sphere { get; set; }
        private Vector3 SpherePosition { get; set; }
        private float Yaw { get; set; }
        private float Pitch { get; set; }
        private float Roll { get; set; }
        private QuadPrimitive Quad { get; set; }
        private List<Matrix> _floorMatrices;
        private Camera Camera { get; set; }
        private BoxPrimitive BoxPrimitive { get; set; }
        private List<Matrix> _boxMatrices;
        private Texture2D StonesTexture { get; set; }

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
            Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 40, 200), size);
            
            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;
            View = Matrix.CreateLookAt(Vector3.UnitZ * 150, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);

            SpherePosition = new Vector3(0f, 10f, 0f);
            Sphere = new SpherePrimitive(GraphicsDevice, 10);

            /*_floorMatrices = new List<Matrix>();
            CreateFloor(new Vector3(50f, 6f, 200f), Vector3.Zero, 0f);
            CreateFloor(new Vector3(50f, 0.001f, 200f), new Vector3(300f, 0f, 0f), 0f);
            CreateFloor(new Vector3(200f, 0.001f, 50f), new Vector3(150f, 0f, -200f), 0f);
            CreateFloor(new Vector3(200f, 0.001f, 50f), new Vector3(150f, 0f, 200f), 0f);*/

            _boxMatrices = new List<Matrix>();
            
            // Platform
            
            // Side platforms
            CreateBox(new Vector3(50f, 6f, 200f), Vector3.Zero);
            CreateBox(new Vector3(50f, 6f, 200f), new Vector3(300f, 0f, 0f));
            CreateBox(new Vector3(200f, 6f, 50f), new Vector3(150f, 0f, -200f));
            CreateBox(new Vector3(200f, 6f, 50f), new Vector3(150f, 0f, 200f));
            
            // Corner platforms
            CreateBox(new Vector3(50f, 6f, 80f), new Vector3(0f, 9.5f, -185f));
            CreateBox(new Vector3(50f, 6f, 80f), new Vector3(0f, 9.5f, 185f));
            CreateBox(new Vector3(50f, 6f, 80f), new Vector3(300f, 9.5f, -185f));
            CreateBox(new Vector3(50f, 6f, 80f), new Vector3(300f, 9.5f, 185f));
            
            // Center platform
            
            CreateBox(new Vector3(50f, 6f, 100f), new Vector3(150f, 0f, 0f));
            
            // Ramp
            
            // Side ramps
            CreateBox(new Vector3(50f, 6f, 50f), new Vector3(0f, 5f, -125f), Matrix.CreateRotationX(0.2f));
            CreateBox(new Vector3(50f, 6f, 50f), new Vector3(300f, 5f, -125f), Matrix.CreateRotationX(0.2f));
            CreateBox(new Vector3(50f, 6f, 50f), new Vector3(0f, 5f, 125f), Matrix.CreateRotationX(-0.2f));
            CreateBox(new Vector3(50f, 6f, 50f), new Vector3(300f, 5f, 125f), Matrix.CreateRotationX(-0.2f));
            
            // Corner ramps
            CreateBox(new Vector3(40f, 6f, 50f), new Vector3(40f, 5f, -200f), Matrix.CreateRotationZ(-0.3f));
            CreateBox(new Vector3(40f, 6f, 50f), new Vector3(40f, 5f, 200f), Matrix.CreateRotationZ(-0.3f));
            CreateBox(new Vector3(40f, 6f, 50f), new Vector3(260f, 5f, -200f), Matrix.CreateRotationZ(0.3f));
            CreateBox(new Vector3(40f, 6f, 50f), new Vector3(260f, 5f, 200f), Matrix.CreateRotationZ(0.3f));

            base.Initialize();
        }
        
        private void CreateFloor(Vector3 scale, Vector3 position, float inclination)
        {
            var floorWorld = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(inclination) * Matrix.CreateTranslation(position);
            _floorMatrices.Add(floorWorld);
        }

        /// <summary>
        ///     Creates a box with the specified scale and position.
        /// </summary>
        /// <param name="scale">The scale of the box</param>
        /// <param name="position">The position of the box</param>
        /// <param name="rotation">The rotation of the box</param>
        private void CreateBox(Vector3 scale, Vector3 position, Matrix rotation)
        {
            var boxWorld = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
            _boxMatrices.Add(boxWorld);
        }
        
        /// <summary>
        ///     Creates a box with the specified scale and position.
        /// </summary>
        /// <param name="scale">The scale of the box</param>
        /// <param name="position">The position of the box</param>
        private void CreateBox(Vector3 scale, Vector3 position)
        {
            var boxWorld = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
            _boxMatrices.Add(boxWorld);
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            StonesTexture = Content.Load<Texture2D>(ContentFolderTextures + "stones");
            
            // Create our Quad (to draw the Floor)
            Quad = new QuadPrimitive(GraphicsDevice);
            
            // Create our box
            BoxPrimitive = new BoxPrimitive(GraphicsDevice, Vector3.One, StonesTexture);

            // Cargo el modelo del logo.
            //Model = Content.Load<Model>(ContentFolder3D + "tgc-logo/tgc-logo");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            // Asigno el efecto que cargue a cada parte del mesh.
            // Un modelo puede tener mas de 1 mesh internamente.
            /*foreach (var mesh in Model.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }*/

            base.LoadContent();
        }
        
        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.
            
            Camera.Update(gameTime);

            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //Salgo del juego.
                Exit();
            }

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            
            var time = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Yaw += time * 0.4f;
            Pitch += time * 0.8f;
            Roll += time * 0.9f;

            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
            /*Effect.Parameters["World"].SetValue(FloorWorld);
            Effect.Parameters["View"].SetValue(Camera.View);
            Effect.Parameters["Projection"].SetValue(Camera.Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.ForestGreen.ToVector3());*/

            /*foreach (var floorWorld in _floorMatrices)
            {
                // Configura la matriz de mundo del efecto con la matriz del Floor actual
                Effect.Parameters["World"].SetValue(floorWorld);
                Effect.Parameters["View"].SetValue(Camera.View);
                Effect.Parameters["Projection"].SetValue(Camera.Projection);
                Effect.Parameters["DiffuseColor"].SetValue(Color.ForestGreen.ToVector3());
                
                Quad.Draw(Effect);
            }*/
            
            foreach (var boxWorld in _boxMatrices)
            {
                // Configura la matriz de mundo del efecto con la matriz del Floor actual
                Effect.Parameters["World"].SetValue(boxWorld);
                Effect.Parameters["View"].SetValue(Camera.View);
                Effect.Parameters["Projection"].SetValue(Camera.Projection);
                Effect.Parameters["DiffuseColor"].SetValue(Color.ForestGreen.ToVector3());
                
                BoxPrimitive.Draw(Effect);
            }  
            

            DrawGeometry(Sphere, SpherePosition, -Yaw, Pitch, Roll, Effect);
        }

        /// <summary>
        ///     Draw the geometry applying a rotation and translation.
        /// </summary>
        /// <param name="geometry">The geometry to draw.</param>
        /// <param name="position">The position of the geometry.</param>
        /// <param name="yaw">Vertical axis (yaw).</param>
        /// <param name="pitch">Transverse axis (pitch).</param>
        /// <param name="roll">Longitudinal axis (roll).</param>
        /// <param name="effect">Used to set and query effects.</param>;
        private void DrawGeometry(GeometricPrimitive geometry, Vector3 position, float yaw, float pitch, float roll, Effect effect)
        {
            Effect.Parameters["World"].SetValue(Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateTranslation(position));
            Effect.Parameters["View"].SetValue(Camera.View);
            Effect.Parameters["Projection"].SetValue(Camera.Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.IndianRed.ToVector3());
            geometry.Draw(effect);
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
    }
}