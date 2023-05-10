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

        #region Declare the Screens
        private ScreenManager CurrentScreen;
        private MainMenu mainMenu;
        private LevelSelect levelSelector;
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

            loading = false;
            #region screen change events
            mainMenu.ChangeScreen += HandleScreenChanged;
            editor.ChangeScreen += HandleScreenChanged;
            levelSelector.ChangeScreen += HandleScreenChanged;
            localBattleScreen.ChangeScreen += HandleScreenChanged;
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
        //The update function for gamestates and for using functions of the current screens
        public void Update()
        {
            if (!loading)
            {
                key = Keyboard.GetState();
                if (key.IsKeyDown(Keys.F1))
                {
                    //camera.ChangeScreenSize(graphicsManager);
                }

                CurrentScreen.Update();
            }
        }
        //Draws the images and textures we use
        public void Draw()
        {
            Main.graphicsDevice.Clear(color);
            
            if (!loading)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());
                spriteBatch.Draw(bgTex, new Rectangle(0,0,Convert.ToInt16(Camera.resolution.X), Convert.ToInt16(Camera.resolution.Y)), bgColor);
                CurrentScreen.Draw();
                spriteBatch.End();
            }
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
    }
}
