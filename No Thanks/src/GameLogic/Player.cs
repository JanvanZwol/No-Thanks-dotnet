
namespace NoThanks;

public class Player
{
    private ulong cards;
    private int chips;
    private Strategy strategy;

    public Player(Strategy strategy, int numPlayers)
    {
        // Set variables
        cards = 0;
        this.strategy = strategy;

        // Determine number of chips based on players
        switch (numPlayers)
        {
            case 6:
                chips = 9;
                break;
            case 7:
                chips = 7;
                break;
            default:
                chips = 11;
                break;
        }
    }

    public bool decide(Gamestate gamestate)
    {
        return strategy.decide(gamestate);
    }

    public void pay()
    {
        // Pay one chip
        chips--;
    }


    public void take(int card, int pot)
    {    
        // Add pot to chips
        chips += pot;

        // Add card to cards
        ulong bitCard = (ulong) 1 << (card - Deck.LOWESTCARD);
        cards = cards | bitCard;
    }

    public int score()
    {
        // Instatiate Score
        int score = 0;

        // Adapt the cards ulong to only include cards that count towards points
        ulong countableCardsMask = ~ (cards << 1); 
        ulong countableCards = cards & countableCardsMask;


        // Add the scores of countable cards
        // Probably can be optimized
        for (int i = 0; i < Deck.NUMCARDS; i++)
        {
            if ((countableCards & (ulong) 1 << i) != 0)
            {
                score += i + Deck.LOWESTCARD;
            }
        }
        
        // Remove chips from score
        score -= chips;

        return score;
    }

    public int getChips()
    {
        return chips;
    }

    public ulong getCards()
    {
        return cards;
    }

    public bool hasChips()
    {
        return chips > 0;
    }
}