using System;
using System.Collections.Generic;

namespace RaceTo21
{
    class Program
    {
        //used by Game class to ask Yes/No questions
        public static bool YesOrNo(string question)
        {
            //an endless loop to check if the answer start with yY or nN
            while (true)
            {
                Console.Write(question + " (Y/N) ");
                //Console.ReadLine() may be null, use ? to include null  
                string? response = Console.ReadLine();
                //check if input start with Y, abd it's not null
                if (response!=null && response.ToUpper().StartsWith("Y"))
                {
                    //accepted
                    return true;
                }
                else if (response!=null && response.ToUpper().StartsWith("N"))
                {
                    //rejected
                    return false;
                }
                else
                {
                    //null or entered wrong character
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }
        public static void Shuffle(List<Player> list)
        {
            Console.WriteLine("Shuffling Players...");
            //claim a new radom
            Random newOrder = new Random();

            for (int i = 0; i < list.Count; i++)
            {
                //store the current index player name
                Player tmp = list[i];
                //re-order the index
                int swapindex = newOrder.Next(list.Count);
                //change the name of the current player by new index
                list[i] = list[swapindex];
                //the new index use the current index player name
                list[swapindex] = tmp;
            }
        }

        //this is the only func that run after game start, other func are trigger by it
        static void Main()
        {
            CardTable cardTable = new CardTable();
            Game game = new Game(cardTable);
            while (game.nextTask != Task.GameOver)
            {
                //Console.WriteLine("----------------------" + game.nextTask);
                game.DoNextTask();
            }
            //if the loop ended (game.nextTask == Task.GameOver) then run following
            //changed from Write to WriteLine to fix the word missing issue
            Console.WriteLine("Press <Enter> to exit... ");
            //do not run to the end until press Enter
            //run endless loop if pressed keys that not Enter
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            //we don't need to exit the program since Main() only runs one time
            //Environment.Exit(0);
        }
    }
}

