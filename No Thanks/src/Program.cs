using System.Diagnostics;

using NoThanks;

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
    game.play(false);
}
watch.Stop();
Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");