using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject
{
    class Program
    {
        static void Main(string[] args)
        {
            RecommenderSystem rs = new RecommenderSystem();
            DateTime ds;

            Console.WriteLine("Loding data files...");
            ds = DateTime.Now;
            rs.Load("../../reviews-small.json", 0.9);
            Console.WriteLine("Loding time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            Console.WriteLine("\nTrain model...");
            ds = DateTime.Now;
            rs.TrainBaseModel(10);
            Console.WriteLine("Training model was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            List<string> lMethods = new List<string>();
            lMethods.Add("NNPearson");
            lMethods.Add("SVD");
            List<int> lLengths = new List<int>();
            lLengths.Add(10);

            ds = DateTime.Now;
            Dictionary<int, Dictionary<string, Dictionary<string, double>>> dResults = rs.ComputePrecisionRecall(lMethods, lLengths);
            Console.WriteLine("\nPrecision-recall scores for all methods are:");
            foreach (int iLength in dResults.Keys)
            {
                foreach (string sMethod in dResults[iLength].Keys)
                {
                    foreach (string sMetric in dResults[iLength][sMethod].Keys)
                    {
                        Console.WriteLine(iLength + "," + sMethod + "," + sMetric + " = " + dResults[iLength][sMethod][sMetric]);
                    }
                }
            }
            Console.WriteLine("Execution time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));
            Console.ReadLine();
        }
    }
}
