﻿using System.Diagnostics;
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

var watch = new System.Diagnostics.Stopwatch();
watch.Start();
TrainingPrograms.SGTEvolution(10000, 0.5, 0.1);
watch.Stop();
Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");