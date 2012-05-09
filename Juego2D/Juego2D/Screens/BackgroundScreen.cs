#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;
#endregion

namespace Juego2D
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;

        #endregion

        #region Farseer&Mercury Test:
        World world;
        Body myBody;
        CircleShape circleShape;
        Fixture fixture;


        // Renderer that draws particles to screen
        Renderer myRenderer;
        // Particle effect object to store the info about particle
        ParticleEffect myEffect;

        Texture2D tex;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            #region Farseer&Mercury Test:
            myEffect = new ParticleEffect();
            #endregion
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                //backgroundTexture = content.Load<Texture2D>("Backgrounds/blank");


                #region Farseer&Mercury Test:
                myRenderer = new SpriteBatchRenderer
                {
                    GraphicsDeviceService = (IGraphicsDeviceService)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceService))
                };



                world = new World(9.8f * Vector2.UnitY);
                myBody = BodyFactory.CreateBody(world);
                myBody.BodyType = BodyType.Dynamic;

                circleShape = new CircleShape(0.5f, 1f);

                fixture = myBody.CreateFixture(circleShape);
                #endregion

                #region Farseer&Mercury Test:
                tex = new Texture2D(ScreenManager.Game.GraphicsDevice, 1, 1);

                tex.SetData<Color>(new Color[1] { Color.White });

                myEffect = content.Load<ParticleEffect>(@"Particles/BasicExplosion");
                myEffect.LoadContent(content);
                myEffect.Initialise();
                myRenderer.LoadContent(content);
                #endregion
            }
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

            #region Farseer&Mercury Test:
            myRenderer.Dispose();
            #endregion
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            #region Farseer&Mercury Test:
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            world.Step(seconds);



            // get the latest mouse state
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                myEffect.Trigger(new Vector2(ms.X, ms.Y));
            }

            myEffect.Update(seconds);
            #endregion


            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            #region Farseer&Mercury Test:
            spriteBatch.Draw(tex, new Rectangle((int)myBody.Position.X, (int)myBody.Position.Y, 64, 64), Color.Red);
            #endregion

            /*spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            */
            spriteBatch.End();

            #region Farseer&Mercury Test:
            myRenderer.RenderEffect(myEffect);
            #endregion
        }


        #endregion
    }
}
