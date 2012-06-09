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
        public Scenario(GameScreen gameScreen, GameWorld world, Camera2D camera)
            : base(gameScreen, world, camera)
        {
            physics.bodys = new Body[1]{ new Body(world) };
        }

        public override void loadContent(ContentManager contentManager)
        {
            Texture2D colTex = contentManager.Load<Texture2D>("Levels/LevelT.1");
            uint[] data = new uint[colTex.Width * colTex.Height];
            colTex.GetData(data);
            colTex.Dispose();

            Vertices verts = PolygonTools.CreatePolygon(data, colTex.Width, true);

            Vector2 scale = new Vector2(0.2f);
            verts.Scale(ref scale);
            

            List<Vertices> list = BayazitDecomposer.ConvexPartition(verts);

            List<Fixture> compund = FixtureFactory.AttachCompoundPolygon(list, 1, mainBody);

            mainBody.Friction = 1.4f;
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
