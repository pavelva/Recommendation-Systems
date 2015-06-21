using MiniProject.Data;
using MiniProject.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hw2.Recommendation;

namespace MiniProject
{
    class RecommenderSystem
    {
        public DataSet<string, string, string, Review> Train {get; private set;}
        DataSet<string, string, string, Review> Test;
        DataSet<string, string, string, Review> Validation;

        Predictor<string, string, string, Review> Person;
        Predictor<string, string, string, Review> PersonDNaive;
        Predictor<string, string, string, Review> PersonDONaive;
        Predictor<string, string, string, Review> PearsonDFeatureVectorMultiply;
        Predictor<string, string, string, Review> PearsonDFeatureVectorSum;
        Predictor<string, string, string, Review> PearsonDOFeatureVector;
        Predictor<String, String, String, Review> BaseModel;
        Predictor<String, String, String, Review> BaseModelD;
        Predictor<String, String, String, Review> BaseModelDO;
        Predictor<string, string, string, Review> Cosine;
        Dictionary<String, RecommendationBase<string, string, string, Review>> RecommendationAlgo;
        

        Dictionary<string, Predictor<string, string, string, Review>> Methods;

        public RecommenderSystem()
        {
            RecommendationAlgo = new Dictionary<string, RecommendationBase<string, string, string, Review>>();
            Methods = new Dictionary<string, Predictor<string,string,string,Review>>();
        }

        public void Load(string sFileName)
        {
            Load(sFileName, 1.0);
        }
        public void Load(string sFileName, double dTrainSetSize)
        {
            UserDataSet userDataSet = DataSetBuilder.buildUserDataSet();
            DataSet<string, string, string, Review>[] dataSets = DataSetBuilder.buildDataSets<string, string, string, Review>(sFileName, dTrainSetSize);
            this.Train = dataSets[0];
            this.Test = dataSets[1];
            this.Validation = Train.splitDataSet(0.05);

            this.Train.setUserDataSet(userDataSet);
            this.Test.setUserDataSet(userDataSet);
            this.Validation.setUserDataSet(userDataSet);

            this.Person = new Pearson<string, string, string, Review>(Train);
            this.PersonDNaive = new PearsonDNaive<string, string, string, Review>(Train);
            this.PersonDONaive = new PearsonDONaive<string, string, string, Review>(Train);
            this.PearsonDFeatureVectorMultiply = new PearsonDFeatureVectorMultiply<string, string, string, Review>(Train);
            this.PearsonDFeatureVectorSum = new PearsonDFeatureVectorSum<string, string, string, Review>(Train);
            this.PearsonDOFeatureVector = new PearsonDOFeatureVector<string, string, string, Review>(Train);
            this.Cosine = new Cosine<string, string, string, Review>(Train);

            

            //RecommendationAlgo.Add("Popularity", new RecommendationPopularity<string, string, string, Review>(Train));
            //RecommendationAlgo.Add("CP", new RecommendationCP<string, string, string, Review>(Train));
            //RecommendationAlgo.Add("Cosine", new RecommendationPredictor<string, string, string, Review>(Train, Cosine));
            RecommendationAlgo.Add("Pearson", new RecommendationPredictor<string, string, string, Review>(Train, Person));
            Methods.Add("Pearson", new Pearson<string, string, string, Review>(Train));
            RecommendationAlgo.Add("PearsonDNaive", new RecommendationPredictor<string, string, string, Review>(Train, PersonDNaive));
            Methods.Add("PearsonDNaive", new PearsonDNaive<string, string, string, Review>(Train));
            RecommendationAlgo.Add("PearsonDONaive", new RecommendationPredictor<string, string, string, Review>(Train, PersonDONaive));
            Methods.Add("PearsonDONaive", new PearsonDONaive<string, string, string, Review>(Train));
            RecommendationAlgo.Add("PearsonDFeatureVectorMultiply", new RecommendationPredictor<string, string, string, Review>(Train, PearsonDFeatureVectorMultiply));
            Methods.Add("PearsonDFeatureVectorMultiply", new PearsonDFeatureVectorMultiply<string, string, string, Review>(Train));
            RecommendationAlgo.Add("PearsonDFeatureVectorSum", new RecommendationPredictor<string, string, string, Review>(Train, PearsonDFeatureVectorSum));
            Methods.Add("PearsonDFeatureVectorSum", new PearsonDFeatureVectorSum<string, string, string, Review>(Train));
            RecommendationAlgo.Add("PearsonDOFeatureVector", new RecommendationPredictor<string, string, string, Review>(Train, PearsonDOFeatureVector));
            Methods.Add("PearsonDOFeatureVector", new PearsonDOFeatureVector<string, string, string, Review>(Train));
            //RecommendationAlgo.Add("NNCosine", new RecommendationNNPredictor<string, string, string, Review>(Train, Cosine));
            //RecommendationAlgo.Add("NNPearson", new RecommendationNNPredictor<string, string, string, Review>(Train, Person));
        }

        public void TrainBaseModel(int cFeatures)
        {
            this.BaseModel = new BaseModel<String, String, String, Review>(Train, Validation, cFeatures);
            this.BaseModelD = new BaseModelD<String, String, String, Review>(Train, Validation, cFeatures);
            this.BaseModelDO = new BaseModelDO<String, String, String, Review>(Train, Validation);
            this.BaseModel.TrainModel();
            this.BaseModelD.TrainModel();
            this.BaseModelDO.TrainModel();
            RecommendationAlgo.Add("SVD", new RecommendationPredictor<String, String, String, Review>(Train, BaseModel));
            RecommendationAlgo.Add("SVDD", new RecommendationPredictor<String, String, String, Review>(Train, BaseModelD));
            RecommendationAlgo.Add("SVDDO", new RecommendationPredictor<String, String, String, Review>(Train, BaseModelDO));
        }

        public double GetRating(string sUID, string sIID)
        {
            return Train.GetRating(sUID, sIID);
        }
        public Dictionary<double, int> GetRatingsHistogram(string sUID)
        {
            throw new NotImplementedException();
        }

        public List<string> Recommend(string sAlgorithm, string sUserId, int cRecommendations)
        {
            return RecommendationAlgo[sAlgorithm].Recommendation(sUserId, cRecommendations);
        }

        public Dictionary<int, Dictionary<string, Dictionary<string, double>>> ComputePrecisionRecall(List<string> lMethods, List<int> lLengths)
        {
            Dictionary<int, Dictionary<string, Dictionary<string, double>>> mapping = new Dictionary<int, Dictionary<string, Dictionary<string, double>>>();
            int maxN = lLengths.Max();
            double totalTestSize = (double)Test.GetUserIDs().Count;

            foreach (int n in lLengths)
            {
                mapping.Add(n, new Dictionary<string, Dictionary<string, double>>());
                foreach (string method in lMethods)
                {
                    mapping[n].Add(method, new Dictionary<string, double>());
                    mapping[n][method].Add("precision", 0);
                    mapping[n][method].Add("recall", 0);
                }
                
            }
            int c = Test.GetUserIDs().Count;
            int c1 = 1;
            foreach (string method in lMethods)
            {
                foreach (string userID in Test.GetUserIDs())
                {
                    //Console.WriteLine("{0}/{1}", c1, c); c1++;
                    List<string> maxNRec = Recommend(method, userID, maxN);
                    List<string> real = Test.GetItems(userID).Select(rev => rev.GetShearedItemID()).ToList();

                    foreach (int n in lLengths)
                    {
                        List<string> rec = maxNRec.Take(n).ToList();
                        double  tp = rec.Where(r => real.Contains(r)).Count(),
                                fp = rec.Where(r => !real.Contains(r)).Count(),
                                fn = real.Where(r => !rec.Contains(r)).Count();
                        double uPrecision = (tp == 0 && fp == 0) ? 1 : tp / (tp + fp);
                        double uRecall = (tp == 0 && fn == 0) ? 1 : tp / (tp + fn);
                        mapping[n][method]["precision"] += uPrecision;
                        mapping[n][method]["recall"] += uRecall;
                    }
                }
            }
            
            foreach(int n in mapping.Keys)
                foreach(string method in mapping[n].Keys)
                {
                    mapping[n][method]["precision"] /= totalTestSize;
                    mapping[n][method]["recall"] /= totalTestSize;
                }
            return mapping;
        }

        /// <summary>
        /// Compute the RMSE of the methods and will return thier dConfidence:
        /// Let P1, P2 be prediction methods where RMSE(P1)>RMSE(P2) => the function will only calculate P(P1 better then P2)
        /// </summary>
        /// <param name="lMethods"></param>
        /// <param name="dConfidence"></param>
        /// <returns></returns>
        public Dictionary<string, double> ComputeRMSE(List<string> lMethods, out Dictionary<string, Dictionary<string, double>> dConfidence)
        {
            Dictionary<string, double> RMSESummary = new Dictionary<string, double>();

            foreach (string method in lMethods)
            {
                RMSESummary.Add(method, Methods[method].RMSE(this.Test));
            }

            List<string> orderedRMSE = RMSESummary.Keys.OrderBy(x => -RMSESummary[x]).ToList();

            dConfidence = DConfidence(orderedRMSE);
            return RMSESummary;
        }

        private Dictionary<string, Dictionary<string, double>> DConfidence(List<string> orderedMethods)
        {
            Dictionary<string, Dictionary<string, double>> winsSum = CalcWins(orderedMethods);
            Dictionary<string, Dictionary<string, double>> dConfidence = CalcDConfidence(orderedMethods, winsSum);

            return dConfidence;
        }

        /// <summary>
        /// Calculate the nethods dConfidence by the given oredered list meaning each method will be compared with its successors.
        /// </summary>
        /// <param name="orderedMethods">The order of the methods to make the calculations</param>
        /// <param name="winsSum">Count of each method's wins against all other methods</param>
        /// <returns>1 - 0.5^(An+Bn)*Sum((An+Bn)!/(i!*(An+Bn-i)!)</returns>
        private Dictionary<string, Dictionary<string, double>> CalcDConfidence(List<string> orderedMethods, Dictionary<string, Dictionary<string, double>> winsSum)
        {
            Dictionary<string, Dictionary<string, double>> dConfidence = new Dictionary<string, Dictionary<string, double>>();

            for (int i = 0; i < orderedMethods.Count - 1; i++)
            {
                string method1 = orderedMethods[i];
                dConfidence.Add(method1, new Dictionary<string, double>());

                for (int j = i + 1; j < orderedMethods.Count; j++)
                {
                    string method2 = orderedMethods[j];

                    double total = winsSum[method1][method2] + winsSum[method2][method1]; // An+Bn
                    double totalLogFactorial = LogSum(total); // log((An + Bn)!)

                    double sumFactorial = 0; // hold the sum_i*0.5^(An+Bn)

                    for (int k = (int)winsSum[method1][method2]; k <= total; k++)
                    {
                        double logFactorial = totalLogFactorial - LogSum(k) - LogSum(total - k);

                        //Convert from log base to regular and multiply by 0.5^(An+Bn) => 2^-(An+Bn)*2^log = 2^(log - (An + Bn))
                        sumFactorial += Math.Pow(2, logFactorial - total);
                    }

                    double conf = 1 - sumFactorial;
                    dConfidence[method1].Add(method2, conf);
                }
            }
            return dConfidence;
        }

        /// <summary>
        /// Calculate the wins of each method against all other methods
        /// </summary>
        /// <param name="orderedMethods"></param>
        /// <returns>for each method it will hold a dictionary of other methods and each entry will hold 
        /// the number of wins the method had against the other method:
        /// Dic[P1][P2] => the amount of wins P1 had over P2
        /// </returns>
        private Dictionary<string, Dictionary<string, double>> CalcWins(List<string> orderedMethods)
        {
            Dictionary<string, Dictionary<string, double>> winsSum = new Dictionary<string, Dictionary<string, double>>();

            foreach (string method1 in orderedMethods)
            {
                Dictionary<string, double> wins = new Dictionary<string, double>();
                winsSum.Add(method1, wins);
                foreach (string method2 in orderedMethods.Where(m => !m.Equals(method1) && !wins.ContainsKey(m)))
                {
                    wins.Add(method2, 0);
                }
            }

            foreach (Review item in this.Test.GetItems())
            {
                Dictionary<string, double> methodsError = CalcMethodsErrors(orderedMethods, item);

                foreach (string method1 in orderedMethods)
                {
                    foreach (string method2 in orderedMethods.Where(m => !m.Equals(method1)))
                    {
                        //count only the wins (doesnt count when equal as you recommended at the forum)
                        if (methodsError[method1] < methodsError[method2])
                            winsSum[method1][method2]++;
                    }
                }
            }
            return winsSum;
        }

        /// <summary>
        /// Calculate the error for each method for a given item
        /// </summary>
        /// <param name="orderedMethods"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private Dictionary<string, double> CalcMethodsErrors(List<string> orderedMethods, Review item)
        {
            double realRating = item.GetRating();
            Dictionary<string, double> methodsError = new Dictionary<string, double>();

            foreach (string method in orderedMethods)
            {
                double prediction = this.Methods[method].PredictRating(item.GetUserID(), item.GetShearedItemID());
                double error;
                if (double.IsNaN(prediction))
                {
                    error = 25;
                    Console.WriteLine("{0} Prediction For User {1} and Item {2} Is NAN", method, item.GetUserID(), item.GetShearedItemID());
                }
                else
                    error = Math.Pow(realRating - prediction, 2);
                methodsError.Add(method, error);
            }
            return methodsError;
        }

        /// <summary>
        /// Calculate the sum of logs from "startFrom" until n
        /// </summary>
        /// <param name="n"></param>
        /// <param name="startFrom"></param>
        /// <returns></returns>
        private double LogSum(double n, double startFrom = 1)
        {
            double ans = 0;

            for (double i = startFrom; i <= n; i++)
            {
                ans += Math.Log(i, 2);
            }

            return ans;
        }
    }
}

