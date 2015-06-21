using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MiniProject.Data
{
    class UserData
    {
        [JsonProperty(PropertyName = "country")]
        private String Country;
        [JsonProperty(PropertyName = "gender")]
        private String Gender;
        [JsonProperty(PropertyName = "age")]
        private int Age;
        [JsonProperty(PropertyName = "user_id")]
        private String ID;

        private List<double> featureVector;
        private double vectorSize;

        public void SetAge(String age)
        {
            Age = Int32.Parse(age);
        }

        public void SetCountry(String country)
        {
            Country = country;
        }

        public void SetGender(String gender)
        {
            Gender = gender;
        }

        public int GetAge()
        {
            return Age;
        }

        public String GetGender()
        {
            return Gender;
        }

        public String GetCountry()
        {
            return Country;
        }

        public String getID()
        {
            return ID;
        }

        public List<double> getFeatureVector()
        {
            return featureVector;
        }

        public double getFeatureVectorSize()
        {
            return vectorSize;
        }

        internal void initFeatureVector(List<string> countrys)
        {
            this.featureVector = new List<double>();

            for (int i = 0; i < (24 + 2 + countrys.Count); i++ )
                featureVector.Add(0);

            featureVector[this.Age / 5] = 1;
            featureVector[23 + countrys.IndexOf(this.Country)] = 1;
            if(this.Gender == "m")
                featureVector[23 + countrys.Count + 1] = 1;
            else
                featureVector[23 + countrys.Count + 2] = 1;

            vectorSize = Math.Sqrt(featureVector.Sum(f => Math.Pow(f, 2)));
        }
    }
}
