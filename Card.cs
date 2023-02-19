using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21
{
    public class Card
    {
        //save card id privatly;
        private string? cardId;
        //save card displayName privatly;
        private string? cardName;
        //the way to store the image to Card class, please refer the comment in Deck.SetCardImg()
        //public string img;
        //string num = Regex.Match(cards[i].id.Substring(0, 1), "[AJQK]").Success ? cards[i].id.Substring(0, 1) : cards[i].id.Substring(0, 1).PadLeft(2, '0');

        //use public property to get and set card short name
        public string id { get { return cardId; } set { cardId = value; } }
        //use public property to get and set card long name
        public string displayName { get { return cardName; } set { cardName = value; } }

        public Card(string name, string nameLong)
        {
            id = name;
            displayName = nameLong;
            ///the way to store the image to Card class, please refer the comment in Deck.SetCardImg()
            //img = "card_" + cards[i].displayName.Substring(cards[i].displayName.IndexOf(" of ") + 4).ToLower() + "_" + num
        }
    }
}
