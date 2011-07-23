using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Language;

namespace Data
{
    public class PartitionVariableOperation : Operation
    {
        public PartitionVariableOperation(int partitions)
        {
            this.Name = "Partition variable";

            this.Add(
                new FuncOperator(
                    this,
                    "Original variable",
                    coplot =>
                    {
                        Language.Environment c = coplot.Clone();
                        return c;
                    }), true);

            this.Add(
                new FuncOperator(
                    this,
                    "Partitioned variable",
                    coplot =>
                    {
                        Language.Environment c = coplot.Clone();
                        if (coplot.OnePredictor() is Variable)
                        {
                            c.modelPredictors.Clear();
                            c.modelPredictors.Add((coplot.OnePredictor() as Variable).partition(partitions));
                        }
                        return c;
                    }), false);            
        }
    }
}
