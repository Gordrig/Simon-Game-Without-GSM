#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using LuaInterface;
#endregion

namespace SimonGame
{
    public class Sprite
    {

        protected LuaInterface.Lua lua;

        private Texture2D spriteTexture;
        public Vector2 Position;
        public Vector2 Scale;
        public float Rotation;
        public Color color;
        public byte colorR;
        public byte colorG;
        public byte colorB;
        public byte colorA;

        private Vector2 origin;

        public float time_most_recent;

        private float width;
        private float height;

        public float PosX
        {
            get { return Position.X; }
            set { Position.X = value; }
        }
        public float PosY
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }
        public float ScaleX
        {
            get { return Scale.X; }
            set { Scale.X = value; }
        }
        public float ScaleY
        {
            get { return Scale.Y; }
            set { Scale.Y = value; }
        }
        public float Width
        {
            get { return width; }
        }
        public float Height
        {
            get { return height; }
        }

        public Sprite()
        {
            Rotation = 0f;
            Position = new Vector2(0, 0);
            Scale = new Vector2(1, 1);
            color = new Color(255, 255, 255, 255);
            width = 0;
            height = 0;
            SetColorZero();

            lua = new Lua();
            lua["GameObject"] = this;
        }

        public Sprite(Vector2 position, Vector2 scale, Color color, float rotation)
        {
            Position = position;
            Scale = scale;
            Rotation = rotation;
            this.color = color;
            width = 0;
            height = 0;
            colorR = color.R;
            colorG = color.G;
            colorB = color.B;
            colorA = color.A;

            lua = new Lua();
            lua["GameObject"] = this;
        }

        public virtual void Update()
        {
            Position.X = PosX;
            Position.Y = PosY;

            color.R = colorR;
            color.G = colorG;
            color.B = colorB;
            color.A = colorA;
        }

        public void SetColorZero()
        {
            colorR = 0;
            colorG = 0;
            colorB = 0;
            colorA = 0;
        }

        public void LoadContent(ContentManager theContentManager, string assetName)
        {
            spriteTexture = theContentManager.Load<Texture2D>(assetName);
            origin = new Vector2(spriteTexture.Width / 2, spriteTexture.Height / 2);

            width = spriteTexture.Bounds.Width;
            height = spriteTexture.Bounds.Height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteTexture, Position, null, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

    }
}