using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using System.IO;
using System.Collections;
using TankGame.Tools;
using TankGame.Screens.Menus;
using TankGame.Screens;

namespace TankGame
{
    internal class GameState
    {
        //UNUSED VARIABLES 
        //Camera camera = new Camera();
        //Vector3 screenScale = Vector3.Zero;
        //SpriteFont font;
        //RasterizerState r;
        //Rectangle cutOff;

        //create the black bars where nothing is rendered
        Color color = Color.Black;
        //color of the game background
        Color bgColor = Color.CornflowerBlue;
        //defualt bg color in side the draw window
        Texture2D bgTex;

        KeyboardState key;
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphicsManager;

        //holds the state of the game, loading or not loading content
        bool loading;
        private static bool paused, justPaused;
        public static bool Paused
        {
            get { return paused; }
            set
            {
                //when the game pauses this tells the game that it just happened. This way we can load the pause screen information
                paused = value;
                if (paused)
                    justPaused = true;
            }
        }

        #region Declare the Screens
        private ScreenManager CurrentScreen;
        private PauseManager pauseScreen;
        //menus
        private MainMenu mainMenu;
        private LevelSelect levelSelector;
        //pause menus
        private PauseMenu mainPauseMenu;

        //gamescreens
        private LevelEditor editor;
        private BattleScreenLocal localBattleScreen;
        #endregion

        //Constructor
        public GameState()
        {
            #region Initialize the Screens
            mainMenu = new MainMenu();
            levelSelector = new LevelSelect();
            editor = new LevelEditor();
            localBattleScreen = new BattleScreenLocal();
            mainPauseMenu = new PauseMenu();
            #endregion
        }
        //Initialize things upon class creation
        public void Initialize()
        {
            //game is loading
            loading = true;

            //r = new RasterizerState();
            //cutOff = new Rectangle(0, 0, 1344, 756);

            if (CurrentScreen == null)
            {
                CurrentScreen = mainMenu;
            }
            CurrentScreen.Initialize();
            if (pauseScreen == null)
            {
                pauseScreen = mainPauseMenu;
            }

            loading = false;
            Paused = false;

            #region screen change events
            mainMenu.ChangeScreen += HandleScreenChanged;
            editor.ChangeScreen += HandleScreenChanged;
            levelSelector.ChangeScreen += HandleScreenChanged;
            localBattleScreen.ChangeScreen += HandleScreenChanged;

            mainPauseMenu.ChangeScreen += HandlePauseScreenChanged;
            #endregion
        }
        //Loads the Content for The gamestate
        public void LoadContent(SpriteBatch spriteBatchMain, GraphicsDeviceManager graphicsManagerMain)
        {
            //font = Main.GameContent.Load<SpriteFont>("myFont");
            //r.ScissorTestEnable = true;
            //spriteBatch.GraphicsDevice.RasterizerState = r;
            //spriteBatch.GraphicsDevice.ScissorRectangle = cutOff;

            loading = true;

            spriteBatch = spriteBatchMain;
            graphicsManager = graphicsManagerMain;

            bgTex = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");

            CurrentScreen.LoadContent(spriteBatch);

            loading = false;
        }
        private void LoadPuaseContent()
        {
            pauseScreen.LoadContent(spriteBatch);
        }
        //The update function for gamestates and for using functions of the current screens
        public void Update()
        {
            if (justPaused)
            {
                LoadPuaseContent();
                justPaused = false;
            }
            if (!loading)
            {
                CurrentScreen.Update();
            }
            if (Paused)
            {
                pauseScreen.Update();
            }
        }
        //Draws the images and textures we use
        public void Draw()
        {
            Main.graphicsDevice.Clear(color);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());
            if (!loading)
            {
                spriteBatch.Draw(bgTex, new Rectangle(0, 0, Convert.ToInt16(Camera.resolution.X), Convert.ToInt16(Camera.resolution.Y)), bgColor);
                CurrentScreen.Draw();
            }
            if (Paused && !justPaused)
            {
                pauseScreen.Draw();
            }
            spriteBatch.End();
        }
        //The Event that Changes the Screens
        public void HandleScreenChanged(object sender, EventArgs eventArgs)
        {
            //gets set to false if no screen is being switched
            //if true initialize and load next screen
            bool Load = true;
            //Next Screen is Based off the buttons Name (not garenteed to even load a new screen)
            switch (CurrentScreen.nextScreen)
            {
                case "play":
                    CurrentScreen = levelSelector;
                    break;
                case "editor":
                    CurrentScreen = editor;
                    break;
                case "back":
                    CurrentScreen = mainMenu;
                    break;
                case "select":
                    if (levelSelector.selectedFile != null && levelSelector.selectedFile != "")
                    {
                        localBattleScreen.selectedFile = levelSelector.selectedFile;
                        CurrentScreen = localBattleScreen;
                    }
                    else { Load = false; }
                    break;
                case "pause":
                    Paused = true;
                    break;
                default:
                    Load = false;
                    break;
            }

            //Loads if a new screen is activated
            if (Load)
            {
                Initialize();
                LoadContent(spriteBatch, graphicsManager);
                CurrentScreen.nextScreen = "";
            }
            //Resets the button on the screen
            CurrentScreen.ButtonReset();
        }

        public void HandlePauseScreenChanged(object sender, EventArgs eventArgs)
        {
            bool Load = true;
            switch (pauseScreen.nextScreen)
            {

                case "pause":
                    Paused = false;
                    break;
                default:
                    Load = false;
                    break;
            }
            if (Load)
            {
                Initialize();
                LoadContent(spriteBatch, graphicsManager);
                pauseScreen.nextScreen = "";
            }
            //Resets the button on the screen
            pauseScreen.ButtonReset();
        }
    }
}
