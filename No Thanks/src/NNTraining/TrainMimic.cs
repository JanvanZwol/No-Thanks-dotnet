using System.Collections;

namespace NoThanks;

using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class TrainMimic
{
    private int exampleGames = 100000;
    public NNStrategy trainMimic(Strategy strategy)
    {
        // variables
        NNStrategy mimicStrategy = new NNStrategy(new int[] {170, 50});
        double naivePerc = 0.85;
        double learningRate = 0.05;
        double testPerc = 0.1;

        // Gather training data
        List<Gamestate> gamestatesList = new List<Gamestate>();

        Strategy[] stratgies = new Strategy[4];
        for (int i = 0; i < 4; i++) {
            stratgies[i] = new NaiveStrategy(naivePerc);
        }

        for (int i = 0; i < exampleGames; i++)
        {
            Game game = new Game(stratgies);
            game.play(false, true);
            History history = game.getHistory();

            foreach ((Gamestate, bool) turn in history.getHistory())
            {
                gamestatesList.Add(turn.Item1);
            }
        }

        Gamestate[] gamestatesTrain = gamestatesList.GetRange(0, (int) (gamestatesList.Count * (1-testPerc))).ToArray();
        Gamestate[] gamestatesTest = gamestatesList.GetRange((int) (gamestatesList.Count * (1 - testPerc)), (int) (gamestatesList.Count * testPerc)).ToArray();

        // Get action of base strategy
        // For training
        int[] answersTrain = new int[gamestatesTrain.Length];

        for (int i = 0; i < answersTrain.Length; i++) {
            answersTrain[i] = strategy.decide(gamestatesTrain[i]) ? 1 : 0;
        }

        // For testing
        int[] answersTest = new int[gamestatesTest.Length];

        for (int i = 0; i < answersTest.Length; i++) {
            answersTest[i] = strategy.decide(gamestatesTest[i]) ? 1 : 0;
        }

        // Train the network
        (Matrix<double>, Vector<double>)[] wbList = mimicStrategy.getTuples();

        double lastPrecision = -1;
        double precision = 0;

        Console.WriteLine("start");

        for (int i = 0; i < gamestatesTrain.Length; i++)
        {
            wbList = NNhelpers.backPropegate(wbList, gamestatesTrain[i], answersTrain[i], learningRate);

            if (i > 0 && i % (int) (gamestatesTrain.Length / 10) == 0)
            {
                mimicStrategy = new NNStrategy(wbList);
                lastPrecision = precision;
                int[] accuracy = getPrecision(mimicStrategy, gamestatesTest, answersTest);
                precision = (((double) accuracy[0] + (double) accuracy[2]) / ( (double) accuracy[0] +  (double) accuracy[1] + (double) accuracy[2] +  (double)accuracy[3]));
                Console.WriteLine($"Progress: {i}/{gamestatesTrain.Length} Presicion: {precision} TP: {accuracy[0]} FP: {accuracy[1]} TN: {accuracy[2]} FN: {accuracy[3]}");

                // if (precision == lastPrecision)
                // {
                //     break;
                // }
            }
        }

        return new NNStrategy(wbList);
    }

    public int[] getPrecision(NNStrategy nn, Gamestate[] testset, int[] answers) {
        return getPrecision(nn, testset, Array.ConvertAll<int, bool>(answers, x => x==1 ? true : false));
    }
    public int[] getPrecision(NNStrategy nn, Gamestate[] testset, bool[] answers)
    {
        int truePositive = 0;
        int falsePositive = 0;
        int trueNegative = 0;
        int falseNegative = 0;

        for (int i = 0; i < testset.Length; i++)
        {
            if (nn.decide(testset[i]))
            {
                if (answers[i])
                {
                    truePositive++;
                }
                else
                {
                    falsePositive++;
                }
            }
            else
            {
                if (answers[i])
                {
                    falseNegative++;
                }
                else
                {
                    trueNegative++;
                }
            }
        }
        
        return new int[] {truePositive, falsePositive, trueNegative, falseNegative}; 
    }

    public void setExampleGames(int n)
    {
        exampleGames = n;
    }
}