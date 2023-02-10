using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        //public string nextTask; // keeps track of game state
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Task.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
            }
            else if (nextTask == Task.PlayerTurn)
            {
                Console.WriteLine("----------------------" + nextTask);
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    //if (cardTable.OfferACard(player))
                    //fix: if no players take cards, they all “bust”
                    //force player to get a card at the begining, run player.score == 0 before cardTable.OfferACard(player)
                    if (player.score == 0 || cardTable.OfferACard(player))
                    {
                        //string card = deck.DealTopCard();
                        (string shortName, string longName) = deck.DealTopCard();
                        //player.cards.Add(card);
                        player.cards.Add(new Card(shortName, longName));
                        player.score = ScoreHand(player);
                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)
            {
                //if (!CheckActivePlayers())
                if (CheckToEnd())
                {
                    Player winner = DoFinalScoring();
                    AnnounceWinner(winner);
                    nextTask = Task.GameOver;
                }
                else
                {
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Task.PlayerTurn;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                //foreach (string card in player.cards)
                foreach (Card card in player.cards)
                {
                    //string faceValue = card.Remove(card.Length - 1);
                    string faceValue = card.id.Remove(card.id.Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        /*public bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }*/

        //check if to go to the end
        public bool CheckToEnd()
        {
            int busted = 0;
            int inactive = 0;
            foreach (var player in players)
            {
                //if someone win
                if (player.status == PlayerStatus.win)
                {
                    return true;
                }
                //check bust number
                if (player.status == PlayerStatus.bust)
                {
                    busted++;
                }
                //stay or bust
                if (player.status != PlayerStatus.active)
                {
                    inactive++;
                }
            }
            //if all stay or bust
            if (numberOfPlayers == inactive)
            {
                return true;
            }
            //if all but one player “busts”
            if (numberOfPlayers - 1 == busted)
            {
                foreach (var player in players)
                {
                    //the remain one win
                    if (player.status == PlayerStatus.active)
                    {
                        player.status = PlayerStatus.win;
                    }
                }
                return true;
            }
            return false;
        }

        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                // someone hit 21 or remain
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // XXX everyone must have busted because nobody won!
        }

        //able to merge with Deck.Shuffle by using args
        public void ShufflePlayers(List<Player> players)
        {
            Console.WriteLine("Shuffling Players...");

            Random newOrder = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                Player tmp = players[i];
                int swapindex = newOrder.Next(players.Count);
                players[i] = players[swapindex];
                players[swapindex] = tmp;
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
                //Console.WriteLine("Everyone busted!");
                Console.WriteLine("Cannot find the winner");
            }
            //
            if (GameReset())
            {
                Program.Main();
                /*Deck deck = new Deck();
                List<Player> players = new List<Player>();
                deck.Shuffle();
                ShufflePlayers(players);
                nextTask = Task.IntroducePlayers;
                DoNextTask();*/
            }
            else
            {
                Console.Write("Press <Enter> to exit... ");
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                //Environment.Exit(0);            }
            }
        }
        //able to merge with OfferACard by using args
        public bool GameReset()
        {
            while (true)
            {
                Console.Write("Play a new game? (Y/N)");
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
        }
    }
}
