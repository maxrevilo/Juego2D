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
        private bool cinema0;

        public FirstLevel()
            :base()
        {
            cinema0 = true;
            playerControl = false;

            UserProfileManger.getProfile().simpleRecolected = 0;
            UserProfileManger.getProfile().specialRecoleced = 0;
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

            scenario.loadImage(content.Load<Texture2D>("Levels/Level0/G0"), new Vector2(-38f, 382f) * scale2, scale2.X / 4f);
            scenario.loadImage(content.Load<Texture2D>("Levels/Level0/G1"), new Vector2(22f, 531f) * scale2, scale2.X / 4f);
            scenario.loadImage(content.Load<Texture2D>("Levels/Level0/G2.2"), new Vector2(520f, 200f)  * scale, scale.X);
            scenario.loadImage(content.Load<Texture2D>("Levels/Level0/G3"), new Vector2(1238f, 841f) * scale, scale.X / 16f, 0.4f);
            scenario.loadImage(content.Load<Texture2D>("Levels/Level0/G3"), new Vector2(1338f, 841f) * scale, scale.X / 16f, 0.4f);
            scenario.loadImage(content.Load<Texture2D>("Levels/Level0/G4"), new Vector2(1860f, 709f) * scale, scale.X / 16f, 0.4f);
            scenario.loadImage(content.Load<Texture2D>("Levels/Level0/G5"), new Vector2(1878f, 665f) * scale, scale.X / 16f, 0.4f);

            collectibles.Add(new SimpleCollectible(this, new Vector2( 83.78f, 82.47f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2( 96.27f, 85.70f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(109.40f, 85.64f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(126.35f, 83.64f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(136.80f, 83.64f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(132.98f, 90.82f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(171.00f, 85.00f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(181.50f, 74.50f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(190.00f, 60.30f), world, camera));
            collectibles.Add(new SimpleCollectible(this, new Vector2(171.60f, 53.00f), world, camera));

            collectibles.Add(new SpecialCollectible(this, new Vector2(162.15f, 97.2F), world, camera));
            collectibles.Add(new SpecialCollectible(this, new Vector2(171f, 79.5F), world, camera));
            collectibles.Add(new SpecialCollectible(this, new Vector2(183.9f, 78.9F), world, camera));
            collectibles.Add(new SpecialCollectible(this, new Vector2(177f, 73.5F), world, camera));
            collectibles.Add(new SpecialCollectible(this, new Vector2(175.98f, 41.95F), world, camera));

            base.loadContent(content);
        }

        protected override void UpdatePaused(GameTime gameTime)
        {
            

            base.UpdatePaused(gameTime);
        }

        protected override void UpdatePlaying(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Level Succed
            if (player.Position.X > 187 && player.Position.Y < 40f)
            {
                ConfirmBoxScreen confirmWin = new ConfirmBoxScreen("Felicidades has ganado");

                confirmWin.Accepted += (s, e) => LevelLoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());

                ScreenManager.AddScreen(confirmWin, ControllingPlayer);
            }


            //Cacht the fish cinematic:
            if (cinema0)
            {
                //Cinematic area:
                if (player.Position.X < 55f && player.Position.Y > 58f)
                {
                    //Jump to catch the fish
                    if (player.Position.X < 2f)
                    {
                        Vector2 dir = new Vector2(0.2f, -0.9f);
                        dir.Normalize();
                        player.moveUp(seconds, dir);
                    }
                    else //Sliding:
                    {
                        Vector2 dir = player.mainBody.GetWorldVector(new Vector2(1f, 0.2f));
                        player.mainBody.ApplyLinearImpulse(dir * 3f * seconds);
                    }

                }//Cinematic end:
                else if (player.Position.X > 55)
                {
                    cinema0 = false;
                    playerControl = true;
                }
            } //Player falling and diying
                if (player.Position.Y > 100f)
            {
                LevelLoadingScreen.Load(ScreenManager, true, this.ControllingPlayer, new FirstLevel());
            }
           

            base.UpdatePlaying(gameTime);
        }

        protected override void OnGameHandleInput(GameTime gameTime, InputState input)
        {
            base.OnGameHandleInput(gameTime, input);
        }

    }
}
