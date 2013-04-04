using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace A_Whole_SnakeWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// The State Manager
        /// </summary>
        StateManager stateManager;
      //  GraphicsDevice device;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            //Creamos y añadido el ManagerState
            stateManager = new StateManager(this, Gameestados.IniMenu);
            Components.Add(stateManager);

            //Añadimos los servicios para poder usarlos en otras clases
           // device = GraphicsDevice;
           // Services.AddService(typeof(GraphicsDevice), device);
            Services.AddService(typeof(ContentManager), Content);
            Services.AddService(typeof(StateManager), stateManager); 

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
          // Debug.GS = new Gearset.GearConsole(this);
         //   Debug.GS.Initialize();
            //Debug.GS.SetFinderSearchFunction(MySearchFunction);

            base.Initialize();

            //Añadimos los estados que tendrá nuestro juego
            stateManager.estados.Add(Gameestados.IniMenu, new IniMenu(this));
            stateManager.estados.Add(Gameestados.PauseMenu, new PauseMenu(this));
            stateManager.estados.Add(Gameestados.PlayingState, new PlayingState(this));
            stateManager.estados.Add(Gameestados.PuntajeState, new PuntajeState(this));
            //Estado inicial del juego
            (stateManager.estados[Gameestados.IniMenu] as IniMenu).Initialize("Press ENTER to start.", "Images/FondoInicial", "Fonts/fuente");
            stateManager.estadoActual = Gameestados.IniMenu; 

        }

        //private FinderResult MySearchFunction(String searchTerms)
        //{
        //    FinderResult result = new FinderResult();
        //    String[] searchParams = searchTerms.Split(new char[] { ' ' },
        //        StringSplitOptions.RemoveEmptyEntries);

        //    // Ignore Case
        //    for (int i = 0; i < searchParams.Length; i++)
        //        searchParams[i] = searchParams[i].ToUpper();

        //    foreach (IGameComponent component in Components)
        //    {
        //        bool matches = true;
        //        String typeName = component.GetType().ToString();

        //        // Check if the object matches all search terms.
        //        for (int i = 0; i < searchTerms.Length; i++)
        //        {
        //            if (!(component.ToString().ToUpper().Contains(searchTerms[i]) ||
        //                typeName.ToUpper().Contains(searchTerms[i])))
        //            {
        //                matches = false;
        //                break;
        //            }
        //        }
        //        if (matches)
        //            result.Add(new ObjectDescription(component, typeName));
        //    }
        //    return result;
        //}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
           
            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();


            // TODO: Add your update logic here

            base.Update(gameTime);
         //   Debug.GS.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
         //   Debug.GS.Draw(gameTime);
        }
    }
}
