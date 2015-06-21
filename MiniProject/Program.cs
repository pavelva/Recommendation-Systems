using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject
{
    class Program
    {
        static void Main(string[] args)
        {
            
            DateTime ds;
            //StreamWriter writer = new StreamWriter("../../results.txt");

            RecommenderSystem rs = new RecommenderSystem();
            Console.WriteLine("Loding data files...");
            ds = DateTime.Now;
            rs.Load("../../reviews.json", 0.9);
            Console.WriteLine("Loding time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            Console.WriteLine("\nTrain model...");
            ds = DateTime.Now;
            rs.TrainBaseModel(10);
            Console.WriteLine("Training model was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            List<string> lMethods = new List<string>();
            lMethods.Add("Pearson");
            lMethods.Add("PearsonDFeatureVectorMultiply");
            lMethods.Add("PearsonDFeatureVectorSum");
            lMethods.Add("PearsonDOFeatureVector");
            lMethods.Add("PearsonDNaive");
            lMethods.Add("PearsonDONaive");
            lMethods.Add("SVD");
            lMethods.Add("SVDD");
            lMethods.Add("SVDDO");
            List<int> lLengths = new List<int>();
            lLengths.Add(10);

            ds = DateTime.Now;
            Console.WriteLine("\nCompute Percision & Recall");
            Dictionary<int, Dictionary<string, Dictionary<string, double>>> PerRecallResults = rs.ComputePrecisionRecall(lMethods, lLengths);
            Console.WriteLine("Precision-recall scores for all methods are:");
            foreach (int iLength in PerRecallResults.Keys)
            {
                foreach (string sMethod in PerRecallResults[iLength].Keys)
                {
                    foreach (string sMetric in PerRecallResults[iLength][sMethod].Keys)
                    {
                        Console.WriteLine(iLength + "," + sMethod + "," + sMetric + " = " + PerRecallResults[iLength][sMethod][sMetric]);
                    }
                }
            }
            Console.WriteLine("Execution time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            ds = DateTime.Now;
            Console.WriteLine("\nCompute RMSE & DConfidence");
            Dictionary<string, Dictionary<string, double>> dConfidence = null;
            Dictionary<string, double> dResults = rs.ComputeRMSE(lMethods, out dConfidence);
            Console.WriteLine("RMSE scores are:");
            foreach (KeyValuePair<string, double> p in dResults)
            {
                Console.Write(p.Key + "=" + p.Value + ", ");
            }
            Console.WriteLine("Confidence P-values are:");
            foreach (string sFirst in dConfidence.Keys)
            {
                foreach (string sSecond in dConfidence[sFirst].Keys)
                {
                    Console.WriteLine("p(" + sFirst + "=" + sSecond + ")=" + dConfidence[sFirst][sSecond].ToString("F10"));
                }
            }

            Console.WriteLine("Execution time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));
            

            //writer.Close();
            Console.ReadLine();
        }
    }
}
