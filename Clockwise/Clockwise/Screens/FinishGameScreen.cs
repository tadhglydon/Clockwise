using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clockwise.Screens
{
    class FinishGameScreen : PhoneMenuScreen
    {
        public FinishGameScreen(int score, int turns)
            : base("Complete")
        {
            Button scoreButton = new InfoButton("Score: " + score);
            MenuButtons.Add(scoreButton);

            Button turnButton = new InfoButton("Attempts: " + turns);
            MenuButtons.Add(turnButton);

            Button scoreHistoryButton = new Button("Score History");
            scoreHistoryButton.Tapped += scoreHistoryButton_Tapped;            
            MenuButtons.Add(scoreHistoryButton);

            Button newGameButton = new Button("New Game");
            newGameButton.Tapped += newGameButton_Tapped;
            MenuButtons.Add(newGameButton);

            Button exitButton = new Button("Back to Menu");
            exitButton.Tapped += exitButton_Tapped;
            MenuButtons.Add(exitButton);
        }

    
        /// <summary>
        /// The "Resume" button handler just calls the OnCancel method so that 
        /// pressing the "Resume" button is the same as pressing the hardware back button.
        /// </summary>
        void newGameButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }

        /// <summary>
        /// The "Exit" button handler uses the LoadingScreen to take the user out to the main menu.
        /// </summary>
        void exitButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        void scoreHistoryButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new HighScoreScreen());
        }

        protected override void OnCancel()
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        public override void BackButtonPushed()
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
            base.BackButtonPushed();
        }
    }
}
