#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;
#endregion

namespace Juego2D
{
    abstract class LevelScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction, zoomInAction, zoomOutAction, debugAction;

        InputAction moveRightAction, moveLeftAction, moveUpAction, moveDownAction;

        #endregion

        private DebugViewXNA debugView;
        private SpriteFont sf;
        private SpriteSheet simple, special;

        protected GameWorld world;
        protected Camera2D camera { get { return world.camera; } }
        protected Player player;
        protected Scenario scenario;
        protected Texture2D background;

        protected bool debug;
        protected bool playerControl;

        protected List<Collectible> collectibles;
        // Renderer that draws particles to screen
        protected SpriteBatchRenderer sbRenderer;
        protected Renderer myRenderer;
        // Particle effect object to store the info about particle
        protected ParticleEffect myEffect;

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            debugAction = new InputAction(
                new Buttons[0],
                new Keys[] { Keys.F1 },
                true);

            zoomInAction = new InputAction(
                null,
                new Keys[] {Keys.Q},
                false);

            zoomOutAction = new InputAction(
                null,
                new Keys[] { Keys.E },
                false);


            #region player controls:
            moveDownAction = new InputAction(
                null,
                new Keys[] { Keys.S, Keys.Down },
                false);

            moveUpAction = new InputAction(
                new Buttons[] {Buttons.A, Buttons.DPadUp },
                new Keys[] { Keys.W, Keys.Up, Keys.Space },
                false);

            moveRightAction = new InputAction(
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
                new Keys[] { Keys.D, Keys.Right },
                false);

            moveLeftAction = new InputAction(
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
                new Keys[] { Keys.A, Keys.Left },
                false);
            #endregion


            world = null;

            player = null;
            scenario = null;
            debug = false;

            playerControl = true;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                #region Instantiating Fields:

                gameFont = ScreenManager.Font;

                world = new GameWorld(9.8f * Vector2.UnitY, 0.4f, new Camera2D(ScreenManager.GraphicsDevice));

                player = new Player(this, world, camera);
                player.Initialize();
                player.Position = new Vector2(0, 50);

                scenario = new Scenario(this, world, camera);
                scenario.Initialize();

                camera.Zoom = 7f;
                camera.trakingSpeedMult = new Vector2(0.5f, 0.25f);
                camera.trakingOffset = new Vector2(0f, -2f);
                camera.TrackingBody = player.getBody(0);
                camera.Update(new GameTime());
                camera.Jump2Target();

                debugView = new DebugViewXNA(world);
                debugView.AppendFlags(DebugViewFlags.DebugPanel | DebugViewFlags.PerformanceGraph);
                debugView.AppendFlags(DebugViewFlags.ContactNormals | DebugViewFlags.ContactPoints);
                debugView.DefaultShapeColor = Color.Black;
                debugView.SleepingShapeColor = Color.LightGray;

                collectibles = new List<Collectible>();
                myEffect = new ParticleEffect();
                myRenderer = new SpriteBatchRenderer
                {
                    GraphicsDeviceService = (IGraphicsDeviceService)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceService))
                };
                sbRenderer = new SpriteBatchRenderer
                {
                    GraphicsDeviceService = (IGraphicsDeviceService)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceService))
                };
                #endregion

                loadContent(content);

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        protected virtual void loadContent(ContentManager content) {
            player.loadContent(content);
            scenario.loadContent(content);

            sf = content.Load<SpriteFont>("Fonts/gamefont");

            if (background == null)
            {
                background = content.Load<Texture2D>("Levels/FondoTest");
            }

            foreach (Collectible c in collectibles)
            {
                c.loadContent(content);
            }

            myEffect = content.Load<ParticleEffect>(@"Particles/BasicExplosion");
            myEffect.LoadContent(content);
            myEffect.Initialise();
            myRenderer.LoadContent(content);
            sbRenderer.LoadContent(content);

            special = new SpriteSheet(ScreenManager.Game, content.Load<Texture2D>("collectiblePez"), 2,2, ScreenManager.SpriteBatch);
            special.scale = 40f / special.frameWidth;

            simple = new SpriteSheet(ScreenManager.Game, content.Load<Texture2D>("collectibleLeche"), 2, 2, ScreenManager.SpriteBatch);
            simple.scale = 40f / simple.frameWidth;
            simple.gotoAndStop(1);

            debugView.LoadContent(ScreenManager.Game.GraphicsDevice, content);
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
            player.Dispose();
            scenario.Dispose();
            myRenderer.Dispose();
            sbRenderer.Dispose();
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override sealed void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                UpdatePlaying(gameTime);
            }
            else
            {
                UpdatePaused(gameTime);
            }

            camera.Update(gameTime);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void UpdatePaused(GameTime gameTime)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void UpdatePlaying(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            player.Update(gameTime);

            float ZoomTarget = Geom.line(player.mainBody.LinearVelocity.Length(), 0f, 9f, 14f, 4f);
            float ZoomDif = camera.Zoom - ZoomTarget;
            float ZoomSign = Math.Sign(ZoomDif);
            double inertia;

            if (ZoomSign < 0f) inertia = Math.Pow(-ZoomDif / 2.5f, 1.2f);
            else inertia = Math.Pow(4f*ZoomDif, 1.7f);

            camera.Zoom -= seconds * (float)inertia * ZoomSign;
            world.Step(seconds);

            foreach (Collectible c in collectibles)
            {
                c.Update(gameTime);
                if (c.shootParticles == true)
                {
                    c.shootParticles = false;
                    myEffect.Trigger(c.Position);
                }
            }
            myEffect.Update(seconds);

        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override sealed void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                OnGameHandleInput(gameTime, input);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void OnGameHandleInput(GameTime gameTime, InputState input)
        {
            float time = (float) gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f;

            PlayerIndex playerI;
            if      (zoomInAction.Evaluate(input, ControllingPlayer, out playerI))  camera.Zoom *= 1.05f;
            else if (zoomOutAction.Evaluate(input, ControllingPlayer, out playerI)) camera.Zoom *= 0.95f;

            if (debugAction.Evaluate(input, ControllingPlayer, out playerI)) debug = !debug;

            if (playerControl)
            {
                bool jumping = moveUpAction.Evaluate(input, ControllingPlayer, out playerI);


                if (moveRightAction.Evaluate(input, ControllingPlayer, out playerI))
                {
                    player.moveRight(time);
                    if (jumping)
                    {
                        Vector2 dir = new Vector2(0.3f, -1f);
                        dir.Normalize();
                        player.moveUp(0f, dir);
                    }
                }
                else if (moveLeftAction.Evaluate(input, ControllingPlayer, out playerI))
                {
                    player.moveLeft(time);
                    if (jumping)
                    {
                        Vector2 dir = new Vector2(-0.3f, -1f);
                        dir.Normalize();
                        player.moveUp(0f, dir);
                    }
                }
                else
                {
                    player.stopMoving();
                    if (jumping) player.moveUp(0f);
                }

            }

            
        }



        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Viewport vp = ScreenManager.Game.GraphicsDevice.Viewport;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, new Rectangle(0, 0, vp.Width, vp.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.View);

            

            player.Draw(gameTime);
            scenario.Draw(gameTime);

            foreach (Collectible c in collectibles) c.Draw(gameTime);
            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, camera.View);
            sbRenderer.RenderEffect(myEffect, ScreenManager.SpriteBatch);
            ScreenManager.SpriteBatch.End();

            
            ScreenManager.SpriteBatch.Begin();
            simple.position = new Vector2(vp.Width - 175, 35);
            simple.Draw(gameTime);
            ScreenManager.SpriteBatch.DrawString(sf, 
                String.Format("{0}", UserProfileManger.getProfile().simpleRecolected), 
                new Vector2(vp.Width - 150, 30), Color.GreenYellow);

            special.position = new Vector2(vp.Width - 175, 75);
            special.Draw(gameTime);
            ScreenManager.SpriteBatch.DrawString(sf,
                String.Format("{0}", UserProfileManger.getProfile().specialRecoleced), 
                new Vector2(vp.Width - 150, 60), Color.GreenYellow);
                
            ScreenManager.SpriteBatch.End();


            Matrix projection = camera.SimProjection, view = camera.SimView;
            if (debug)
            {
                debugView.RenderDebugData(ref projection, ref view);

                debugView.BeginCustomDraw(ref projection, ref view);
                debugView.DrawPoint(camera.TargetPosition, 0.1f, Color.Red);

                debugView.DrawString(5, 5, "Cat speed: {0}\nposition {1}", player.mainBody.LinearVelocity.Length(), player.Position);
                debugView.DrawString(300, 5, "RCont {0}, LCont {1},RImp {2}, LImp {3}\nNormal Collect {4}\nSpecial Collect {5}", player.WheelContacs[0], player.WheelContacs[1], player.WheelContacs[2], player.WheelContacs[3], UserProfileManger.getProfile().simpleRecolected, UserProfileManger.getProfile().specialRecoleced);
            
                debugView.EndCustomDraw();
            }

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
            
        }


        #endregion
    }
}
