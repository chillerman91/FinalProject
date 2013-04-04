using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Infinity_TD
{
    class Interface
    {
        Texture2D sidebar, border, textBox;
        Texture2D[] runes = new Texture2D[10];
        Texture2D[] recipes = new Texture2D[12];
        KeyboardState previousKeyboard;
        public Texture2D altMouseTex;
        SpriteFont interfaceFont;
        string currentDrawString;
        public Combinator.TowerType currentTower;
        public Rectangle[] runeRectangles = new Rectangle[10];
        public Rectangle[] recipeRectangles = new Rectangle[12];
        public Combinator combinator = new Combinator();
        public Combinator.Runes[] combinatorRunes = new Combinator.Runes[3];
        public Texture2D[] combinatorRuneText = new Texture2D[3];
        int runeCount = 0;
        
        float positionX = 800;

        public void InitializeInterface(ContentManager content)
        {
            combinator.InitializeRecipes();
            sidebar = content.Load<Texture2D>("Graphics/Stuff/hud_base");

            interfaceFont = content.Load<SpriteFont>("Fonts/hud_font");
            //border = content.Load<Texture2D>("");

            textBox = content.Load<Texture2D>("Graphics/Stuff/textbox");

            runes[0] = content.Load<Texture2D>("Graphics/Runes/fogo-runa");
            runes[1] = content.Load<Texture2D>("Graphics/Runes/agua-runa");
            runes[2] = content.Load<Texture2D>("Graphics/Runes/ar-runa");
            runes[3] = content.Load<Texture2D>("Graphics/Runes/terra-runa");
            runes[4] = content.Load<Texture2D>("Graphics/Runes/raio-runa");
            runes[5] = content.Load<Texture2D>("Graphics/Runes/natureza-runa");
            runes[6] = content.Load<Texture2D>("Graphics/Runes/luz-runa");
            runes[7] = content.Load<Texture2D>("Graphics/Runes/dark-runa");
            runes[8] = content.Load<Texture2D>("Graphics/Runes/cosmos-runa");
            runes[9] = content.Load<Texture2D>("Graphics/Runes/fogo-runa");
            

            for (int i = 0; i < 9; i++)
            {
                runeRectangles[i] = new Rectangle((int)positionX + 35 + (int)((i % 3) * 60), 50 + (int)((i / 3) * 70), 30, 30);
            }

            for (int i = 0; i < 12; i++)
            {
                recipeRectangles[i] = new Rectangle((int)positionX + 35 + (int)((i % 3) * 60), 420 + (int)((i / 3) * 70), 30, 30);
            }

            recipes[0] = content.Load<Texture2D>("Graphics/Tower/fireball rune");
            recipes[1] = content.Load<Texture2D>("Graphics/Tower/glacier rune1");
            recipes[2] = content.Load<Texture2D>("Graphics/Tower/tornado rune");
            recipes[3] = content.Load<Texture2D>("Graphics/Tower/thunderstorm rune");
            recipes[4] = content.Load<Texture2D>("Graphics/Tower/earthquake rune");
            recipes[5] = content.Load<Texture2D>("Graphics/Tower/corrosive rune");
            recipes[6] = content.Load<Texture2D>("Graphics/Tower/magmatic glyph");
            recipes[7] = content.Load<Texture2D>("Graphics/Tower/blinding_light");
            recipes[8] = content.Load<Texture2D>("Graphics/Tower/dark_flames");
            recipes[9] = content.Load<Texture2D>("Graphics/Tower/sunglyph");
            recipes[10] = content.Load<Texture2D>("Graphics/Tower/blackhole");
            recipes[11] = content.Load<Texture2D>("Graphics/Tower/infinity");

        }

        public void UpdateInterface(GameTime gameTime, Rectangle mouseRec, MouseState previousState)
        {
            #region UpdateMainRunes
            for (int i = 0; i < 10; i++)
            {
                if (mouseRec.Intersects(runeRectangles[i]))
                {
                    if ((Mouse.GetState().LeftButton == ButtonState.Pressed) && (previousState.LeftButton == ButtonState.Released))
                    {

                        if (RuneManager.RuneBag[i] >= 1)
                        {
                            combinatorRunes[runeCount] = RuneManager.RuneList[i];
                            combinatorRuneText[runeCount] = runes[i];
                            runeCount++;
                            RuneManager.RemoveRune(i, 1);

                        }

                        if( runeCount == 3)
                        {
                             runeCount = 0;
                             currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                             if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[i];

                             if (currentTower == Combinator.TowerType.INVALID)
                             {
                                 RuneManager.AddExistingRune(combinatorRunes[0]);
                                 RuneManager.AddExistingRune(combinatorRunes[1]);
                                 RuneManager.AddExistingRune(combinatorRunes[2]);
                             }

                        }


                    }
                }
            }

            #region KeyboardInput

            if ((Keyboard.GetState().IsKeyDown(Keys.Q)) && (previousKeyboard.IsKeyUp(Keys.Q)))
            {
                if (RuneManager.RuneBag[0] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[0];
                    combinatorRuneText[runeCount] = runes[0];
                    runeCount++;
                    RuneManager.RemoveRune(0, 1);
                }
                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[0];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }
                }


            }

            if ((Keyboard.GetState().IsKeyDown(Keys.W)) && (previousKeyboard.IsKeyUp(Keys.W)))
            {
                if (RuneManager.RuneBag[1] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[1];
                    combinatorRuneText[runeCount] = runes[1];
                    runeCount++;
                    RuneManager.RemoveRune(1, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[1];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }
                }


            }

            if ((Keyboard.GetState().IsKeyDown(Keys.E)) && (previousKeyboard.IsKeyUp(Keys.E)))
            {
                if (RuneManager.RuneBag[2] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[2];
                    combinatorRuneText[runeCount] = runes[2];
                    runeCount++;
                    RuneManager.RemoveRune(2, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[2];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }
                }


            }


            if ((Keyboard.GetState().IsKeyDown(Keys.A)) && (previousKeyboard.IsKeyUp(Keys.A)))
            {
                if (RuneManager.RuneBag[3] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[3];
                    combinatorRuneText[runeCount] = runes[3];
                    runeCount++;
                    RuneManager.RemoveRune(3, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[3];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }

                }


            }


            if ((Keyboard.GetState().IsKeyDown(Keys.S)) && (previousKeyboard.IsKeyUp(Keys.S)))
            {
                if (RuneManager.RuneBag[4] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[4];
                    combinatorRuneText[runeCount] = runes[4];
                    runeCount++;
                    RuneManager.RemoveRune(4, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[4];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }
                }


            }


            if ((Keyboard.GetState().IsKeyDown(Keys.D)) && (previousKeyboard.IsKeyUp(Keys.D)))
            {
                if (RuneManager.RuneBag[5] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[5];
                    combinatorRuneText[runeCount] = runes[5];
                    runeCount++;
                    RuneManager.RemoveRune(5, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[5];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }
                }


            }


            if ((Keyboard.GetState().IsKeyDown(Keys.Z)) && (previousKeyboard.IsKeyUp(Keys.Z)))
            {
                if (RuneManager.RuneBag[6] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[6];
                    combinatorRuneText[runeCount] = runes[6];
                    runeCount++;
                    RuneManager.RemoveRune(6, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[6];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }
                }


            }


            if ((Keyboard.GetState().IsKeyDown(Keys.X)) && (previousKeyboard.IsKeyUp(Keys.X)))
            {
                if (RuneManager.RuneBag[7] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[7];
                    combinatorRuneText[runeCount] = runes[7];
                    runeCount++;
                    RuneManager.RemoveRune(7, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[7];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }
                }


            }


            if ((Keyboard.GetState().IsKeyDown(Keys.C)) && (previousKeyboard.IsKeyUp(Keys.C)))
            {
                if (RuneManager.RuneBag[8] >= 1)
                {
                    combinatorRunes[runeCount] = RuneManager.RuneList[8];
                    combinatorRuneText[runeCount] = runes[8];
                    runeCount++;
                    RuneManager.RemoveRune(8, 1);
                }

                if (runeCount == 3)
                {
                    runeCount = 0;
                    currentTower = (combinator.parseCombination(combinatorRunes[0], combinatorRunes[1], combinatorRunes[2]));
                    if (currentTower != Combinator.TowerType.INVALID) altMouseTex = recipes[8];

                    if (currentTower == Combinator.TowerType.INVALID)
                    {
                        RuneManager.AddExistingRune(combinatorRunes[0]);
                        RuneManager.AddExistingRune(combinatorRunes[1]);
                        RuneManager.AddExistingRune(combinatorRunes[2]);
                    }

                }


            }

            #endregion

            #endregion

            #region UpdateRecipes

            for (int i = 0; i < 12; i++)
            {
                if ((RuneManager.CheckRecipe[i] == true) && (mouseRec.Intersects(recipeRectangles[i])))
                {
                    currentDrawString = RuneManager.RecipeBag[i];
                    break;
                }

                currentDrawString = null;
            }

            #endregion

            previousKeyboard = Keyboard.GetState();
        }

        public void DrawInterface(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sidebar, new Vector2(positionX, 0), Color.White);

            for (int i = 0; i < 9; i++)
            {
                spriteBatch.Draw(runes[i], new Vector2(positionX + 35 + (int)((i % 3) * 60), 50 + (int)((i / 3) * 70)), Color.White);
                spriteBatch.DrawString(interfaceFont, Infinity_TD.RuneManager.RuneBag[i].ToString(), new Vector2(positionX + 45 + (int)((i % 3) * 60), 75 + (int)((i / 3) * 70)), Color.Orange);
                spriteBatch.DrawString(interfaceFont, Infinity_TD.RuneManager.RuneBag[i].ToString(), new Vector2(positionX + 46 + (int)((i % 3) * 60), 76 + (int)((i / 3) * 70)), Color.Red);
            }

            for (int i = 0; i < runeCount; i++)
            {
                spriteBatch.Draw(combinatorRuneText[i], new Vector2(830 + 64 * i, 300), Color.White);
            }

            for (int i = 0; i < 12; i++)
            {
                if (RuneManager.CheckRecipe[i] == true)
                {
                    spriteBatch.Draw(recipes[i], new Rectangle((int)positionX + 35 + (int)((i % 3) * 60), 420 + (int)((i / 3) * 70), 32, 32), new Rectangle(0,0,32,32), Color.White);
                }
            }

            spriteBatch.Draw(textBox, new Vector2(positionX, 0), Color.White);

            if (currentDrawString != null)
            {
                spriteBatch.DrawString(interfaceFont, currentDrawString, new Vector2(822, 687), Color.Black);
                spriteBatch.DrawString(interfaceFont, currentDrawString, new Vector2(820, 685), Color.Yellow);

            }

                

            if (altMouseTex != null)
            {
                spriteBatch.Draw(altMouseTex, new Rectangle(Mouse.GetState().X, Mouse.GetState().Y,32,32), new Rectangle(0, 0, 32, 32), Color.White);
            }
            
        }

    }
}
