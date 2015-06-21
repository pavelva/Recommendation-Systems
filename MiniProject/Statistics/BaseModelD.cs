using MiniProject.Data;
using MiniProject.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniProject.Statistics
{
    class BaseModelD<T, K, M, I> : BaseModel<T, K, M, I> where I : IItem<T, K, M>
    {
        protected Dictionary<String, double> CountryCodes;
        private RandomUniqeGen<double> CodeGen;

        public BaseModelD(DataSet<T, K, M, I> train, DataSet<T, K, M, I> validation, int vSize)
            : base(train, validation, vSize + 3)
        {
            CountryCodes = new Dictionary<string, double>();
            List<double> codes = new List<double>();

            for (double i = -1.0; i <= 1.0; i += 0.02)
                codes.Add(i);
            CodeGen = new RandomUniqeGen<double>(codes);

            foreach(T UID in train.GetMatrixRating().Keys){
                addUserData(Pu[UID], train.GetUserData(UID.ToString()));
            }
        }

        private void addUserData(List<double> pu, UserData userData)
        {
            double countryCode = GetCountryCode(userData.GetCountry());
            double ageRep = userData.GetAge() / 100.0;
            double genderRep = userData.GetGender().Equals("m") ? -1.0 : 1.0;

            pu[0] = countryCode / 100;
            pu[1] = ageRep / 100;
            pu[2] = genderRep / 100;
        }

        private double GetCountryCode(string country)
        {
            if (CountryCodes.ContainsKey(country))
                return CountryCodes[country];
            double code = CodeGen.next();
            CountryCodes.Add(country, code);
            return code;
        }
    }
}