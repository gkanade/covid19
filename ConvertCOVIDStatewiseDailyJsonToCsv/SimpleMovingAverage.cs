using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCOVIDStatewiseDailyJsonToCsv
{
    class SimpleMovingAverage
    {
        // queue used to store list so that we get the average 
        private Queue<Double> Dataset = new Queue<Double>();
        private int period;
        private double sum;

        // constructor to initialize period 
        public SimpleMovingAverage(int period)
        {
            this.period = period;
        }

        // function to add new data in the 
        // list and update the sum so that 
        // we get the new mean 
        public void addData(double num)
        {
            sum += num;
            Dataset.Enqueue(num);

            // Updating size so that length 
            // of data set should be equal 
            // to period as a normal mean has 
            if (Dataset.Count > period)
            {
                sum -= Dataset.Dequeue();
            }
        }

        // function to calculate mean 
        public double getMean()
        {
            return sum / period;
        }

        public int Count()
        {
            return Dataset.Count;
        }

        public int Last()
        {
            return (int)Dataset.Last();
        }
    }
}
