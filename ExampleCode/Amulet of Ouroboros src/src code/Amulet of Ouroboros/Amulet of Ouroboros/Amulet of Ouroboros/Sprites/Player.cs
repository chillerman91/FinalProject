using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Amulet_of_Ouroboros.Sprites
{
    class Player : Sprite
    {
        string[] Classes; 
        
        const int TileWidth = 64;
        const int TileHeight = 32;

        const int TileXOffset = 10;
        const int TileYOffset = 105;

        public Vector2 TilePosition;

        public int Pieces;

        public int Potions;

        public string Name = "Player";

        public bool TurnBattle;
        public int Level;
        public string Class;

        public int MAGDAM;
        public int MAGCOS;
        public int DAM;
        public int CurrentXP;
        public int XPtoNext;
        public int CurrentHP;
        public int MaxHP;
        public int CurrentMP;
        public int MaxMP;

        public Player(ContentManager content)
            : base(content, "Images/PlayerPiece")
        {
            Pieces = 0;
            Potions = 3;
            CurrentHP = 25;
            MaxHP = 25;
            CurrentMP = 10;
            MaxMP = 10;
            TurnBattle = true;
            Classes = new string[3];
            Classes[0] = "Paladin";
            Classes[1] = "Caster";
            Classes[2] = "Rogue";
        }

        public void CreatePlayer(int clas, string name)
        {
            Class = Classes[clas];
            Name = name;
            Level = 1;
            CurrentXP = 0;
            XPtoNext = 500;

            if (clas == 0)
            {
                DAM = 5;
                MAGDAM = 5;
                MAGCOS = 5;
            }
            else if (clas == 1)
            {
                DAM = 3;
                MAGDAM = 5;
                MAGCOS = 2;
            }
            else
            {
                DAM = 4;
                MAGDAM = 4;
                MAGCOS = 1;
            }
        }

        public void UpdatePosition()
        {
            if (CurrentXP >= XPtoNext)
            {
                Level++;
                CurrentXP -= XPtoNext;
                XPtoNext += 250;
                DAM++;
                MAGDAM++;
                MaxHP += 5;
                MaxMP += 5;
                CurrentHP = MaxHP;
                CurrentMP = MaxMP;
            }
            this.Position = new Vector2(TilePosition.X * TileWidth - TileXOffset, TilePosition.Y * TileHeight - TileYOffset);
        }

    }
}
