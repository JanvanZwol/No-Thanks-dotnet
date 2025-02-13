using NoThanks;

namespace NoThanks;
public class NaiveStrategy : Strategy {
    private Random random;
    private double threshold = 0.5;

    public NaiveStrategy() {
        random = new Random();
    }

    public NaiveStrategy(double threshold)
    {
        random = new Random();
        this.threshold = threshold;
    }

    public override bool decide(Gamestate gamestate)
    {
        return random.NextDouble() > threshold;
    }
}