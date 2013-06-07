using System;
using System.Collections;
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
    class BoardHandler
    {
        private int xGrid;
        private int yGrid;
        private Piece[,] grid;
        private bool bInProgress;
        private List<Piece> pieces;
        private Piece selectedPiece;
        private int score;
        private int turnScore;
        private int targetScore;
        private int attempts;

        private int initialPieceX;
        private int initialPieceY; 

        private Texture2D menuTexture;
        private Vector2 menuOrigin;
        private Vector2 menuSpritePosition;

        public BoardHandler(int xGrid, int yGrid)
        {
            this.xGrid = xGrid;
            this.yGrid = yGrid;

            grid = new Piece[xGrid, yGrid];
            bInProgress = false;
            pieces = new List<Piece>();
        }

        public bool InProgress
        {
            get { return bInProgress; }
        }

        public int Attempts
        {
            get { return attempts; }
        }
        
        public void Load(ContentManager Content, SpriteBatch spriteBatch)
        {
            turnScore = 0;
            targetScore = 0;
            menuTexture = Content.Load<Texture2D>("menuButton2");
            menuOrigin = new Vector2(0,0);
            menuSpritePosition = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width - menuTexture.Width, 0);
            
            //Set up first Positions
            for (int i = 0; i < xGrid; i++)
            {
                for (int j = 0; j < yGrid; j++)
                {
                    Vector2 position = GetPosition(i,j,spriteBatch);
                    Piece currentPiece = new Piece(position,i,j);
                    grid[i, j] = currentPiece;
                    grid[i, j].Load(Content);
                    grid[i, j].RandomisePosition();
                    grid[i, j].OnLoadState = grid[i, j].CurrentState;
                }
            }

            //Find target score
            for (int i = 0; i < xGrid; i++)
            {
                for (int j = 0; j < yGrid; j++)
                {
                    grid[i, j].Rotate(false);
                    int currentPieceScore = 1;
                    List<Piece> targetPieces = new List<Piece>();
                    targetPieces = RotateNeigbours(i, j);
                    currentPieceScore += targetPieces.Count;
                    while (targetPieces.Count != 0)
                    {
                        List<Piece> secondaryTargetPieces = new List<Piece>();
                        foreach (Piece piece in targetPieces)
                        {
                            piece.Rotate(false);
                        }

                        foreach (Piece piece in targetPieces)
                        {
                            List<Piece> currentList = RotateNeigbours(piece.xGrid, piece.yGrid);
                            foreach (Piece secondaryPiece in currentList)
                            {
                                //Is piece already in sr List?
                                if (!secondaryTargetPieces.Contains(secondaryPiece)) secondaryTargetPieces.Add(secondaryPiece);
                            }
                        }

                        targetPieces.Clear();
                        targetPieces = secondaryTargetPieces;
                        currentPieceScore += targetPieces.Count;
                    }                    
                    if (currentPieceScore > targetScore) targetScore = currentPieceScore;
                    ResetBoard();
                }
            }
        }
        
        public void ResetBoard()
        {
            for (int i = 0; i < xGrid; i++)
            {
                for (int j = 0; j < yGrid; j++)
                {
                    grid[i, j].Reset();                    
                }
            }

        }

        public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuTexture, menuSpritePosition, null, Color.White, 0, menuOrigin, 1.0f, SpriteEffects.None, 0f);

            for (int i = 0; i < xGrid; i++)
            {
                for (int j = 0; j < yGrid; j++)
                {
                    grid[i,j].Draw(gameTime,spriteBatch);
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

        public void Touch(TouchLocation location, SpriteBatch spriteBatch)
        {

            turnScore = 0;
            int x = Convert.ToInt16((location.Position.X / (spriteBatch.GraphicsDevice.Viewport.Width / (xGrid + 1))) - 1);
            int y = Convert.ToInt16(((location.Position.Y - (menuTexture.Height / 2)) / (spriteBatch.GraphicsDevice.Viewport.Height / (yGrid + 1))) - 1);
            try
            {
                //Check x,y is within Grid and not been tried before
                if (!(x < 0 || y < 0 || x > xGrid || y > yGrid) && !grid[x, y].InvertColour)
                {
                    score = 0;
                    bInProgress = true;
                    selectedPiece = grid[x, y];
                    grid[x, y].Selected = true;
                    grid[x, y].Rotate(true);
                    initialPieceX = x;
                    initialPieceY = y;
                    score = 1;
                    attempts++;
                    selectedPiece.Score = score;
                    turnScore = 1;
                    pieces.Add(grid[x, y]);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                //If the player touches the far right or extreme bottom, we do nothing!
            }
        }

        public bool Rotating()
        {
            bool result = false;
            foreach (Piece piece in pieces)
            {
                if (piece.RotatingInProcess) result = true;
            }
            return result;
        }

        public void CauseChainReaction()
        {
            turnScore = 0;
            List<Piece> secondaryReaction = new List<Piece>();
            foreach (Piece piece in pieces)
            {
                List<Piece> currentList = RotateNeigbours(piece.xGrid, piece.yGrid);
                foreach (Piece currentPiece in currentList)
                {
                    //Is piece already in sr List?
                    if(!secondaryReaction.Contains(currentPiece)) 
                        secondaryReaction.Add(currentPiece);
                }
            }
            if (secondaryReaction.Count == 0)
            {
                bInProgress = false;
                pieces.Clear();
                ResetBoard();
                //attempts += 1;
                grid[initialPieceX, initialPieceY].InvertColour = true;
            }
            else
            {
                pieces.Clear();
                pieces = secondaryReaction;
                turnScore = pieces.Count();
                score += pieces.Count();
                selectedPiece.Score = score;
                foreach (Piece piece in pieces)
                {
                    piece.Rotate(true);
                }
            }

        }
        
        public int GetTurnScore
        {
            set { turnScore = value; }
            get { return turnScore; }
        }

        public int GetTargetScore
        {
            get { return targetScore; }
        }

        public int GetScore
        {
            get { return score; }
        }

        private Vector2 GetPosition(int x, int y, SpriteBatch spriteBatch)
        {
            float xPosition = (x+1) * (spriteBatch.GraphicsDevice.Viewport.Width / (xGrid + 1));
            float yPosition = (y+1) * (spriteBatch.GraphicsDevice.Viewport.Height / (yGrid + 1)) + (menuTexture.Height /2);
            return new Vector2(xPosition,yPosition);
        }

        private List<Piece> RotateNeigbours(int x, int y)
        {
            List<Piece> chain = new List<Piece>();

            //Check left
            if (x - 1 > -1 && (grid[x, y].CurrentState == State.TopLeft || grid[x, y].CurrentState == State.BottomLeft))
            {
                if (grid[x - 1, y].CurrentState == State.BottomRight || grid[x - 1, y].CurrentState == State.TopRight)
                {
                    chain.Add(grid[x - 1, y]);
                }
            }

            //Check top
            if (y - 1  > -1 && (grid[x, y].CurrentState == State.TopLeft || grid[x, y].CurrentState == State.TopRight))
            {
                if (grid[x, y - 1].CurrentState == State.BottomRight || grid[x, y - 1].CurrentState == State.BottomLeft)
                {
                    chain.Add(grid[x, y - 1]);
                }
            }

            //Check right
            if (x + 1 < xGrid && (grid[x, y].CurrentState == State.BottomRight || grid[x, y].CurrentState == State.TopRight))
            {
                if (grid[x + 1, y].CurrentState == State.BottomLeft || grid[x + 1, y].CurrentState == State.TopLeft)
                {
                    chain.Add(grid[x + 1, y]);
                }
            }

            //Check bottom
            if (y + 1 < yGrid && (grid[x, y].CurrentState == State.BottomRight || grid[x, y].CurrentState == State.BottomLeft))
            {
                if (grid[x, y + 1].CurrentState == State.TopLeft || grid[x, y + 1].CurrentState == State.TopRight)
                {
                    chain.Add(grid[x, y + 1]);
                }
            }

            return chain;
        }

    }
}
