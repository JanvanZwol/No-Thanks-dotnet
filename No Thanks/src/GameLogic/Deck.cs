using System.Collections;
using System.Text;

namespace NoThanks;
public class Deck
{
    public static readonly int LOWESTCARD = 3;
    public static readonly int NUMCARDS = 33;

    private int[] deck;
    private int pointer;


    public Deck()
    {
        // Initialize cards
        deck = new int[NUMCARDS];
        for (int i = 0; i < NUMCARDS; i++)
        {
            deck[i] = i + LOWESTCARD;
        }

        // Shuffle deck
        Random random = new Random();
        random.Shuffle(deck);

        // Set pointer
        pointer = 9;
    }

    public int drawCard()
    {
        // Return the top card of the deck
        int card = deck[pointer];
        pointer++;
        return card;
    }

    public int getPointer()
    {
        // Return the pointer
        return pointer;
    }

    public override String ToString()
    {
        // Instanitate Stringbuilder
        StringBuilder sb = new StringBuilder();

        // Add values of the cards as strings
        for (int i = 0; i < NUMCARDS; i++)
        {
            sb.Append(deck[i].ToString());
            
            if (i < NUMCARDS - 1) {
                sb.Append(", ");
            }
        }

        // Return resulting string
        return sb.ToString();
    }
}
