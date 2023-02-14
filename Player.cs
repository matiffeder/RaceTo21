using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name;
        //replaced string with Card class to store short and long name
        //public List<string> cards = new List<string>();
        public List<Card> cards = new List<Card>();
        public PlayerStatus status = PlayerStatus.active;
		public int score;

		public Player(string n)
		{
			name = n;
        }

		/* Introduces player by name
		 * Called by CardTable object
		 */
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum);
		}
	}
}

