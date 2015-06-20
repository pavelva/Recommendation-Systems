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
        private String Contry;
        private String Gender;
        private int Age;
        [JsonProperty(PropertyName = "user_id")]
        private String ID;

        public void SetAge(String age)
        {
            Age = Int32.Parse(age);
        }

        public void SetCountry(String contry)
        {
            Contry = contry;
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

        public String GetContry()
        {
            return Contry;
        }

        public String getID()
        {
            return ID;
        }
    }
}
