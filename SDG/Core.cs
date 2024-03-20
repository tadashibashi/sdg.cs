using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SDG
{
    public class Core : Game
    {
        private GraphicsDeviceManager _graphics;
        
        private static Core _instance;
        public static Core Instance => _instance;

        public new SdgContentManager Content { get; private set; }
        
        public new GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        public Core()
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content = new SdgContentManager(this);
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }
    }
}
