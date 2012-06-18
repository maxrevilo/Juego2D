using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public class SpriteSheet : DrawableGameComponent
{
    private float _frame;
    private bool playing;
    private int rows, columns;

    protected Texture2D sheet;
    public Vector2 position;
    public float scale;
    public Color color;
    public Matrix view;
    public float rotation;
    public Vector2 rotationCenter;
    public SpriteEffects spriteEffects;
    public float depth;


    public float Width { get { return sheet.Width / columns * scale; } }
    public float Height { get { return sheet.Height / rows * scale; } }

    public float frameWidth { get { return sheet.Width / columns; } }
    public float frameHeight { get { return sheet.Height / rows; } }

    public bool Playing { get { return playing; } }

    public SpriteBatch spriteBatch;
    public float framesPerSecond = 24f;
    public int frame { get { return (int)Math.Round(_frame); } }

    public SpriteSheet(Game game, Texture2D sheet, int rows, int columns, SpriteBatch spriteBatch)
        : base(game)
    {
        this.sheet = sheet;
        this.rows = rows;
        this.columns = columns;
        this.spriteBatch = spriteBatch;
        _frame = 0;
        playing = true;

        spriteEffects = SpriteEffects.None;
        color = Color.White;
        scale = 1f;
        view = Matrix.Identity;
        rotation = 0f;
        rotationCenter = new Vector2(Width / 2f, Height / 2f);
        depth = 0f;
    }

    public override void Update(GameTime gameTime)
    {
        float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (playing && Enabled)
        {
            _frame += seconds * framesPerSecond;
            if (frame >= rows * columns) _frame = 0f;

        }
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        if (Visible)
        {
            Point frameSize = new Point(sheet.Width / columns, sheet.Height / rows);
            Rectangle source = new Rectangle((frame % columns) * frameSize.X, (frame / columns) * frameSize.Y, frameSize.X, frameSize.Y);

            spriteBatch.Draw(sheet, position, source, color, rotation, rotationCenter, scale, spriteEffects, depth);

            base.Draw(gameTime);
        }

    }

    private void go(float frame)
    {
        _frame = frame;
        if (frame >= rows * columns) _frame = 0f;
        else if (_frame < 0) throw new IndexOutOfRangeException("Frame out of bounds");
    }

    public void play() { playing = true; }

    public void stop() { gotoAndStop(0); }

    public void pause() { playing = false; }

    public void gotoAndPlay(int frame) {
        go(frame);
        playing = true;
    }

    public void gotoAndStop(int frame)
    {
        go(frame);
        playing = false;
    }


}