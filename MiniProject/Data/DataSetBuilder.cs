using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Assignment1.Data
{
    class DataSetBuilder
    {
        public static DataSet<T, K, M, I>[] buildDataSets<T, K, M, I>(string sFileName, double dTrainSetSize) where I : IItem<T, K, M>
        {
            //generate dataset with all the reviews.
            DataSet<T, K, M, I> reviews = getDataFromFile<T, K, M, I>(sFileName);

            //split the dataset to test dataset.
            //the remainig dataset will be the train dataset.
            DataSet<T, K, M, I> test = reviews.splitDataSet(1 - dTrainSetSize);
            return new DataSet<T, K, M, I>[2] { reviews, test };
        }

        private static DataSet<T, K, M, I> getDataFromFile<T, K, M, I>(string sFileName) where I : IItem<T, K, M>
        {
            DataSet<T, K, M, I> newDataSet = new DataSet<T, K, M, I>();
            StreamReader file = new StreamReader(sFileName);
            string json;

            while ((json = file.ReadLine()) != null)
            {
                I item = JsonConvert.DeserializeObject<I>(json);
                if (item.GetRating() < 3.0)
                    continue;
                newDataSet.AddItem(item);
            }
            return newDataSet;
        }
    }
}
