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
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
#endregion

namespace Juego2D
{
    public class Player : PhysicObject
    {
        private const float wheelsFriction = 1.0f, speed = 37f, jumpImpulse = 4.7f;
        

        private uint rightwheelContacts;
        private uint leftwheelContacts;
        private uint wheelContacts { get { return rightwheelContacts + leftwheelContacts; } }

        private float rightwheelNimpulse;
        private float leftwheelNimpulse;
        private float wheelNimpulse { get { return rightwheelNimpulse + leftwheelNimpulse; } }

        private float rightwheelTimpulse;
        private float leftwheelTimpulse;
        private float wheelTimpulse { get { return rightwheelTimpulse + leftwheelTimpulse; } }
        
        private float leftwheelFCoef;
        private float rightwheelFCoef;
        private float wheelFCoef {
            get {
                return 
                    ((rightwheelFCoef > 0.01 ? rightwheelFCoef : leftwheelFCoef) + 
                    (leftwheelFCoef > 0.01 ? leftwheelFCoef : rightwheelFCoef)) 
                    / 2f; 
            }
        }

        private FixedAngleJoint StabilizeJoint;
        private bool moving;
        private bool jumped { get { return jumpedTime < .2; } }
        private float timeOnAir, jumpedTime;
        private bool lookingLeft;

        public bool onAir { get { return timeOnAir > 0.2f; } }
        public bool isMoving { get { return moving; } }

        public float[] WheelContacs { get { return new float[]{rightwheelContacts, leftwheelContacts, rightwheelNimpulse, leftwheelNimpulse}; }}

        public Fixture WheelR { get; set;}
        public Fixture WheelL { get; set;}
        public Vector2 Position
        {
            get { return mainBody.Position; }
            set { mainBody.Position = value; }
        }

        protected Texture2D image;
        protected SpriteSheet sprite, standSprite, runSprite, walkSprite, fallSprite;

        #region Construction&Loading:
        public Player(GameScreen gameScreen, GameWorld world, Camera2D camera)
            : base(gameScreen, world, camera)
        {
            rightwheelContacts = 0;
            leftwheelContacts = 0;
            jumpedTime = 0f;
            lookingLeft = false;
            timeOnAir = 0f;
            moving = false;

            resetNTImpulses();
        }

        public override void loadContent(ContentManager contentManager)
        {
            image = new Texture2D(gameScreen.ScreenManager.Game.GraphicsDevice, 1, 1);
            image.SetData<Color>(new Color[1] { Color.White });

            //WALK SPRITE:
            walkSprite = new SpriteSheet(
                gameScreen.ScreenManager.Game, 
                contentManager.Load<Texture2D>("walkSprite"), 
                2, 5,
                gameScreen.ScreenManager.SpriteBatch)
                {
                    framesPerSecond = 10f,
                    rotationCenter = new Vector2(128, 152),
                    depth = .5f,
                };
            walkSprite.scale = 2f / walkSprite.Width;

            //RUN SPRITE:
            runSprite = new SpriteSheet(
                gameScreen.ScreenManager.Game,
                contentManager.Load<Texture2D>("runSprite"),
                2, 5,
                gameScreen.ScreenManager.SpriteBatch)
            {
                framesPerSecond = 10f,
                rotationCenter = new Vector2(128, 152),
                depth = .5f,
            };
            runSprite.scale = 2f / runSprite.Width;
            
            //FALL SPRITE:
            fallSprite = new SpriteSheet(
                gameScreen.ScreenManager.Game,
                contentManager.Load<Texture2D>("fallSprite"),
                2, 3,
                gameScreen.ScreenManager.SpriteBatch)
            {
                framesPerSecond = 6f,
                rotationCenter = new Vector2(128, 152),
                depth = .5f,
            };
            fallSprite.scale = 2f / fallSprite.Width;

            standSprite = new SpriteSheet(
                gameScreen.ScreenManager.Game,
                contentManager.Load<Texture2D>("standSprite"),
                2, 3,
                gameScreen.ScreenManager.SpriteBatch)
            {
                framesPerSecond = 3f,
                rotationCenter = new Vector2(128, 152),
                depth = .5f,
            };
            standSprite.scale = 2f / standSprite.Width;


            sprite = fallSprite;

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
            WheelL.AfterCollision += WheelAfterContactHandler;
            WheelL.OnCollision    += WheelContactHandler;
            WheelL.OnSeparation   += WheelSeparateHandler;

            WheelR = FixtureFactory.AttachCircle(0.2f, 0f, mainBody, new Vector2(0.4f, 0.25f));
            WheelR.Friction = wheelsFriction;
            WheelR.AfterCollision += WheelAfterContactHandler;
            WheelR.OnCollision    += WheelContactHandler;
            WheelR.OnSeparation   += WheelSeparateHandler;
            
            StabilizeJoint = JointFactory.CreateFixedAngleJoint(physics.world, mainBody);


            mainBody.Restitution = 0f;
            mainBody.CollisionCategories = Category.Cat2;
            mainBody.CollidesWith = Category.Cat1 | Category.Cat3;

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

            #region Stabilization:
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
                    StabilizeJoint.MaxImpulse = 1 / 200f;
                    StabilizeJoint.TargetAngle = 0f;
                    if (onAir)
                    {
                        StabilizeJoint.MaxImpulse = float.MaxValue;
                        StabilizeJoint.TargetAngle = MathHelper.ToRadians(lookingLeft ? 20f : -20f);
                    }
                }
            }
            else
            {
                if (rightwheelContacts == 0)
                {
                    Vector2 worldImpulse = mainBody.GetWorldVector(Vector2.UnitY) * 10f * seconds;
                    Vector2 worldPoint = mainBody.GetWorldPoint(((CircleShape)WheelR.Shape).Position);
                    mainBody.ApplyLinearImpulse(worldImpulse, worldPoint);
                }
                else
                {
                }
            }
            #endregion

            

            if (wheelContacts == 0) timeOnAir += seconds;
            else timeOnAir = 0f;
            jumpedTime += seconds;

            float linearSpeed = mainBody.LinearVelocity.Length();
            if (!onAir && isMoving)
            {
                
                if (linearSpeed < 10f)
                {
                    sprite = walkSprite;
                    sprite.framesPerSecond = Geom.line(linearSpeed, 0f, 4f, 10f, 18f);
                }
                else
                {
                    sprite = runSprite;
                    sprite.framesPerSecond = Geom.line(linearSpeed, 10f, 8f, 20f, 24f);
                }
                sprite.play();
            }
            else if (onAir)
            {
                sprite = fallSprite;
                sprite.play();
            }
            else if (linearSpeed > 0.1f)
            {
                sprite = runSprite;
                sprite.gotoAndStop(9);
            }
            else
            {
                sprite = standSprite;
                sprite.play();
            }

            if(mainBody.Awake) resetNTImpulses();

            sprite.rotation = getBody(0).Rotation;
            sprite.position = getBody(0).Position;
            sprite.Update(gameTime);

            base.Update(gameTime);

            
        }

        public override void Draw(GameTime gameTime)
        {
            sprite.spriteEffects = lookingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.view = camera.View;
            sprite.Draw(gameTime);

            base.Draw(gameTime);
        }
        #endregion

        #region controls:
        private Vector2 moveForce(Vector2 local_direction, bool traction = true) {
            float tractionCoef = traction ? Math.Min(wheelsFriction, wheelFCoef) * Math.Min(wheelNimpulse, 0.4f) : 0.05f;
            return speed * tractionCoef * mainBody.GetWorldVector(local_direction);
        }

        public bool moveRight(float time)
        {
            lookingLeft = false;
            startMoving();
            mainBody.ApplyLinearImpulse(moveForce(Vector2.UnitX, wheelContacts > 0) * time);
            return true;
        }

        public bool moveLeft(float time)
        {
            lookingLeft = true;
            startMoving();
            mainBody.ApplyLinearImpulse(moveForce(-Vector2.UnitX, wheelContacts > 0) * time);
            return true;
        }

        public bool moveUp(float time)
        {
           return moveUp(time, -Vector2.UnitY);
        }

        public bool moveUp(float time, Vector2 direction)
        {
            if (wheelContacts > 0 && !jumped)
            {
                //Posibly wall jump:
                if (rightwheelContacts == 0 || leftwheelContacts == 0)
                {
                    lookingLeft = leftwheelContacts == 0;
                    direction = new Vector2(0.3f, -1f);
                    if (lookingLeft) direction.X *= -1f;
                    direction.Normalize();
                }

                mainBody.ApplyLinearImpulse(mainBody.GetWorldVector(direction) * jumpImpulse);
                jumpedTime = 0f;
                
                return true;
            }
            else return false;
        }

        public void startMoving()
        {
            WheelL.Friction = 0;
            WheelR.Friction = 0;
            moving = true;
        }

        public void stopMoving()
        {
            WheelL.Friction = wheelsFriction;
            WheelR.Friction = wheelsFriction;
            moving = false;
        }
        #endregion

        #region functions:
        private void resetNTImpulses()
        {
            leftwheelNimpulse  = 0f;
            rightwheelNimpulse = 0f;
            leftwheelTimpulse  = 0f;
            rightwheelTimpulse = 0f;
            leftwheelFCoef     = 0f;
            rightwheelFCoef    = 0f;

            rightwheelContacts = 0;
            leftwheelContacts = 0;
        }

        private void WheelAfterContactHandler(Fixture Wheel, Fixture floor, Contact contact)
        {
            ManifoldPoint mfp = contact.Manifold.Points[0];
            if (Wheel == WheelL)
            {
                leftwheelNimpulse += mfp.NormalImpulse;
                leftwheelTimpulse += mfp.TangentImpulse;
                leftwheelFCoef = floor.Friction;

                leftwheelContacts++;
            }
            else
            {
                rightwheelNimpulse += mfp.NormalImpulse;
                rightwheelTimpulse += mfp.TangentImpulse;
                rightwheelFCoef = floor.Friction;

                rightwheelContacts++;
            }
        }

        private bool WheelContactHandler(Fixture Wheel, Fixture floor, Contact contact)
        {
            //if (Wheel == WheelL) leftwheelContacts++;
            //else rightwheelContacts++;
            return true;
        }

        private void WheelSeparateHandler(Fixture Wheel, Fixture floor)
        {
            //if (Wheel == WheelL) leftwheelContacts--;
            //else rightwheelContacts--;
        }
        #endregion

    }
}
