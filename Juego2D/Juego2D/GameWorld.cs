using System;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;

namespace Juego2D
{
    public class GameWorld : World
    {
        public Camera2D camera;
        public float airDrag;

        public GameWorld(Vector2 gravity, float airDrag, Camera2D camera)
            : base(gravity)
        {
            this.camera = camera;
            this.airDrag = airDrag;
            this.BodyAdded += BodyAddedHandler;
        }

        private void BodyAddedHandler(Body body)
        {
            body.LinearDamping = airDrag;
        }
    }
}
