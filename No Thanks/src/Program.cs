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
        strategies[i] = new NaiveStrategy();
    }

    watch.Start();
    for (int i = 0; i < n; i++) {
        Game game = new Game(strategies);
        game.play(false, true);
    }
    watch.Stop();
    Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
}

void printGame()
{
    Strategy[] strategies = new Strategy[4];
    for (int i = 0; i < 4; i++)
    {
        strategies[i] = new NaiveStrategy();
    }
    Game game = new Game(strategies);
    int[] scores = game.play(false, true);
    Console.WriteLine(String.Join(", ",  scores));
    game.GetHistory().printGame();
}

Matrix<double> A = DenseMatrix.OfArray(new double[,] {
        {1,1,1,1},
        {1,2,3,4},
        {4,3,2,1}});

Matrix<double> B = DenseMatrix.OfArray(new double[,] {
        {1,1,1,1},
        {1,2,3,4},
        {4,3,2,1},
        {3,2,4,1}});


Console.WriteLine(A * B);
