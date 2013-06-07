using System;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using GameStateManagement;

namespace Clockwise.Screens
{
    class HighScoreScreen : GameScreen
    {
        ContentManager content;
        SpriteFont font;
        InputAction backAction;
        float pauseAlpha;
        List<ScoreObject> scores;
        int maxScore = 0;
        int minScore = 1000;
        int maxAttempts = 0;
        int minAttempts = 1000;

        private Texture2D menuTexture;
        private Vector2 menuOrigin;
        private Vector2 menuSpritePosition;

        private const string HIGHSCORE_FILE = "HighScores.txt";

        public HighScoreScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            backAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
            
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                
                font = content.Load<SpriteFont>("gameFont");

                menuTexture = content.Load<Texture2D>("menuButton2");
                menuOrigin = new Vector2(0, 0);
                menuSpritePosition = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width - menuTexture.Width, 0);

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


        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());

            base.Update(gameTime, otherScreenHasFocus, false);
            
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            
            if (IsActive)
            {
                    
                TouchCollection touches = TouchPanel.GetState();
                if (touches.Count > 0)
                {
                    TouchLocation touch = touches[0];
                    if(MenuPressed(touch,spriteBatch))
                        LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());
                }
                IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
                if (scores == null)
                {
                    scores = new List<ScoreObject>();
                    if (store.FileExists(HIGHSCORE_FILE)) // Check if file exists
                    {
                        string scoreHistory;
                        IsolatedStorageFileStream save = new IsolatedStorageFileStream(HIGHSCORE_FILE, FileMode.Open, store);
                        StreamReader reader = new StreamReader(save);
                        while ((scoreHistory = reader.ReadLine()) != null)
                        {
                            string[] lineParts = scoreHistory.Split(',');
                            int lineScore = int.Parse(lineParts[0].ToString());
                            if (lineScore > maxScore) maxScore = lineScore;
                            if (lineScore < minScore) minScore = lineScore;
                            int lineAttempts = int.Parse(lineParts[1].ToString());
                            if (lineAttempts > maxAttempts) maxAttempts = lineAttempts;
                            if (lineAttempts < minAttempts) minAttempts = lineAttempts;
                            DateTime lineDate = DateTime.Parse(lineParts[2].ToString());
                            scores.Add(new ScoreObject(lineScore, lineAttempts, lineDate));
                        }
                        reader.Close();

                        scores.Sort(delegate(ScoreObject s1, ScoreObject s2) { return s1.GetRatio.CompareTo(s2.GetRatio); });
                        scores.Reverse();
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
        /// Draws the about screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(menuTexture, menuSpritePosition, null, Color.White, 0, menuOrigin, 1.0f, SpriteEffects.None, 0f);
            Vector2 scoreArea = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y);
            spriteBatch.DrawString(font, "Score History", scoreArea, Color.White);

            Texture2D blank = ScreenManager.BlankTexture;
            int graphX = 40;
            int graphY = 70;
            int graphWidth = ScreenManager.GraphicsDevice.Viewport.Width - graphX - 40;
            int graphHeight = ScreenManager.GraphicsDevice.Viewport.Height - graphY - 90;

            spriteBatch.DrawString(font, "Top Scores", new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 55, 70), new Color(255, 229, 0));

            if (scores != null)
            {
                if (scores.Count > 0)
                {
                    int orderX = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 20;
                    int scoreX = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 55;
                    int attemptsX = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 150;
                    int dateX = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 300;
                    int rowY = 105;
                    int n = 1;

                    spriteBatch.DrawString(font, "Score", new Vector2(scoreX, rowY), Color.White);
                    spriteBatch.DrawString(font, "Attempts", new Vector2(attemptsX, rowY), Color.White);
                    spriteBatch.DrawString(font, "Date", new Vector2(dateX, rowY), Color.White);
                    foreach (ScoreObject score in scores)
                    {
                        rowY = 105 + (n * 35);
                        spriteBatch.DrawString(font, n.ToString(), new Vector2(orderX, rowY), new Color(255, 229,0));
                        spriteBatch.DrawString(font, score.GetScore.ToString(), new Vector2(scoreX, rowY), Color.White);
                        spriteBatch.DrawString(font, score.GetAttempts.ToString(), new Vector2(attemptsX, rowY), Color.White);
                        spriteBatch.DrawString(font, score.GetDate, new Vector2(dateX, rowY), Color.White);
                        n++;
                        if (n > 15) break;
                    }
                }
                else
                {
                    Vector2 noScoreVector = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 50, ScreenManager.GraphicsDevice.Viewport.Height / 2);
                    spriteBatch.DrawString(font, "No Score Registered yet", noScoreVector, Color.White);
                }
            }

            spriteBatch.End();

            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        public bool MenuPressed(TouchLocation location, SpriteBatch spriteBatch)
        {

            if (location.Position.X > (spriteBatch.GraphicsDevice.Viewport.Width - menuTexture.Width) &&
                location.Position.Y < menuTexture.Height)
                return true;
            else
                return false;
        }

    }

    class ScoreObject
    {
        int _score;
        int _attempts;
        DateTime _date;

        public ScoreObject(int score, int attempts, DateTime date)
        {
            this._score = score;
            this._attempts = attempts;
            this._date = date;
        }

        public int GetScore
        {
            get { return _score; }
        }

        public int GetAttempts
        {
            get { return _attempts; }
        }

        public string GetDate
        {
            get { return _date.ToShortDateString(); }
        }

        public float GetRatio
        {
            get { return (float)_score / (float)_attempts; }
        }

    }
}
