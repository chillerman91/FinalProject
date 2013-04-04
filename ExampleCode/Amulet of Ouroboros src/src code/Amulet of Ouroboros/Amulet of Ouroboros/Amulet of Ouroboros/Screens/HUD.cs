using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Amulet_of_Ouroboros.Texts;
using Amulet_of_Ouroboros.Sprites;

namespace Amulet_of_Ouroboros.Screens
{
    class HUD
    {
        Texture2D PlayerPieceImage;
        Text PlayerName;
        Text PlayerHP;
        Text PlayerMP;
        Text PlayerPotions;
        Text PlayerPieces;
        Text PlayerXP;
        Text PlayerXPNeeded;

        public HUD(ContentManager content, Player p)
        {
            PlayerPieceImage = content.Load<Texture2D>("Images/PlayerPiece");
            PlayerName = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), p.Name + "   Lvl: " + p.Level, new Vector2(125, 615), Color.Blue);
            PlayerHP = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "HP: " + p.CurrentHP + "/" + p.MaxHP, new Vector2(125, 655), Color.Blue);
            PlayerMP = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "MP: " + p.CurrentMP + "/" + p.MaxMP, new Vector2(125, 685), Color.Blue);
            PlayerPieces = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Pieces: " + p.Pieces , new Vector2(325, 655), Color.Blue);
            PlayerPotions = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Potions: " + p.Potions, new Vector2(325, 685), Color.Blue);
            PlayerXP = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "EXP: " + p.CurrentXP, new Vector2(525, 655), Color.Blue);
            PlayerXPNeeded = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "To Next: " + p.XPtoNext, new Vector2(525, 685), Color.Blue);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(PlayerPieceImage, new Rectangle(40, 600, PlayerPieceImage.Width, PlayerPieceImage.Height), Color.White);
            PlayerName.Draw(batch);
            PlayerHP.Draw(batch);
            PlayerMP.Draw(batch);
            PlayerPieces.Draw(batch);
            PlayerPotions.Draw(batch);
            PlayerXP.Draw(batch);
            PlayerXPNeeded.Draw(batch);
        }

        public void Update(Player p)
        {
            PlayerName.ChangeText(p.Name + "   Lvl: " + p.Level);
            PlayerHP.ChangeText("HP: " + p.CurrentHP + "/" + p.MaxHP);
            PlayerMP.ChangeText("MP: " + p.CurrentMP + "/" + p.MaxMP);
            PlayerPieces.ChangeText("Pieces: " + p.Pieces);
            PlayerPotions.ChangeText("Potions: " + p.Potions);
            PlayerXP.ChangeText("EXP: " + p.CurrentXP);
            PlayerXPNeeded.ChangeText("To Next: " + p.XPtoNext);
        }
    }
}
