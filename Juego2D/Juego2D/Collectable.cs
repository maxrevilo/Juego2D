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
using FarseerPhysics.Dynamics.Contacts;
#endregion

namespace Juego2D
{
    public abstract class Collectible : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields:

        #region Protected:

        protected GameScreen gameScreen;
        protected Camera2D camera;
        protected Body body;
        protected World world;

        protected SpriteSheet image;
        protected bool collected;

        #endregion

        public bool shootParticles;
        #endregion



        public Collectible(GameScreen gameScreen, Shape shape, World world, Camera2D camera)
            : base(gameScreen.ScreenManager.Game)
        {
            this.gameScreen = gameScreen;

            this.world = world;
            body = new Body(world);
            body.CreateFixture(shape);
            body.CollisionCategories = Category.Cat3;
            body.CollidesWith = Category.Cat2;
            body.BodyType = BodyType.Kinematic;

            body.OnCollision += body_OnCollision;

            shootParticles = false;
            collected = false;

            this.camera = camera;
        }

        public Vector2 Position
        {
            get { return body.Position; }
            set { body.Position = value; }
        }

        public abstract void loadContent(ContentManager contentManager);

        public override void Initialize()
        {
            base.Initialize();
        }

        private bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (Collected() && !collected)
            {
                world.RemoveBody(body);
                collected = true;
                shootParticles = true;
            }
            return false;
        }

        protected abstract bool Collected();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (image != null)
            {
                image.position = Position;
                image.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (image != null) image.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
