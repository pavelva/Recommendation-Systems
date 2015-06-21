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
            rs.Load("../../reviews-small.json", 0.9);
            Console.WriteLine("Loding time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            Console.WriteLine("\nTrain model...");
            ds = DateTime.Now;
            //rs.TrainBaseModel(10);
            Console.WriteLine("Training model was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            List<string> lMethods = new List<string>();
            lMethods.Add("Pearson");
            lMethods.Add("PearsonDFeatureVectorMultiply");
            lMethods.Add("PearsonDFeatureVectorSum");
            lMethods.Add("PearsonDOFeatureVector");
            lMethods.Add("PearsonDNaive");
            lMethods.Add("PearsonDONaive");
            //lMethods.Add("SVD");
            //lMethods.Add("SVDD");
            //lMethods.Add("SVDDO");
            List<int> lLengths = new List<int>();
            lLengths.Add(10);

            ds = DateTime.Now;
            Console.WriteLine("Compute Percision & Recall");
            Dictionary<int, Dictionary<string, Dictionary<string, double>>> PerRecallResults = rs.ComputePrecisionRecall(lMethods, lLengths);
            Console.WriteLine("\nPrecision-recall scores for all methods are:");
            //writer.WriteLine("\nPrecision-recall scores for all methods are:");
            foreach (int iLength in PerRecallResults.Keys)
            {
                foreach (string sMethod in PerRecallResults[iLength].Keys)
                {
                    foreach (string sMetric in PerRecallResults[iLength][sMethod].Keys)
                    {
                        //writer.WriteLine(iLength + "," + sMethod + "," + sMetric + " = " + PerRecallResults[iLength][sMethod][sMetric]);
                        Console.WriteLine(iLength + "," + sMethod + "," + sMetric + " = " + PerRecallResults[iLength][sMethod][sMetric]);
                    }
                }
            }
            Console.WriteLine("Execution time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));

            ds = DateTime.Now;
            Console.WriteLine("Compute RMSE & DConfidence");
            //writer.WriteLine("\nCompute RMSE & DConfidence");
            Dictionary<string, Dictionary<string, double>> dConfidence = null;
            Dictionary<string, double> dResults = rs.ComputeRMSE(lMethods, out dConfidence);
            Console.WriteLine("RMSE scores are:");
            foreach (KeyValuePair<string, double> p in dResults)
            {
                //writer.WriteLine(p.Key + "=" + Math.Round(p.Value, 4) + ", ");
                Console.Write(p.Key + "=" + Math.Round(p.Value, 4) + ", ");
            }
            Console.WriteLine("Confidence P-values are:");
            //writer.WriteLine("\nConfidence P-values are:");
            foreach (string sFirst in dConfidence.Keys)
            {
                foreach (string sSecond in dConfidence[sFirst].Keys)
                {
                    //writer.WriteLine("p(" + sFirst + "=" + sSecond + ")=" + dConfidence[sFirst][sSecond].ToString("F10"));
                    Console.WriteLine("p(" + sFirst + "=" + sSecond + ")=" + dConfidence[sFirst][sSecond].ToString("F10"));
                }
            }

            Console.WriteLine("Execution time was " + Math.Round((DateTime.Now - ds).TotalSeconds, 0));
            

            //writer.Close();
            Console.ReadLine();
        }
    }
}
