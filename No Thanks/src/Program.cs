using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using NoThanks;


void time10000()
{
    var watch = new System.Diagnostics.Stopwatch();
    int n = 100000;


    Strategy[] strategies = new Strategy[4];
    for (int i = 0; i < 4; i++)
    {
        strategies[i] = new NNStrategy();
    }

    watch.Start();
    for (int i = 0; i < n; i++) {
        Game game = new Game(strategies);
        game.play(false, false);
    }
    watch.Stop();
    Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
}

void oneGame()
{
    Strategy[] strategies = new Strategy[4];
    for (int i = 0; i < 4; i++)
    {
        strategies[i] = new NNStrategy();
    }
    Game game = new Game(strategies);
    int[] scores = game.play(false, true);
    Console.WriteLine(String.Join(", ",  scores));
}


TrainMimic mt = new TrainMimic();
NNStrategy nn = mt.trainMimic(new Strategy_1());
NNhelpers.writeNN(NNhelpers.NNPATH + "/test.nn", nn);


// Strategy[] strategies = new Strategy[4];
// for (int i = 0; i < 4; i++)
// {
//     strategies[i] = new Strategy_1();
// }
// Game game = new Game(strategies);
// game.play(false, true);
// History hist = game.getHistory();

// Console.WriteLine("" + hist.getVoluntaryTakeRate());