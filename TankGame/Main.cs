﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Tools;
using System.Threading;
using System;

namespace TankGame
{
    public class Main : Game
    {
        //Create GameState which will handle what is showing on the screen - AKA the state of the game
        GameState gameState = new GameState();
        private static Matrix defualtMatrix;
        private static Viewport defualtView;

        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphicsManager;


        //private FrameCounter framC = new FrameCounter();
        //SpriteFont spritefont;

        //Allows other classes to load code from content manager - Convient
        private static ContentManager content;
        public static ContentManager GameContent
        {
            get { return content; }
            set { content = value; }
        }
        //GET SET for GameWindow
        private static GameWindow window;
        public static GameWindow gameWindow
        {
            get { return window; }
            set { window = value; }
        }
        //GET SET for GraphicsDevice
        private static GraphicsDevice graphics;
        public static GraphicsDevice graphicsDevice
        {
            get { return graphics; }
            set { graphics = value; }
        }
        //GET SET for GraphicsAdapter
        private static GraphicsAdapter graphicsAdaptr;
        public static GraphicsAdapter graphicsAdapter
        {
            get { return graphicsAdaptr; }
            set { graphicsAdaptr = value; }
        }
        private static GameTime gametime;
        public static GameTime gameTime
        {
            get { return gametime; }
            set { gametime = value; }
        }

        //constructor
        public Main()
        {
            Content.RootDirectory = "Content";
            //lower case ones are non static
            content = Content;
            window = Window;
            IsMouseVisible = true;
            graphicsManager = new GraphicsDeviceManager(this);
            graphicsAdaptr = new GraphicsAdapter();
        }

        protected override void Initialize()
        {
            //window size for game on start up
            graphicsManager.PreferredBackBufferWidth = 1600;
            graphicsManager.PreferredBackBufferHeight = 900;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += windowSizeChanged;
            graphicsManager.ApplyChanges();

            //defualt resolution of the game
            Camera.resolution = new Vector2(1920, 1080);
            Camera.AspectRatio = Camera.resolution.X / Camera.resolution.Y;

            //initialize my Graphics Device and SpriteBatch 
            graphics = GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            

            //get the defualt Matrix for the resolution. No special viewports
            defualtView = Main.graphicsDevice.Viewport;
            Camera.setBound(defualtView);
            defualtMatrix = Camera.getScalingMatrix(Camera.ResolutionScale.X, Camera.ResolutionScale.Y);
            //initialize the gamestate
            gameState.Initialize();
            //initialize the defualt
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gameState.LoadContent(spriteBatch, graphicsManager);
            //spritefont = GameContent.Load<SpriteFont>("myFont");

        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //runs the current game action - state of game
            if (this.IsActive)
            {
                gameState.Update();
            }
            gametime = gameTime;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //color of the game background, defualt
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //draws the game
            gameState.Draw();

            //code for frame counter
            //-------------------------------------------------------------
            //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //framC.Update(deltaTime);
            //var fps = string.Format("FPS: {0}", framC.AverageFramesPerSecond);
            //spriteBatch.Begin();
            //spriteBatch.DrawString(spritefont, fps, new Vector2(1, 1), Color.Green);
            //spriteBatch.End();
            //---------------------------------------------------------------------

            base.Draw(gameTime);
        }
        public static Matrix DefualtMatrix()
        {
            return defualtMatrix;
        }
        public static Viewport DefualtView()
        {
            return defualtView;
        }

        private void windowSizeChanged(object sender, EventArgs e)
        {
            //whenever the window is resized, recalculate the backbuffer and viewport/matrix information
            if (!graphicsManager.IsFullScreen && !gameState.exitingBorderless)
            {
                graphicsManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphicsManager.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphicsManager.ApplyChanges();
            }           
            defualtView = Main.graphicsDevice.Viewport;
            Camera.setBound(defualtView);
            defualtMatrix = Camera.getScalingMatrix(Camera.ResolutionScale.X, Camera.ResolutionScale.Y);
        }
    }
}