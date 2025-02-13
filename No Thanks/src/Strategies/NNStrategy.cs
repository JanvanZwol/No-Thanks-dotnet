using System.Dynamic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace NoThanks;

public class NNStrategy : Strategy
{
    private Matrix<Double>[] weights;
    private Vector<double>[] biases;

    public NNStrategy(Matrix<double>[] weights, Vector<double>[] biases)
    {
        this.weights = weights;
        this.biases = biases;
    }

    public NNStrategy((Matrix<double>, Vector<double>)[] wbList)
    {
        weights = new Matrix<Double>[wbList.Length];
        biases = new Vector<double>[wbList.Length];

        for (int i=0; i < wbList.Length; i++)
        {
            weights[i] = wbList[i].Item1;
            biases[i] = wbList[i].Item2;
        }
    }

    public NNStrategy(String path)
    {
        NNhelpers.readNN(path);
    }


    public NNStrategy()
    {
        this.weights = new Matrix<double>[2];
        this.biases = new Vector<double>[2];

        // generate first layer
        weights[0] = Matrix<double>.Build.Random(50,170);
        biases[0] = Vector.Build.Random(50);

        //generate second layer
        weights[1] = Matrix<double>.Build.Random(1, 50);
        biases[1] = Vector.Build.Random(1);
    }

    public override bool decide(Gamestate gamestate)
    {   
        Vector<double> layerVector = NNhelpers.getInputVector4(gamestate);

        for (int i = 0; i < weights.Length; i++)
        {
            layerVector = weights[i] * layerVector;
            layerVector = layerVector + biases[i];
            layerVector = NNhelpers.sigmoidActivateVector(layerVector);
        }

        double outputNode = layerVector.At(0);

        return outputNode > 0.5;
    }

    public (Matrix<double>, Vector<double>)[] reproduce(double magnitude, double probability)
    {
        // Matrix<double>[] childWeights = new Matrix[weights.Length];
        // Vector<double>[] childBiases = new Vector[biases.Length];

        (Matrix<double>, Vector<double>)[] wbList = new (Matrix<double>, Vector<double>)[weights.Length];

        Random rng = new Random();

        // Mutate weights
        for (int i = 0; i < wbList.Length; i++)
        {
            //First do the MATRIX
            // Create a mutation matrix and a magnitude matrix
            Matrix<double> mutationMatrix = Matrix<double>.Build.Random(weights[i].RowCount, weights[i].ColumnCount);
            Matrix<double> magnitudeMatrix = Matrix<double>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount, magnitude);

            // Make sure only some are mutated
            Func<double, double> randomMutation = x=> rng.NextDouble() < probability ? x : 0;
            magnitudeMatrix.Map(randomMutation);

            // Pointwise multiply the matrices to get the real mutation matrix
            mutationMatrix =  mutationMatrix.PointwiseMultiply(magnitudeMatrix);

            // Add it to the existing matrix
            Matrix<double> mutatedMatrix = weights[i].Add(mutationMatrix);

            // Then do the VECTOR
            // Create a mutation vector and a magnitude vector
            Vector<double> mutationVector = Vector<double>.Build.Random(biases[i].Count);
            Vector<double> magnitudeVector = Vector<double>.Build.Dense(biases[i].Count, magnitude);

            // Make sure only some values are mutated
            // randomMutation already defined
            magnitudeVector.Map(randomMutation);

            // Pointwise multiply the matrices to get the real mutation matrix
            mutationVector =  mutationVector.PointwiseMultiply(magnitudeVector);

            // Add it to the existing matrix
            Vector<double> mutatedVector = biases[i].Add(mutationVector);

            // Add the weights and biases to the tuple list    
            wbList[i] = (mutatedMatrix, mutatedVector);
        }

        return wbList;

    }

    public Matrix<double>[] getWeights() {
        return weights;
    }

    public Vector<double>[] getBiases() {
        return biases;
    }

    public (Matrix<double>, Vector<double>)[] getwbList()
    {
        (Matrix<double>, Vector<double>)[] wbList = new (Matrix<double>, Vector<double>)[weights.Length];

        for (int i = 0; i < weights.Length; i++)
        {
            wbList[i] = (weights[i], biases[i]);
        }

        return wbList;
    }
}