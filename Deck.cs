using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method
using System.Text.RegularExpressions;

namespace RaceTo21
{
    public class Deck
    {
        //List<string> cards = new List<string>();
        List<Card> cards = new List<Card>();
        public Dictionary<string, string> cardImg = new Dictionary<string, string>() { };

        public Deck()
        {
            Console.WriteLine("*********** Building deck...");
            //string[] suits = { "S", "H", "C", "D" };
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string longName;
                    switch (cardVal)
                    {
                        case 1:
                            cardName = "A";
                            longName = "Ace";
                            break;
                        case 11:
                            cardName = "J";
                            longName = "Jack";
                            break;
                        case 12:
                            cardName = "Q";
                            longName = "Queen";
                            break;
                        case 13:
                            cardName = "K";
                            longName = "King";
                            break;
                        default:
                            cardName = cardVal.ToString();
                            //get long name by cardVal and index of numToWords
                            string[] numToWords = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
                            longName = numToWords[cardVal-1];
                            break;
                    }
                    //cards.Add(cardName + cardSuit);
                    //cards.Add(new Card(cardName + cardSuit));
                    //cards.Add(new Card(cardName + cardSuit.Last<char>(), longName + " of " + cardSuit));
                    cards.Add(new Card(cardName + cardSuit.First<char>(), longName + " of " + cardSuit));
                }
            }
        }

        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i=0; i<cards.Count; i++)
            {
                //string tmp = cards[i];
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
            //add dict
            SetCardImg();
            /* //testing
            Console.WriteLine("---------------------------"+cards[2].displayName.Substring(cards[2].displayName.LastIndexOf(" of ")+4).ToLower());
            string test = Regex.Match(cards[2].id.Substring(0, 1), "[AJQK]").Success ? cards[2].id.Substring(0, 1) : cards[2].id.Substring(0, 1).PadLeft(2, '0');
            Console.WriteLine("---------------------------" + cards[2].id);
            Console.WriteLine("---------------------------" + test);
            Console.WriteLine(cardImg[cards[2].id]);
            */
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */

        public void ShowAllCards()
        {
            for (int i=0; i<cards.Count; i++)
            {
                //Console.Write(i+":"+cards[i]); // a list property can look like an Array!
                //Console.Write(i + ":" + cards[i].id); // a list property can look like an Array!
                Console.Write(i + ":" + cards[i].displayName); // a list property can look like an Array!
                if (i < cards.Count -1)
                {
                    Console.Write(" ");
                } else
                {
                    Console.WriteLine("");
                }
            }
        }

        //public string DealTopCard()
        public (string, string) DealTopCard()
        {
            //string card = cards[cards.Count - 1];
            string card = cards[cards.Count - 1].id;
            string cardLong = cards[cards.Count - 1].displayName;
            cards.RemoveAt(cards.Count - 1);
            // Console.WriteLine("I'm giving you " + card);
            //return card;
            return (card, cardLong);
        }
        public void SetCardImg()
        {
            for (int i=0; i<cards.Count; i++)
            {
                //Substring(0, 1) : remain string characters from 0 to 1
                //Use regular expression to find if the string contain A, J, Q, or K
                //if contain do not add 0 before it
                //todo: diff between Match and IsMatch
                string num = Regex.Match(cards[i].id.Substring(0, 1), "[AJQK]").Success ? cards[i].id.Substring(0, 1) : cards[i].id.Substring(0, 1).PadLeft(2, '0');
                //find the index of " of " and remain the characters after " of " according to the index
                cardImg.Add(cards[i].id, "card_" + cards[i].displayName.Substring(cards[i].displayName.IndexOf(" of ") + 4).ToLower() + "_" + num);
            }
        }
    }
}

