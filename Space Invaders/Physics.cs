using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGame
{
    public class Physics
    {
        public Physics()
        {

        }

        public void Update(ref List<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {

                bool valid = true;
                if(gameObject.GetGameObjectType() == GameObjectType.Alien)
                    if(((Alien)gameObject).IsVirtual())
                        valid = false;
                
                if(valid)
                    foreach(var other in gameObjects)
                    {
                        valid = true;
                        if(gameObject != other)
                        {
                            if(other.GetGameObjectType() == GameObjectType.Alien)
                                if(((Alien)other).IsVirtual())
                                    valid = false;
                            if(other.GetGameObjectType() == GameObjectType.Block)
                                valid = false;
                            
                            if(gameObject.GetGameObjectType() == GameObjectType.Block &&
                               other.GetGameObjectType() != GameObjectType.Bullet)
                                valid = false;
                            
                            if(valid)
                            {

                                bool collide = false;
                                collide = collide || ObjectOverlaps(gameObject, other);
                                collide = collide || ObjectOverlaps(other, gameObject);
                                
                                if(collide)
                                {
                                    gameObject.OnCollisionEnter(other);
                                }
                            }
                        }
                    }
            }
        }

        private bool ObjectOverlaps(GameObject gameObject, GameObject other)
        {
            bool collide = false;

            double objWidth = gameObject.GetWidth();
            double objHeight = gameObject.GetHeight();
            double otherWidth = other.GetWidth();
            double otherHeight = other.GetHeight();

            collide = collide || PointInsideRect(gameObject.GetPosition() + new SVector2D(objWidth / 2.0, objHeight / 2.0), 
                                                 other.GetPosition(), 
                                                 otherWidth, 
                                                 otherHeight);
            collide = collide || PointInsideRect(gameObject.GetPosition() + new SVector2D(objWidth / 2.0, -objHeight / 2.0), 
                                                 other.GetPosition(), 
                                                 otherWidth, 
                                                 otherHeight);
            collide = collide || PointInsideRect(gameObject.GetPosition() + new SVector2D(-objWidth / 2.0, objHeight / 2.0), 
                                                 other.GetPosition(), 
                                                 otherWidth, 
                                                 otherHeight);
            collide = collide || PointInsideRect(gameObject.GetPosition() + new SVector2D(-objWidth / 2.0, -objHeight / 2.0), 
                                                 other.GetPosition(), 
                                                 otherWidth, 
                                                 otherHeight);
            
            return collide;
        }

        private bool PointInsideRect(SPoint point, SPoint center, double width, double height)
        {
            if(point.x >= center.x - (width  / 2.0) && point.x <= center.x + (width  / 2.0) &&
               point.y >= center.y - (height / 2.0) && point.y <= center.y + (height / 2.0))
               return true;
            
            return false;
        }
    }
}