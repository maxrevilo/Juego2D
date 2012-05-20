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

        #endregion

        private DebugViewXNA debugView;

        protected World world;
        protected Camera2D camera;
        protected Player player;
        protected Scenario scenario;

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
                new Keys[] {Keys.Up},
                false);

            zoomOutAction = new InputAction(
                null,
                new Keys[] { Keys.Down },
                false);

            camera = null;
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

                world = new World(0.98f * Vector2.UnitY);
                camera = new Camera2D(ScreenManager.GraphicsDevice);

                player = new Player(this, world, camera);
                player.Initialize();
                player.getBody(0).Position = new Vector2(400, 300);

                scenario = new Scenario(this, world, camera);
                scenario.Initialize();
                
                camera.TrackingBody = player.getBody(0);
                camera.Update(new GameTime());
                camera.Jump2Target();

                debugView = new DebugViewXNA(world);
                debugView.AppendFlags(DebugViewFlags.DebugPanel);
                debugView.DefaultShapeColor = Color.White;
                debugView.SleepingShapeColor = Color.LightGray;

                #endregion

                loadContent(content);

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        protected virtual void loadContent(ContentManager content) {
            player.loadContent(content);
            scenario.loadContent(content);

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

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
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
            PlayerIndex player;
            if      (zoomInAction.Evaluate(input, ControllingPlayer, out player))  camera.Zoom *= 1.2f;
            else if (zoomOutAction.Evaluate(input, ControllingPlayer, out player)) camera.Zoom *= 0.8f;
        }



        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            player.Draw(gameTime);
            scenario.Draw(gameTime);

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
