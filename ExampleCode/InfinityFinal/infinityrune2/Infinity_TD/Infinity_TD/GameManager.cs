using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity_TD
{
    static class GameManager
    {
        public static int vidas = 5;

        public static int currentWave, totalWaves, currentLevel;

        public static bool hard = false;

    }

    class EnemyWave
    {
        public int numEnemies;
        public int numEnemyTypes;

        public EnemyWave(int level)
        {
            Random random = new Random(level);

            numEnemies = random.Next(1, 10);

            numEnemyTypes = 0;
        }
    }

    static class WaveArrays
    {


    }



}

