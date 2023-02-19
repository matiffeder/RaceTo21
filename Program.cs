using System;
using System.Collections.Generic;

namespace RaceTo21
{
    class Program
    {
        /* 
         * ask Yes/No questions on console and get input then return bool
         * use static to share the method, since it works the same in different class
         * call: n/a
         * called by: Game
         * parameter: string - question
         * return: bool - yes (true), no (false)
         */
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
                    //accepted, because it returned so the loop stopped
                    return true;
                }
                else if (response!=null && response.ToUpper().StartsWith("N"))
                {
                    //rejected, because it returned so the loop stopped
                    return false;
                }
                else
                {
                    //null or entered wrong character, run the loop again
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /* 
         * this is the only func that run after game start, other func are trigger by it
         * call: game.nextTask - the next task
         * called by: no
         * parameter: no
         * return: no
         */
        static void Main()
        {
            //create a table for the new game
            CardTable cardTable = new CardTable();
            //create a game use the table
            Game game = new Game(cardTable);
            //an endless loop to check if nextTask is GameOver, stop the loop
            while (game.nextTask != Task.GameOver)
            {
                //Console.WriteLine("----------------------" + game.nextTask);
                game.DoNextTask();
            }
            //if the loop ended (game.nextTask == Task.GameOver) then run following
            //changed from Write to WriteLine to fix the word missing issue
            Console.WriteLine("Press <Enter> to exit... ");
            //do not run to the end until press Enter
            //run endless loop if pressed keys is not Enter
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            //we don't need to exit the program since Main() only runs one time
            //Environment.Exit(0);
        }
    }
}

