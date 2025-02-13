namespace NoThanks;

public class Strategy_1 : Strategy
{
    public override bool decide(Gamestate gamestate) {
        // Check whether you can earn more chips than than the cards worth
        if (gamestate.getPot() >= gamestate.getRevealedCard())
        {
            return true;
        }
        // Check whether you have a card that is one lower or higher
        ulong mycards = gamestate.getPlayerCards()[0];
        bool hasCardAbove = (((mycards >> gamestate.getRevealedCard() + 1) & 1)) == 1;
        bool hasCardBelow = (((mycards >> gamestate.getRevealedCard() - 1) & 1)) == 1;
        if (hasCardBelow && !(gamestate.getRevealedCard() == 3))
        {
            return true;
        }
        else if (hasCardAbove) {
            return true;
        }

        // If no conditions hold do not take the card
        return false;
    }
}