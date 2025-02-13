
using MathNet.Numerics.LinearAlgebra;

namespace NoThanks;

public class TrainingPrograms
{
    public static (Matrix<double>[], Vector<double>[]) SGTEvolution(int games, double magnitude, double probability)
    {
        //Plays a single game tournament let the winners reproduce

        // Generate 4 initial strategies
        Strategy[] strategies = new Strategy[4];
        for (int i = 0; i < 4; i++) { strategies[i] = new NNStrategy(); }

        // Play n games
        Game game;
        int[] scores;
        int winnerIndex;
        NNStrategy winner;

        double voluntaryTakesRate = 0;

        for (int i = 0; i < games + 1; i++)
        {
            // Let them play a game
            game = new Game(strategies);
            scores = game.play(false, true);

            if ((i + 1) % 1000 == 0)
            {
                Console.WriteLine(voluntaryTakesRate / 100);
                voluntaryTakesRate = 0;
            }

            voluntaryTakesRate += game.GetHistory().getVoluntaryTakeRate();

            // reproduce first place
            winnerIndex = Array.IndexOf(scores, scores.Min());
            winner = (NNStrategy) strategies[winnerIndex];
            for (int j = 0; j < 4; j++)
            {
                (Matrix<double>, Vector<double>)[] childwb = winner.reproduce(magnitude, probability);
                strategies[j] = new NNStrategy(childwb);
            }
        }

        // Play last game to see scores
        game = new Game(strategies);
        scores = game.play(false, false);
        winnerIndex = Array.IndexOf(scores, scores.Min());
        Console.WriteLine(String.Join(", ",  scores));
        winner = (NNStrategy) strategies[winnerIndex];
        return (winner.getWeights(), winner.getBiases());
    }

    public static (Matrix<double>[], Vector<double>[]) eliminateSuckers(int n)
    {
        // Generate 4 initial strategies
        Strategy[] strategies = new Strategy[4];
        for (int i = 0; i < 4; i++) { strategies[i] = new NNStrategy(); }

        
        Game game;
        int[] scores;
        for (int i = 0; i < n - 1; i++)
        {
            // Let them play a game
            game = new Game(strategies);
            scores = game.play(false, false);

            // Replace last place
            int suckerIndex = Array.IndexOf(scores, scores.Max());
            strategies[suckerIndex] = new NNStrategy();
        }

        //return best of the last game
        game = new Game(strategies);
        scores = game.play(false, false);
        int winnerIndex = Array.IndexOf(scores, scores.Min());
        Console.WriteLine(String.Join(", ",  scores));
        NNStrategy winner = (NNStrategy) strategies[winnerIndex];
        return (winner.getWeights(), winner.getBiases());
    }
}