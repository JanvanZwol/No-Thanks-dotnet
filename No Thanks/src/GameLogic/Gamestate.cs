

namespace NoThanks;
public class Gamestate
{
    private int revealedCard;
    private int pot;
    private int[] playerChips;
    private ulong[] playerCards;
    private int turn;

    public Gamestate(int revealedCard, int pot, Player[] players, int turn)
    {
        int numPlayers = players.Length;
        this.turn = turn;

        // Assign values for revealed card and pot
        this.revealedCard = revealedCard;
        this.pot = pot;
        
        // Assign values for the player attributes
        playerChips = new int[numPlayers];
        playerCards = new ulong[numPlayers];

        for (int i = 0; i < numPlayers; i++)
        {
            int j = (turn + i) % numPlayers;
            playerChips[j] = players[j].getChips();
            playerCards[j] = players[j].getCards();
        }
    }

    public int getRevealedCard()
    {
        return revealedCard;
    }

    public int getPot()
    {
        return pot;
    }

    public int[] getPlayerChips()
    {
        return playerChips;
    }

    public ulong[] getPlayerCards()
    {
        return playerCards;
    }

    public int getTurn()
    {
        return turn;
    }

    public ulong allCardsPlayed()
    {
        // Create empty card set
        ulong res = 0;

        // Or it with all player card sets (adding would also work)
        for (int i = 0; i < playerCards.Length; i++) {
            res = res | playerCards[i];
        }        

        return res;
    }

    public int numberCardsPlayed()
    {   
        // Get all cards played
        int cardsPlayed = 0;
        ulong allCardsPlayed = this.allCardsPlayed();

        //Check how many 1's in the ulong
        for (int i = 0; i < Deck.NUMCARDS; i++)
        {
            if ((allCardsPlayed & (ulong) 1 << i) != 0)
            {
                cardsPlayed++;
            }
        }

        return cardsPlayed;
    }
}