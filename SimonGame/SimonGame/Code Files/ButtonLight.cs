#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using LuaInterface;
#endregion

namespace SimonGame
{
    public class ButtonLight : Sprite
    {
        public delegate void ButtonPressedEventHandler();
        public event EventHandler ButtonPressedEvent;

        public bool buttonPressed;
        private byte id;
        private Color start_color;

        public ButtonLight(Vector2 position, Vector2 scale, Color color, float rotation, byte id)
            : base(position, scale, color, rotation)
        {
            buttonPressed = false;
            this.id = id;
            time_most_recent = 0;
            start_color = color;


            lua.DoFile(@"Content\Scripts\ButtonLightScript.txt");

        }

        public byte ID
        {
            get { return id; }
        }

        public override void Update()
        {
            if (!buttonPressed)
                SetColorZero();
            if (buttonPressed)
                ButtonPressedEvent(this, null);

            base.Update();
        }

        public void ActivateButtonPress()
        {
            buttonPressed = true;
            colorA = start_color.A;
            colorR = start_color.R;
            colorG = start_color.G;
            colorB = start_color.B;
        }

    }

}