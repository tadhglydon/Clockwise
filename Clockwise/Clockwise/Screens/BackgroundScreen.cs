#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
#endregion

namespace Clockwise.Screens
{
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D currentTexture;
        float rotationAngle = 0;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                currentTexture = content.Load<Texture2D>("whitePiece");
            }

        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            
            float xPosition = (3) * (spriteBatch.GraphicsDevice.Viewport.Width / (3 + 1));
            float yPosition = (4) * (spriteBatch.GraphicsDevice.Viewport.Height / (4 + 1));
            Vector2 spritePosition = new Vector2(xPosition, yPosition);
            
            spriteBatch.Begin();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float circle = MathHelper.Pi * 2;
            rotationAngle += elapsed % circle;

            Vector2 origin = new Vector2();
            origin.X = currentTexture.Width / 2;
            origin.Y = currentTexture.Height / 2;

            spriteBatch.Draw(currentTexture, spritePosition, null, Color.White, rotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                        
            spriteBatch.End();
        }


        #endregion
    }
}
