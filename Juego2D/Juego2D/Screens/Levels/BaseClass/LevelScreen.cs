#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;
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

        InputAction pauseAction, zoomInAction, zoomOutAction;

        InputAction moveRightAction, moveLeftAction, moveUpAction, moveDownAction;

        #endregion

        private DebugViewXNA debugView;

        protected GameWorld world;
        protected Camera2D camera { get { return world.camera; } }
        protected Player player;
        protected Scenario scenario;
        protected Texture2D background;

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
                null,
                new Keys[] { Keys.W, Keys.Up },
                false);

            moveRightAction = new InputAction(
                null,
                new Keys[] { Keys.D, Keys.Right },
                false);

            moveLeftAction = new InputAction(
                null,
                new Keys[] { Keys.A, Keys.Left },
                false);
            #endregion


            world = null;

            player = null;
            scenario = null;
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

                camera.Zoom = 5f;
                camera.TrackingBody = player.getBody(0);
                camera.Update(new GameTime());
                camera.Jump2Target();

                debugView = new DebugViewXNA(world);
                debugView.AppendFlags(DebugViewFlags.DebugPanel | DebugViewFlags.PerformanceGraph);
                debugView.AppendFlags(DebugViewFlags.ContactNormals | DebugViewFlags.ContactPoints);
                debugView.DefaultShapeColor = Color.Black;
                debugView.SleepingShapeColor = Color.LightGray;

                #endregion

                loadContent(content);

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        protected virtual void loadContent(ContentManager content) {
            player.loadContent(content);
            scenario.loadContent(content);

            if (background == null)
            {
                background = content.Load<Texture2D>("Levels/FondoTest");
            }

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

            debugView.DrawString(5, 5, "Cat speed: {0}", player.mainBody.LinearVelocity.Length());

            world.Step(seconds);
            
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
                //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
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
            else {
                player.stopMoving();
                if (jumping) player.moveUp(0f);
            }

            
        }



        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, new Rectangle(0,0,1024, 600) , Color.White);
            ScreenManager.SpriteBatch.End();

            //player.Draw(gameTime);
            //scenario.Draw(gameTime);

            Matrix projection = camera.SimProjection, view = camera.SimView;
            debugView.RenderDebugData(ref projection, ref view);

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
