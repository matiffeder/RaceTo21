using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        //a list to save playerNames
        public List<string> playerNames = new List<string>();
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            //Console.ReadLine() may be null, use ? to include null  
            string? response = Console.ReadLine();
            //add the function to check if there is the same name in the list
            while (response==null || response.Length<1 || CheckSameName(response))
            {
                Console.WriteLine("Invalid name or the name have been taken");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            //add the names that users inputted to the playerNames list
            //so that we can use them after restart the game
            playerNames.Add(response);
            return response;
        }

        //replaced by Program.YesOrNo(string question)
        /*public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N) ");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }*/

        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                Console.Write(player.name + " has ");
                //replaced string with Card class to store short and long name
                //foreach (string card in player.cards)
                /*foreach (Card card in player.cards)
                {
                    //Console.Write(card + " ");
                    Console.Write(", " + card.displayName);
                }*/
                //the first card should shows without ","
                Console.Write(player.cards[0].displayName);
                //the other cards should shows with ","
                //use for loop to start from the second card (index 1)
                for (var i=1; i<player.cards.Count; i++)
                {
                    Console.Write(", " + player.cards[i].displayName);
                }
                Console.Write(" = " + player.score + "/21 ");
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        public void ShowHands(List<Player> players)
        {
            Console.WriteLine("Current Table:");
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        public void AnnounceWinner(Player player)
        {
            if (player != null)
            {
                Console.WriteLine(player.name + " wins!");
            }
            else
            {
                //there will not be a situation that everyone busted
                //since all but one player “busts”, remaining player should immediately win
                //Console.WriteLine("Everyone busted!");
                Console.WriteLine("Cannot find the winner");
            }
            //moved to Program.cs, because users are able to restart the game
            //Console.Write("Press <Enter> to exit... ");
            //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        //check if there is the same name in the playerNames list
        bool CheckSameName(string addName)
        {
            //check each name in the playerNames
            foreach (string name in playerNames)
            {
                //if found the same name then return true for checking
                if (addName == name)
                {
                    return true;
                }
            }
            //if there is no the same name
            return false;
        }
    }
}