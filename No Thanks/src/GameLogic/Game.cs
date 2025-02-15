

namespace NoThanks;

public class Game
{
    private Player[] players;
    private int numPlayers;
    private Deck deck;
    private History history;
    
    public Game(Strategy[] strategies)
    {
        // Create Deck
        deck = new Deck();
        // Create history object
        history = new History();
        

        // Create players
        numPlayers = strategies.Length;
        players = new Player[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            players[i] = new Player(strategies[i], numPlayers);
        }
    }

    public int[] play(bool verbose, bool recordHistory) {
        // Turn counter
        int turn = 0;

        // Stop when end of deck is reached
        while (deck.getPointer() < Deck.NUMCARDS)
        {
            // Draw a card
            int revealedCard = deck.drawCard();
            int pot = 0;
            bool cardNotTaken = true;

            // Draw a new card when the current card has been taken
            while (cardNotTaken)
            {
                Player activePlayer = players[turn];
                Gamestate gamestate = new Gamestate(revealedCard, pot, players, turn);

                // Let the player decide
                bool take;
                if (activePlayer.hasChips())
                {
                    take = activePlayer.decide(gamestate);
                }
                else
                {
                    take = true;
                }

                // Record move
                if (recordHistory) {history.recordAction(gamestate, take);}

                // Handle decision
                if (take)
                {   
                    // Give player the card and the pot and start new round
                    activePlayer.take(revealedCard, pot);
                    cardNotTaken = false;
                }
                else {
                    // Take a chip from the player, put in the pot and go to the next player
                    activePlayer.pay();
                    pot++;
                    turn = (turn + 1) % numPlayers;
                }
            }
        }

        // Calculate the scores when all cards have been dealt
        int[] scores = new int[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            scores[i] = players[i].score();
        }

        return scores;
    }

    public History getHistory() {
        return history;
    }
}