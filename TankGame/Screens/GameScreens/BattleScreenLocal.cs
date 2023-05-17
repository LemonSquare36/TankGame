using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;

namespace TankGame
{
    internal class BattleScreenLocal : ScreenManager
    {
        //hold the level file location
        private string file;
        //int activePlayer;
        private bool placementStage, battleStarted;

        InputBox tanksCount, minesCount;
        Button tanks, mines, ready;
        List<Button> placementButtonList = new List<Button>();

        int tanksUsed = 0, minesUsed = 0;
        ButtonState curRightClick, oldRightClick;

        public override void Initialize()
        {
            base.Initialize();
            placementStage = true;
            battleStarted = false;
            //activePlayer = 1;
            //create input boxes for displaying tanks and mine count
            tanksCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1550, 700), new Vector2(80, 70), 0);
            minesCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1750, 700), new Vector2(80, 70), 0);

        }

        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            //load the board (load for host before sending to peer)
            LoadBoardfromFile();

            //SendLoadToPeer();

            #region load buttons and input boxes
            //load buttons
            tanks = new Button(new Vector2(1565, 640), 50, 50, "GameSprites/BattleSprites/Tank", "tanks", "toggle");
            tanks.ChangeButtonColor(Color.Black);
            mines = new Button(new Vector2(1765, 640), 50, 50, "GameSprites/BattleSprites/Mine", "mines", "toggle");
            ready = new Button(new Vector2(1590, 800), 200, 100, "Buttons/BattleScreen/Ready", "ready", "toggle");

            //load inputboxes
            tanksCount.LoadContent();
            minesCount.LoadContent();
            //populate the information to show how many tanks and mines they get to place
            tanksCount.Text = Convert.ToString(TanksAndMines.X);
            minesCount.Text = Convert.ToString(TanksAndMines.Y);
            #endregion

            #region list adds
            placementButtonList.Add(tanks);
            placementButtonList.Add(mines);
            placementButtonList.Add(ready);
            #endregion

            #region Event listeners
            tanks.ButtonClicked += AddTankPressed;
            mines.ButtonClicked += AddMinePressed;
            #endregion
        }

        public override void Update()
        {
            base.Update();
            //update code for the placement stage. Placing tanks and mines
            if (placementStage)
            {
                //update the text to show how many tanks are left
                tanksCount.Text = Convert.ToString(TanksAndMines.X - tanksUsed);
                minesCount.Text = Convert.ToString(TanksAndMines.Y - minesUsed);

                //buttons in the placement stage
                foreach (Button b in placementButtonList)
                {
                    b.Update(mouse, worldPosition);
                }
                //code for placing and picking up tanks
                Placement();
            }
            //update code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {

            }
        }

        public override void Draw()
        {
            //board draw 
            curBoard.drawCheckers(spriteBatch);
            curBoard.DrawOutline(spriteBatch);
            foreach (Entity e in entities)
            {
                e.Draw(spriteBatch);
            }
            //draw code for the placement stage. Placing tanks and mines
            if (placementStage)
            {
                minesCount.Draw(spriteBatch);
                tanksCount.Draw(spriteBatch);
                //buttons in the placement stage
                foreach (Button b in placementButtonList)
                {
                    b.Draw(spriteBatch);
                }
            }
            //draw code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {

            }
        }

        public override void ButtonReset()
        {

        }
       
        private void Placement()
        {
            if (tanks.Texture == tanks.Pressed)
            {
                AddEntity("tank");
            }
            else if (mines.Texture == mines.Pressed)
            {
                AddEntity("mine");
            }
        }
        #region Add objects code
        private void AddTankPressed(object sender, EventArgs e)
        {
            if (mines.Texture == mines.Pressed)
            {
                mines.toggleTexture();
            }
        }
        private void AddMinePressed(object sender, EventArgs e)
        {
            if (tanks.Texture == tanks.Pressed)
            {
                tanks.toggleTexture();
            }
        }
        private void AddEntity(string type)
        {
            //track old click to see when the mouse state changes from unpressed to pressed. Prevent holding mouse
            oldClick = curClick;
            curClick = mouse.LeftButton;
            oldRightClick = curRightClick;
            curRightClick = mouse.RightButton;

            Entity entity;
            //find out if the mouse is inside the board
            if (new RectangleF(curBoard.getInnerRectangle().Location, curBoard.getInnerRectangle().Size).Contains(worldPosition))
            {
                //if the mouse is left clicked once inside the board (add code)
                if (mouse.LeftButton == ButtonState.Pressed && oldClick != curClick) 
                {                   
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    if (type == "tank")
                    {
                        entity = new Tank(curGrid, curGridLocation);
                    }
                    else if (type == "mine")
                    {
                        entity = new Mine(curGrid, curGridLocation);
                    }
                    else
                    {
                        entity = new Entity(curGrid, curGridLocation);
                    }
                    if (type != "tank" || type != "mine")
                    {
                        //check if the type of object has some count left
                        if ((type == "tank" && Convert.ToInt16(tanksCount.Text) > 0) || (type == "mine" && Convert.ToInt16(minesCount.Text) > 0))
                        {
                            //if the grid has nothing there then add the object
                            if (!gridLocations.Contains(curGridLocation))
                            {
                                entity.LoadContent();
                                entities.Add(entity);
                                gridLocations.Add(entity.gridLocation);
                                //remove one from the count
                                if (type == "tank")
                                {
                                    tanksUsed++;
                                }
                                else if (type == "mine")
                                {
                                   minesUsed++;
                                }
                            }
                        }                      
                    }
                }
                //if mouse is right clicked once in the board (remove code)
                else if (mouse.RightButton == ButtonState.Pressed && oldRightClick != curRightClick)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    //of the gridlocations tracker has an object there
                    if (gridLocations.Contains(curGridLocation))
                    {
                        //check which entity is there 
                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (entities[i].gridLocation == curGridLocation)
                            {
                                //if we are working with tanks
                                if (type == "tank")
                                {
                                    //if its a tank
                                    if (entities[i].Type == "tank")
                                    {
                                        //remove its records
                                        entities.Remove(entities[i]);
                                        gridLocations.Remove(curGridLocation);
                                        tanksUsed--;
                                    }
                                }
                                //if we are working with mines
                                else if (type == "mine")
                                {
                                    //if its a mine
                                    if (entities[i].Type == "mine")
                                    {
                                        //remove its records
                                        entities.Remove(entities[i]);
                                        gridLocations.Remove(curGridLocation);
                                        minesUsed--;
                                    }
                                }
                            }
                        }
                    }                   
                }
            }
        }
        #endregion

        private void LoadBoardfromFile()
        {
            //load the board and additional data from the file passed in levelselect.
            file = relativePath + "\\TankGame\\" + selectedFile + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file, 0.2468F, 0.05F);
                    //grab the informatin from the levelManager
                    entities = levelManager.getEntities();
                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();
                    sweeps = levelManager.getSweeps();
                    //finish loading the board
                    curBoard.LoadContent();
                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].LoadContent();
                        gridLocations.Add(entities[i].gridLocation);
                    }                    
                    RowsCol = curBoard.Rows;
                }
                catch {  }
            }
            else { }
        }
    }
}
