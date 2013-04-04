using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorsAndSuch.Mobs.AI
{
    internal class BrainRow
    {
        protected BrainRow previousRow;
        protected float[] Charges;
        protected float[,] Connections;
        protected Func<float, float> sigmoid = (input => (float) ( 1 / (1 + Math.Pow(Math.E, (double) (-1 * input)))));
        float learningRate = .01f;

        internal BrainRow(int NumNodes, int NumInputs)
        {
            Charges = new float[NumNodes];
            Connections = new float[NumInputs, NumNodes];
            SetRandConnections();
            Flush();
        }

        internal BrainRow(int NumNodes, BrainRow previousRow)
        {
            this.previousRow = previousRow;
            Charges = new float[NumNodes];
            Connections = new float[ previousRow.GetChargeLength(), NumNodes];
            SetRandConnections();
            Flush();
        }

        private void SetRandConnections()
        {
            for (int i = 0; i < Connections.GetLongLength(0); i++)
            {
                for (int j = 0; j < Connections.GetLongLength(1); j++)
                {
                    Connections[i, j] = (float)(Globals.rand.NextDouble() * 2 - 1) * 2f;
                }
            }
        }

        internal void Calculate()
        {
            CalculateStart(previousRow.GetCharges());
        }

        #region BackProps
        internal void BackProOldV1(float[] desiredOut, float[] inputs, bool jgfdjhk)
        {
            float[] Ek = new float[Charges.Length];
            //float[,] Wjk = new float[,]; 
            for (int i = 0; i < Charges.Length; i++)
            {
                Ek[i] = (desiredOut[i] - Charges[i]) * Charges[i] * (1 - Charges[i]);
                if (float.IsPositiveInfinity(Ek[i]))
                {
                    Ek[i] = 100000000000;
                }
                else if (float.IsNegativeInfinity(Ek[i]))
                {
                    Ek[i] = -100000000000;
                } 
                //for (int j = 0; j < previousRow.Charges.Length && Math.Abs(Ek) > 0.0001f; j++)

                for (int j = 0; j < inputs.Length && Math.Abs(Ek[i]) > 0.0000000001f; j++)
                {
                    Connections[j, i] += learningRate * Ek[i] * inputs[j];

                    if (float.IsInfinity(Connections[j, i]))
                        return;
                }
            }
            float[] Ej = new float[previousRow.Charges.Length];
            if (previousRow == null) return;
            for (int j = 0; j < previousRow.Charges.Length; j++)
            {
                for (int k = 0; k < Charges.Length; k++)
                {
                    Ej[j] = (Charges[k]) * (1 - Charges[k]);
                    float other = 0;
                    for (int k2 = 0; k2 < Charges.Length; k2++)
                    {
                        other += Ek[k2] * learningRate * Ek[k2] * inputs[j];
                    }
                    Ej[j] += other;
                    if (float.IsPositiveInfinity(Ej[j]))
                    {
                        Ej[j] = 100000000000;
                    }
                    else if (float.IsNegativeInfinity(Ej[j]))
                    {
                        Ej[j] = -100000000000;
                    }
                    //for (int j = 0; j < previousRow.Charges.Length && Math.Abs(Ek) > 0.0001f; j++)
                    /*
                    for (int j = 0; j < inputs.Length && Math.Abs(Ej[j]) > 0.0000000001f; j++)
                    {
                        previousRow.Connections[GetConnectionIndex(inputID: j, outputID: i)] += learningRate * Ej[j] * inputs[j];

                        if (float.IsInfinity(Connections[GetConnectionIndex(inputID: j, outputID: i)]))
                            return;
                    }*/
                }
            }
            //float Wij = learningRate * 
        }
                
        internal void BackPropOldV2(float[] inputs, float[] t)
        {
            float[] Ek = new float[Charges.Length];
            float[,] Wjk = new float[previousRow.Charges.Length, Charges.Length];
            Charges = GetCharges();
            previousRow.Charges = previousRow.GetCharges();
            for (int k = 0; k < Charges.Length; k++)
            {
                Ek[k] = (t[k] - Charges[k]) * Charges[k] * (1 - Charges[k]);
                Ek[k] = Math.Abs(Ek[k]) * Math.Abs((t[k] - Charges[k])) / (t[k] - Charges[k]);
                Ek[k] = Math.Min(1, Math.Max(-1, Ek[k]));
                if (float.IsPositiveInfinity(Ek[k]) || Ek[k] > 100)
                {
                    Ek[k] = 100000000000;
                }
                else if (float.IsNegativeInfinity(Ek[k]) || Ek[k] < -100)
                {
                    Ek[k] = -100000000000;
                } 
                //for (int j = 0; j < previousRow.Charges.Length && Math.Abs(Ek) > 0.0001f; j++)

                for (int j = 0; j < previousRow.GetChargeLength() && Math.Abs(Ek[k]) > 0.0000000001f; j++)
                {
                    Wjk[j, k] = learningRate * Ek[k] * previousRow.Charges[j] + Connections[j, k];
                    Connections[j, k] = Wjk[j, k];

                    if (float.IsInfinity(Connections[j, k]))
                        return;
                }
            }
            float[] Ej = new float[previousRow.Charges.Length];
            float[,] Wij = new float[inputs.Length, previousRow.Charges.Length]; 
            if (previousRow == null) return;
            for (int j = 0; j < previousRow.Charges.Length; j++)
            {
                for (int k = 0; k < Charges.Length; k++)
                {
                    Ej[j] = (Charges[k]) * (1 - Charges[k]);
                    Ej[j] = Math.Abs(Ej[j]) * Math.Abs((t[k] - Charges[k])) / (t[k] - Charges[k]);
                    Ej[j] = Math.Min(1, Math.Max(-1, Ej[j]));
                    float other = 0;
                    for (int k2 = 0; k2 < Charges.Length; k2++)
                    {
                        other += Ek[k2] * Wjk[j, k2];
                    }
                    Ej[j] *= other;
                    if (float.IsPositiveInfinity(Ej[j]))
                    {
                        Ej[j] = 100000000000;
                    }
                    else if (float.IsNegativeInfinity(Ej[j]))
                    {
                        Ej[j] = -100000000000;
                    }
                    //for (int j = 0; j < previousRow.Charges.Length && Math.Abs(Ek) > 0.0001f; j++)

                    for (int i = 0; i < inputs.Length && Math.Abs(Ej[j]) > 0.0000000001f; i++)
                    {
                        Wij[i, j] = learningRate * Ej[j] * inputs[i];
                        previousRow.Connections[i, j] += Wij[i, j];

                        if (float.IsInfinity(previousRow.Connections[i, j]))
                            return;
                    }
                }
            }
            //float Wij = learningRate * 
        }

        internal void BackProp(float[] inputs, float[] t)
        {
            float[] Ek = new float[Charges.Length];
            float[,] Wjk = new float[previousRow.Charges.Length, Charges.Length];
            Charges = GetCharges();
            previousRow.Charges = previousRow.GetCharges();
            for (int k = 0; k < Charges.Length; k++)
            {
                Ek[k] = (t[k] - Charges[k]) * Charges[k] * (1 - Charges[k]);
                if (float.IsPositiveInfinity(Ek[k]))
                {
                    Ek[k] = 100000000000;
                }
                else if (float.IsNegativeInfinity(Ek[k]))
                {
                    Ek[k] = -100000000000;
                }
                //&& Math.Abs(Ek[k]) > 0.0000000001f
                for (int j = 0; j < previousRow.GetChargeLength() ; j++)
                {
                    Wjk[j, k] = learningRate * Ek[k] * previousRow.Charges[j];
                    Wjk[j, k] += Connections[j, k];
                    if (float.IsInfinity(Wjk[j, k]) || Wjk[j, k] > 100)
                        return;
                    Connections[j, k] = Wjk[j, k];
                }
            }
            float[] Ej = new float[previousRow.Charges.Length];
            float[,] Wij = new float[inputs.Length, previousRow.Charges.Length];
            if (previousRow == null) return;
            for (int j = 0; j < previousRow.Charges.Length&& Math.Abs(Ej[j]) > 0.0000000001f; j++)
            {
                    Ej[j] = (previousRow.Charges[j]) * (1 - previousRow.Charges[j]);
                    float other = 0;
                    for (int k2 = 0; k2 < Charges.Length; k2++)
                    {
                        other += Ek[k2] * Wjk[j, k2];
                    }
                    Ej[j] *= other;
                    if (float.IsPositiveInfinity(Ej[j]))
                    {
                        Ej[j] = 100000000000;
                    }
                    else if (float.IsNegativeInfinity(Ej[j]))
                    {
                        Ej[j] = -100000000000;
                    }

                    for (int i = 0; i < inputs.Length ; i++)
                    {
                        Wij[i, j] = learningRate * Ej[j] * inputs[i];
                        Wij[i, j] += previousRow.Connections[i, j];
                        if (float.IsInfinity(Wij[i, j]) || Wij[i, j] > 100)
                            return;
                        previousRow.Connections[i, j] = Wij[i, j];
                    }
                }
            //float Wij = learningRate * && Math.Abs(Ej[j]) > 0.0000000001f
        }

        #endregion

        internal void Flush() 

        {
            for (int i = 0; i < Charges.Length; i++)
            {
                Charges[i] = 0;
            }
        }

        internal void CalculateStart(float[] input)
        {                
            for (int j = 0; j < Charges.Length; j++)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    Charges[j] += input[i] * Connections[i, j];
                }
                Charges[j] = sigmoid(Charges[j]);
            }
        }

        #region Getters And Setters

        protected int GetConnLength()
        {
            return Connections.Length;
        }

        internal float[,] GetConn()
        {
            return Connections;
        }

        internal float[] GetCharges()
        {
            float[] ret = new float[Charges.Length];
            for (int i = 0; i < Charges.Length; i++)
            {
                ret[i] = Charges[i];
            }
            return ret;
        }

        private int GetChargeLength()
        {
            return Charges.Length;
        }

        internal void SetConn(float[,] setValues)
        {
            for (int i = 0; i < Connections.GetLongLength(0); i++)
            {
                for (int j = 0; j < Connections.GetLongLength(1); j++)
                {
                    Connections[i, j] = setValues[i, j];
                }
            }
        }

        #endregion

        internal void Modify(float ModAmount)
        {
            int rand1 = Globals.rand.Next((int)Connections.GetLongLength(0));
            int rand2 = Globals.rand.Next((int)Connections.GetLongLength(1));
            Connections[rand1, rand2] += ((float)Globals.rand.NextDouble() * 2f - 1f) * ModAmount;
        }
    }
}
