using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace NoThanks;

public class History 
{
    public LinkedList<(Gamestate, bool)> history;

    public History() 
    {
        history = new LinkedList<(Gamestate, bool)>();
    }

    public void recordAction(Gamestate gamestate, bool action)
    {
        history.AddLast((gamestate, action));
    }

    public LinkedList<(Gamestate, bool)> getHistory()
    {
        return history;
    }

    public void printGame() {
        foreach ((Gamestate, bool) turn in history)
        {
            Console.WriteLine($"Player Turn: {turn.Item1.getTurn()}  Revealed card: {turn.Item1.getRevealedCard(), 2} | Pot: {turn.Item1.getPot(), 2} | action: {turn.Item2}");
        }
    }

    public double getVoluntaryTakeRate()
    {
        double voluntaryTakes = 0;

        foreach ((Gamestate, bool) turn in history)
        {  
            if (turn.Item1.getPlayerChips()[0] > 0 && turn.Item2)
            {
                voluntaryTakes++;
            }
        }

        return voluntaryTakes / history.Count;
    }


}