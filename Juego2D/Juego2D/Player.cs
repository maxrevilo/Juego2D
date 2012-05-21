#region Using Statements
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameStateManagement;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
#endregion

namespace Juego2D
{
    public class Player : PhysicObject
    {
        private const float wheelsFriction = 5f, speed = 5f, jumpImpulse = 6.7f;
        

        private uint rightwheelContacts;
        private uint leftwheelContacts;
        private uint wheelContacts { get { return rightwheelContacts + leftwheelContacts; } }

        private FixedAngleJoint StabilizeJoint;
        private bool jumped;


        public Fixture WheelR { get; set;}
        public Fixture WheelL { get; set;}
        public Vector2 Position
        {
            get { return mainBody.Position; }
            set { mainBody.Position = value; }
        }

        protected Texture2D image;

        #region Construction&Loading:
        public Player(GameScreen gameScreen, GameWorld world, Camera2D camera)
            : base(gameScreen, world, camera)
        {
            rightwheelContacts = 0;
            leftwheelContacts = 0;
            jumped = false;
        }

        public override void loadContent(ContentManager contentManager)
        {
            image = new Texture2D(gameScreen.ScreenManager.Game.GraphicsDevice, 1, 1);
            image.SetData<Color>(new Color[1] { Color.White });
        }

        public override void Initialize()
        {
            const float width = 1f;
            const float height = 0.5f;

            physics.bodys = new Body[1]{new Body(physics.world)};

            mainBody.BodyType = BodyType.Dynamic;

            FixtureFactory.AttachRectangle(width, height, 1f, Vector2.Zero, mainBody);
            FixtureFactory.AttachCircle(0.25f, 0f, mainBody, new Vector2(-0.40f, -0.1f)).Friction = 0f;
            FixtureFactory.AttachCircle(0.25f, 0f, mainBody, new Vector2(0.40f, -0.1f)).Friction = 0f;

            WheelL = FixtureFactory.AttachCircle(0.2f, 0f, mainBody, new Vector2(-0.4f, 0.25f));
            WheelL.Friction = wheelsFriction;
            WheelL.OnCollision += WheelContactHandler;
            WheelL.OnSeparation += WheelSeparateHandler;

            WheelR = FixtureFactory.AttachCircle(0.2f, 0f, mainBody, new Vector2(0.4f, 0.25f));
            WheelR.Friction = wheelsFriction;
            WheelR.OnCollision += WheelContactHandler;
            WheelR.OnSeparation += WheelSeparateHandler;

            
            StabilizeJoint = JointFactory.CreateFixedAngleJoint(physics.world, mainBody);
            StabilizeJoint.MaxImpulse = 1f / 200f;
            StabilizeJoint.TargetAngle = 0;

            mainBody.Restitution = 0f;

            base.Initialize();
        }


        protected override void Dispose(bool disposing)
        {
            image.Dispose();
            base.Dispose(disposing);
        }
        #endregion

        #region Update&Draw:
        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Correcting angle:
            mainBody.Rotation = mainBody.Rotation % MathHelper.TwoPi;
            //This object wont get high algunar speeds:
            mainBody.AngularVelocity = 0;
            //Stabilizators logic:
            StabilizeJoint.Enabled = false;
            if (leftwheelContacts == 0)
            {
                if (rightwheelContacts > 0)
                {
                    Vector2 worldImpulse = mainBody.GetWorldVector(Vector2.UnitY) * 10f * seconds;
                    Vector2 worldPoint   = mainBody.GetWorldPoint( ((CircleShape)WheelL.Shape).Position );
                    mainBody.ApplyLinearImpulse(worldImpulse, worldPoint);
                }
                else
                {
                    StabilizeJoint.Enabled = true;
                    StabilizeJoint.TargetAngle = 0;
                }
            }
            else
            {
                if (rightwheelContacts == 0)
                {
                    Vector2 worldImpulse = mainBody.GetWorldVector(Vector2.UnitY) * 10f * seconds;
                    Vector2 worldPoint = mainBody.GetWorldPoint( ((CircleShape)WheelR.Shape).Position );
                    mainBody.ApplyLinearImpulse(worldImpulse, worldPoint);
                }
                else
                {
                }
            }


            if (wheelContacts == 0) jumped = false;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            SpriteBatch spriteBatch = gameScreen.ScreenManager.SpriteBatch;
            Viewport viewport = gameScreen.ScreenManager.GraphicsDevice.Viewport;
            Vector2 pos;
            float size = 2f * getBody(0).FixtureList[0].Shape.Radius;
            pos.X = physics.bodys[0].Position.X - size/2f;
            pos.Y = physics.bodys[0].Position.Y - size/2f;
            float scale = size / image.Width;

            spriteBatch.Begin(0, null, null, null, null, null, camera.View );

            spriteBatch.Draw(image, pos, null, Color.Red, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region controls:
        public bool moveRight(float time)
        {
            if (wheelContacts > 0)
            {
                mainBody.ApplyLinearImpulse(mainBody.GetWorldVector(Vector2.UnitX) * speed * time);
                startMoving();
                return true;
            }
            else return false;
        }

        public bool moveLeft(float time)
        {
            if (wheelContacts > 0)
            {
                mainBody.ApplyLinearImpulse(mainBody.GetWorldVector(-Vector2.UnitX) * speed * time);
                startMoving();
                return true;
            }
            else return false;
        }

        public bool moveUp(float time)
        {
            if (wheelContacts > 0 && !jumped)
            {
                Console.WriteLine("Jump at " + mainBody.Position.ToString());
                mainBody.ApplyLinearImpulse(mainBody.GetWorldVector(-Vector2.UnitY) * jumpImpulse);
                jumped = true;
                return true;
            }
            else return false;
        }

        public void startMoving()
        {
            WheelL.Friction = 0;
            WheelR.Friction = 0;
        }

        public void stopMoving()
        {
            WheelL.Friction = wheelsFriction;
            WheelR.Friction = wheelsFriction;
        }

       

        #endregion
        
        private bool WheelContactHandler(Fixture Wheel, Fixture floor, Contact contact)
        {
            if (Wheel == WheelL) leftwheelContacts++;
            else rightwheelContacts++;
            return true;
        }

        private void WheelSeparateHandler(Fixture Wheel, Fixture floor)
        {
            if (Wheel == WheelL) leftwheelContacts--;
            else rightwheelContacts--;
        }
        
    }
}
