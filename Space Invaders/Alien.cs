using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGame
{
    public class Alien : GameObject
    {
        public const double ALIEN_WIDTH        = 5;
        public const double ALIEN_SPEED        = 2.0;
        public const double TIME_TO_UPDATE_POS = 2.0;
        
        private bool           m_isVirtualAlien;
        private bool           m_goingRight;
        
        private SPoint         m_hiddenPosition;
        private double         m_accumulatedTimePos;

        private int            m_layer;

        private AlienBehaviour m_behaviour;
        private int            m_behaviourIndex;
        private double         m_accumulatedBehaviourTime;

        public Alien(ContentManager content, SPoint position, SpaceInvaders gameRoot, bool virt, int layer, AlienBehaviour behaviour) :
            base(content, position, "Alien1", ALIEN_WIDTH, gameRoot)
        {
            m_gameObjectType = GameObjectType.Alien;
            m_isVirtualAlien = virt;
            m_goingRight = true;

            m_hiddenPosition = m_position;
            m_accumulatedTimePos = 0.0;
            m_layer = layer;

            m_gameRoot.GetAliensCountPerRow()[layer]++;

            m_behaviour                = behaviour;
            m_behaviourIndex           = 0;
            m_accumulatedBehaviourTime = 0.0;
        }

        public override void Update(double timeStep)
        {
            UpdateTimers(timeStep);
            UpdatePosition(timeStep);

            if(m_isVirtualAlien)
            {
                UpdateCollisions();
            }

            if(!m_isVirtualAlien)
            {
                if(m_behaviourIndex < m_behaviour.timesToShoot.Length)
                {
                    if(m_accumulatedBehaviourTime >= m_behaviour.timesToShoot[m_behaviourIndex])
                    {
                        Shoot();
                        m_behaviourIndex++;
                    }
                }
            }
        }

        public override void OnCollisionEnter(GameObject otherObject)
        {
            if(otherObject.GetGameObjectType() == GameObjectType.Bullet)
            {
                Bullet bullet = (Bullet)otherObject;
                if(bullet.GetDirection().y > 0.0)
                {
                    Kill();
                    bullet.Kill();
                }
            }
        }

        public override void Render(GameRenderer gameRenderer, SpriteBatch spriteBatch)
        {
            Color color = GetCurrentColor();
            color.A = 255;

            if(!m_isVirtualAlien)
                base.Render(gameRenderer, spriteBatch, color);
        }

        public override void Kill()
        {
            m_gameRoot.GetAliensCountPerRow()[m_layer]--;
            base.Kill();
        }

        public void ChangeDirection(bool newDirection)
        {
            m_goingRight = newDirection;
        }

        public bool IsVirtual()
        {
            return m_isVirtualAlien;
        }

        public int  GetLayer()
        {
            return m_layer;
        }

        private Color GetCurrentColor()
        {
            HSLColor newHSL = new HSLColor((MathF.Sin(((float)m_accumulatedBehaviourTime + (float)m_layer) * 0.15f) + 1.0f) / 2.0f, 1.0f, 0.5f);

            return newHSL.ToRgbColor();
        }

        private void Shoot()
        {
            m_gameRoot.Instantiate(new Bullet(m_content, 
                                              m_position - new SVector2D(0.0, m_width / 2.0), // works because the texture is a square
                                              m_gameRoot, 
                                              new SVector2D(0.0, -1.0),
                                              GetCurrentColor()));
        }

        private void UpdateTimers(double timeStep)
        {
            m_accumulatedTimePos += timeStep;
            if(m_accumulatedTimePos >= TIME_TO_UPDATE_POS)
                m_accumulatedTimePos = TIME_TO_UPDATE_POS;
            
            m_accumulatedBehaviourTime += timeStep;
        }

        private void UpdatePosition(double timeStep)
        {
            if(m_goingRight)
                m_hiddenPosition.x += timeStep * ALIEN_SPEED;
            else
                m_hiddenPosition.x -= timeStep * ALIEN_SPEED;
            
            if(m_accumulatedTimePos >= TIME_TO_UPDATE_POS)
            {
                m_hiddenPosition.y = m_position.y;
                m_position = m_hiddenPosition;
                m_accumulatedTimePos = 0.0;
            }
        }

        private void UpdateCollisions()
        {
            if(m_goingRight)
            {
                if(m_hiddenPosition.x + (ALIEN_WIDTH / 2.0) >= SpaceInvaders.SCREEN_WIDTH)
                {
                    m_gameRoot.UpdateAliensDirection(false);
                    m_accumulatedTimePos = TIME_TO_UPDATE_POS;
                }
            }
            else
                if(m_hiddenPosition.x - (ALIEN_WIDTH / 2.0) <= 0.0)
                {
                    m_gameRoot.UpdateAliensDirection(true);
                    m_accumulatedTimePos = TIME_TO_UPDATE_POS;
                }
        }
    }
}