using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using GameStateManagement;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        ScreenFactory screenFactory;
        SpriteBatch spriteBatch;

        public Game()
        {
            try
            {
                graphics = new GraphicsDeviceManager(this);
                Content.RootDirectory = "Content";

                // Frame rate is 30 fps by default for Windows Phone.
                TargetElapsedTime = TimeSpan.FromTicks(333333);

                graphics.IsFullScreen = true;
                InitializePortraitGraphics();

                // Create the screen factory and add it to the Services
                screenFactory = new ScreenFactory();
                Services.AddService(typeof(IScreenFactory), screenFactory);

                // Create the screen manager component.
                screenManager = new ScreenManager(this);
                Components.Add(screenManager);

                // Hook events on the PhoneApplicationService so we're notified of the application's life cycle
                Microsoft.Phone.Shell.PhoneApplicationService.Current.Launching +=
                    new EventHandler<Microsoft.Phone.Shell.LaunchingEventArgs>(GameLaunching);
                Microsoft.Phone.Shell.PhoneApplicationService.Current.Activated +=
                    new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(GameActivated);
                Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated +=
                    new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(GameDeactivated);
            }
            catch (Exception ex)
            {
                //Do something
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {            
            base.Update(gameTime);

            if (screenManager.KillGame == true) this.Exit();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Helper method to the initialize the game to be a portrait game.
        /// </summary>
        private void InitializePortraitGraphics()
        {
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Helper method to initialize the game to be a landscape game.
        /// </summary>
        private void InitializeLandscapeGraphics()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new Screens.BackgroundScreen(), null);

            // We have different menus for Windows Phone to take advantage of the touch interface
            screenManager.AddScreen(new Screens.PhoneMainMenuScreen(), null);
        }

        void GameLaunching(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            AddInitialScreens();
        }

        public void ExitGame()
        {
            this.Exit();
        }

        void GameActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            // Try to deserialize the screen manager
            if (!screenManager.Activate(e.IsApplicationInstancePreserved))
            {
                // If the screen manager fails to deserialize, add the initial screens
                AddInitialScreens();
            }
        }

        void GameDeactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            // Serialize the screen manager when the game deactivated
            screenManager.Deactivate();
        }
        
    }

}
