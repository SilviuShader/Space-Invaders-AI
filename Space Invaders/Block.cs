using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGame
{
    public class Block : GameObject
    {
        public const double BLOCK_WIDTH = 0.5;
        public Block(ContentManager content, 
                     SPoint         position,
                     SpaceInvaders  gameRoot) :
                     base(content, position, "Block", BLOCK_WIDTH, gameRoot)
        {
            m_gameObjectType = GameObjectType.Block;
        }

        public override void OnCollisionEnter(GameObject otherObject)
        {
            if(otherObject.GetGameObjectType() == GameObjectType.Bullet)
            {
                Kill();
                otherObject.Kill();
            }
        }

        public override void Render(GameRenderer gameRenderer, SpriteBatch spriteBatch)
        {
            Render(gameRenderer, spriteBatch, new Color(0.0f, 1.0f, 0.0f));
        }
    }
}