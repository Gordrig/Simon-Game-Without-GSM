#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

using LuaInterface;
#endregion

namespace SimonGame
{
    
    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LuaInterface.Lua lua;
        private KeyboardState oldState;
        private KeyboardState newState;

        public delegate void EnterZenModeEventHandler();
        public event EventHandler EnterZenModeEvent;
        
        private static float screen_width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        private static float screen_height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public double delta_time; //will be utilized in lua
        private double start_time;
        private double end_time;
        public bool computer_turn;
        public bool player_turn;
        public int player_input;

        private enum GameState { Loading, Menu, Playing, Lose };
        private GameState gameState;
        private enum Mode { None, Zen, Rush, Rhythm }; //None will be default
        private Mode mode;

        private Texture2D homeBG;
        private Texture2D homeCenterPiece;
        private Texture2D homeHighScores;
        public Texture2D menuIndicator;

        private Texture2D playing_bg;
        private Texture2D turn_indicator;
        public ButtonLight[] lights;

        private SoundEffect buttonSoundEffect1;
        private SoundEffect buttonSoundEffect2;
        private SoundEffect buttonSoundEffect3;
        private SoundEffect buttonSoundEffect4;

        #region Properties

        public static float Screen_Width
        { get { return screen_width; } }

        public static float Screen_Height
        { get { return screen_height; } }

        #endregion

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            lua = new Lua();
            lua["Game"] = this;

            player_turn = false;
            player_input = -1; //some value that isnt one of the button values (0-3)

            lights = new ButtonLight[4];
            lights[0] = new ButtonLight(new Vector2(screen_width / 2, screen_height / 2 - 230), new Vector2(0.75f, 0.75f), Color.Green, 0f, 0);
            lights[1] = new ButtonLight(new Vector2(screen_width/2 - 165, screen_height/2 - 65), new Vector2(0.75f, 0.75f), Color.Red, 0f, 1);
            lights[2] = new ButtonLight(new Vector2(screen_width / 2 + 165, screen_height / 2 - 65), new Vector2(0.75f, 0.75f), Color.Blue, 0f, 2);
            lights[3] = new ButtonLight(new Vector2(screen_width / 2, screen_height / 2 + 90), new Vector2(0.75f, 0.75f), Color.Yellow, 0f, 3);
            
            gameState = GameState.Playing;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            playing_bg = Content.Load<Texture2D>("Images/gameMode_bg");
            turn_indicator = Content.Load<Texture2D>("Images/turn_indicator");            
            for (int i = 0; i < lights.Length; i++)
                lights[i].LoadContent(this.Content, "Images/buttonOverlay");

            lua.DoFile(@"Content\Scripts\ZenModeScript.txt");

            homeBG = Content.Load<Texture2D>("Images/home_bg");
            homeCenterPiece = Content.Load<Texture2D>("Images/centerpiece");
            homeHighScores = Content.Load<Texture2D>("Images/highScores_link");
            menuIndicator = Content.Load<Texture2D>("Images/yellow_arrow_highlight");

            

            buttonSoundEffect1 = Content.Load<SoundEffect>("Music/WAV/SimonGameSounds1");
            //buttonSoundEffect2 = Content.Load<SoundEffect>("Music/WAV/Simon_Game_Sounds2");
            //buttonSoundEffect3 = Content.Load<SoundEffect>("Music/WAV/Simon_Game_Sounds3");
            //buttonSoundEffect4 = Content.Load<SoundEffect>("Music/WAV/Simon_Game_Sounds4");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        #region Update States


        protected override void Update(GameTime gameTime)
        {
            start_time = gameTime.ElapsedGameTime.Seconds;

            // TODO: Add your update logic here
            switch (gameState)
            {
                case GameState.Loading: UpdateLoading(); break;
                case GameState.Menu: UpdateMenu(); break;
                case GameState.Playing:
                    CheckPlayInput();
                    UpdatePlaying();
                    break;
                case GameState.Lose: DrawLose(); break;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        private void UpdateLoading()
        {

        }

        private void UpdateMenu()
        {

        }

        private void UpdatePlaying()
        {
            foreach (ButtonLight l in lights)
                l.Update();

            switch (mode)
            {
                case Mode.Zen: EnterZenModeEvent(this, null); break;
            }

            //Debug.WriteLine("Lua round_number: " + lua["round_number"]);
            //Debug.WriteLine("Lua notes_played: " + lua["notes_played"]);
            //Debug.WriteLine("Lua computer_input: " + lua["random_num"]);
            //Debug.WriteLine("Lua computer_turn: " + lua["computer_turn"]);
            //Debug.WriteLine("delta_time: " + delta_time);
            //Debug.WriteLine("lua delay_counter: " + lua["delay_counter"]);
            Debug.WriteLine("lua computer_input_size: " + lua["computer_input_size"]);
            Debug.WriteLine("lua player_input_size: " + lua["player_input_size"]);
        }

        private void UpdateLose()
        {

        }


        #endregion 



        #region Input Checking

        private void CheckPlayInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                mode = Mode.Zen;
            }
        }

        #endregion


        #region Lua Helper Functions

        //for computer use
        public void activate_button(byte button_id)
        {
            switch (button_id)
            {
                case 0:
                    if (!lights[0].buttonPressed)
                    {
                        lights[0].ActivateButtonPress();
                        //buttonSoundEffect1.Play();
                    }
                    break;
                case 1:
                    if (!lights[1].buttonPressed)
                    {
                        lights[1].ActivateButtonPress();
                        //buttonSoundEffect2.Play();
                    }
                        
                    break;
                case 2:
                    if (!lights[2].buttonPressed)
                    {
                        lights[2].ActivateButtonPress();
                        //buttonSoundEffect3.Play();
                    }
                    break;
                case 3:
                    if (!lights[3].buttonPressed)
                    {
                        lights[3].ActivateButtonPress();
                        //buttonSoundEffect4.Play();
                    }
                    break;
            }
        }

        public void check_player_input()
        {
            newState = Keyboard.GetState();
            if (oldState.IsKeyDown(Keys.Up) && !newState.IsKeyDown(Keys.Up))
                player_input = 0;
            else if (oldState.IsKeyDown(Keys.Left) && !newState.IsKeyDown(Keys.Left))
                player_input = 1;
            else if (oldState.IsKeyDown(Keys.Right) && !newState.IsKeyDown(Keys.Right))
                player_input = 2;
            else if (oldState.IsKeyDown(Keys.Down) && !newState.IsKeyDown(Keys.Down))
                player_input = 3;
            else
                player_input = -1;
            oldState = Keyboard.GetState();
        }

        public void GameOver()
        {
            Debug.WriteLine("******WRONG BUTTON*******");
            Debug.WriteLine("******WRONG BUTTON*******");
            Debug.WriteLine("******WRONG BUTTON*******");
            Debug.WriteLine("******WRONG BUTTON*******");
            Debug.WriteLine("********ROUND ONE*******");
        }

        public void RoundComplete()
        {
            Debug.WriteLine("*******ROUND WON*********");
            Debug.WriteLine("*******ROUND WON*********");
            Debug.WriteLine("*******ROUND WON*********");
            Debug.WriteLine("*******ROUND WON*********");
            Debug.WriteLine("******NEXT ROUND*********");
        }


        #endregion


        #region Draw Calls

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            switch(gameState)
            {
                case GameState.Loading: 
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Playing: DrawPlaying(); break;
                case GameState.Lose: DrawLose(); break;
            }

            base.Draw(gameTime);

            end_time = gameTime.ElapsedGameTime.TotalSeconds;
            delta_time = end_time - start_time;
        }

        private void DrawMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(homeBG, Vector2.Zero, Color.White);

            Vector2 centerPiecePos = new Vector2(screen_width /7, 0);
            spriteBatch.Draw(homeCenterPiece, centerPiecePos, Color.White);

            Vector2 highScoresPos = new Vector2(screen_width - homeHighScores.Width - 10, screen_height - (homeHighScores.Height * (screen_height/homeHighScores.Height)) + 13);
            spriteBatch.Draw(homeHighScores, highScoresPos, Color.White);

            spriteBatch.End();
        }

        private void DrawPlaying()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(playing_bg, Vector2.Zero, Color.White);
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i].buttonPressed)
                    lights[i].Draw(spriteBatch);
            }
            if (computer_turn)
                spriteBatch.Draw(turn_indicator, Vector2.Zero, Color.Red);
            if (player_turn)
                spriteBatch.Draw(turn_indicator, Vector2.Zero, Color.Green);

            spriteBatch.End();
        }

        private void DrawLose()
        {

        }

        #endregion 

    }
}