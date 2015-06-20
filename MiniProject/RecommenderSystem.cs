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

        Predictor<string, string, string, Review> BaseModel;
        Predictor<string, string, string, Review> Person;
        Predictor<string, string, string, Review> Cosine;
        Dictionary<String, RecommendationBase<string, string, string, Review>> RecommendationAlgo;

        public RecommenderSystem()
        {
            RecommendationAlgo = new Dictionary<string, RecommendationBase<string, string, string, Review>>();
        }

        public void Load(string sFileName)
        {
            Load(sFileName, 1.0);
        }
        public void Load(string sFileName, double dTrainSetSize)
        {
            DataSet<string, string, string, Review>[] dataSets = DataSetBuilder.buildDataSets<string, string, string, Review>(sFileName, dTrainSetSize);
            this.Train = dataSets[0];
            this.Test = dataSets[1];
            this.Validation = Train.splitDataSet(0.05);

            this.Person = new Pearson<string, string, string, Review>(Train);
            this.Cosine = new Cosine<string, string, string, Review>(Train);

            RecommendationAlgo.Add("Popularity", new RecommendationPopularity<string, string, string, Review>(Train));
            RecommendationAlgo.Add("CP", new RecommendationCP<string, string, string, Review>(Train));
            RecommendationAlgo.Add("Cosine", new RecommendationPredictor<string, string, string, Review>(Train, Cosine));
            RecommendationAlgo.Add("Pearson", new RecommendationPredictor<string, string, string, Review>(Train, Person));
            RecommendationAlgo.Add("NNCosine", new RecommendationNNPredictor<string, string, string, Review>(Train, Cosine));
            RecommendationAlgo.Add("NNPearson", new RecommendationNNPredictor<string, string, string, Review>(Train, Person));
        }

        public void TrainBaseModel(int cFeatures)
        {
            this.BaseModel = new BaseModel<string, string, string, Review>(Train, Validation, cFeatures);
            this.BaseModel.TrainModel();
            RecommendationAlgo.Add("SVD", new RecommendationPredictor<string, string, string, Review>(Train, BaseModel));
            RecommendationAlgo.Add("NNSVD", new RecommendationNNPredictor<string, string, string, Review>(Train, BaseModel));
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
    }
}
