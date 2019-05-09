using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
    public class GameRenderer
    {
        double m_baseWidth;
        double m_baseHeight;
        
        double m_screenWidth;
        double m_screenHeight;
        public      GameRenderer(double baseWidth, double baseHeight)
        {
            m_baseWidth    = baseWidth;
            m_baseHeight   = baseHeight;
            m_screenWidth  = baseWidth;
            m_screenHeight = baseHeight;
        }

        public void Update(double screenWidth, double screenHeight)
        {
            m_screenWidth  = screenWidth;
            m_screenHeight = screenHeight;
        }

        public void RenderSprite(SpriteBatch spriteBatch, Texture2D texture, SPoint position, double width, Color color)
        {
            double height  = (width / texture.Height) * texture.Width;
            
            // center the sprite
            position.x -= width / 2.0;
            position.y += height / 2.0;

            // flip the sprite
            position.y = m_baseHeight - position.y;
            
            SVector2D rectangleOrigin = new SVector2D(0, 0);
            double graphicsScale = 1.0;
            double modifiedWidth = m_screenWidth;
            if(m_screenWidth / m_screenHeight > m_baseWidth / m_baseHeight)
            {
                double center = m_screenWidth / 2.0;
                graphicsScale = (m_screenHeight / m_baseHeight);
                double actualWidthSide = graphicsScale * m_baseWidth;
                modifiedWidth = actualWidthSide;
                rectangleOrigin.x = center - (actualWidthSide / 2.0);
            }
            else
            {
                double center = m_screenHeight / 2.0;
                graphicsScale = (m_screenWidth / m_baseWidth);
                double actualHeightSide = graphicsScale * m_baseHeight;
                rectangleOrigin.y = center - (actualHeightSide / 2.0);
            }

            double textureWidthPercentage = texture.Width / modifiedWidth;
            double targetWidthPercentage  = width / m_baseWidth;
            double scale = targetWidthPercentage / textureWidthPercentage;

            SPoint drawPosition = new SPoint(position.x * graphicsScale, position.y * graphicsScale);
            drawPosition += rectangleOrigin;

            spriteBatch.Draw(texture, new Vector2((float)drawPosition.x, (float)drawPosition.y), null, color, 0.0f, Vector2.Zero, (float)scale, SpriteEffects.None, 1.0f);
        }
    }
}