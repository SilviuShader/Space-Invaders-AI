using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGame
{
    public struct AlienBehaviour
    {
        public double[] timesToShoot;
    };
    public struct RowBehaviours
    {
        public AlienBehaviour[] alienBehaviours;
    };
    public class SpaceInvaders
    {
        public const int    SCREEN_WIDTH            = 100;
        public const int    SCREEN_HEIGHT           = 100;
        public const int    ALIENS_PER_ROW          = 6;
        public const int    ALIEN_ROWS              = 6;
        public const double DISTANCE_BETWEEN_ALIENS = 12;
        public const double DISTANCE_BETWEEN_ROWS   = 7.5;

        private ContentManager    m_content;
        private Texture2D         m_castleTexture;
        private Texture2D         m_blockTexture;
        private GameRenderer      m_gameRenderer;
        private RowBehaviours[]   m_rowsBehaviours;
        private List<GameObject>  m_gameObjects;
        private Queue<GameObject> m_pendingObjects;
        private Physics           m_physics;
        private Alien             m_leftVirtualAlien;

        private bool              m_aliensGoingRight;
        private double            m_blocksHeight;

        private Random            m_random;

        private int[]             m_aliensAlivePerLayer;
        
        public          SpaceInvaders(ContentManager content)
        {
            m_blocksHeight = 30.0;
            m_content = content;
            m_castleTexture = m_content.Load<Texture2D>("Castle");
            m_blockTexture = m_content.Load<Texture2D>("Block");

            m_gameRenderer = new GameRenderer(SCREEN_WIDTH, SCREEN_HEIGHT);
            m_pendingObjects = new Queue<GameObject>();

            m_physics = new Physics();

            m_gameObjects = new List<GameObject>();

            m_aliensAlivePerLayer = new int[ALIEN_ROWS];

            ReadAliensBehaviour();

            Instantiate(new Player(content, new SPoint(50.0, 10.0), this));
            for(int i = 0; i < ALIEN_ROWS; i++)
                CreateAlienRow(SCREEN_HEIGHT - ((i + 1) * DISTANCE_BETWEEN_ROWS), i);
            
            m_aliensGoingRight = true;

            m_random = new Random();

            CreateCastle(new SPoint(20.0, m_blocksHeight));
            CreateCastle(new SPoint(40.0, m_blocksHeight));
            CreateCastle(new SPoint(60.0, m_blocksHeight));
            CreateCastle(new SPoint(80.0, m_blocksHeight));
        }

        public void     Update(double timeStep, double screenWidth, double screenHeight)
        {
            m_gameRenderer.Update(screenWidth, screenHeight);

            for(int i = 0; i < m_pendingObjects.Count; i++)
                m_gameObjects.Add(m_pendingObjects.Dequeue());

            foreach(var gameObject in m_gameObjects)
                gameObject.Update(timeStep);
            
            m_physics.Update(ref m_gameObjects);

            UpdateLastVirtualAlien();
            UpdateObstacles();
            
            m_gameObjects.RemoveAll(x => x.IsDead());
        }

        public void     Render(SpriteBatch spriteBatch)
        {
            m_gameRenderer.RenderSprite(spriteBatch, m_blockTexture, new SPoint(SCREEN_WIDTH / 2.0, SCREEN_HEIGHT / 2.0), SCREEN_WIDTH, Color.Black);
            foreach(var gameObject in m_gameObjects)
                gameObject.Render(m_gameRenderer, spriteBatch);
        }

        public void     Instantiate(GameObject obj)
        {
            m_pendingObjects.Enqueue(obj);
        }

        public void      UpdateAliensDirection(bool newDirection)
        {
            m_aliensGoingRight = newDirection;

            foreach(var gameObject in m_gameObjects)
                if(gameObject.GetGameObjectType() == GameObjectType.Alien)
                {
                    ((Alien)gameObject).ChangeDirection(newDirection);
                    gameObject.Translate(new SVector2D(0.0, -DISTANCE_BETWEEN_ROWS));
                }
        }

        public Random    GetRandom()
        {
            return m_random;
        }

        public ref int[] GetAliensCountPerRow()
        {
            return ref m_aliensAlivePerLayer;
        }

        private void CreateCastle(SPoint origin)
        {
            Color[] rawData = new Color[m_castleTexture.Width * m_castleTexture.Height];
            m_castleTexture.GetData(rawData);
            for(int i = 0; i < m_castleTexture.Height; i++)
            {
                for(int j = 0; j < m_castleTexture.Width; j++)
                {
                    Color pixel = rawData[i * m_castleTexture.Width + j];
                    if(pixel.A >= 0.5)
                    {
                        Instantiate(new Block(m_content, 
                                              new SPoint(origin.x + (j - m_castleTexture.Width / 2.0) * Block.BLOCK_WIDTH, 
                                              origin.y + ((m_castleTexture.Height - i) - (m_castleTexture.Height / 2.0)) * Block.BLOCK_WIDTH), 
                                              this));
                    }
                }
            }
        } 

        private void ReadAliensBehaviour()
        {
            using (StreamReader file = new StreamReader("Content/aliens.txt"))  
            {
                string text = file.ReadLine();
                string[] bits = text.Split(' ');

                int rows = Int32.Parse(bits[0]);
                int columns = Int32.Parse(bits[1]);

                m_rowsBehaviours = new RowBehaviours[rows];

                for(int i = 0; i < rows; i++)
                {
                    m_rowsBehaviours[i] = new RowBehaviours();
                    m_rowsBehaviours[i].alienBehaviours = new AlienBehaviour[columns];
                    for(int j = 0; j < columns; j++)
                    {
                        m_rowsBehaviours[i].alienBehaviours[j] = new AlienBehaviour();
                        text = file.ReadLine();
                        bits = text.Split(' ');

                        int lgh = Int32.Parse(bits[0]);
                        m_rowsBehaviours[i].alienBehaviours[j].timesToShoot = new double[lgh];
                        for(int k = 1; k <= lgh; k++)
                        {
                            m_rowsBehaviours[i].alienBehaviours[j].timesToShoot[k - 1] = Double.Parse(bits[k]);
                        }
                    }
                }
            }
        }

        private void CreateAlienRow(double startHeight, int layer)
        {
            SPoint startingPos = new SPoint(Alien.ALIEN_WIDTH / 2.0, 
                                            startHeight - (Alien.ALIEN_WIDTH / 2.0)); // it should be height, but it work because it's a square
            
            for(int i = 0; i < ALIENS_PER_ROW; i++)
            {
                Instantiate(new Alien(m_content, startingPos, this, false, layer, m_rowsBehaviours[layer].alienBehaviours[i]));
                
                if(i == 0 || i == ALIENS_PER_ROW - 1)
                {
                    Alien alien = new Alien(m_content, startingPos, this, true, layer, m_rowsBehaviours[layer].alienBehaviours[i]);
                        if(i == 0)
                            m_leftVirtualAlien = alien;
                    Instantiate(alien);
                }

                startingPos += new SVector2D(DISTANCE_BETWEEN_ALIENS, 0.0);
            }
        }

        private void UpdateLastVirtualAlien()
        {
            int selectedIndex = 0;
            for(int i = 0; i < m_aliensAlivePerLayer.Length; i++)
                if(m_aliensAlivePerLayer[i] > 2)
                    selectedIndex = i;
            
            foreach(var gameObject in m_gameObjects)
            {
                if(gameObject.GetGameObjectType() == GameObjectType.Alien)
                {
                    Alien alien = (Alien)gameObject;
                    if(alien.GetLayer() == selectedIndex)
                    {
                        if(alien.IsVirtual())
                        {
                            m_leftVirtualAlien = alien;
                        }
                    }
                }
            }
        }

        private void UpdateObstacles()
        {
            if(m_blocksHeight + (Block.BLOCK_WIDTH * m_castleTexture.Height) / 2.0 > 
               m_leftVirtualAlien.GetPosition().y - (m_leftVirtualAlien.GetHeight() / 2.0))
            {
                foreach(var gameObject in m_gameObjects)
                    if(gameObject.GetGameObjectType() == GameObjectType.Block)
                        gameObject.Kill();
            }
        }
    }
}