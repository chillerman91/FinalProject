using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Amulet_of_Ouroboros.Texts;
using TiledLib;

namespace Amulet_of_Ouroboros.Screens
{
    class Gameplay : Screen
    {
        Background background;
        HUD hud;
        Map[] map;
        Player player;
        Button leftButton;
        Button rightButton;
        Button upButton;
        Button downButton;

        Button attackButton;
        Button magicButton;
        Button potionButton;
        Minion minion;
        Boss boss;
        bool inBattle;
        bool bossBattle;
        #region Pregame
        int charAmt;
        char[] enteredName;
        bool playerCreated;
        KeyboardState currentState;
        char keydown;
        Texture2D textenterbox;
        Text enterplayernameText;
        bool nameComplete;
        Text playername;
        Button confirmnameButton;
        Text chooseclasstext;
        Button paladinButton;
        Button casterButton;
        Button rogueButton;
        bool classComplete;
        Texture2D[] storytexts;
        int storyCount;
        Button storyContinueButton;
        #endregion

        TileLayer tileLayer;
        TileLayer copyTileLayer;
        Tile blankTile;
        Tile walkableTile;
        Tile enemykilledTile;
        Tile enemyTile;
        Tile exitTile;
        int mapNumber;

        public Gameplay(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
            : base(game, batch, changeScreen, graphics)
        {

        }

        protected override void SetupInputs()
        {

        }

        public override void Activate()
        {

        }
        private void UnloadMap()
        {
        }
        protected override void LoadScreenContent(ContentManager content)
        {
            background = new Background(content, "Images/GameplayBackground");
            map = new Map[7];
            map[0] = content.Load<Map>("Maps/map01");
            map[1] = content.Load<Map>("Maps/map02");
            map[2] = content.Load<Map>("Maps/map03");
            map[3] = content.Load<Map>("Maps/map04");
            map[4] = content.Load<Map>("Maps/map05");
            map[5] = content.Load<Map>("Maps/map06");
            map[6] = content.Load<Map>("Maps/map07");
            player = new Player(content);
            player.TilePosition = new Vector2(1, 17);
            hud = new HUD(content, player);
            mapNumber = 0;
            leftButton = new Button(content, "", new Vector2(800, 650), Color.White, Color.White, "Images/LeftButton");
            rightButton = new Button(content, "", new Vector2(900, 650), Color.White, Color.White, "Images/RightButton");
            upButton = new Button(content, "", new Vector2(850, 595), Color.White, Color.White, "Images/UpButton");
            downButton = new Button(content, "", new Vector2(850, 705), Color.White, Color.White, "Images/DownButton");

            copyTileLayer = map[mapNumber].GetLayer("Layer") as TileLayer;
            tileLayer = copyTileLayer;
            blankTile = tileLayer.Tiles[19, 0];
            walkableTile = tileLayer.Tiles[19, 1];
            enemyTile = tileLayer.Tiles[19, 2];
            exitTile = tileLayer.Tiles[19, 3];
            enemykilledTile = tileLayer.Tiles[19, 4];

            for (int y = 0; y < tileLayer.Height; y++)
            {
                for (int x = 0; x < 19; x++)
                {
                    if (tileLayer.Tiles[x, y] == enemykilledTile)
                    {
                        tileLayer.Tiles[x, y] = enemyTile;
                    }
                }
            }
            attackButton = new Button(content, "Attack", new Vector2(ScreenWidth / 2 + 220, 250), Color.Blue, Color.White);
            magicButton = new Button(content, "Magic", new Vector2(ScreenWidth / 2 + 220, 325), Color.Blue, Color.White);
            potionButton = new Button(content, "Potion", new Vector2(ScreenWidth / 2 + 220, 400), Color.Blue, Color.White);
            minion = new Minion(content);
            boss = new Boss(content);
            inBattle = false;
            bossBattle = false;

            #region Pregame
            playerCreated = false;
            keydown = (char)0;
            enterplayernameText = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Enter Characters Name:", new Vector2(350, 200));
            confirmnameButton = new Button(content, "Enter", new Vector2(425, 550), Color.Blue, Color.White);
            textenterbox = content.Load<Texture2D>("Images/TextInputBar");
            playername = new Text(content.Load<SpriteFont>("Fonts/buttonFont"),"", new Vector2(375, 400), Color.Blue);
            charAmt = 0;
            nameComplete = false;
            enteredName = new char[8];
            for (int i = 0; i < enteredName.Length; i++)
            {
                enteredName[i] = ' ';
            }
            chooseclasstext = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Choose Characters Class:", new Vector2(340, 200));
            paladinButton = new Button(content, "Paladin", new Vector2(425, 400), Color.Blue, Color.White);
            casterButton = new Button(content, "Caster", new Vector2(425, 500), Color.Blue, Color.White);
            rogueButton = new Button(content, "Rogue", new Vector2(425, 600), Color.Blue, Color.White);
            storyContinueButton = new Button(content, "Continue", new Vector2(800, 700), Color.Blue, Color.White);
            classComplete = false;
            storytexts = new Texture2D[3];
            storyCount = 0;
            storytexts[0] = content.Load<Texture2D>("Images/Storytext1");
            storytexts[1] = content.Load<Texture2D>("Images/Storytext2");
            storytexts[2] = content.Load<Texture2D>("Images/Storytext3");
            #endregion
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            if (player.CurrentHP < 1)
            {
                changeScreenDelegate(ScreenState.GameOver);
                this.LoadScreenContent(content);
            }

            if (playerCreated && !inBattle)
            {
                GameplayTurnCheck();
            }
            else if (inBattle)
            {
                if (!bossBattle)
                {
                    hud.Update(player);
                    minion.Update(gameTime);

                    if (input.CheckMouseRelease(attackButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed && player.TurnBattle && !minion.dead)
                    {
                        minion.HP -= random.Next(0, player.DAM);
                        player.TurnBattle = false;
                    }

                    if (input.CheckMouseRelease(magicButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed && player.TurnBattle && !minion.dead && player.CurrentMP >= player.MAGCOS)
                    {
                        minion.HP -= random.Next(0, player.MAGDAM);
                        player.CurrentMP -= player.MAGCOS;
                        player.TurnBattle = false;
                    }

                    if (input.CheckMouseRelease(potionButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed && player.TurnBattle && !minion.dead && player.Potions > 0)
                    {
                        player.CurrentHP = player.MaxHP;
                        player.TurnBattle = false;
                        player.Potions--;
                    }

                    if (!player.TurnBattle)
                    {
                        player.CurrentHP -= random.Next(0, minion.DAM);
                        player.TurnBattle = true;
                    }

                    if (minion.HP <= 0)
                    {
                        minion.dead = true;
                        player.CurrentXP += minion.XP;
                        inBattle = false;
                        minion.Reset();
                    }
                }
                else
                {
                    hud.Update(player);
                    boss.Update(gameTime);

                    if (input.CheckMouseRelease(attackButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed && player.TurnBattle && !boss.dead)
                    {
                        boss.HP -= random.Next(0, player.DAM);
                        player.TurnBattle = false;
                    }

                    if (input.CheckMouseRelease(magicButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed && player.TurnBattle && !boss.dead && player.CurrentMP >= player.MAGCOS)
                    {
                        boss.HP -= random.Next(0, player.MAGDAM);
                        player.CurrentMP -= player.MAGCOS;
                        player.TurnBattle = false;
                    }

                    if (input.CheckMouseRelease(potionButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed && player.TurnBattle && !boss.dead && player.Potions > 0)
                    {
                        player.CurrentHP = player.MaxHP;
                        player.TurnBattle = false;
                        player.Potions--;
                    }

                    if (!player.TurnBattle)
                    {
                        player.CurrentHP -= random.Next(0, boss.DAM);
                        player.TurnBattle = true;
                    }

                    if (boss.HP <= 0)
                    {
                        if (mapNumber < 6)
                        {
                            player.Potions += 2;
                            boss.dead = true;
                            player.Pieces++;
                            player.CurrentXP += boss.XP;
                            inBattle = false;
                            boss.Reset();
                            player.TilePosition = new Vector2(1, 17);
                            mapNumber++;
                            tileLayer = null;
                            copyTileLayer = map[mapNumber].GetLayer("Layer") as TileLayer;
                            tileLayer = copyTileLayer;
                            blankTile = tileLayer.Tiles[19, 0];
                            walkableTile = tileLayer.Tiles[19, 1];
                            enemyTile = tileLayer.Tiles[19, 2];
                            exitTile = tileLayer.Tiles[19, 3];
                            enemykilledTile = tileLayer.Tiles[19, 4];
                            bossBattle = false;
                            for (int y = 0; y < tileLayer.Height; y++)
                            {
                                for (int x = 0; x < 19; x++)
                                {
                                    if (tileLayer.Tiles[x, y] == enemykilledTile)
                                    {
                                        tileLayer.Tiles[x, y] = enemyTile;
                                    }
                                }
                            }
                        }
                        else
                        {
                            changeScreenDelegate(ScreenState.GameWin);
                            LoadScreenContent(content);
                        }
                    }
                }
            }
            else
            {
                if (!nameComplete)
                {
                    currentState = Keyboard.GetState();

                    if (currentState.IsKeyDown(Keys.Back))
                    {
                        keydown = '.';
                    }

                    if (currentState.IsKeyUp(Keys.Back) && keydown == '.' && charAmt >= 0)
                    {
                        keydown = (char)0;
                        enteredName[charAmt] = ' ';
                        if (charAmt != 0)
                            charAmt--;
                    }

                    char tempc = GetCharFromKeyboard(currentState);
                    if (charAmt < 8 && tempc != (char)0)
                    {
                        enteredName[charAmt] = tempc;
                        charAmt++;
                    }
                    enteredName[0] = char.ToUpper(enteredName[0]);
                    playername.ChangeText(new string(enteredName));

                    if (input.CheckMouseRelease(confirmnameButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        nameComplete = true;
                    }
                }
                else if (!classComplete)
                {
                    if (input.CheckMouseRelease(paladinButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        player.CreatePlayer(0, new string(enteredName).ToString());
                        classComplete = true;
                    }

                    if (input.CheckMouseRelease(casterButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        player.CreatePlayer(1, new string(enteredName).ToString());
                        classComplete = true;

                    }

                    if (input.CheckMouseRelease(rogueButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        player.CreatePlayer(2, new string(enteredName).ToString());
                        classComplete = true;
                    }
                }
                else
                {
                    if (input.CheckMouseRelease(storyContinueButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (storyCount < 2)
                        {
                            storyCount++;
                        }
                        else
                        {
                            playerCreated = true;
                        }
                    }
                }
            }
        }

        char GetCharFromKeyboard(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.A))
                keydown = 'a';
            if (state.IsKeyDown(Keys.B) )
                keydown = 'b';
            if (state.IsKeyDown(Keys.C) )
                keydown = 'c';
            if (state.IsKeyDown(Keys.D) )
                keydown = 'd';
            if (state.IsKeyDown(Keys.E) )
                keydown = 'e';
            if (state.IsKeyDown(Keys.F) )
                keydown = 'f';
            if (state.IsKeyDown(Keys.G) )
                keydown = 'g';
            if (state.IsKeyDown(Keys.H) )
                keydown = 'h'; 
            if (state.IsKeyDown(Keys.I) )
                keydown = 'i';
            if (state.IsKeyDown(Keys.J) )
                keydown = 'j';
            if (state.IsKeyDown(Keys.K) )
                keydown = 'k';
            if (state.IsKeyDown(Keys.L) )
                keydown = 'l';
            if (state.IsKeyDown(Keys.M) )
                keydown = 'm';
            if (state.IsKeyDown(Keys.N) )
                keydown = 'n';
            if (state.IsKeyDown(Keys.O) )
                keydown = 'o';
            if (state.IsKeyDown(Keys.P) )
                keydown = 'p';
            if (state.IsKeyDown(Keys.Q) )
                keydown = 'q';
            if (state.IsKeyDown(Keys.R) )
                keydown = 'r';
            if (state.IsKeyDown(Keys.S) )
                keydown = 's';
            if (state.IsKeyDown(Keys.T) )
                keydown = 't';
            if (state.IsKeyDown(Keys.U) )
                keydown = 'u';
            if (state.IsKeyDown(Keys.V) )
                keydown = 'v';
            if (state.IsKeyDown(Keys.W) )
                keydown = 'w';
            if (state.IsKeyDown(Keys.X) )
                keydown = 'x';
            if (state.IsKeyDown(Keys.Y) )
                keydown = 'y';
            if (state.IsKeyDown(Keys.Z))
                keydown = 'z';

            if (state.IsKeyUp(Keys.A) && keydown == 'a')
            {
                keydown = (char)0;
                return 'a';
            }
            if (state.IsKeyUp(Keys.B) && keydown == 'b')
            {
                keydown = (char)0;
                return 'b';
            }
            if (state.IsKeyUp(Keys.C) && keydown == 'c')
            {
                keydown = (char)0;
                return 'c';
            }
            if (state.IsKeyUp(Keys.D) && keydown == 'd')
            {
                keydown = (char)0;
                return 'd';
            }
            if (state.IsKeyUp(Keys.E) && keydown == 'e')
            {
                keydown = (char)0;
                return 'e';
            }
            if (state.IsKeyUp(Keys.F) && keydown == 'f')
            {
                keydown = (char)0;
                return 'f';
            }
            if (state.IsKeyUp(Keys.G) && keydown == 'g')
            {
                keydown = (char)0;
                return 'g';
            }
            if (state.IsKeyUp(Keys.H) && keydown == 'h')
            {
                keydown = (char)0;
                return 'h';
            }
            if (state.IsKeyUp(Keys.I) && keydown == 'i')
            {
                keydown = (char)0;
                return 'i';
            }
            if (state.IsKeyUp(Keys.J) && keydown == 'j')
            {
                keydown = (char)0;
                return 'j';
            }
            if (state.IsKeyUp(Keys.K) && keydown == 'k')
            {
                keydown = (char)0;
                return 'k';
            }
            if (state.IsKeyUp(Keys.L) && keydown == 'l')
            {
                keydown = (char)0;
                return 'l';
            }
            if (state.IsKeyUp(Keys.M) && keydown == 'm')
            {
                keydown = (char)0;
                return 'm';
            }
            if (state.IsKeyUp(Keys.N) && keydown == 'n')
            {
                keydown = (char)0;
                return 'n';
            }
            if (state.IsKeyUp(Keys.O) && keydown == 'o')
            {
                keydown = (char)0;
                return 'o';
            }
            if (state.IsKeyUp(Keys.P) && keydown == 'p')
            {
                keydown = (char)0;
                return 'p';
            }
            if (state.IsKeyUp(Keys.Q) && keydown == 'q')
            {
                keydown = (char)0;
                return 'q';
            }
            if (state.IsKeyUp(Keys.R) && keydown == 'r')
            {
                keydown = (char)0;
                return 'r';
            }
            if (state.IsKeyUp(Keys.S) && keydown == 's')
            {
                keydown = (char)0;
                return 's';
            }
            if (state.IsKeyUp(Keys.T) && keydown == 't')
            {
                keydown = (char)0;
                return 't';
            }
            if (state.IsKeyUp(Keys.U) && keydown == 'u')
            {
                keydown = (char)0;
                return 'u';
            }
            if (state.IsKeyUp(Keys.V) && keydown == 'v')
            {
                keydown = (char)0;
                return 'v';
            }
            if (state.IsKeyUp(Keys.W) && keydown == 'w')
            {
                keydown = (char)0;
                return 'w';
            }
            if (state.IsKeyUp(Keys.X) && keydown == 'x')
            {
                keydown = (char)0;
                return 'x';
            }
            if (state.IsKeyUp(Keys.Y) && keydown == 'y')
            {
                keydown = (char)0;
                return 'y';
            }
            if (state.IsKeyUp(Keys.Z) && keydown == 'z')
            {
                keydown = (char)0;
                return 'z';
            }

            return (char)0;
        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation displayOrientation)
        {
            background.Draw(batch);
            if (playerCreated && !inBattle)
            {
                DrawGameplayItems(batch);
            }
            else if (inBattle)
            {
                DrawBattleItems(batch);
            }
            else
            {
                DrawPregameStuff(batch);
            }
        }

        private bool CheckTileCollision(string direction)
        {
            if(direction == "left")
            {
                if (tileLayer.Tiles[(int)player.TilePosition.X - 1, (int)player.TilePosition.Y] != blankTile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (direction == "right")
            {
                if (tileLayer.Tiles[(int)player.TilePosition.X + 1, (int)player.TilePosition.Y] != blankTile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (direction == "up")
            {
                if (tileLayer.Tiles[(int)player.TilePosition.X, (int)player.TilePosition.Y - 1] != blankTile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (direction == "down")
            {
                if (tileLayer.Tiles[(int)player.TilePosition.X, (int)player.TilePosition.Y + 1] != blankTile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }

        private bool CheckTileForBattle()
        {
            if (tileLayer.Tiles[(int)player.TilePosition.X, (int)player.TilePosition.Y] == enemyTile)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckTileForExit()
        {
            if (tileLayer.Tiles[(int)player.TilePosition.X, (int)player.TilePosition.Y] == exitTile)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GameplayTurnCheck()
        {
            player.UpdatePosition();
            hud.Update(player);

            if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(leftButton) && CheckTileCollision("left"))
            {
                player.TilePosition.X--;
            }

            if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(rightButton) && CheckTileCollision("right"))
            {
                player.TilePosition.X++;
            }

            if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(upButton) && CheckTileCollision("up"))
            {
                player.TilePosition.Y--;
            }

            if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(downButton) && CheckTileCollision("down"))
            {
                player.TilePosition.Y++;
            }

            if (CheckTileForBattle())
            {
                inBattle = true;
                tileLayer.Tiles[(int)player.TilePosition.X, (int)player.TilePosition.Y] = enemykilledTile;
            }

            if (CheckTileForExit())
            {
                bossBattle = true;
                inBattle = true;
            }
        }

        private void DrawBattleItems(SpriteBatch batch)
        {
            attackButton.Draw(batch);
            magicButton.Draw(batch);
            potionButton.Draw(batch);
            if (bossBattle)
            {
                boss.Draw(batch);
            }
            else
            {
                minion.Draw(batch);
            }
            hud.Draw(batch);
        }

        private void DrawGameplayItems(SpriteBatch batch)
        {
            map[mapNumber].Draw(batch);
            player.Draw(batch);
            hud.Draw(batch);
            leftButton.Draw(batch);
            rightButton.Draw(batch);
            upButton.Draw(batch);
            downButton.Draw(batch);
        }

        private void DrawPregameStuff(SpriteBatch batch)
        {
            if (!nameComplete)
            {
                enterplayernameText.Draw(batch);
                confirmnameButton.Draw(batch);
                batch.Draw(textenterbox, new Rectangle(375, 400, textenterbox.Width, textenterbox.Height), Color.White);
                playername.Draw(batch);
            }
            else if (!classComplete)
            {
                chooseclasstext.Draw(batch);
                paladinButton.Draw(batch);
                casterButton.Draw(batch);
                rogueButton.Draw(batch);
            }
            else
            {
                batch.Draw(storytexts[storyCount], new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
                storyContinueButton.Draw(batch);
            }
        }
    }
}
