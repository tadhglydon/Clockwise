using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using GameStateManagement;

namespace Clockwise.Screens
{
    class AboutScreen : GameScreen
    {
        ContentManager content;
        SpriteFont font;
        Texture2D aboutImage;
        InputAction backAction;
        float pauseAlpha;

        private Texture2D menuTexture;
        private Vector2 menuOrigin;
        private Vector2 menuSpritePosition;

        private Piece normalExample;
        private Piece selectedExample;
        private Piece unsuccessfulExample;

        public AboutScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            backAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            normalExample = new Piece(new Vector2(50,375), 0, 0);

            selectedExample = new Piece(new Vector2(50,525), 0, 1);
            selectedExample.Selected = true;
            selectedExample.Score = 3;
            
            unsuccessfulExample = new Piece(new Vector2(50,675), 0, 2);
            unsuccessfulExample.InvertColour = true;
            unsuccessfulExample.Score = 42;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                menuTexture = content.Load<Texture2D>("menuButton2");
                menuOrigin = new Vector2(0, 0);
                menuSpritePosition = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width - menuTexture.Width, 0);

                font = content.Load<SpriteFont>("gameFont");
                aboutImage = content.Load<Texture2D>("AboutScreen");

                normalExample.Load(content);
                selectedExample.Load(content);
                unsuccessfulExample.Load(content);
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
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());

                TouchCollection touches = TouchPanel.GetState();
                if (touches.Count > 0)
                {
                    TouchLocation touch = touches[0];
                    if (MenuPressed(touch, spriteBatch))
                        LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());
                }
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
            spriteBatch.DrawString(font, "About", scoreArea, Color.White);
            //spriteBatch.Draw(aboutImage, new Vector2(0, 50), Color.White);

            spriteBatch.DrawString(font, "TARGET: 89", new Vector2(25, 80), Color.White);
            spriteBatch.DrawString(font, "This is the target score for your", new Vector2(25, 120), Color.LimeGreen);
            spriteBatch.DrawString(font, " board.", new Vector2(25, 160), Color.LimeGreen);
            spriteBatch.DrawString(font, "ATTEMPTS: 2", new Vector2(25, 210), Color.White);
            spriteBatch.DrawString(font, "number of attempts will be ", new Vector2(25, 250), Color.LimeGreen);
            spriteBatch.DrawString(font, " tracked. Less the better.", new Vector2(25, 290), Color.LimeGreen);
            normalExample.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(font, "this piece is available.", new Vector2(25, 400), Color.LimeGreen);
            spriteBatch.DrawString(font, "it will always move clockwise.", new Vector2(25, 440), Color.LimeGreen);
            selectedExample.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(font, "your chosen piece with its", new Vector2(25, 550), Color.LimeGreen);
            spriteBatch.DrawString(font, " current score.", new Vector2(25, 590), Color.LimeGreen);
            unsuccessfulExample.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(font, "if no more moves, then piece will", new Vector2(25, 700), Color.LimeGreen);
            spriteBatch.DrawString(font, " fail. The board is then reset.", new Vector2(25, 740), Color.LimeGreen);
            spriteBatch.End();

            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

    }
}
