using NoThanks;

namespace NoThanks;
public class NaiveStrategy : Strategy {
    private Random random;

    public NaiveStrategy() {
        random = new Random();
    }

    public override bool decide(Gamestate gamestate)
    {
        return random.NextDouble() > 0.5;
    }
}