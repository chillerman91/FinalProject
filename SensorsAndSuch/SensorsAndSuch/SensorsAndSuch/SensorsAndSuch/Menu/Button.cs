using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using SensorsAndSuch.Texts;

namespace SensorsAndSuch.Sprites
{
    class Button : Sprite
    {
        Text Text;
        SpriteFont buttonFont;

        public Rectangle TouchArea;

        string displayText;

        public string DisplayText
        {
            get { return displayText; }
        }

        public Button(ContentManager content,
                      string displayText,
                      Vector2 displayPosition, Color textColor,
                      Color color, string assetName = "Images/Button")
            : base(content, assetName)
        {
            Position = displayPosition;
            this.displayText = displayText;
            this.color = color;

            TouchArea = new Rectangle((int)displayPosition.X,
                                      (int)displayPosition.Y,
                                      texture.Width,
                                      texture.Height);

            buttonFont = content.Load<SpriteFont>("Fonts/buttonFont");
            Text = new Text(buttonFont,
                                  displayText,
                                  Vector2.Zero,
                                  textColor,
                                  Text.Alignment.Both,
                                  TouchArea);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            Text.Draw(batch);
        }
    }
}
