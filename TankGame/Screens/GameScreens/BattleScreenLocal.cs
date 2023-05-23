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
    internal class BattleScreenLocal : GameScreensManager
    {
        //UI stuff
        SpriteFont font;
        Texture2D UI_bg, UI_filler, spawnTex;
        Color bgColor, bgFillerColor;
        int R, G, B;
        Vector3 bgColorOffset;
        

        //hold the level file location
        private string file;
        //int activePlayer;
        //logic that determines what stage the game is in
        private bool placementStage, battleStarted;
        //logic for various purposes
        private bool placementWarning = false;

        InputBox tanksCount, minesCount;
        Button tanks, mines, ready, endturn;
        List<Button> placementButtonList = new List<Button>();
        List<Button> battleButtonList = new List<Button>();

        int tanksUsed = 0, minesUsed = 0;

        Player curPlayer, P1, P2;
        int curPlayerTurn = 1;
        //info for current turn state
        int AP = 4;
        int turn = 1;

        //information for start of turn state
        List<Entity> oldEntities = new List<Entity>();

        public override void Initialize()
        {
            base.Initialize();
            placementStage = true;
            battleStarted = false;
            //activePlayer = 1;
            //create input boxes for displaying tanks and mine count
            tanksCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1550, 700), new Vector2(80, 70), 0);
            minesCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1750, 700), new Vector2(80, 70), 0);

            //color stuff for background
            bgFillerColor = new Color(255, 255, 255); //filler color
            bgColor = Color.LightBlue; //current background color shade
            bgColorOffset = new Vector3(bgColor.R, bgColor.G, bgColor.B);
            //offset the filler color to match with the bg color changes applied to the bg sprites
            R = Convert.ToInt16((float)bgFillerColor.R * (float)(bgColorOffset.X / 255));
            G = Convert.ToInt16((float)bgFillerColor.G * (float)(bgColorOffset.Y / 255));
            B = Convert.ToInt16((float)bgFillerColor.B * (float)(bgColorOffset.Z / 255));
            bgFillerColor = new Color(R,G,B);


            P1 = new Player(AP , sweeps);
            P2 = new Player(AP, sweeps);
            curPlayer = P1;
        }

        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);

            //load the board (load for host before sending to peer)
            LoadBoardfromFile();
            //player spawn rows calc
            getSpawnRegion();

            //font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");
            //textures
            UI_bg = Main.GameContent.Load<Texture2D>("Backgrounds/Battlescreen/UI_BG");
            UI_filler = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            spawnTex = Main.GameContent.Load<Texture2D>("GameSprites/spawnHighLight");

            //get the starter wall locations for later use
            wallLocations = getWallLocations(entities);
            //SendLoadToPeer();

            #region load buttons and input boxes
            //load buttons
            tanks = new Button(new Vector2(1565, 640), 50, 50, "GameSprites/BattleSprites/Tank", "tanks", "toggle");
            tanks.ChangeButtonColor(Color.Black);
            mines = new Button(new Vector2(1765, 640), 50, 50, "GameSprites/BattleSprites/Mine", "mines", "toggle");
            ready = new Button(new Vector2(1590, 800), 200, 100, "Buttons/BattleScreen/Ready", "ready");
            endturn = new Button(new Vector2(1590, 800), 200, 100, "Buttons/BattleScreen/EndTurn", "endturn");

            //load inputboxes
            tanksCount.LoadContent();
            minesCount.LoadContent();
            //populate the information to show how many tanks and mines they get to place
            tanksCount.Text = Convert.ToString(TanksAndMines.X);
            minesCount.Text = Convert.ToString(TanksAndMines.Y);
            #endregion

            #region list adds
            //palcement stage buttons
            placementButtonList.Add(tanks);
            placementButtonList.Add(mines);
            placementButtonList.Add(ready);
            //battle stage buttons
            battleButtonList.Add(endturn);
            #endregion

            #region Event listeners
            tanks.ButtonClicked += AddTankPressed;
            mines.ButtonClicked += AddMinePressed;
            ready.ButtonClicked += ReadyPressed;
            #endregion
        }

        public override void Update()
        {
            base.Update();
            //set MouseInBoard to true or false, if the mouse is in the board
            getMouseInBoard();
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
                foreach (Button b in battleButtonList)
                {
                    b.Update(mouse, worldPosition);
                }
                //check if the mouse is in the board
                if (mouseInBoard)
                {
                    checkSelectedTank();
                }               
            }
        }

        public override void Draw()
        {
            //background for ui elements draw
            spriteBatch.Draw(UI_filler, new Rectangle(0, 0, 1920, 54), bgFillerColor);
            spriteBatch.Draw(UI_filler, new Rectangle(0, 1026, 1920, 54), bgFillerColor);
            spriteBatch.Draw(UI_bg, Vector2.Zero, bgColor);
            spriteBatch.Draw(UI_bg, new Vector2(1446, 0), bgColor);
            //board draw 
            curBoard.drawCheckers(spriteBatch);
            curBoard.DrawOutline(spriteBatch);
            foreach (Entity e in entities)
            {
                if (e.Type.ToString() != "mine")
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
                
                foreach (Mine mine in curPlayer.mines)
                {
                    mine.Draw(spriteBatch);
                }

                //give a warning the player pressed ready when they had tanks and mines left
                if (placementWarning)
                {
                    spriteBatch.DrawString(font, "You still have Tanks and\n    Mines left to place", new Vector2(1485, 500), Color.DarkRed);
                }
                //draw the tank spawn region highlight for visual indication of where tanks can go
                spriteBatch.Draw(spawnTex, curPlayer.spawn.Location, null, Color.LightGray, 0, Vector2.Zero, curPlayer.spawn.Size, SpriteEffects.None, 0);
            }

            //draw code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {
                foreach (Button b in battleButtonList)
                {
                    b.Draw(spriteBatch);
                }
            }

            //draw current players turn info
            spriteBatch.DrawString(font, "Current Player: " + curPlayerTurn, new Vector2(1550, 350), Color.Black);
        }

        public override void ButtonReset()
        {

        }

        #region placement Phase Code
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
        private void getSpawnRegion()
        {
            int spawnRows = Convert.ToInt32(curBoard.Rows * .1F);
            if (spawnRows == 0) { spawnRows = 1; }

            P2.spawnRows = curBoard.getSubGrid(new Vector2(0, 0), new Vector2(spawnRows, curBoard.Columns));
            P1.spawnRows = curBoard.getSubGrid(new Vector2(curBoard.Rows - spawnRows, 0), new Vector2(spawnRows, curBoard.Columns));
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
            Entity entity;
            Mine tempMine;
            Tank tempTank;
            //find out if the mouse is inside the board
            if (mouseInBoard)
            {
                //if the mouse is left clicked once inside the board (add code)
                if (mouse.LeftButton == ButtonState.Pressed && oldLeftClick != curLeftClick) 
                {                   
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    if (type == "tank")
                    {
                        tempTank = new Tank(curGrid, curGridLocation);
                        entity = tempTank;

                        //to satisfy bs of needed to be declared in some way
                        tempMine = new Mine(curGrid, curGridLocation);
                    }
                    else if (type == "mine")
                    {
                        tempMine = new Mine(curGrid, curGridLocation);
                        entity = tempMine;

                        //to satisfy bs of needed to be declared in some way
                        tempTank = new Tank(curGrid, curGridLocation);
                        //
                        //
                    }
                    else
                    {
                        entity = new Entity(curGrid, curGridLocation);
                        tempMine = new Mine(curGrid, curGridLocation);
                        tempTank = new Tank(curGrid, curGridLocation);
                    }
                    if (type == "tank" || type == "mine")
                    {
                        //check if the type of object has some count left //Tanks Check (also checks for mouse in spawn)
                        if (((type == "tank" && Convert.ToInt16(tanksCount.Text) > 0) && curPlayer.spawn.Contains(worldPosition)) 
                            //mines check
                            || (type == "mine" && Convert.ToInt16(minesCount.Text) > 0))
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
                                    tempTank.LoadContent();
                                    curPlayer.tanks.Add(new Tank(curGrid, curGridLocation));                                    
                                }
                                else if (type == "mine")
                                {
                                    minesUsed++;
                                    tempMine.LoadContent();
                                    curMines.Add(tempMine);
                                    curPlayer.mines.Add(tempMine);
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
                                        for (int j = 0; j < curPlayer.tanks.Count; j++)
                                        {
                                            //find which tank is the current one based on gridlocation to remove it
                                            if (curPlayer.tanks[j].gridLocation == curGridLocation)
                                            {
                                                curPlayer.tanks.Remove(curPlayer.tanks[j]);
                                                //remove its records
                                                entities.Remove(entities[i]);
                                                gridLocations.Remove(curGridLocation);
                                                tanksUsed--;
                                            }
                                        }                                   
                                    }
                                }
                                //if we are working with mines
                                else if (type == "mine")
                                {
                                    //if its a mine
                                    if (entities[i].Type == "mine")
                                    {
                                        
                                        for (int j = 0; j < curPlayer.mines.Count; j++)
                                        {
                                            //find which mine based on gridlocation and remove it
                                            if (curPlayer.mines[j].gridLocation == curGridLocation)
                                            {
                                                curMines.Remove(curPlayer.mines[j]);
                                                curPlayer.mines.Remove(curPlayer.mines[j]);
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
            }
        }
        #endregion

        private void ReadyPressed(object sender, EventArgs e)
        {
            //check to see if the player is actaully done placing thier stuff when they hit ready
            if (tanksCount.Text != "0" || minesCount.Text != "0")
            {
                //if they are done issue a warning (set bool to true for draw)
                placementWarning = true;
            }
            else
            {
                if (curPlayerTurn == 1)
                {
                    curPlayer = P2;
                    curPlayerTurn = 2;
                    tanksUsed = 0;
                    minesUsed = 0;
                    //remove warning for other player
                    placementWarning = false;
                }
                else if (curPlayerTurn == 2)
                {
                    placementStage = false;
                    battleStarted = true;
                    curPlayer = P1;
                    curPlayerTurn = 1;
                    //remove warning 
                    placementWarning = false;
                }
                foreach (Button b in placementButtonList)
                {
                    b.ButtonReset();
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

        #region turnTakingCode
        /// <summary> Set the old information to represent the start of the turn. </summary>
        private void SetStartofTurnState()
        {
            curPlayer.oldItems = curPlayer.Items;
            curPlayer.oldSweeps = curPlayer.sweeps;
            curPlayer.oldTanks = curPlayer.tanks;

            oldEntities = entities;
        }
        /// <summary> Get the old information and apply it to the tracked information. 
        /// This will act as an undo effect. Setting the turn back to the beginning </summary>
        private void GetStartofTurnState()
        {
            curPlayer.Items = curPlayer.oldItems;
            curPlayer.sweeps = curPlayer.oldSweeps;
            curPlayer.tanks = curPlayer.oldTanks;

            entities = oldEntities;
        }

        private void TakeTurn()
        {
            
        }

        private void checkSelectedTank()
        {
            //check each tank for the mouse inside it
            foreach (Tank tank in curPlayer.tanks)
            {
                //returns true if the mouse is inside the tank
                if (tank.curSquare.Contains(worldPosition))
                {
                    //if the mouse is clicked on the tank and the tanks isnt already active
                    if (curLeftClick == ButtonState.Pressed && !tank.Active)
                    {
                        //set all tanks to inactive
                        foreach (Tank tank2 in curPlayer.tanks)
                        {
                            tank2.Active = false;
                        }
                        //set this tank to active
                        tank.Active = true;
                        //get the circle around the selected tank
                        CircleTiles = curBoard.getRectanglesInRadius(new Vector2(tank.gridLocation.X, tank.gridLocation.Y), tank.range, wallLocations, out wallsInCircle);
                        drawCircle = true;
                    }
                }
            }
        }
        #endregion
    }
}

