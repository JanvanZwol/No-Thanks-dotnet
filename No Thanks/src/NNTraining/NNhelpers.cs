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

    public static double computeLoss((Matrix<double>, Vector<double>)[] nn, Vector<double> layerVector, double answer)
    {
        for (int i = 0; i < nn.Length; i++)
        {
            layerVector = nn[i].Item1 * layerVector;
            layerVector = layerVector + nn[i].Item2;
            layerVector = NNhelpers.sigmoidActivateVector(layerVector);
        }

        double outputNode = layerVector.At(0);

        return Math.Pow(answer - outputNode, 2) / 2;
    }

    public static (Matrix<double>, Vector<double>)[] backPropegate((Matrix<double>, Vector<double>)[] nn, Gamestate gamestate, int answer, double learningRate)
    {
        // Get activations
        Vector<double>[] layerActivations = new Vector<double>[nn.Length + 1];

        layerActivations[0] = getInputVector4(gamestate);

        for (int i = 1; i < nn.Length + 1; i++)
        {
            Vector<double> previousLayer;
            // get the previous layer
            if (i == 0) {previousLayer = NNhelpers.getInputVector4(gamestate);}
            else {previousLayer = layerActivations[i-1];}

            // calculate activations
            Vector<double> layer = nn[i-1].Item1 * previousLayer;
            layer = layer + nn[i-1].Item2;
            layer = NNhelpers.sigmoidActivateVector(layer);

            // Add it to the array
            layerActivations[i] = layer;
        }

        // calculate output errors
        Vector<double>[] errors = new Vector<double>[nn.Length + 1];
        for (int i = nn.Length; i > 0; i--)
        {
            errors[i] = Vector<double>.Build.Dense(layerActivations[i].Count);

            if (i == nn.Length)
            {
                // The error values of the last layer is:
                // (Gradient) Costfunction (pointwise multiplication) (derivative) (activationfunction) activation value
                Vector<double> gradientCost =  Vector<double>.Build.Dense(1, answer) - layerActivations[i];
                Func<double, double> sigmoidPrime = x => x * (1 - x);
                Vector<double> sigmoidDerivative = layerActivations[i].Map(sigmoidPrime);

                errors[i] = gradientCost.PointwiseMultiply(sigmoidDerivative);
            }
            else
            {
                // The error for an intermediate layer is (l)
                // (transpose) weights from l to l+1 * error vector of l+1 (pointwise multiplication) (derivative) (activationfunction) activation value
                Matrix<double> transposedWeights = nn[i].Item1.Transpose();
                Func<double, double> sigmoidPrime = x => x * (1 - x);
                Vector<double> sigmoidDerivative = layerActivations[i].Map(sigmoidPrime);

                errors[i] = (transposedWeights * errors[i + 1]).PointwiseMultiply(sigmoidDerivative);
            }
        }

        // Adjust weights and biases
        (Matrix<double>, Vector<double>)[] nnPrime =  new (Matrix<double>, Vector<double>)[nn.Length];
        for (int i = 0; i < nn.Length; i++)
        {
            // Weights
            Matrix<double> weightPrime = nn[i].Item1 + (layerActivations[i+1] * errors[i+1] * learningRate);

            // Bias
            Vector<double> biasPrime = nn[i].Item2 + (errors[i+1] * learningRate);

            nnPrime[i] = (weightPrime, biasPrime);
        }

        return nnPrime;
    }
}