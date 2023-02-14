using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        //use enum to store the next task
        //public string nextTask; // keeps track of game state
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            Console.WriteLine("================================");
            //it seems we don't need this line since the first task in enum is GetNumberOfPlayers
            //nextTask = Task.GetNumberOfPlayers;
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
                Console.WriteLine("================================");
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
                Console.WriteLine("================================");
            }
            else if (nextTask == Task.PlayerTurn)
            {
                Player player = players[currentPlayer];
                //the first run player will get a card automatically
                //so, it is not necessary to show all hands every time for each player
                if (player.score != 0)
                {
                    cardTable.ShowHands(players);
                    Console.WriteLine("================================");
                }
                if (player.status == PlayerStatus.active)
                {
                    //fix: if no players take cards, they all “bust”
                    //force player to get a card at the begining (player.score == 0)
                    //should check player.score == 0 before cardTable.OfferACard(player)
                    //if (cardTable.OfferACard(player))
                    //if (player.score == 0 || cardTable.OfferACard(player))
                    if (player.score == 0 || Program.YesOrNo(player.name + ", do you want a card?"))
                    {
                        //There are two args in the Card.Card field
                        //so we need to give the two args back when we need to add the card into player's hand
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
                Console.Write(player.name + "'s turn > ");
                cardTable.ShowHand(player);
                nextTask = Task.CheckForEnd;
                Console.WriteLine("================================");
            }
            else if (nextTask == Task.CheckForEnd)
            {
                //Wrote a new field to check if the game is finished
                //if (!CheckActivePlayers())
                if (CheckToEnd())
                {
                    Player winner = DoFinalScoring();
                    //AnnounceWinner uses arg.name to show the name
                    cardTable.AnnounceWinner(winner);
                    //check if to reset the game, GameReset() is the previous version of Program.YesOrNo()
                    //if (GameReset())
                    if (Program.YesOrNo("Play a new game?"))
                    {
                        //restart a game with different players
                        //Program.Main();

                        //-----two ways to do the same thing
                        //-----remove players who don't want to join the new game
                        //Method 1: this way will ask players question in the original order
                        //create a new lost to store the index that have to remove
                        /*List<int> removeList = new List<int>();
                        for (var i=0; i<players.Count(); i++)
                        {
                            //if player choose to leave the game
                            if (!Program.YesOrNo(players[i].name + ", do you want to join the new game?"))
                            {
                                //add the index of the player to the removeList
                                removeList.Add(i);
                            }
                        }
                        //use for loop from the end, therefore, the index of player will not change
                        for (var i=removeList.Count()-1; i>=0; i--)
                        {
                            //remove player by the stored index
                            players.Remove(players[removeList[i]]);
                        }*/
                        //Method 2: this way will ask players question in the reverse order
                        //use for loop from the end, therefore, the index of player will not change
                        for (var i=players.Count()-1; i>=0; i--)
                        {
                            //if player choose to leave the game
                            if (!Program.YesOrNo(players[i].name + ", do you want to join the new game?"))
                            {
                                //remove player from the list
                                players.Remove(players[i]);
                            }
                        }
                        Console.WriteLine("================================");
                        //if only one player left or no one left or other situations make players.Count()<=1
                        if (players.Count() <= 1)
                        {
                            //if only one player left
                            if (players.Count() == 1)
                            {
                                //the only player in the list is index 0, the player win the game
                                players[0].status = PlayerStatus.win;
                                Console.WriteLine(players[0].name + " is the only player left");
                                //AnnounceWinner uses arg.name to show the player name
                                cardTable.AnnounceWinner(players[0]);
                            }
                            //no one left or other situations make players.Count()<=1
                            else
                            {
                                Console.WriteLine("No enough players :(");
                            }
                            //end the game
                            nextTask = Task.GameOver;
                        }
                        else
                        {
                            //claim a new deck for new game since some cards have removed
                            Deck newDeck = new Deck();
                            //apply the new deck for the game
                            deck = newDeck;
                            deck.Shuffle();
                            deck.ShowAllCards();
                            //check if winner left the game
                                //the original code I use
                                //bool winnerIn = players.Contains(winner);
                                //players.Remove(winner);
                            //tried if below is possible, and it is
                            //run players.Remove(winner) and return the bool value of if it found "winner" to remove
                            bool winnerIn = players.Remove(winner);
                            //change the order for player to play the game
                            ShufflePlayers();
                            //if winner in the game
                            if (winnerIn)
                            {
                                Console.WriteLine("================================");
                                Console.WriteLine("The previous winner, {0}, will be the last one in this turn", winner.name);
                                //add winner to the last of players list
                                players.Add(winner);
                            }
                            foreach (var player in players)
                            {
                                //reset player's cards, clean the list
                                player.cards.Clear();
                                //reset player's status to active
                                player.status = PlayerStatus.active;
                                //reset player's score to 0
                                player.score = 0;
                            }
                            //save the new list number in case some players left
                            numberOfPlayers = players.Count();
                            //start from the new turn's first player
                            //player might leave the game the, currentPlayer couldn't bigger than the player list
                            currentPlayer = 0;
                            nextTask = Task.IntroducePlayers;
                            Console.WriteLine("================================");
                        }
                    }
                    else
                    {
                        nextTask = Task.GameOver;
                    }
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
                //use ? to make the response can be null
                string? response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be? ");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                //replaced string with Card class
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

        //added more conditions to check if to end the game, so this field is abandoned
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

        //check if the game need to go to the end
        public bool CheckToEnd()
        {
            int busted = 0;
            int inactive = 0;
            //check each player by foreach
            foreach (var player in players)
            {
                //if someone win, then end the game
                if (player.status == PlayerStatus.win)
                {
                    return true;
                }
                //count the bust number
                if (player.status == PlayerStatus.bust)
                {
                    busted++;
                }
                //count the number of stay or bust
                //the status win has returned, so it will not count win in
                if (player.status != PlayerStatus.active)
                {
                    inactive++;
                }
            }
            //if all stay or bust, then end the game
            if (numberOfPlayers == inactive)
            {
                return true;
            }
            //if all but one player “busts”, then end the game
            if (numberOfPlayers - 1 == busted)
            {
                foreach (var player in players)
                {
                    //check the remain one and change the status of the player to win
                    if (player.status == PlayerStatus.active)
                    {
                        player.status = PlayerStatus.win;
                        return true;
                    }
                }
            }
            //if not above condition, then continue the game
            return false;
        }

        public Player DoFinalScoring()
        {
            int highScore = 0;
            //show result
            cardTable.ShowHands(players);
            foreach (var player in players)
            {
                //do not show each player's cards by index if someone win
                //because it will stop at the winner index and will not show all players' card
                //cardTable.ShowHand(player);
                //someone hit 21 or remain
                if (player.status == PlayerStatus.win) // someone hit 21 or remain
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
            Console.WriteLine("================================");
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                //--------------------------find the first match--------------------------//
                return players.Find(player => player.score == highScore);
            }
            return null; // XXX everyone must have busted because nobody won!
        }

        //able to merge with Deck.Shuffle by using args, if there is a way that args accept List<Player> List<Cards>
        //public void ShufflePlayers(List<Player> players)
        public void ShufflePlayers()
        {
            Console.WriteLine("Shuffling Players...");
            //claim a new radom
            Random newOrder = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                //store the current index player name
                Player tmp = players[i];
                //re-order the index
                int swapindex = newOrder.Next(players.Count);
                //change the name of the current player by new index
                players[i] = players[swapindex];
                //the new index use the current index player name
                players[swapindex] = tmp;
            }
        }
        //merged with CardTable.OfferACard by using args
        /*public bool GameReset()
        {
            //an endless loop to check if the answer start with yY or nN
            while (true)
            {
                Console.Write("Play a new game? (Y/N) ");
                //Console.ReadLine() can be null
                string? response = Console.ReadLine();
                //check if it start with Y
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
    }
}
