using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffectiveGame.Comparison
{
    class Correlation
    {
        public Correlation(double[] result, IEnumerable<double> data1, IEnumerable<double> data2, out int offset, out double maximumNormalizedCrossCorrelation)
        {
            var data1Array = data1.ToArray();
            var data2Array = data2.ToArray();

            var max = double.MinValue;
            var index = 0;
            var i = 0;          

            // Find the maximum cross correlation value and its index
            foreach (var d in result)
            {
                if (d > max)
                {
                    index = i;
                    max = d;
                }
                ++i;
            }
            // if the index is bigger than the length of the first array, it has to be
            // interpreted as a negative index
            if (index >= data1Array.Length)
            {
                index *= -1;
            }

            var matchingData1 = data1;
            var matchingData2 = data2;
            var biggerSequenceCount = Math.Max(data1Array.Length, data2Array.Length);
            var smallerSequenceCount = Math.Min(data1Array.Length, data2Array.Length);

            offset = index;

            if (index > 0)
                matchingData1 = data1.Skip(offset).Take(smallerSequenceCount).ToList();
            else if (index < 0)
            {
                offset = biggerSequenceCount + smallerSequenceCount + index;
                matchingData2 = data2.Skip(offset).Take(smallerSequenceCount).ToList();
                matchingData1 = data1.Take(smallerSequenceCount).ToList();
            }

            var mx = matchingData1.Average();
            var my = matchingData2.Average();
            var denom1 = Math.Sqrt(matchingData1.Sum(x => (x - mx) * (x - mx)));
            var denom2 = Math.Sqrt(matchingData2.Sum(y => (y - my) * (y - my)));
            maximumNormalizedCrossCorrelation = max / (denom1 * denom2);
        }
    }
}
