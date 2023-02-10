using System;
using System.Collections.Generic;

namespace RaceTo21
{
    class Program
    {
        //static void Main(string[] args)
        public static void Main()
        {
            CardTable cardTable = new CardTable();
            Game game = new Game(cardTable);
            while (game.nextTask != Task.GameOver)
            {
                game.DoNextTask();
            }
        }
    }
}

