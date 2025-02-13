using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace NoThanks;

public class History 
{
    // A turn is represented as a 32 bit unsigned integer
    // The bits are as follows:
    // 0000 0000 0000 0PPP CCCC CRRR RROO OOOA
    // P: player index                  18-16
    // C: player chips before action    15-11
    // R: revealed card                 10-6
    // O: pot size before action        5-1
    // A: action taken                  0
    public LinkedList<uint> history;

    public History() 
    {
        history = new LinkedList<uint>();
    }

    public void recordAction(int playerIndex, int playerChips, int revealedCard, int pot, bool action) {
        uint record = 0;
        record += (uint) playerIndex << 16;
        record += (uint) playerChips << 11;
        record += (uint) revealedCard << 6;
        record += (uint) pot << 1;
        if (action)
        {
            record += 1;
        }
        history.AddLast(record);
    }

    public LinkedList<uint> getHistory()
    {
        return history;
    }

    public Dictionary<String, int> decypherTurn(uint turn)
    {
        Dictionary<String, int> decTurn = new Dictionary<String, int>();
        uint playerIndex = (turn >> 16);
        turn -= playerIndex << 16;
        uint playerChips = (turn >> 11);
        turn -= playerChips << 11;
        uint revealedCard = (turn >> 6);
        turn -= revealedCard << 6;
        uint pot = (turn >> 1);
        turn -= pot << 1;

        decTurn["playerIndex"] = (int) playerIndex;
        decTurn["playerChips"] = (int) playerChips;
        decTurn["revealedCard"] = (int) revealedCard;
        decTurn["pot"] = (int) pot;
        decTurn["action"] = (int) turn;

        return decTurn;
    }

    public void printGame() {
        foreach (uint turn in history)
        {
            Dictionary<String, int> decTurn = decypherTurn(turn);
            Console.WriteLine($"Player Turn: {decTurn["playerIndex"]}  Revealed card: {decTurn["revealedCard" ], 2} | Pot: {decTurn["pot"], 2} | action: {decTurn["action"]}");
        }
    }

    public double getVoluntaryTakeRate()
    {
        int voluntaryTakes = 0;
        Dictionary<String, int> decTurn;

        foreach (uint turn in history)
        {
            decTurn = decypherTurn(turn);
            
            if (decTurn["playerChips"] > 0 && decTurn["action"] == 1)
            {
                voluntaryTakes++;
            }
        }

        return voluntaryTakes / history.Count;
    }
}