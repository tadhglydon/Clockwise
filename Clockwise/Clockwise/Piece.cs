using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Clockwise
{
    class Piece
    {
        private Texture2D texture;
        private Texture2D background;
        private Texture2D finishedTexture;
        private Texture2D selectedTexture;
        //private Texture2D graySquare;

        private Vector2 origin;
        private Vector2 spritePosition;
        private Vector2 backGroundPosition;
        private int _xGrid;
        private int _yGrid;

        private State currentState;
        private State onLoadState;
        private float rotationAngle;
        private float nextTurn;
        private int _score;
        private bool bRotatePiece;
        private bool bInvertColour;
        private bool bSelected;
        private SpriteFont spriteFont;
        private SpriteFont borderSpriteFont;

        private int mAlphaValue;
        private int mFadeIncrement;
        private double mFadeDelay;

        //private SoundEffect bong;

        public Piece(Vector2 position, int xGrid, int yGrid)
        {
            spritePosition = position;
            backGroundPosition = position;
            _xGrid = xGrid;
            _yGrid = yGrid;
            _score = 0;

            bRotatePiece = false;
            
            mAlphaValue = 1;
            mFadeIncrement = 10;
            mFadeDelay = .02;
        }

        public void RandomisePosition()
        {
            Random rand = new Random();
            Double i = rand.NextDouble();
            i = i * 4;

            if (i < 1)
            {
                rotationAngle = 0;
                currentState = State.TopRight;
            }
            else if (i >= 1 && i < 2)
            {
                rotationAngle = (MathHelper.Pi / 2);
                currentState = State.BottomRight;
            }
            else if (i >= 2 && i < 3)
            {
                rotationAngle = MathHelper.Pi;
                currentState = State.BottomLeft;
            }
            else
            {
                rotationAngle = (MathHelper.Pi / 2) * 3;
                currentState = State.TopLeft;
            }

            nextTurn = rotationAngle + (MathHelper.Pi / 2);
        }

        public State OnLoadState
        {
            set { onLoadState = value; }
            get { return onLoadState; }
        }


        public bool InvertColour
        {
            set { bInvertColour = value; }
            get { return bInvertColour; }
        }

        public bool Selected
        {
            set { bSelected = value; }
            get { return bSelected; }
        }

        public bool RotatingInProcess
        {
            get { return bRotatePiece; }
        }

        public void Rotate(bool bReDraw)
        {
            if (bReDraw) bRotatePiece = true;
            else currentState = NextState();
        }

        private void FadeGraySquare(GameTime gameTime)
        {
            //Decrement the delay by the number of seconds that have elapsed since
            //the last time that the Update method was called
            mFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;
            
            //If the Fade delays has dropped below zero, then it is time to 
            //fade out the image a little bit more.
            if (mFadeDelay <= 0)
            {
                //Reset the Fade delay
                mFadeDelay = .01;
                //Increment/Decrement the fade value for the image
                mAlphaValue -= mFadeIncrement;
            }
        }

        private void ResetFadeValue()
        {
            mFadeDelay = .035;
            mAlphaValue = 1;
            mFadeIncrement = 3;
        }

        public float RotationAngle
        {
            get { return rotationAngle; }
        }

        public int xGrid
        {
            get { return _xGrid; }
        }

        public int yGrid
        {
            get { return _yGrid; }
        }

        public Vector2 GetPosition
        {
            get
            {
                return spritePosition;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D currentTexture;
            Color scoreColour = Color.Black;

            if (bInvertColour)
            {
                currentTexture = finishedTexture;
                scoreColour = Color.Red;
            }
            else if(bSelected)
            {
                currentTexture = selectedTexture;
                scoreColour = Color.Yellow;
            }
            else
            {
                currentTexture = texture;
            }

            if (bRotatePiece)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                //FadeGraySquare(gameTime);
                //Color fadeColour = new Color(255, 255, 255, (byte)MathHelper.Clamp(mAlphaValue, 0, 255))
                //spriteBatch.Draw(graySquare, spritePosition, null, fadeColour, rotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                
                float circle = MathHelper.Pi * 2;
                rotationAngle += (elapsed + 0.05f) % circle;
                if (rotationAngle > nextTurn)
                {
                    bRotatePiece = false;
                    rotationAngle = nextTurn;
                    nextTurn = rotationAngle + (MathHelper.Pi / 2);
                    currentState = NextState();
                }
            }
            else
            {
                ResetFadeValue();
            }

            spriteBatch.Draw(currentTexture, spritePosition, null, Color.White, rotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
            if (scoreColour != Color.Black)
            {
                DrawScore(borderSpriteFont, Color.Black, spriteBatch);
                DrawScore(spriteFont, scoreColour, spriteBatch);

            }
        }

        private void DrawScore(SpriteFont font, Color fontColor, SpriteBatch spriteBatch)
        {
            string scoreText = _score.ToString();
            Vector2 textSize = font.MeasureString(scoreText);
            spriteBatch.DrawString(font, scoreText, spritePosition - (textSize / 2), fontColor);
        }


        public int Score
        {
            set { _score = value; }
        }

        public void Reset()
        {
            if (onLoadState == State.TopRight)
            {
                rotationAngle = 0;
                currentState = State.TopRight;
            }
            else if (onLoadState == State.BottomRight)
            {
                rotationAngle = (MathHelper.Pi / 2);
                currentState = State.BottomRight;
            }
            else if (onLoadState == State.BottomLeft)
            {
                rotationAngle = MathHelper.Pi;
                currentState = State.BottomLeft;
            }
            else
            {
                rotationAngle = (MathHelper.Pi / 2) * 3;
                currentState = State.TopLeft;
            }

            nextTurn = rotationAngle + (MathHelper.Pi / 2);

            bSelected = false;
        }

        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("whitePiece");
            background = Content.Load<Texture2D>("whiteBackground");
            finishedTexture = Content.Load<Texture2D>("redPiece");
            selectedTexture = Content.Load<Texture2D>("yellowPiece");
            //graySquare = Content.Load<Texture2D>("graySquare");

            spriteFont = Content.Load<SpriteFont>("scoreGameFont");
            borderSpriteFont = Content.Load<SpriteFont>("borderGameFont");
            

            origin.X = texture.Width / 2;
            origin.Y = texture.Height / 2;
            backGroundPosition.X = spritePosition.X - (background.Width / 2);
            backGroundPosition.Y = spritePosition.Y - (background.Height / 2);
            /*
            Random rand = new Random();
            Double soundPicker = (rand.NextDouble()) * 4;
            if (soundPicker < 1) bong = Content.Load<SoundEffect>("Sound/gong-burmese_vvlow");
            else if (soundPicker >= 1 && soundPicker < 2) bong = Content.Load<SoundEffect>("Sound/gong-burmese_med");
            else if (soundPicker >= 2 && soundPicker < 3) bong = Content.Load<SoundEffect>("Sound/gong-burmese_low");
            else bong = Content.Load<SoundEffect>("Sound/gong-burmese_vlow");
             * */
        }

        public State CurrentState
        {
            set { currentState = value; }
            get { return currentState; }
        }

        private State NextState()
        {
            switch(currentState)
            {
                case State.TopRight:
                    return State.BottomRight;
                case State.BottomRight:
                    return State.BottomLeft;
                case State.BottomLeft:
                    return State.TopLeft;
                case State.TopLeft:
                    return State.TopRight;
                default:
                    return State.TopRight;
            }
       }
    }

    public enum State
    {
        TopLeft,
        BottomLeft,
        BottomRight,
        TopRight
    }
}
