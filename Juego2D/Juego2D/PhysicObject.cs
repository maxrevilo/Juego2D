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
    public abstract class PhysicObject : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected struct Physics
        {
            public World     world;
            public Body[]    bodys;
        }

        protected GameScreen gameScreen;
        protected Physics    physics;
        protected Camera2D   camera;

        public PhysicObject(GameScreen gameScreen, World world, Camera2D camera)
            : base(gameScreen.ScreenManager.Game)
        {
            this.gameScreen = gameScreen;
            
            physics.world = world;
            physics.bodys = null;

            this.camera = camera;
        }

        public abstract void loadContent(ContentManager contentManager);

        public override void Initialize()
        {
            if (physics.bodys == null)
            {
                physics.bodys = new Body[1] { new Body(physics.world) };
                physics.bodys[0].BodyType = BodyType.Dynamic;
                physics.bodys[0].CreateFixture(new CircleShape(1f, 1f));
            }

            base.Initialize();
        }

        public Body getBody(int index) { return physics.bodys[index]; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
