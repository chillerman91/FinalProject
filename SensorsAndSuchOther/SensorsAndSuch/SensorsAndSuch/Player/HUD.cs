using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using SensorsAndSuch.Texts;
using SensorsAndSuch.Sprites;

namespace SensorsAndSuch.Screens
{
    internal class HUDPlayerInfo
    {
        #region Datafields
        Vector2 BaseLocation = new Vector2(25, 635);
        private string playerValues;
        private string[] whiskerValues;
        private string adjascentValues;
        private string pieSliceValues;
        public static string GenInfo;
        protected Player player;
        internal States currentState = States.PlayerData;
        public enum States 
        { 
            PlayerData = 0
        }
        #endregion

        public HUDPlayerInfo(ContentManager content, Player p)
        {
            player = p;
            p.SetThisHUD(this);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(player.font, "MobNumber: " + Globals.Mobs.GetMobAmount() + "\n", new Vector2(50, 10), Color.Black);
            if (!string.IsNullOrEmpty(GenInfo))
            {
                //batch.DrawString(player.font, GenInfo, new Vector2(50, 80), Color.Black);
            } 
            
            string text = "";
            switch (currentState)
            { 
                case States.PlayerData:

                    text += player.BonusText + "\n";
                    text += "Kills: " + player.kills + "\n";
                    text += "Lives: " + player.lives + "\n";
                    break;
            }
            batch.DrawString(player.font, text, new Vector2(50, 80), Color.Black);
        }

        public void UpdatePlayer(string val)
        {
            playerValues = val;
        }

        public void UpdateWhiskers(params string[] val)
        {
            whiskerValues = val;
        }

        public void UpdateAdjacents(string val)
        {
            adjascentValues = val;
        }

        public void UpdatePieSlices(string val)
        {
            pieSliceValues = val;
        }
    }
}
