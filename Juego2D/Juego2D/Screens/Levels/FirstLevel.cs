#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;
using FarseerPhysics.Collision.Shapes;
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
            Vector2 scale = new Vector2(1f / 10f), scale2 = new Vector2(1f / 9f);

            scenario.loadFloor(content, "T0", 2f, new Vector2(-38f, 382f), ref scale2, 0);
            scenario.loadFloor(content, "T1", 0f, new Vector2(22f, 531f), ref scale2, 0);
            scenario.loadFloor(content, "T2", 1.5f, new Vector2(520f, 200f), ref scale, 0);
            scenario.loadFloor(content, "T3", 2f, new Vector2(1238f, 841f), ref scale, 0);
            scenario.loadFloor(content, "T3", 2f, new Vector2(1338f, 841f), ref scale, 0);
            scenario.loadFloor(content, "T4", 2f, new Vector2(1860f, 709f), ref scale, 0);
            scenario.loadFloor(content, "T5", 2f, new Vector2(1878f, 665f), ref scale, 0);

            Collectable col0 = new Collectable(this, new CircleShape(0.5f, 1f), world, camera);
            col0.Position = new Vector2(1626f, 993f);

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
