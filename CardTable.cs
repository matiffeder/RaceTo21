using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceTo21
{
    public class CardTable
    {
        //a list to save playerNames
        //save player names list privatly
        private List<string> _playerNames = new List<string>();
        //use public property to get and set player's names list
        public List<string> playerNames { get { return _playerNames; } set { _playerNames = value; } }

        public CardTable()
        {
            //output "Setting Up Table..." on console
            Console.WriteLine("Setting Up Table...");
        }

        /* 
         * output each player's intronduction with their order
         * call: Introduce - player
         * called by: game
         * parameter: List<Player> - players data
         * return: no (void)
         */
        public void ShowPlayers(List<Player> players)
        {
            //run for loop to show each each player's intronduction by count of players
            for (int i = 0; i < players.Count; i++)
            {
                //the for loop start with 0, but player num start with 1, so num=index+1
                players[i].Introduce(i+1);
            }
        }

        /* 
        * get the number from input
        * call: no
        * called by: game
        * parameter: no
        * return: int - count of players, scores to win the final game
        */
        public static int GetNumber(string question, int minNum, int maxNum)
        {
            Console.Write(question);
            //read the input in the console and save as response
            string response = Console.ReadLine();
            //use for checking if response is number
            int numberOut;
            //an endless loop to check if response is number, <minNum, >maxNum, if it is num, it will out numberOut
            //will stop loop until response is number, >=minNum, <=8
            while (int.TryParse(response, out numberOut) == false
                                || numberOut < minNum
                                || numberOut > maxNum)
            {
                //use $ to allowe write value in string with {}
                //https://learn.microsoft.com/zh-tw/dotnet/csharp/language-reference/tokens/interpolated
                Console.WriteLine($"Invalid number of players ({minNum}-{maxNum})");
                //ask again until response is number, >=minNum, <=maxNum
                Console.Write(question);
                response = Console.ReadLine();
            }
            return numberOut;
        }

        /* 
        * get the name of player by input, ask by player #
        * call: no
        * called by: game
        * parameter: int - player #
        * return: string - name of player
        */
        public string GetPlayerName(int playerNum)
        {
            //ask by question by player #
            Console.Write("What is the name of player# " + playerNum + "? ");
            //Console.ReadLine() may be null, use ? to include null  
            string? response = Console.ReadLine();
            //an endless loop to check if response is null, <1 character, a same name in the list
            //will stop loop until response is not null, >1 character, no same name in the list
            //add the function to check if there is the same name in the list
            while (response==null || response.Length<1 || CheckSameName(response))
            {
                Console.WriteLine("Invalid name or the name have been taken");
                //ask again until response is not null, >1 character, no same name in the list
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            //add the names that users inputted to the playerNames list
            //so that we can use them after restart the game
            playerNames.Add(response);
            return response;
        }

        /* 
         * show the cards that a player has by player
         * call: no
         * called by: cardtable, game
         * parameter: Player - player data
         * return: no (void)
         */
        public void ShowHand(Player player)
        {
            //if player have one more cards
            if (player.cards.Count > 0)
            {
                //show player's name
                Console.Write(player.name + " has: ");
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
                    //show other cards
                    Console.Write(", " + player.cards[i].displayName);
                }
                //show player's score
                Console.Write(" = " + player.points + "/21 ");
                //show bust, stay, win in the console
                if (player.status != PlayerStatus.active)
                {
                    //shows as upper case and string
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                //write above in a line and output
                Console.WriteLine();
            }
        }

        /* 
         * show the cards that all player has by player list
         * call: ShowHand
         * called by: game
         * parameter: List<Player> - players data
         * return: no (void)
         */
        public void ShowHands(List<Player> players)
        {
            Console.WriteLine("Current Table");
            //run loop in players list to call ShowHand
            foreach (Player player in players)
            {
                //show the cards that a player has by player
                ShowHand(player);
            }
        }

        /* 
         * show the winner of the game by player (winner)
         * call: no
         * called by: game
         * parameter: Player - winner
         * return: no (void)
         */
        public void AnnounceWinner(Player player)
        {
            //if cannot get winner
            if (player != null)
            {
                //show the winner
                Console.WriteLine(player.name + " wins this time!");
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

        /* 
         * show the scores that all player has by player list
         * call: no
         * called by: game
         * parameter: List<Player> - players data
         * return: no (void)
         */
        public void ShowScores(List<Player> players)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Players Score Summary");

            //run loop in players list to show player score by name
            /*foreach (Player player in players)
            {
                Console.WriteLine(player.name + ": " + player.gamesScore);
            }*/

            //show the score in order
            //save players list in temp because we will remove the highest after we found it
            //so that we can compare the second high and so on
            List<Player> playesTemp = new List<Player>(players);
            //the list for show scores in order 
            List<Player> scoresRanking = new List<Player>();
            //save the player who have the highest score in the list
            //the player will change because the highest player will remove from playesTemp
            Player highPlayer = new Player("");
            //save the highest score in the list
            //the score will change because the highest player will remove from playesTemp
            int highScore;

            int lowScore = 0;
            //-----find the lowest score in the playesTemp list to compare the highest later
            for (int i = 0; i < players.Count; i++)
            {
                //if the score of current player is lower then current lowScore, then the player has the lowest score at the moment 
                if (playesTemp[i].gamesScore <= lowScore)
                {
                    lowScore = playesTemp[i].gamesScore;
                }
            }   //so, if the loop ended, we can find the lowest score and the player in the current playesTemp list

            //compare player scores by "players.Count" times since every card needs to comapre with others
            //and we need to make sure the last one also in the scoresRanking list
            for (int i = 0; i < players.Count; i++)
            {
                //reset highScore to lowest to compare the next high score, also a defualt value
                highScore = lowScore;
                /* 
                //-----reset highScore to compare the next high score, also a defualt value
                the min score got in one game would be 21-(20+10)=-9, the min point (will be score) to win a game is 1, the max goal is 210, -9*(210/1)=-1890.
                but if there are other 4 players (4 Ace), the possiblity to win a game with 1 point will be very small (4/52) * (3/51) * (2/50) * (1/49), and I am not very sure about these maths
                the winner get 210, the other 3 player could get 219, -9*219*3=-1971, the lowest score would be -1890+(-1971)
                there will also be 2 points winner in 8 players games, -9*(210/2)=945 + -9*(208/2)*6=-5616 | *6 is because 1 player always bust the other is winner who get 210
                highScore = -6461;
                */
                //Console.WriteLine("------------------------ID-" + i);
                //Console.WriteLine("---------------------Count-" + playesTemp.Count);
                //I use j-- just because I want to show the list in original order if players have the same score
                //find the highest score in the playesTemp list
                for (int j = playesTemp.Count - 1; j >= 0; j--)
                {
                    //if the score of current player is higher then current highScore, then the player has the highest score at the moment 
                    if (playesTemp[j].gamesScore >= highScore)
                    {
                        //save the highest score from playesTemp list
                        highScore = playesTemp[j].gamesScore;
                        //save player who have the highest score from playesTemp list
                        highPlayer = playesTemp[j];
                    }
                }   //so, if the loop ended, we can find the highest score and the player in the current playesTemp list

                //remove the player who have the highest score, to find the next highest score
                playesTemp.Remove(highPlayer);
                //add the current highest score player to last index of the scoresRanking, so the list will order by scores
                scoresRanking.Add(highPlayer);
                //Console.WriteLine("-----------------highScore-" + highScore);
                //Console.WriteLine("===------------------------");

                //then find the next highest score in next loop
            }
            //run loop in scoresRanking list to show player score by name
            foreach (Player player in scoresRanking)
            {
                Console.WriteLine(player.name + ": " + player.gamesScore);
            }
            //clear the list to store a new ranking
            scoresRanking.Clear();
        }
        /* 
         * heck if there is the same name in the playerNames list to avoid confusing
         * call: no
         * called by: cardtable
         * parameter: string - the name will add
         * return: bool - true if there is the same name
         */
        private bool CheckSameName(string addName)
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