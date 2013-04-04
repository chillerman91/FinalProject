using System;

namespace A_Whole_SnakeWorld
{
#if WINDOWS || XBOX
    static class Program
    {
        [STAThread]
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

