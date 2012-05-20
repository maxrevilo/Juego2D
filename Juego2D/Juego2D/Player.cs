#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameStateManagement;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
#endregion

namespace Juego2D
{
    public class Player : PhysicObject
    {
        protected Texture2D image;

        public Player(GameScreen gameScreen, World world, Camera2D camera)
            : base(gameScreen, world, camera)
        {
        }

        public override void loadContent(ContentManager contentManager)
        {
            image = new Texture2D(gameScreen.ScreenManager.Game.GraphicsDevice, 1, 1);
            image.SetData<Color>(new Color[1] { Color.White });
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            image.Dispose();
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = gameScreen.ScreenManager.SpriteBatch;
            Viewport viewport = gameScreen.ScreenManager.GraphicsDevice.Viewport;
            Rectangle rec;
            rec.X = (int)physics.bodys[0].Position.X;
            rec.Y = (int)physics.bodys[0].Position.Y;
            rec.Width = (int)physics.bodys[0].FixtureList[0].Shape.Radius;
            rec.Height = rec.Width;

            spriteBatch.Begin(0, null, null, null, null, null, camera.View );

            spriteBatch.Draw(image, rec, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
