using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGame
{
    public class Bullet : GameObject
    {
        public const double BULLET_WIDTH = 0.5;
        public const double BULLET_POWER = 20.0;
        public const double FLICK_TIME   = 0.1;
        public const double VISIBLE_TIME = 0.5;

        private SVector2D m_velocity;
        private double    m_accumulatedFlickTime;
        private bool      m_visibleState;

        private Color     m_color;

        public                    Bullet(ContentManager content, SPoint position, SpaceInvaders gameRoot, SVector2D velocity, Color color) :
            base(content, position, "Bullet", BULLET_WIDTH, gameRoot)
        {
            m_gameObjectType = GameObjectType.Bullet;
            m_velocity = velocity;
            m_accumulatedFlickTime = 0.0;
            m_visibleState = false;

            m_color = color;
        }

        public override void      Update(double timeStep)
        {
            m_position += m_velocity * timeStep * BULLET_POWER;

            if(m_position.y + GetHeight() / 2.0 <= 0 ||
               m_position.y - (GetHeight() / 2.0) >= SpaceInvaders.SCREEN_HEIGHT)
               {
                   Kill();
               }
            
            m_accumulatedFlickTime += timeStep;

            double timeReference = VISIBLE_TIME;
            if(!m_visibleState)
                timeReference = FLICK_TIME;
            
            if(m_accumulatedFlickTime >= timeReference)
            {
                m_visibleState = !m_visibleState;
                m_accumulatedFlickTime = 0.0;
            }
        }

        public override void      Render(GameRenderer gameRenderer, SpriteBatch spriteBatch)
        {
            if(m_visibleState)
                base.Render(gameRenderer, spriteBatch, m_color);
        }
        
        public          SVector2D GetDirection()
        {
            return m_velocity;
        }
    }
}