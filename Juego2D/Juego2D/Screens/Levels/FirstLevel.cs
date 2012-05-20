#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;
#endregion


namespace Juego2D
{
    class FirstLevel : LevelScreen
    {
        public FirstLevel()
            :base()
        {


        }


        protected override void loadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.loadContent(content);
        }

        protected override void UpdatePaused(GameTime gameTime)
        {
            base.UpdatePaused(gameTime);
        }

        protected override void UpdatePlaying(GameTime gameTime)
        {
            base.UpdatePlaying(gameTime);
        }

        protected override void OnGameHandleInput(GameTime gameTime, InputState input)
        {
            base.OnGameHandleInput(gameTime, input);
        }

    }
}
