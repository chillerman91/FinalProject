using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorsAndSuch.Mobs.AI
{
    internal class Brain
    {
        internal int inputs;
        private int outputs;
        private int HiddenRows;
        private int NodesPerRow;
        private BrainRow[] Rows;
        protected float[] LastOutput;
        float learningRate = 1f;

        public static Brain Clone(Brain parent) 
        {
            Brain ret = new Brain(parent.inputs, parent.outputs, parent.HiddenRows, parent.NodesPerRow);
            for (int i = 0; i < parent.Rows.Length; i++)
            {
                ret.Rows[i].SetConn(parent.Rows[i].GetConn());
            }
            return ret;
        }

        internal Brain(int inputs, int outputs, int HiddenRows, int NodesPerRow)
        {
            this.inputs = inputs;
            this.outputs = outputs;
            this.HiddenRows = HiddenRows;
            this.NodesPerRow = NodesPerRow;
            Rows = new BrainRow[1 + HiddenRows];
            if (HiddenRows == 0)
            {
                Rows[0] = new BrainRow(outputs, inputs);
            }
            else
            {
                Rows[0] = new BrainRow(inputs, inputs);

                for (int i = 1; i <= Rows.Length - 2; i++)
                {
                    Rows[i] = new BrainRow(NodesPerRow, Rows[i - 1]);
                }
                Rows[Rows.Length - 1] = new BrainRow(outputs, Rows[Rows.Length - 2]);
            }
        }

        internal void Flush()
        {
            for (int i = 0; i < Rows.Length; i++)
                Rows[i].Flush();
        }

        internal float[] Calculate(float[] input)
        {
            Rows[0].CalculateStart(input);
            for (int i = 1; i < Rows.Length; i++)
            {
                Rows[i].Calculate();
            }
            LastOutput = Rows[Rows.Length - 1].GetCharges();
            return LastOutput;
        }

        internal float[] BackProp(float[] input, float[] desiredOut)
        {
            for (int i = 0; i < 4; i++)
            {
                Flush();
                Calculate(input);
                Rows[Rows.Length - 1].BackProp(input, desiredOut);
            }
            Flush();
            Calculate(input);
            return Rows[Rows.Length - 1].GetCharges();
            //float Wij = learningRate * 
        }

        internal void Modify(int RowNumber = 4, int PerRow = 2, float ModAmount = 1f)
        {
            for (int i = 0; i < RowNumber; i++)
            {
                int rand = Globals.rand.Next(Rows.Length);
                for (int j = 0; j < PerRow; j++)
                {
                    Rows[rand].Modify(ModAmount: ModAmount);
                }
            }
        }
    }
}
