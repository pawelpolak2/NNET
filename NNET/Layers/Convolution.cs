﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NNET
{
    /// <summary>
    /// A convolution layer. Used for spacially relative data.
    /// </summary>
    [Serializable]
    public class Convolution : Layer
    {
        public InitializationMethod weightInit = new RandomInitialization();

        Matrix[] input;
        Matrix[] output;
        Matrix[][] weights;
        Matrix[] biases;

        Vector2Int kernelSize;
        Vector3Int inputSize;
        Vector3Int outputSize;
        public Vector2Int stride = new Vector2Int(1,1);
        public Vector2Int padding = new Vector2Int(0,0);

        int kernelNumber;
        public Convolution(Vector2Int _kernelSize, int _kernelNumber)
        {
            kernelSize = _kernelSize;
            kernelNumber = _kernelNumber;
        }
        public override object Init(object _inputSize, Random rand)
        {
            inputSize = _inputSize as Vector3Int;
            outputSize = new Vector3Int((int)((inputSize.x + (2 * padding.x) - kernelSize.x) / stride.x) + 1, (int)((inputSize.y + (2 * padding.y) - kernelSize.y) / stride.y) + 1,kernelNumber);
            weights = new Matrix[kernelNumber][];
            biases = new Matrix[kernelNumber];
            for(int k = 0; k < kernelNumber; k++)
            {
                weights[k] = new Matrix[inputSize.z];
                biases[k] = new Matrix(outputSize.y, outputSize.x);
                for(int n = 0; n < inputSize.z; n++)
                {
                    weights[k][n] = weightInit.Initialize(kernelSize.y, kernelSize.x);
                }
            }
            return outputSize;
        }
        public override object FeedForward(object _input)
        {
            input = _input as Matrix[];
            output = new Matrix[kernelNumber];
            for(int k = 0; k < kernelNumber; k++)
            {
                output[k] = new Matrix(outputSize.y, outputSize.x);
                for(int x = 0; x < outputSize.x; x++)
                    for(int y = 0; y < outputSize.y; y++)
                    {
                        Vector2Int pos = new Vector2Int((x * stride.x) - padding.x, (y * stride.y) - padding.y);
                        for(int n = 0; n < inputSize.z; n++)
                        {
                            for (int xK = 0; xK < kernelSize.x; xK++)
                                for(int yK = 0; yK < kernelSize.y; yK++)
                                {
                                    if (pos.x + xK >= 0 && pos.x + xK < inputSize.x && pos.y + yK >= 0 && pos.y + yK < inputSize.y)
                                        output[k][y, x] += weights[k][n][yK, xK] * input[n][pos.y + yK, pos.x + xK];
                                }
                        }
                    }
            }
            //for (int i = 0; i < kernelNumber; i++) output[i] += biases[i];
            return activationFunc.Apply(output);
        }
        public override object Backpropagate(object _error, float LR)
        {
            Matrix[] error = _error as Matrix[];
            Matrix[] newError = new Matrix[input.Length];
            output = activationFunc.Derivative(output);
            for (int i = 0; i < error.Length; i++) error[i] = Matrix.Dot(error[i], output[i]);

            for(int k = 0; k < kernelNumber; k++)
            {
                for(int n = 0; n < inputSize.z; n++)
                {
                    newError[n] = new Matrix(inputSize.y, inputSize.x);
                    for(int x = 0; x < outputSize.x; x++)
                    {
                        for(int y = 0; y < outputSize.y; y++)
                        {
                            Vector2Int pos = new Vector2Int((x * stride.x) - padding.x, (y * stride.y) - padding.y);
                            float con = error[k][y, x];
                            biases[k][y, x] -= con * LR * baseLR;
                            for(int xK = 0; xK < kernelSize.x; xK++)
                                for(int yK = 0; yK < kernelSize.y; yK++)
                                {
                                    if (pos.x + xK >= 0 && pos.x + xK < inputSize.x && pos.y + yK >= 0 && pos.y + yK < inputSize.y)
                                    {
                                        weights[k][n][yK, xK] -= input[n][pos.y + yK, pos.x + xK] * con * LR * baseLR;
                                        newError[n][pos.y + yK, pos.x + xK] += con * weights[k][n][yK, xK];
                                    }
                                }
                        }
                    }
                }
            }
            return newError;
        }
    }
}
