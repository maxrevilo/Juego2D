#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameStateManagement;

using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
#endregion

namespace Juego2D
{
    public class Scenario : PhysicObject
    {
        private int levelNumber;

        public Scenario(GameScreen gameScreen, GameWorld world, Camera2D camera)
            : base(gameScreen, world, camera)
        {
            levelNumber = 0;
            physics.bodys = new Body[1]{ new Body(world) };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="friction"></param>
        /// <param name="position">Position in pixels (before scale).</param>
        /// <param name="scale"></param>
        /// <param name="body"></param>
        public void loadFloor(ContentManager contentManager, 
            string name, float friction, Vector2 position, ref Vector2 scale, int bodyNumber)
        {
            Texture2D colTex = contentManager.Load<Texture2D>(String.Format("Levels/Level{0}/{1}", levelNumber, name));
            uint[] data = new uint[colTex.Width * colTex.Height];
            colTex.GetData(data);
            //colTex.Dispose();

            Body body = getBody(bodyNumber);

            Vertices verts = PolygonTools.CreatePolygon(data, colTex.Width, true);

            verts.Translate(ref position);
            verts.Scale(ref scale);

            List<Vertices> list = BayazitDecomposer.ConvexPartition(verts);

            List<Fixture> compund = FixtureFactory.AttachCompoundPolygon(list, 1, body);
            foreach (Fixture f in compund)
            {
                f.Friction = friction;
            }
        }
        public override void loadContent(ContentManager contentManager)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
        }

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
