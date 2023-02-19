using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static System.Formats.Asn1.AsnWriter;

namespace RaceTo21
{
    public class Game
    {
        //save fields privatly;
        private int numberOfPlayers; //players count
        private List<Player> players = new List<Player>(); //players data
        private CardTable cardTable; //a card table (object) to show the info of game
        private Deck deck = new Deck(); //cards deck
        private int currentPlayer = 0; //current turn of players
        //use enum to store the next task
        //public string nextTask;
        //public Task nextTask;
        //save next task privatly
        private Task _nextTask; //to define what to do next
        //use public property to get and set next task
        public Task nextTask { get { return _nextTask; } set { _nextTask = value; } }
        //track if it is the first time that player get card
        private bool firstRound = true;
        //track the first player who get the "highest score"
        private int firstHighPoints = 0;
        //track the first "player" who get the "highest score"
        private Player firstHighPlayer;
        //the score to end the game
        private int gamesGoal = 30;

        public Game(CardTable c)
        {
            cardTable = c;
            //shuffle the deck
            deck.ShuffleDeck();
            //show all cards is for testing, we don't need it in a real game
            //deck.ShowAllCards();
            //after setting up output the line
            Console.WriteLine("================================");
            //it seems we don't need this line since the first task in enum is GetNumberOfPlayers
            //nextTask = Task.GetNumberOfPlayers;
        }

        /* 
         * add a player of the game to players list by name
         * why do we need a method here? why don't we just use players.Add(new Player(n)) in DoNextTask()
         * call: no
         * called by: DoNextTask()
         * parameter: string - player name
         * return: no (void)
         */
        private void AddPlayer(string n)
        {
            //add a player of the game to players list by name
            players.Add(new Player(n));
        }

        /* 
         * the method to do different next tasks
         * call: GetNumberOfPlayers, GetPlayerName, AddPlayer, ShowPlayers, ShowHands, YesOrNo, DealTopCard, PointsInHand, CheckToEnd, CheckWinner, AnnounceWinner, Shuffle, ShufflePlayers
         * called by: Program
         * parameter: no
         * return: no (void)
         */
        public void DoNextTask()
        {
            //task for getting score of game goal
            if (nextTask == Task.GetGamesGoal)
            {
                //get score of game goal gametable and store in gamesGoal
                gamesGoal = CardTable.GetNumber("How many scores to win the final game? ", 0, 210);
                //next task is to get players' name
                nextTask = Task.GetNumberOfPlayers;
            }
            //task for getting players count
            else if (nextTask == Task.GetNumberOfPlayers)
            {
                //get players count from gametable and store in numberOfPlayers
                numberOfPlayers = CardTable.GetNumber("How many players? ", 2, 8);
                //next task is to get players' name
                nextTask = Task.GetNames;
            }
            //task for getting players names
            else if (nextTask == Task.GetNames)
            {
                //for loop start from 1 since we ask player name by their order instead of index in list
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    //get the name of player by input, ask by player #
                    var name = cardTable.GetPlayerName(count);
                    //add name in the players list (index)
                    AddPlayer(name);
                }
                //next task is to introduce players
                nextTask = Task.IntroducePlayers;
                Console.WriteLine("================================");
            }
            //task for introducing players
            else if (nextTask == Task.IntroducePlayers)
            {
                //output each player's intronduction with their order
                cardTable.ShowPlayers(players);
                //next task is to deliver cards for a player
                nextTask = Task.PlayerTurn;
                Console.WriteLine("================================");
                Console.WriteLine("Players get one card at the first round");
                Console.WriteLine("--------------------------------");
            }
            //task for delivering cards for a player
            else if (nextTask == Task.PlayerTurn)
            {
                //store current player as "player"
                Player player = players[currentPlayer];
                //the first run player will get a card automatically
                //so, it is not necessary to show all hand
                //ys every time for each player
                if (player.points != 0)
                {
                    //show all players' cards
                    cardTable.ShowHands(players);
                    Console.WriteLine("================================");
                }
                //if player is not stay or bust
                if (player.status == PlayerStatus.active)
                {
                    //-----the better way to fix----- if no players take cards, they all “bust”
                    //force players to get a card at the begining (player.points == 0)
                    //should check player.points==0 before Program.YesOrNo()
                    //ask current player if they want a card and get the result : Program.YesOrNo(player.name + ", do you want a card?"))
                    //if (cardTable.OfferACard(player))
                    //if (player.points == 0 || cardTable.OfferACard(player))
                    if (player.points == 0 || Program.YesOrNo(player.name + ", do you want a card?"))
                    {
                        //There are two args in the Card.Card field
                        //so we need to give the two args back when we need to add the card into player's hand
                        //string card = deck.DealTopCard();
                        //get a card and store the short and long name in shortName, longName
                        (string shortName, string longName) = deck.DealTopCard();
                        //player.cards.Add(card);
                        //add the card into player's hand with shortName, longName
                        player.cards.Add(new Card(shortName, longName));
                        //calculate the points in player's hand and save in player.points
                        player.points = PointsInHand(player);
                        //if the points of player > 21 then his status would be bust
                        if (player.points > 21)
                        {
                            player.status = PlayerStatus.bust;
                        }
                        //if the points of player > 21 then his status would be win
                        else if (player.points == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else
                    {
                        //if player didn't take a card then his status would be stay
                        player.status = PlayerStatus.stay;
                        //check the highest points (player.points < 21 has exluded by above conditions
                        if (player.points > firstHighPoints)
                        {
                            //save new highest points
                            firstHighPoints = player.points;
                            //save the player with the highest points
                            firstHighPlayer = player;
                        }
                    }
                }
                //if it is not the first time that player get card, doesn't need to show hand each time
                if (!firstRound)
                {
                    //show the cards that current the player has
                    cardTable.ShowHand(player);
                    Console.WriteLine("================================");
                }
                //next task is to check if to go to the end of the current game
                nextTask = Task.CheckForEnd;
            }
            //task for checking if to go to the end of the current game
            else if (nextTask == Task.CheckForEnd)
            {
                //Wrote a new field to check if the game is finished
                //if (!CheckActivePlayers())
                //check if current game needs to end
                if (CheckToEnd())
                {
                    //check the highest points of players and players' status to find the winner
                    Player winner = CheckWinner();
                    //-----the simple way to fix----- if no players take cards, they all “bust”
                    //if no one takes card, every player's points = 0, CheckWinner() will show "No one takes a card, no one win"
                    //if someone took a card (player's points!=0), then the winner will not be null, so we can show the winner result correctlly
                    //but we don't need this now, since everyone will get a card at first round
                    //if (players[currentPlayer].points != 0)
                    //{
                    //AnnounceWinner uses arg.name to show the name
                        cardTable.AnnounceWinner(winner);
                    //}
                    winner.gamesScore += winner.points;
                    cardTable.ShowScores(players);
                    Console.WriteLine("                       Goal: " + gamesGoal);
                    Console.WriteLine("================================");
                    //if gamesGoal score reached
                    if (winner.gamesScore >= gamesGoal)
                    {
                        //use $ to allowe write value in string with {}
                        //https://learn.microsoft.com/zh-tw/dotnet/csharp/language-reference/tokens/interpolated
                        Console.WriteLine($"{winner.name} reached {gamesGoal} and is the final winner!!!");
                        Console.WriteLine("");
                        //next task is to end the current game
                        nextTask = Task.GameOver;
                    }
                    //if "Play a new game?" get "yes" answer
                    else if (Program.YesOrNo("Play the next game?"))
                    {
                        //restart a game with different players
                        //Program.Main();

                        //save winner's score, because we will remove and add later if he join the next game
                        int winnerScore = winner.gamesScore;
                        //-----two ways----- to do the same thing
                        //-----remove players who don't want to join the new game
                        //-----Method 1: this way will ask players question in the original order
                        //create a new list to store the index that have to remove
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
                        //-----Method 2: this way will ask players question in the reverse order
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
                                //shows who is the only one left
                                Console.WriteLine(players[0].name + " is the only player left");
                                //AnnounceWinner uses arg.name to show the player name of the winner
                                cardTable.AnnounceWinner(players[0]);
                            }
                            else //no one left or other situations make players.Count()<=1
                            {
                                Console.WriteLine("No enough players :(");
                            }
                            //end the game
                            nextTask = Task.GameOver;
                        }
                        else //if more than one player left the new game
                        {
                            //claim a new deck for new game since some cards have removed
                            Deck newDeck = new Deck();
                            //apply the new deck for the game
                            deck = newDeck;
                            deck.ShuffleDeck();
                            //ShowAllCards is for testing, we don't need it in a real game
                            //deck.ShowAllCards();
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
                                //{0} is the first arg, the line will show like: "The previous winner, AAA,..."
                                //https://learn.microsoft.com/zh-tw/dotnet/csharp/language-reference/tokens/interpolated
                                Console.WriteLine("The previous winner, {0}, will be the last one in this time", winner.name);
                                //add winner to the last of players list
                                players.Add(winner);
                                //get back winner's score
                                winner.gamesScore = winnerScore;
                            }
                            //run loop to reset the players
                            foreach (var player in players)
                            {
                                //reset player's cards, clean the list
                                player.cards.Clear();
                                //reset player's status to active
                                player.status = PlayerStatus.active;
                                //reset player's points to 0
                                player.points = 0;
                            }
                            //save the new list number in case some players left
                            numberOfPlayers = players.Count();
                            //start from the new turn's first player
                            //player might leave the game the, currentPlayer couldn't bigger than the player list
                            currentPlayer = 0;
                            //reset to the first round
                            firstRound = true;
                            //reset first high points
                            firstHighPoints = 0;
                            //next task is to introduce players
                            nextTask = Task.IntroducePlayers;
                            Console.WriteLine("================================");
                        }
                    }
                    else //if "Play a new game?" get "no" answer and no winner reach gamesGoal, or else
                    {
                        //next task is to end the current game
                        nextTask = Task.GameOver;
                    }
                }
                else //CheckToEnd() get false, can't find the winner
                {
                    //go to next player's turn
                    currentPlayer++;
                    //currentPlayer start with 0, players count is count from 1
                    //if currentPlayer > players.Count - 1, means there is no player with the index
                    if (currentPlayer > players.Count - 1)
                    {
                        //so we need to start from the first again
                        currentPlayer = 0;
                        //if it is the first time that player get card in the current game (defult value is true)
                        if (firstRound)
                        {
                            //change value to false
                            firstRound = false;
                        }
                    }
                    //next task is to deliver cards for next player
                    nextTask = Task.PlayerTurn;
                }
            }
            else //can't find nextTask or other situation
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                //next task is to end the current game
                nextTask = Task.GameOver;
            }
        }

        /* 
         * get player's points in hand by player
         * call: no
         * called by: DoNextTask()
         * parameter: Player - player data
         * return: int - points in hand
         */
        private int PointsInHand(Player player)
        {
            //set point as 0 at start
            int points = 0;
            //replaced string with Card class
            //foreach (string card in player.cards)
            //check all cards in player's hand
            foreach (Card card in player.cards)
            {
                //string faceValue = card.Remove(card.Length - 1);
                //get the first character (card num) by remove the last character of the card id (eg 5C - 2 character)
                string faceValue = card.id.Remove(card.id.Length - 1);
                //check card num in different condition
                switch (faceValue)
                {
                    //if card num is K, J, Q
                    case "K":
                    case "Q":
                    case "J":
                        //+10 points
                        points = points + 10;
                        break;
                    case "A":
                        points = points + 1;
                        break;
                    default:
                        //change number string to int and add the points
                        points = points + int.Parse(faceValue);
                        break;
                }
            }
            return points;
        }

        /* 
         * added more conditions to check if to end the game
         * check if the game need to go to the end
         * call: no
         * called by: DoNextTask()
         * parameter: no
         * return: bool - falst to continue, true to end
         */
        private bool CheckToEnd()
        {
            //set busted players count as 0
            int busted = 0;
            //set inactive players count as 0
            int inactive = 0;
            //check each player in players list by foreach
            foreach (var player in players)
            {
                //if someone win, then end the game
                if (player.status == PlayerStatus.win)
                {
                    //true to end the current game
                    return true;
                }
                //count the busted player
                if (player.status == PlayerStatus.bust)
                {
                    //busted=busted+1;
                    busted++;
                }
                //count the number of stay or the number of bust
                //the status win has returned, so it will not count win in
                if (player.status != PlayerStatus.active)
                {
                    inactive++;
                }
            }
            //if all stay or bust, then end the game
            if (numberOfPlayers == inactive)
            {
                //busted will not be firstHighPlayer because firstHighPlayer is the player who stay
                firstHighPlayer.status = PlayerStatus.win;
                return true;
            }
            //if all but one player “busts”, then end the game
            if (numberOfPlayers - 1 == busted)
            {
                //check each player in players list by foreach
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

        /* 
         * check the winner by players' points and status
         * call: cardTable.ShowHands - show current table
         * called by: DoNextTask()
         * parameter: no
         * return: Player - player data
         */
        private Player CheckWinner()
        {
            //define the highPoints as 0 first to check highest point later
            //int highPoints = 0;
            //show result
            cardTable.ShowHands(players);
            Console.WriteLine("================================");
            //check each player's status in players list
            foreach (var player in players)
            {
                //do not show each player's cards by index if someone win
                //because it will stop at the winner index and will not show all players' card
                //cardTable.ShowHand(player);
                //someone got 21 points or remain or first stay in highest points
                if (player.status == PlayerStatus.win) 
                {
                    //return the winner
                    return player;
                }
                //players who is stay
                /*if (player.status == PlayerStatus.stay)
                {
                    //get the points of players who is stay, if the current highPoints < the "player"'s points
                    if (player.points > highPoints)
                    {
                        //set new high points
                        highPoints = player.points;
                    }
                }*/
            }
            //someone get a card and points > 0
            /*if (highPoints > 0)
            {
                //find the first match in players list, and return the player as winner
                return players.Find(player => player.points == highPoints);
            }*/
            //-----the simple way-----to fix if no players take cards, they all “bust”
            //if no one takes card (the highPoints = 0), no one win
            //if (highPoints==0) { Console.WriteLine("No one takes a card, no one win"); }
            return null;
        }

        /* 
         * shuffle players in list
         * able to merge with Deck.Shuffle by using args, if there is a way that args accept List<Player> List<Cards>
         *    private void ShufflePlayers(List<Player> players)
         * call: no
         * called by: DoNextTask()
         * parameter: no
         * return: no (void)
         */
        private void ShufflePlayers()
        {
            Console.WriteLine("Shuffling Players...");
            //claim a new radom
            Random newOrder = new Random();

            //run for loop by players' count to shuffle players in list
            for (int i = 0; i < players.Count; i++)
            {
                //create a new player by current index to exchange to the new index, save in tmp
                Player tmp = players[i];
                //get a random num in players.Count
                int swapindex = newOrder.Next(players.Count);
                //change the data of the current player by new index
                players[i] = players[swapindex];
                //the new index use the current index of player data
                players[swapindex] = tmp;
            }
        }
    }
}
