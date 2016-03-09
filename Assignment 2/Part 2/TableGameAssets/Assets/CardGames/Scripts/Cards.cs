using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CardLib
{
	/// <summary>
	/// Card class for playing cards.
	/// </summary>
	/// 
	public class Card
	{
        // Card size (dimension) in meters
        public static float Width   = 0.064f;
        public static float Height  = 0.089f;

		// Data members
        private GameObject cardObject;   // card game object name, e.g., "Card_c01", "Card_s12", etc.
		private char color;             // 'C', 'D', 'H', and 'S'
		private int  number;            // card number: 1,2,3, .., 13

        private float x, y;             // card position (z is alywas 0) in the world space
        private bool isFaceUp = false;    

		// Function member
		public Card()
		{
			color = 'U';
			number = 0;
			cardObject = null;
		}
		public Card (char c, int n)
		{
			color = c;
			number = n;
			cardObject = null;
		}
		public Card (char c, int n, GameObject obj)
		{
			color = c;
			number = n;
            cardObject = obj;
		}

        public GameObject CardObject
        {
            get { return cardObject; }
            set { cardObject = value; }
        }

        public char Color
        {
            get { return color; }
            set { if (value == 'S' || value == 'H' || value == 'D' || value == 'C') color = value; }
        }

        public int Number
        {
            get { return number; }
            set { if (value >= 1 && value <= 13) number = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public bool IsFaceUp
        {
            get { return isFaceUp; }
            set { isFaceUp = value; }
        }

		public void Set (char c, int n)
		{
			if (c == 'S' || c == 'H' || c == 'D' || c == 'C')
				color = c;
			else return;
			if (n > 0 && n < 14)
				number = n;
		}
	}

	/// <summary>
	/// Deck class for playing card deck.
	/// </summary>
	/// 
	public class Deck
	{
		// Data members
		//ArrayList cards;
        List<Card> cards;
		int last;

		// Function members
		public Deck ()
		{
            last = 0;
            cards = new List<Card>();

/*
			int i;

			//cards = new ArrayList (53);
            cards = new List<Card>(53);
			cards.Add (new Card ());
			for (i = 1; i <= 13; ++i)
				cards.Add (new Card('S', i));
			for (i = 1; i <= 13; ++i)
				cards.Add (new Card('H', i));
			for (i = 1; i <= 13; ++i)
				cards.Add (new Card('D', i));
			for (i = 1; i <= 13; ++i)
				cards.Add (new Card('C', i));

			last = 52;
 */
		}
		public Deck (Card[] in_cards)
		{
			int i, n;
			n = in_cards.Length;
			if (n > 0)
			{
				//cards = new ArrayList (n+1);
                cards = new List<Card>(n + 1);
				for (i = 1; i <= n; ++i)
					cards.Add (in_cards[i]);
			}
			last = n;
		}

        public List<Card> Cards
        {
            get { return cards; }
        }

        public void AddCard(Card card)
        {
            cards.Add(card);
            last++;
        }

		public Card this [int index]
		{
			get
			{
				if (index >= 1 && index <= last) 
					return (Card) cards[index];
				else
					return (Card) cards[0];
			}
			set
			{
				if (index >=1 && index <= last) 
					cards[index] = value;
			}
		}
		public Card Last
		{
			get
			{
				return (Card) cards[last];
			}
			set
			{
				cards[last] = value;
			}
		}

		public void Set ()
		{
			int i;
			if (last != 52)
			{
				//cards = new ArrayList (53);
                cards = new List<Card>(53);

				for (i = 1; i <= 13; ++i)
					cards.Add (new Card('S', i));
				for (i = 1; i <= 13; ++i)
					cards.Add (new Card('H', i));
				for (i = 1; i <= 13; ++i)
					cards.Add (new Card('D', i));
				for (i = 1; i <= 13; ++i)
					cards.Add (new Card('C', i));

				last = 52;
			}
			else
			{
				for (i = 1; i <= 13; ++i)
					this[i].Set ('S', i);
				for (i = 1; i <= 13; ++i)
					this[i+13].Set('H', i);
				for (i = 1; i <= 13; ++i)
					this[i+26].Set('D', i);
				for (i = 1; i <= 13 ; ++i)
					this[i+39].Set('C', i);
			}
		}

		public void Set (Card[] in_cards)
		{
			int i, n;
			n = in_cards.Length;
			if (n > 0)
			{
				//cards = new ArrayList (n+1);
                cards = new List<Card>(n + 1);
				for (i = 1; i <= n; ++i)
					cards.Add (in_cards[i]);
				last = n;
			}
		}

		public void Shuffle (int n)
		{
			if (n <= 0) return;
			int i, j;
			Card temp;
			System.Random rand = new System.Random ();
			while (--n >= 0)
			{
				for (i = 0; i < last; ++i)
				{
					// Random card swap
					j = rand.Next (last);
					temp = cards[i];
					cards[i] = cards[j];
					cards[j] = temp;
				}
			}
		}

		public void Cut (int n)
		{
			if (n <= 0) return;
			int i, j, k;
			Card[] temp = new Card[last + 1];
			System.Random rand = new System.Random();

			while (--n >= 0)
			{	
				k = rand.Next(last) + 1;
				if (k == 1) continue;
				j = 1;
				// Cut the deck
				for (i = k; i <= last; ++i)
					temp[j++] = this[i];
				for (i = 1; i < k; ++i)
					temp[j++] = this[i];

				for (i = 1; i <= last; ++i)
					this[i] = temp[i];
			}
		}
	}

}
