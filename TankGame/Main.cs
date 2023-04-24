using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TankGame
{
    public class Main : Game
    {
        //Create GameState which will handle what is showing on the screen - AKA the state of the game
        GameState gameState = new GameState();

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
        //constructor
        public Main()
        {
            Content.RootDirectory = "Content";
            //lower case ones are non static
            content = Content;
            window = Window;
            IsMouseVisible = true;
            graphicsManager = new GraphicsDeviceManager(this);
            //window size for game on start up
            graphicsManager.PreferredBackBufferWidth = 1344;
            graphicsManager.PreferredBackBufferHeight = 756;
            graphicsManager.ApplyChanges();
        }

        protected override void Initialize()
        {
            //initialize my Graphics Device and SpriteBatch 
            graphics = GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //runs the current game action - state of game
            gameState.Update();

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

            base.Draw(gameTime); ;
        }
    }
}