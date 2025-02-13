using System.Text.Json.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Newtonsoft.Json;
using System;
using System.IO;

namespace NoThanks;

public class NNhelpers
{
    public static readonly String NNPATH = "C:\\Users\\janvz\\programming\\No Thanks dotNet\\No Thanks\\src\\Strategies\\NN\\";

    public static Vector<double> getInputVector4(Gamestate gamestate)
    {
        // Creates a vector from a gamestate for a 4 player game
        // It has 170 nodes
        // 0-32: revealed card
        // 33: pot
        // 34-66: my cards
        // 67: my Chip
        // 68-100 101 player n+1 cards and chips
        // 102-134 135 player n+2 cards and chips
        // 136-168 169 player n+3 cards and chips

        Vector<double> vector = Vector<double>.Build.Dense(170);

        // Set revealed card
        vector.At(gamestate.getRevealedCard() - Deck.LOWESTCARD, 1);

        // Set pot
        vector.At(33, gamestate.getPot());

        //Set player cards and chips
        ulong[] playerCards = gamestate.getPlayerCards();
        int[] playerChips = gamestate.getPlayerChips();

        for (int i = 0; i < 4; i++)
        {
            int startIndex = 34 + 34 * i;
            // Set cards
            ulong cards = playerCards[i];
            for (int j = 0; j < Deck.NUMCARDS; j++)
            {
                if (((cards >> j) & 1) != 0)
                {
                    vector.At(startIndex + j, 1);
                }
            }

            // Set chips
            vector.At(startIndex + 33, playerChips[i]);
        }

        return vector;
    }

    public static Vector<double> sigmoidActivateVector(Vector<double> vector)
    {
        // For each value in the layer use the sigmoid activation function
        Func<double, double> sigmoid = x => 1.0 / (1.0 + Math.Exp(-x));

        Vector<double> sigmoidVector = vector.Map(sigmoid);

        return sigmoidVector;
    }

    public static void writeNN(String path, NNStrategy nn)
    {
        Matrix<double>[] weights = nn.getWeights();
        Vector<double>[] biases = nn.getBiases();

        (double[,], double[])[] wbList = new (double[,], double[])[weights.Length];

        for (int i = 0; i < wbList.Length; i++)
        {
            wbList[i] = (weights[i].ToArray(), biases[i].ToArray());
        }

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(wbList, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static NNStrategy readNN(string path)
    {
        String readJson = File.ReadAllText(path);
        var readData = JsonConvert.DeserializeObject<(double[,], double[])[]>(readJson);

        (Matrix<double>, Vector<double>)[] wbList = new (Matrix<double>, Vector<double>)[readData.Length];
        for (int i = 0; i < wbList.Length; i++)
        {
            wbList[i] = (Matrix<double>.Build.DenseOfArray(readData[i].Item1), Vector<double>.Build.DenseOfArray(readData[i].Item2));
        }
        
        return new NNStrategy(wbList);
    }

    public static (Matrix<double>, Vector<double>)[] readwbList(string path)
    {
        String readJson = File.ReadAllText(path);
        var readData = JsonConvert.DeserializeObject<(double[,], double[])[]>(readJson);

        Console.WriteLine(Matrix<double>.Build.DenseOfArray(readData[0].Item1));

        (Matrix<double>, Vector<double>)[] wbList = new (Matrix<double>, Vector<double>)[readData.Length];
        for (int i = 0; i < wbList.Length; i++)
        {
            wbList[i] = (Matrix<double>.Build.DenseOfArray(readData[i].Item1), Vector<double>.Build.DenseOfArray(readData[i].Item2));
        }
        
        return wbList;
    }
}