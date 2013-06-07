using System;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using GameStateManagement;

namespace Clockwise.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont font;

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;
        
        BoardHandler boardHandler;
        Texture2D gridTexture;
        private bool bTouching;

        private const string HIGHSCORE_FILE = "HighScores.txt";

        private const int X_GRID_VALUE = 6;
        private const int Y_GRID_VALUE = 9;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
                
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                //Create board
                boardHandler = new BoardHandler(X_GRID_VALUE, Y_GRID_VALUE);
                                                
                gridTexture = content.Load<Texture2D>("Grid");
                            
                boardHandler.Load(content, spriteBatch);

                font = content.Load<SpriteFont>("gameFont");

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

        }


        public override void Deactivate()
        {
            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer); 

                TouchCollection touches = TouchPanel.GetState();
                if (!bTouching && touches.Count > 0)
                {
                    bTouching = true;
                    TouchLocation touch = touches[0];
                    if (boardHandler.MenuPressed(touch, spriteBatch))
                        ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
                    else
                    {
                        if (!boardHandler.InProgress) boardHandler.Touch(touch, spriteBatch);
                    }
                }
                else if (touches.Count == 0)
                {
                    bTouching = false;
                }

                if (boardHandler.InProgress && !boardHandler.Rotating())
                {
                    boardHandler.CauseChainReaction();
                }
                else
                {
                    if (boardHandler.GetScore == boardHandler.GetTargetScore)
                    {
                        //Add save score here
                        string saveString = string.Format("{0},{1},{2}{3}", boardHandler.GetScore, boardHandler.Attempts, DateTime.Now.ToString(), Environment.NewLine);
                        
                        // Open a storage container.
                        try
                        {
                            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication(); // grab the storage
                            FileStream stream;
                            if (!store.FileExists(HIGHSCORE_FILE))
                            {
                                stream = store.CreateFile(HIGHSCORE_FILE);
                            }
                            else
                            {
                                stream = store.OpenFile(HIGHSCORE_FILE, FileMode.Append);
                            }
                            StreamWriter writer = new StreamWriter(stream);
                            writer.Write(saveString);
                            writer.Close();
                        }
                        catch (Exception ex)
                        {
 
                        }
                        ScreenManager.AddScreen(new FinishGameScreen(boardHandler.GetScore, boardHandler.Attempts), ControllingPlayer);
                    }
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin();
            boardHandler.Draw(gameTime, spriteBatch);

            // Draw the score
            string scoreText = string.Format("Target:{0}  Attempts:{1}", boardHandler.GetTargetScore, boardHandler.Attempts);
            Vector2 scoreArea = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y);
            spriteBatch.DrawString(font, scoreText, scoreArea, Color.White);
            
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        #endregion
    }
}
