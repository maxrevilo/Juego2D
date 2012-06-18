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
    public class SpecialCollectible : Collectible
    {
        public SpecialCollectible(GameScreen gameScreen, Vector2 position, World world, Camera2D camera)
            : base(gameScreen, new CircleShape(0.5f, 1f), world, camera)
        {
            this.Position = position;
        }

        public override void loadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            Texture2D sheet = contentManager.Load<Texture2D>("collectiblePez");
            image = new SpriteSheet(Game, sheet, 2, 2, gameScreen.ScreenManager.SpriteBatch) {
                position = Position,
                framesPerSecond = 8f
            };
            image.scale = 1f / image.frameWidth;
        }

        protected override bool Collected()
        {
            UserProfileManger.getProfile().specialRecoleced++;
            image.Visible = false;
            return true;
        }
    }
}
