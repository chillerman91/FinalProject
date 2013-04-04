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
        Texture2D PlayerPieceImage;
        Text Level;
        Player player;
        Text PlayerHealth;
        Text Strength;
        Text exp;
        private string playerValues;
        private string[] whiskerValues;
        private string adjascentValues;
        private string pieSliceValues;
        #endregion
        public static string GenInfo;
        public HUDPlayerInfo(ContentManager content, Player p)
        {
            player = p;
            p.SetThisHUD(this);
        }

        public void Draw(SpriteBatch batch)
        {
            if (!string.IsNullOrEmpty(GenInfo))
            {
                batch.DrawString(player.font, GenInfo, new Vector2(50, 10), Color.Black);
            }
            if (whiskerValues != null)
            {
                string text =  "MobNumber: " + Globals.Mobs.GetMobAmount() + "\n";
                text += string.Format("Wisker Distances: [0]={0}, [1]={1}, [2]={2}", whiskerValues[0], whiskerValues[1], whiskerValues[2]);
                //batch.DrawString(player.font, text, new Vector2(50, 30), Color.AliceBlue);
            }
            if (!string.IsNullOrEmpty(adjascentValues))
            {
                //batch.DrawString(player.font, adjascentValues, new Vector2(50, 50), Color.Black); 
            }
            if (!string.IsNullOrEmpty(pieSliceValues))
            {
                //batch.DrawString(player.font, pieSliceValues, new Vector2(50, 70), Color.Black); 
            }
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
