using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method
using System.Text.RegularExpressions;

namespace RaceTo21
{
    public class Deck
    {
        //replaced string with Card class to store short and long name
        //List<string> cards = new List<string>();
        //save cards privatly
        private List<Card> cards = new List<Card>();
        //claim cardImg as a Dictionary to store the img
        public Dictionary<string, string> cardImg = new Dictionary<string, string>() { };

        public Deck()
        {
            Console.WriteLine("Building deck...");
            //string[] suits = { "S", "H", "C", "D" };
            //since we need longname and shortname
            //we can only store longnames of the suits and get its first character for shortname
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };

            //run for loop with card number, and add into the deck
            for (int cardNum = 1; cardNum <= 13; cardNum++)
            {
                //run loop in suits array to create cards for each number
                foreach (string cardSuit in suits)
                {
                    string shortName;
                    string longName;
                    //check card num in different condition
                    switch (cardNum)
                    {
                        //if card num = 1
                        case 1:
                            shortName = "A";
                            longName = "Ace";
                            break;
                        case 11:
                            shortName = "J";
                            longName = "Jack";
                            break;
                        case 12:
                            shortName = "Q";
                            longName = "Queen";
                            break;
                        case 13:
                            shortName = "K";
                            longName = "King";
                            break;
                        //if not above
                        default:
                            //change int to string
                            shortName = cardNum.ToString();
                            //get long name by cardNum and index of numToWords
                            string[] numToWords = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
                            //the index start from 0, so we need to - 1
                            longName = numToWords[cardNum-1];
                            break;
                    }
                    //cards.Add(shortName + cardSuit);
                    //cards.Add(new Card(shortName + cardSuit));
                    //cards.Add(new Card(shortName + cardSuit.Last<char>(), longName + " of " + cardSuit));
                    //add the card to the card list with id and displayName
                    cards.Add(new Card(shortName + cardSuit.First<char>(), longName + " of " + cardSuit));
                    //-----the other way to add the card image without a method or using Regex
                    /*if (shortName == "A" || shortName == "J" || shortName == "Q" || shortName == "K")
                    {
                        cardImg.Add(shortName + cardSuit.First<char>(), "card_" + cardSuit.ToLower() + "_" + shortName + ".png");
                    }
                    else
                    {
                        cardImg.Add(shortName + cardSuit.First<char>(), "card_" + cardSuit.ToLower() + "_" + shortName.PadLeft(2, '0') + ".png");
                    }*/
                }
            }
            //add card images to the dictionary by the index of the cards
            SetCardImg();
            /*//testing, see comments on SetCardImg
            Console.WriteLine("---------------------------"+cards[2].displayName.Substring(cards[2].displayName.LastIndexOf(" of ")+4).ToLower());
            string numTest = Regex.IsMatch(cards[2].id.Substring(0, 1), "[AJQK]") ? cards[2].id.Substring(0, 1) : cards[2].id.Substring(0, 1).PadLeft(2, '0');
            Console.WriteLine("---------------------------" + cards[2].id);
            Console.WriteLine("---------------------------" + numTest);
            //*****call the card image by id*****
            Console.WriteLine(cardImg[cards[2].id]);
            */
        }

        /* 
         * shuffle the card in the deck
         * call: SetCardImg()
         * called by: game
         * parameter: no
         * return: no (void)
         */
        public void ShuffleDeck()
        {
            Console.WriteLine("Shuffling Cards...");
            //create a Random as rng
            Random rng = new Random();

            //run for loop by cards' count to shuffle cards list
            for (int i=0; i<cards.Count; i++)
            {
                //replaced string with Card class
                //string tmp = cards[i];
                //create a new card by current index to exchange to the new index, save in tmp
                Card tmp = cards[i];
                //get a random num in cards.Count
                int swapindex = rng.Next(cards.Count);
                //change current card to the new index's card
                cards[i] = cards[swapindex];
                //change the new index's card to current card (tmp)
                cards[swapindex] = tmp;
            }
        }


        /* 
         * show all cards on the console
         * call: no
         * called by: game if needed
         * parameter: no
         * return: no (void)
         */
        public void ShowAllCards()
        {
            //run for loop by cards.Count to write cards in a line
            for (int i=0; i<cards.Count; i++)
            {
                //Console.Write(i+":"+cards[i]); // a list property can look like an Array!
                //Console.Write(i + ":" + cards[i].id);
                //show the long name in console
                //shows like 1:Ace of Club
                Console.Write(i + ":" + cards[i].displayName);
                if (i < cards.Count -1)
                {
                    //separate names
                    Console.Write(" ");
                }
                else
                {
                    //the last one write above names on console
                    Console.WriteLine("");
                }
            }
        }

        /* 
         * get the last card in deck
         * There are two args in the Card.Card field
         * so we need to give the two args back when we need to add the card into player's hand
         * when (nextTask == Task.PlayerTurn)
         * call: no
         * called by: game
         * parameter: no
         * return: string, string - card id, card long name
         */
        public (string, string) DealTopCard()
        {
            //string card = cards[cards.Count - 1];
            //the index start from 0, so we need to -1
            //get the last card's id
            string card = cards[cards.Count - 1].id;
            //get the last card's long name
            string cardLong = cards[cards.Count - 1].displayName;
            //remove the last card from the deck
            cards.RemoveAt(cards.Count - 1);
            return (card, cardLong);
        }

        /* 
         * set the img of each card
         * call: no
         * called by: deck
         * parameter: no
         * return: no (void)
         */
        private void SetCardImg()
        {
            //run for loop by cards's count to set the img of each card
            for (int i=0; i<cards.Count; i++)
            {
                //source: I used a lot of regex in the projects I wrote long time ago, https://www.curseforge.com/members/matif525/projects
                //https://learn.microsoft.com/zh-tw/dotnet/api/system.text.regularexpressions.regex.ismatch?view=net-7.0
                //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.substring?view=net-7.0
                //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.padleft?view=net-7.0
                //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.indexof?view=net-7.0
                //Substring(0, 1) : remain string characters from 0 to 1
                //Use regular expression to find if the string contain A, J, Q, or K
                //if contained, do not add 0 before it
                //todo: diff between Match and IsMatch - seems nothing different
                //string num = Regex.Match(cards[i].id.Substring(0, 1), "[AJQK]").Success ? cards[i].id.Substring(0, 1) : cards[i].id.Substring(0, 1).PadLeft(2, '0');
                string num = Regex.IsMatch(cards[i].id.Substring(0, 1), "[AJQK]") ? cards[i].id.Substring(0, 1) : cards[i].id.Substring(0, 1).PadLeft(2, '0');
                //find the index of " of " and remain the characters after " of " according to the index
                //there are four index in " of ", so we need to + 4 to cut it
                cardImg.Add(cards[i].id, "card_" + cards[i].displayName.Substring(cards[i].displayName.IndexOf(" of ") + 4).ToLower() + "_" + num + ".png");
            }
        }
    }
}

