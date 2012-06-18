using System;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Input;

using GameStateManagement;


namespace Juego2D
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        ScreenManager screenManager;
        ScreenFactory screenFactory;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1440;
            graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";
            
            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this, "Backgrounds/blank", "Fonts/menufont");
            Components.Add(screenManager);
            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            //Pantalla de fondo a los menus:
            screenManager.AddScreen(new BackgroundScreen(), null);

            //Menu principal:
            screenManager.AddScreen(new MainMenuScreen(), null);
        }


        protected override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent() { }


        protected override void UnloadContent() { }


        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
