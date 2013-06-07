using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clockwise.Screens
{
    class PhoneMainMenuScreen : PhoneMenuScreen
    {
        public readonly string highScoresFilename = "HighScores.txt";
        public readonly string gameOptionsFilename = "GameOptions.txt";

        public PhoneMainMenuScreen()
            : base("Clockwise")
        {
            // Create a button to start the game
            Button playButton = new Button("Play");
            playButton.Tapped += playButton_Tapped;
            MenuButtons.Add(playButton);

            // Create two buttons to toggle sound effects and music. This sample just shows one way
            // of making and using these buttons; it doesn't actually have sound effects or music
            /*
            BooleanButton sfxButton = new BooleanButton("Sound  ", true);
            sfxButton.Tapped += sfxButton_Tapped;
            MenuButtons.Add(sfxButton);
            */

            // Create a button to start the game
            Button highscoreButton = new Button("Score History");
            highscoreButton.Tapped += highScore_Tapped;
            MenuButtons.Add(highscoreButton);

            // Create a button to start the game
            Button aboutButton = new Button("About");
            aboutButton.Tapped += aboutButton_Tapped;
            MenuButtons.Add(aboutButton);
        }
        /*
        bool GetSoundOption()
        {
            System.IO.Stream stream = TitleContainer.OpenStream(gameOptionsFilename);
            System.IO.StreamReader sreader = new System.IO.StreamReader(stream);
            string line;
            while ((line = sreader.ReadLine()) != null)
            {
                string[] options = line.Split(',');
                if (options[0].ToLower() == "sounds" && options[1].ToLower() == "on")
                    _sounds = true;
                else
                    _sounds = false;
            }
        }
         */
 
        void playButton_Tapped(object sender, EventArgs e)
        {
            // When the "Play" button is tapped, we load the GameplayScreen
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }

        void aboutButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new AboutScreen());
        }

        void highScore_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new HighScoreScreen());
        }

        void sfxButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off sounds here. :)
        }

        public override void BackButtonPushed()
        {
            ScreenManager.Game.Exit();
            base.BackButtonPushed();
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
            base.OnCancel();
        }
    }
}
