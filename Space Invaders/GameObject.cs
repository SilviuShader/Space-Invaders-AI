using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGame
{
    public enum GameObjectType
    {
        Default = 0,
        Player  = 1,
        Bullet  = 2,
        Alien   = 3,
        Block   = 4
    }
    public class GameObject
    {
        protected SPoint         m_position;
        protected ContentManager m_content;
        protected Texture2D      m_texture;
        protected double         m_width;
        protected string         m_textureFileName;
        protected SpaceInvaders  m_gameRoot;
        protected bool           m_dead;

        protected GameObjectType m_gameObjectType;
        public                GameObject(ContentManager content, 
                                         SPoint         position, 
                                         string         texutreFilename, 
                                         double         width,
                                         SpaceInvaders  gameRoot)
        {
            m_content = content;
            m_position = position;
            m_textureFileName = texutreFilename;
            m_width = width;

            m_texture = m_content.Load<Texture2D>(m_textureFileName);

            m_gameRoot = gameRoot;
            m_gameObjectType = GameObjectType.Default;

            m_dead = false;
        }

        public virtual void   Update(double timeStep)
        {
        }

        public virtual void   OnCollisionEnter(GameObject otherObject)
        {
        }

        public virtual void   Render(GameRenderer gameRenderer, SpriteBatch spriteBatch, Color color)
        {
            gameRenderer.RenderSprite(spriteBatch, m_texture, m_position, m_width, color);
        }

        public virtual void   Render(GameRenderer gameRenderer, SpriteBatch spriteBatch)
        {
            Render(gameRenderer, spriteBatch, Color.White);
        }

        public virtual void   Kill()
        {
            m_dead = true;
        }

        public double         GetWidth()
        {
            return m_width;
        }

        public double         GetHeight()
        {
            return (GetWidth() / m_texture.Height) * m_texture.Width;
        }

        public SPoint         GetPosition()
        {
            return m_position;
        }

        public bool           IsDead()
        {
            return m_dead;
        }

        public void           Translate(SVector2D translation)
        {
            Matrix2D transformMatrix = new Matrix2D();
            transformMatrix.Translate(translation.x, translation.y);
            transformMatrix.TransformPoint(ref m_position);
        }

        public GameObjectType GetGameObjectType()
        {
            return m_gameObjectType;
        }
    }
}