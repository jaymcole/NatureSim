using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSimulation.GameObjects;
using MonoSimulation.GameObjects.Behaviors;
using MonoSimulation.Generators;
using MonoSimulation.Globals;
using MonoSimulator;
using MonoSimulator.Map;
using System;
using System.Linq;
using System.Security.Claims;
using MonoSimulation.Engine.Cameras;
using MonoSimulation.Engine.Utilities;
using MonoSimulation.Engine.Events;
using System.Diagnostics;
using MonoSimulation.Models;

namespace MonoSimulation
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        RenderTarget2D renderTarget;
        //MouseState playerMouseState;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        private BasicEffect basicEffect;
        private BasicEffect basicEffect2;
        private AlphaTestEffect alphaTestEffect;

        public static Random random = new Random();
        private SimpleFpsCounter fpsCounter;
        public static DisplayMessages messages;
        public static ObjectManager objectManager;
        public static TimeNotifier MasterTimeScheduler;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            UpdateWatch = new Stopwatch();
            DrawWatch = new Stopwatch();
        }

        public static GameMap map;
        public static Basic3dExampleCamera cam;
        public static Basic3dExampleCamera cam2;
        public static Effect basic2;

        private GameObject player;

        private SpriteFont font;
        private Stopwatch UpdateWatch;
        private Stopwatch DrawWatch;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            GlobalVariables.Initialize(Content);
            

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            
            graphics.ApplyChanges();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MasterTimeScheduler = new TimeNotifier();
            fpsCounter = new SimpleFpsCounter();
            font = GlobalVariables.Content.Load<SpriteFont>("Fonts/Font");
            messages = new DisplayMessages(25, font);
            #region Cameras
            cam = new Basic3dExampleCamera(GraphicsDevice, this.Window);
            cam.LookAtDirection = Vector3.Forward;
            cam.fieldOfViewDegrees = 100;
            cam.farClipPlane = 10000;
            cam.Position = new Vector3(0, 40, 0);
            cam.TargetPositionToLookAt = new Vector3(0, 40, 10);

            cam.Position = new Vector3(60, 146, -24);
            cam.TargetPositionToLookAt = new Vector3(60, 140, -17);

            cam.CameraUi(Basic3dExampleCamera.CAM_UI_OPTION_FPS_LAYOUT);
            cam.CameraType(Basic3dExampleCamera.CAM_TYPE_OPTION_FIXED);

            cam.CameraUi(Basic3dExampleCamera.CAM_UI_OPTION_EDIT_LAYOUT);
            cam.CameraType(Basic3dExampleCamera.CAM_TYPE_OPTION_FIXED);

            cam2 = new Basic3dExampleCamera(GraphicsDevice, this.Window);
            cam2.LookAtDirection = cam.Position;
            cam2.fieldOfViewDegrees = 100;
            cam2.Position = new Vector3(10, 10, 52);
            cam2.CameraUi(Basic3dExampleCamera.CAM_UI_OPTION_FPS_LAYOUT);
            cam2.CameraType(Basic3dExampleCamera.CAM_TYPE_OPTION_FREE);
            #endregion

            renderTarget = new RenderTarget2D(
               graphics.GraphicsDevice,
               graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
               graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
               false,
               graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
               DepthFormat.Depth24);

            basic2 = Content.Load<Effect>("Shaders/basic2");

            map = new GameMap();
            basicEffect = new BasicEffect(graphics.GraphicsDevice);

            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.1f, 0, 0); // a red light
            basicEffect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // coming along the x-axis
            basicEffect.DirectionalLight0.SpecularColor = new Vector3(1, 0, 0); // with green highlights

            basicEffect.EnableDefaultLighting();
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.LightingEnabled = false;

            basicEffect.AmbientLightColor = new Vector3(0,0,0.1f);
            basicEffect.EmissiveColor = new Vector3(0.1f, 0.1f, 0.1f);


            basicEffect2 = new BasicEffect(graphics.GraphicsDevice);
           
            //basicEffect2.DirectionalLight0.DiffuseColor = new Vector3(0.1f, 0, 0); // a red light
            //basicEffect2.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // coming along the x-axis
            //basicEffect2.DirectionalLight0.SpecularColor = new Vector3(1, 0, 0); // with green highlights

            //basicEffect2.EnableDefaultLighting();
            basicEffect2.DirectionalLight0.Enabled = false;
            basicEffect2.LightingEnabled = false;
            basicEffect2.TextureEnabled = true;
            basicEffect2.AmbientLightColor = new Vector3(0, 0, 0.1f);
            basicEffect2.EmissiveColor = new Vector3(0.1f, 0.1f, 0.1f);


            alphaTestEffect = new AlphaTestEffect(graphics.GraphicsDevice);


            objectManager = new ObjectManager();



            player = new AnimalObject(map.GetGroundTile(random.Next(0, map._tiles.GetLength(0)), random.Next(0, map._tiles.GetLength(1))));
            JModel objectModel = ModelGenerator.CreateQuad(1, 3, 20, 1);
            objectModel.texture = GlobalVariables.Man;
            player.model = objectModel;
            player.behavior = new PlayerRunner(player);
            objectManager.AddObject(player);


            for (int i = 0; i < 1; i++)
            {
                GameObject ob2 = new AnimalObject(map.GetGroundTile(random.Next(0, map._tiles.GetLength(0)), random.Next(0, map._tiles.GetLength(1))));
                JModel objectModel2 = ModelGenerator.CreateQuad(1, 3, 20, 1);
                objectModel2.texture = GlobalVariables.Man;

                ob2.model = objectModel2;
                ob2.behavior = new Wonderer(ob2);
                objectManager.AddObject(ob2);
            }

            for (int i = 0; i < 1; i++)
            {
                GameObject ob2 = new AnimalObject(map.GetGroundTile(random.Next(0, map._tiles.GetLength(0)), random.Next(0, map._tiles.GetLength(1))));
                JModel objectModel2 = ModelGenerator.CreateQuad(1, 3, 18, 1);
                objectModel2.texture = Content.Load<Texture2D>("Images/Misc/leaning_woman");

                ob2.model = objectModel2;
                ob2.behavior = new Wonderer(ob2);
                objectManager.AddObject(ob2);
            }

            for (int i = 0; i < 15; i++)
            {
                GameObject ob2 = new AnimalObject(map.GetGroundTile(random.Next(0, map._tiles.GetLength(0)), random.Next(0, map._tiles.GetLength(1))));
                JModel objectModel2 = ModelGenerator.CreateQuad(2);
                objectModel2.texture = Content.Load<Texture2D>("Images/Misc/crab");
                ob2.model3D = Content.Load<Microsoft.Xna.Framework.Graphics.Model>("Models/Animals/Crab");
                ob2.model = objectModel2;
                ob2.behavior = new Wonderer(ob2);
                if (ob2.model3D == null)
                {

                }
                objectManager.AddObject(ob2);
                if (ob2.model3D == null)
                {

                }
                

            }

            for (int i = 0; i < 35; i++)
            {
                PlantObject plant = new PlantObject(map.GetGroundTile(random.Next(0, map._tiles.GetLength(0)), random.Next(0, map._tiles.GetLength(1))),
                new Texture2D[]
                {
                    Content.Load<Texture2D>("Images/Foliage/Grass/grass_short"),
                    Content.Load<Texture2D>("Images/Foliage/Grass/grass_medium"),
                    Content.Load<Texture2D>("Images/Foliage/Grass/grass_tall"),
                    Content.Load<Texture2D>("Images/Foliage/Grass/grass_bush")
                },
                new Vector3[]
                {
                    new Vector3(1,1,1),
                    new Vector3(2,2,2),
                    new Vector3(3,3,3),
                    new Vector3(4,4,4)
                },
                GlobalVariables.random.Next(1, 6)
                );
                plant.model3D = Content.Load<Microsoft.Xna.Framework.Graphics.Model>("Models/Foliage/FOrestPack/Grass/Grass");
                plant.behavior = new PlantBehavior(plant);
                Game1.objectManager.AddObject(plant);
            }

            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private bool UpdateCamera = true;
        public static TimeSpan PreviousGameTime;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateWatch.Restart();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                UpdateCamera = !UpdateCamera;
            }

            MasterTimeScheduler.Update(gameTime);

            if (UpdateCamera)
            {
                cam.Update(gameTime);
                cam2.LookAtDirection = cam.Position;
                map.Update(gameTime);
                objectManager.Update(gameTime);
            }

            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            //    renderGround = !renderGround;
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
            //    renderWater = !renderWater;
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
            //    renderDebug = !renderDebug;
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            //    renderClouds = !renderClouds;

            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
            //    GlobalWaterLevel += 1.33f;
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad9))
            //    GlobalWaterLevel -= 1.33f;

            messages.AddDataMessage("Player Direction", player.FacingDirection.ToString());
            messages.AddDataMessage("Camera Position", $"{cam.Position}");
            messages.AddDataMessage("Camera Look Pos", $"{(cam.Forward * 10) + cam.Position}");

            messages.AddDataMessage("Player Row/Col", $"{player.Tile.row}, {player.Tile.col}");
            messages.AddDataMessage($"Total Subscribers", $"{NotifierBase.TotalSubScribers}");
            messages.AddDataMessage($"Total Subscriptions", $"{NotifierBase.TotalSubScriptions}");
            messages.AddDataMessage($"Total Time Events", $"{MasterTimeScheduler.TimeEvents.Count}");
            messages.AddDataMessage($"Total Time Nodes Avail", $"{MasterTimeScheduler.AvailableNodes.Count}");
            messages.AddDataMessage("Total Game Time (ms)", $"{gameTime.TotalGameTime.TotalMilliseconds}");
            
            messages.AddDataMessage($"Objects Sleeping", $"{objectManager.ObjectsSleeping.Count}");
            messages.AddDataMessage($"Objects Waiting (add)", $"{objectManager.ObjectsToAdd.Count}");
            messages.AddDataMessage($"Objects Waiting (rem)", $"{objectManager.ObjectsToRemoveFromActive.Count}");
            messages.AddDataMessage($"Objects Active", $"{objectManager.Objects.Count}");


            string ObjectsOnTile = "";
            foreach (GameObject go in player.Tile.ObjectsOnTile)
            {
                ObjectsOnTile += $"\n    {go.GetType()}";
                if (go.GetType().IsEquivalentTo(typeof(PlantObject))) {
                    ObjectsOnTile += $"\n        Adjacent Tiles: {(go as PlantObject).AdjacentTiles.Count}";
                    ObjectsOnTile += $"\n        Is Asleep: {(go as PlantObject).IsSleeping}";
                    ObjectsOnTile += $"\n        Is RemoveMe: {(go as PlantObject).RemoveMe}";

                }
            }
            messages.AddDataMessage($"Objects On Tile", $"{ObjectsOnTile}");



            fpsCounter.Update(gameTime);
            messages.Update();
            PreviousGameTime = gameTime.TotalGameTime;
            UpdateWatch.Stop();
            messages.AddDataMessage($"Update Time", $"{UpdateWatch.ElapsedMilliseconds}");
            messages.AddDataMessage($"  Draw Time", $"{DrawWatch.ElapsedMilliseconds}");

            base.Update(gameTime);
        }

        public static float GlobalWaterLevel = 10;
        public static bool renderGround = true;
        public static bool renderClouds = false;
        public static bool renderWater = false;
        public static bool renderDebug = false;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            fpsCounter.DrawFps(spriteBatch, font, new Vector2(15, 15), Color.Blue);
            messages.Render(spriteBatch, new Vector2(15, 100), Color.Yellow, Color.Green);
            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected  void Draw2(GameTime gameTime)
        {
            DrawWatch.Restart();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            basicEffect.World = Matrix.Identity;
            basicEffect.View = cam.View;
            basicEffect.Projection = cam.Projection;
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = false;
            basicEffect.EnableDefaultLighting();

            basicEffect2.World = Matrix.Identity;
            basicEffect2.View = cam.View;
            basicEffect2.Projection = cam.Projection;
            basicEffect2.VertexColorEnabled = false;
            basicEffect2.TextureEnabled = true;
            basicEffect2.EnableDefaultLighting();

            Game1.basic2.Parameters["World"].SetValue(Matrix.Identity);
            Game1.basic2.Parameters["View"].SetValue(Game1.cam.View);
            Game1.basic2.Parameters["Projection"].SetValue(Game1.cam.Projection);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.DepthClipEnable = false;
            rasterizerState.ScissorTestEnable = false;
            
            GraphicsDevice.RasterizerState = rasterizerState;

            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (renderGround)
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    map.RenderGroundLevel(graphics, spriteBatch);
                }

            if (renderWater)
                map.RenderWaterLevel(graphics, spriteBatch);

            objectManager.Render(basicEffect2, graphics, spriteBatch);








            #region alpha depth testing/blending - would require sorting geometry by distance from camera

            //BlendState blend = new BlendState();
            //blend.ColorWriteChannels = ColorWriteChannels.None;

            //graphics.GraphicsDevice.BlendState = blend;
            //var s1 = new DepthStencilState
            //{
            //    StencilEnable = true,
            //    StencilFunction = CompareFunction.Always,
            //    StencilPass = StencilOperation.Replace,
            //    ReferenceStencil = 1,
            //    DepthBufferEnable = true,
            //    DepthBufferWriteEnable = true,

            //};
            //graphics.GraphicsDevice.DepthStencilState = s1;
            //graphics.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0, 1);

            //if (renderGround)
            //    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        map.RenderGroundLevel(graphics, spriteBatch);
            //    }
            //objectManager.Render(basicEffect2, graphics, spriteBatch);

            ////---------------------------------------------------------------------
            //// STENCIL BUFFER 2
            ////---------------------------------------------------------------------
            //var s2 = new DepthStencilState
            //{
            //    StencilEnable = true,
            //    StencilFunction = CompareFunction.LessEqual,
            //    StencilPass = StencilOperation.Keep,
            //    ReferenceStencil = 1,
            //    DepthBufferEnable = true,
            //    DepthBufferWriteEnable = false,

            //};
            //graphics.GraphicsDevice.DepthStencilState = s2;
            //graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            //if (renderWater)
            //    map.RenderWaterLevel(graphics, spriteBatch);

            //objectManager.Render(basicEffect2, graphics, spriteBatch);

            #endregion


            //BlendState blend = new BlendState();
            //blend.ColorWriteChannels = ColorWriteChannels.All;
            //blend.AlphaBlendFunction = BlendFunction.ReverseSubtract;

            //graphics.GraphicsDevice.BlendState = blend;
            //var s1 = new DepthStencilState
            //{
            //    StencilEnable = true,
            //    StencilFunction = CompareFunction.Greater,
            //    StencilPass = StencilOperation.Replace,
            //    ReferenceStencil = 1,
            //    DepthBufferEnable = true,
            //    DepthBufferWriteEnable = true,
            //    DepthBufferFunction = CompareFunction.Less

            //};
            //graphics.GraphicsDevice.DepthStencilState = s1;
            //graphics.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0, 1);

            //if (renderGround)
            //    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        map.RenderGroundLevel(graphics, spriteBatch);
            //    }
            //objectManager.Render(basicEffect2, graphics, spriteBatch);

            //if (renderWater)
            //    map.RenderWaterLevel(graphics, spriteBatch);

            //objectManager.Render(basicEffect2, graphics, spriteBatch);









            //basicEffect.World = Matrix.Identity;
            //basicEffect2.World = Matrix.Identity;

            //graphics.GraphicsDevice.BlendState = BlendState.Opaque;

            //if (renderClouds)
            //    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        map.RenderClouds(graphics, spriteBatch);
            //    }


            //if (renderDebug)
            //    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        map.debugRender(graphics, spriteBatch);
            //    }


            //if (renderDebug)
            //{
            //    spriteBatch.Begin();
            //    foreach (EffectPass pass in basicEffect2.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        map.debugCoords(graphics, spriteBatch);
            //    }
            //    spriteBatch.End();
            //}

            spriteBatch.Begin();
            fpsCounter.DrawFps(spriteBatch, font, new Vector2(15, 15), Color.Blue);
            messages.Render(spriteBatch, new Vector2(15, 100), Color.Yellow, Color.Green);
            spriteBatch.End();



            base.Draw(gameTime);
            DrawWatch.Stop();
        }
    }
}
