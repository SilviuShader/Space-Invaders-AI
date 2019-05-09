using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGame
{
    public class Player : GameObject
    {
        public const double PLAYER_SPEED          = 20.0;
        public const double PLAYER_TIME_TO_RELOAD = 1.0;
        public const int    PLAYER_WIDTH          = 5;
        private double      m_reloadingTime;

        public               Player(ContentManager content, SPoint position, SpaceInvaders gameRoot) :
                                    base(content, position, "Player", PLAYER_WIDTH, gameRoot)
        {
            m_gameObjectType = GameObjectType.Player;
            m_reloadingTime = PLAYER_TIME_TO_RELOAD;
        }

        public override void Update(double timeStep)
        {
            UpdateTimers(timeStep);
            UpdateInput(timeStep);
            UpdateCollisions();
        }

        public override void OnCollisionEnter(GameObject otherObject)
        {
            if(otherObject.GetGameObjectType() == GameObjectType.Bullet)
            {
                Bullet bullet = (Bullet)otherObject;
                if(bullet.GetDirection().y < 0.0)
                {
                    Kill();
                    bullet.Kill();
                }
            }
        }

        public override void Render(GameRenderer gameRenderer, SpriteBatch spriteBatch)
        {
            Render(gameRenderer, spriteBatch, new Color(0.0f, 1.0f, 0.0f));
        }

        private void UpdateTimers(double timeStep)
        {
            m_reloadingTime += timeStep;
            if(m_reloadingTime >= PLAYER_TIME_TO_RELOAD)
                m_reloadingTime = PLAYER_TIME_TO_RELOAD;
        }

        private void UpdateInput(double timeStep)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Right))
                m_position.x += timeStep * PLAYER_SPEED;

            if(Keyboard.GetState().IsKeyDown(Keys.Left))
                m_position.x -= timeStep * PLAYER_SPEED;
            
            if(Keyboard.GetState().IsKeyDown(Keys.Space))
                if(m_reloadingTime >= PLAYER_TIME_TO_RELOAD)
                    Shoot();
        }
        private void UpdateCollisions()
        {
            if(m_position.x - (PLAYER_WIDTH / 2.0) <= 0.0)
                m_position.x = (PLAYER_WIDTH / 2.0);
            if(m_position.x + (PLAYER_WIDTH / 2.0) >= SpaceInvaders.SCREEN_WIDTH)
                m_position.x = SpaceInvaders.SCREEN_WIDTH - (PLAYER_WIDTH / 2.0);    
        }

        private void Shoot()
        {
            m_gameRoot.Instantiate(new Bullet(m_content, 
                                              m_position + new SVector2D(0.0, m_width / 2.0), // works because the texture is a square
                                              m_gameRoot, 
                                              new SVector2D(0.0, 1.0),
                                              new Color(0.0f, 1.0f, 0.0f)));
            m_reloadingTime = 0.0;
        }
    }
}