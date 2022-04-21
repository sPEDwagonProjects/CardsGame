using System;
using System.Collections.Generic;

namespace CardGame
{
    public static class GameConstants
    {
        public static readonly Dictionary<int, int> TimeOfLevel =
            new Dictionary<int, int>()
            {
                {1,180 }, {2,160 },
                {3,150 }, {4,140 },
                {5,130 }, {6,120 },
                {7,110 }, {8,130 }
            };

        public static readonly Dictionary<int, (string path, int count)> GameDataOfLevel =
            new Dictionary<int, (string, int)>()
            {
                { 1, (@"Data\Варежки166_203",4)}, { 2, (@"Data\Носки65_90",4)},
                { 3, (@"Data\Варежки166_203",6)}, { 4, (@"Data\Носки65_90",6)},
                { 5, (@"Data\Варежки166_203",8)}, { 6, (@"Data\Носки65_90",8)},
                { 7, (@"Data\Варежки78_110", 8)}, { 8, (@"Data\Варежки78_110",10)}
            };

        public static readonly string DefaultImageUri =
            new Uri(@"pack://application:,,,/CardGame;component/Resources/cathungry.png", UriKind.Absolute).AbsolutePath;

        public static readonly string AudioUri = "Main.mp3";
    }
}
