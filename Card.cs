using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21
{
    public class Card
    {
        public string id;
        public string displayName;
        //store the image to Card class
        //public string img;
        //string num = Regex.Match(cards[i].id.Substring(0, 1), "[AJQK]").Success ? cards[i].id.Substring(0, 1) : cards[i].id.Substring(0, 1).PadLeft(2, '0');

        public Card(string name, string nameLong)
        {
            id = name;
            displayName = nameLong;
            //img = "card_" + cards[i].displayName.Substring(cards[i].displayName.IndexOf(" of ") + 4).ToLower() + "_" + num
        }
    }
}
