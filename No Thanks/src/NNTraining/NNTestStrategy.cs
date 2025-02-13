namespace NoThanks;

public class NNTestStrategy : Strategy
{
    public override bool decide(Gamestate gamestate)
    {
        return gamestate.getRevealedCard() < 19;       
    }
}