using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
    class Root : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpaceInvaders m_spaceInvaders;

        private bool m_wasResizeCalled;

        public void OnResize(Object sender, EventArgs e)
        {
            m_wasResizeCalled = true;
        }

        public Root()
        {
            graphics = new GraphicsDeviceManager(this);

            // Window
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 480;

            // Fullscreen Window (Max Resolution)
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // Fullscreen (Max Resolution)
            graphics.IsFullScreen = true;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            m_wasResizeCalled = false;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Log.Clear();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            Log.Print("Terminal Active\n");
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_spaceInvaders = new SpaceInvaders(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(m_wasResizeCalled)
            {
                ApplyResizeChanges();
                m_wasResizeCalled = false;
            }

            m_spaceInvaders.Update(gameTime.ElapsedGameTime.TotalSeconds, 
                                   graphics.PreferredBackBufferWidth, 
                                   graphics.PreferredBackBufferHeight);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(new Color(0.1f, 0.1f, 0.1f));

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            m_spaceInvaders.Render(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ApplyResizeChanges()
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.ApplyChanges();
        }
    }
}
