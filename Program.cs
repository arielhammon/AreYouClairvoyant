using System;
using System.Collections.Generic; //for implementing the List<T> Class

namespace AreYouClairvoyant
{
    class PokerDeck
    {
        public const int size = 52;
        public string[] faces = new string[13] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        //public string[] suits = new string[4] { "'\u2661'", "'\u2662'", "'\u2664'", "'\u2667'" }; //Hearts, Diamonds, Spades, Clubs
        public string[] suits = new string[4] { "H", "D", "S", "C" };
        public List<Card> cards = new List<Card>(size);

        private int Orderedness0; //initial Orderness measurement, Orderness of an unsuffled deck
        public PokerDeck()
        {
            for (int j = 0; j < suits.Length; j++)
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    cards.Add(new Card(face: i, suit: j, index: j*faces.Length + i));
                }
            }
            Orderedness0 = Orderedness();
        }
        public string cardName(int index)
        {
            Card card = cards[index];
            return faces[card.face] + suits[card.suit];
        }

        public int InitialOrderedness()
        {
            return Orderedness0;
        }

        private int Orderedness()
        //measures the degree of orderedness by performing a granular check (card-by-card) and non-granular check (histograms)
        {
            int n = 0; //used to measure the orderedness

            int i, j; //counters

            //perform card-by-card comparison
            Card card0;
            Card card1 = cards[0];
            int s0, s1; //the suits
            int f0, f1; //the face values
            for (i = 1; i < size; i++)
            {
                card0 = card1;
                card1 = cards[i];
                s0 = card0.suit;
                s1 = card1.suit;
                f0 = card0.face;
                f1 = card1.face;
                if (s0 == s1) { n += 2; } //adjacent cards have the same suit
                if (f0 == f1) { n += 8; } //adjacent cards have same face value
                if (f0 == (f1 == 0 ? 13 : f1) - 1) { n += 6; } //adjacent cards are consecutive
                if (f0 == (f1 == 12 ? -1 : f1) + 1) { n += 6; } //adjacent cards are reverse-consecutive
                if (f0 == (f1 == 1 ? 14 : f1) - 2) { n += 1; } //adjacent cards are semi-consecutive
                if (f0 == (f1 == 11 ? -2 : f1) + 2) { n += 1; } //adjacent cards are semi-reverse-consecutive
            }
            //Console.Write(n + " ");

            //now produce a histogram for each suit
            //histogram of 4 intervals representing the quarters of the deck
            int[] histogram = new int[4] { 0, 0, 0, 0 };
            int interval = 0; //the current interval
            int intervalLength = size / 4;
            int high, low; //the highest bar and lowest bar in the histogram
            for (j = 0; j < suits.Length; j++)
            {
                interval = -1;
                for (i = 0; i < size; i++)
                {
                    if (i % intervalLength == 0)
                    {
                        interval += 1;
                    }
                    if (cards[i].suit == j) //the current suit matches the target suit
                    {
                        histogram[interval] += 1;
                    }
                }
                high = 0;
                low = size;
                for (i = 0; i < 4; i++) //look at histogram
                {
                    high = (histogram[i] > high ? histogram[i] : high);
                    low = (histogram[i] < low ? histogram[i] : low);
                    histogram[i] = 0; //initialize for next iteration
                }
                n += (high - low)*3;
            }
            //Console.Write(n + " ");

            //now produce a histogram for each face value
            for (j = 0; j < faces.Length; j++)
            {
                interval = -1;
                for (i = 0; i < size; i++)
                {
                    if (i % intervalLength == 0)
                    {
                        interval += 1;
                    }
                    if (cards[i].face == j) //the current face value matches the target face value
                    {
                        histogram[interval] += 1;
                    }
                }
                high = 0;
                low = size;
                for (i = 0; i < 4; i++) //look at histogram
                {
                    high = (histogram[i] > high ? histogram[i] : high);
                    low = (histogram[i] < low ? histogram[i] : low);
                    histogram[i] = 0; //initialize for next iteration
                }
                n += high - low;
            }
            //Console.WriteLine(n + " ");

            return n;
        }
        public void Shuffle()
        //shuffles the deck by swapping cards until it becomes lowly ordered
        {
            int i, j; //counters

            Random rand = new Random(DateTime.Now.GetHashCode());
            int rndNum1, rndNum2; //random numbers
            Card temp;

            //cut the deck
            int cut = rand.Next(size / 3, 2 * size / 3);
            for (i = cut; i<size; i++) //move bottom of deck to top of deck
            {
                temp = cards[size-1];
                for (j=size-1; j>0; j--)
                {
                    cards[j] = cards[j - 1];
                }
                cards[0] = temp;
            }

            //now shuffle
            int n = 0; //the number of times cards have been suffled
            double orderThreshold = 0.2; //this is a fairly strict requirement and may take a long time to reach
            do
            {
                for (i = 0; i < size / 2; i++)
                {
                    rndNum1 = rand.Next(size);
                    rndNum2 = rand.Next(size);
                    temp = cards[rndNum2];
                    cards[rndNum2] = cards[rndNum1];
                    cards[rndNum1] = temp;
                }
                n += 1;
                if (n % 100 == 0)
                {
                    orderThreshold *= 1.05; //gradually increase orderThreshold by 5% in order to ensure stop condition is met
                }
            } while ((Orderedness() > (InitialOrderedness() * orderThreshold)) || n < 5);
            //Console.WriteLine(n);
        }
        public void PrintDeck()
        {
            int i, j;
            for (j=0; j<suits.Length; j++)
            {
                for (i=0; i<faces.Length; i++)
                {
                    Console.Write(cardName(j * faces.Length + i) + " ");
                }
                Console.WriteLine();
            }
        }
    }
    class Card
    {
        public int suit;
        public int face;
        public int index; //different from list index. Can represent initial index (deck position) or any other as desired.
        public Card(int face, int suit, int index)
        {
            this.face = face;
            this.suit = suit;
            this.index = index;
        }
    }

    class Program
    {
        static double binProb(int correctguesses, int totalguesses)
        //the binomial probability
        {
            int i; //counters
            int r = correctguesses;
            int n = totalguesses;
            int nn = PokerDeck.size;
            double p = (1.0 / nn + 1.0 / (nn - n + 1)) / 2; //average of probability for first guess and probability for last guess
            double q = 1.0 - p; //probability of not guessing correctly
            double nPr = 1.0;  //combinatorics nPr = n!/((n-r)!r!) as calculated below
            double P = 1.0; //P = p^r
            double Q = 1.0; //Q = q^(n-r)
            for (i = 0; i < r; i++)
            {
                nPr = nPr * (n - i) / (r - i);
                P *= p;
            }
            for (i = 0; i<n-r; i++)
            {
                Q *= q;
            }
            return nPr * P * Q;
        }
        static void Main(string[] args)
        {
            int i, ct; //counters

            //initialize the deck
            PokerDeck deck = new PokerDeck();
            PokerDeck staticDeck = new PokerDeck(); //this deck to remain unshuffled for instructional purposes

            bool play = true;
            string playAgain = "";
            Console.WriteLine("Welcome to \"Are You Clairvoyant\" where you can test your ability to predict the future!");
            Console.WriteLine("We'll be using a standard poker deck.");
            while (play)
            {
                Console.WriteLine("To make a prediction, simply enter one of the following:  ");
                staticDeck.PrintDeck();
                Console.WriteLine("Get ready to guess five cards. Press [Enter] to begin.");
                Console.ReadLine();
                Console.WriteLine("Shuffling the deck....");
                deck.Shuffle();
                deck.PrintDeck();
                string guess, topCard;
                ct = 0; //The number of correct guesses
                double prob = 1; //used to compute the probability
                for (i = 0; i < 5; i++)
                {
                    Console.Write("Okay, now guess the top card: ");
                    guess = Console.ReadLine();
                    topCard = deck.cardName(i);
                    if (guess == topCard)
                    {
                        Console.WriteLine("Wow! Excellent guess! You were right.");
                        ct += 1;
                        prob *= 1.0 / (PokerDeck.size - i);
                    }
                    else
                    {
                        Console.WriteLine("Sorry, that's not correct. It was actually {0}.", topCard);
                        prob *= 1.0 - 1.0 / (PokerDeck.size - i);
                    }
                }
                Console.WriteLine();
                if (ct == 0)
                {
                    Console.WriteLine("You didn't guess any correctly. Probably not clairvoyant.");
                }
                else
                {
                    Console.WriteLine("You were right {0} out of 5 times.", ct);
                    //following line is for math nerds only
                    //Console.WriteLine("The probablity of randomly guessing correctly/incorrectly in this exact sequence is {0}.", prob.ToString("P"));
                    Console.WriteLine("The probablity of randomly guessing any {0} correct out of 5 is {1}.", ct, binProb(ct, 5).ToString("P"));
                    switch (ct)
                    {
                        case 1:
                            Console.WriteLine("You may be clairvoyant.");
                            break;
                        case 2:
                            Console.WriteLine("That's quite rare. You might be clairvoyant.");
                            break;
                        case 3:
                            Console.WriteLine("That's unheard of. You've gotta be clairvoyant.");
                            break;
                        default:
                            Console.WriteLine("You've gotta be clairvoyant! Can I take you to Vegas with me?");
                            break;
                    }
                }
                Console.WriteLine("Would you like to play again? y/n: ");
                playAgain = Console.ReadLine();
                if (playAgain == "n")
                {
                    play = false;
                }
            }
            
        }
    }
}
