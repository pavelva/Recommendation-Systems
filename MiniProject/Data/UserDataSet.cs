using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Data
{
    class UserDataSet
    {
        private Dictionary<String, UserData> UsersData;
        private List<string> countrys;

        public UserDataSet()
        {
            UsersData = new Dictionary<string,UserData>();
            countrys = new List<string>();
        }

        public void AddUser(String key, UserData data)
        {
            if (!UsersData.ContainsKey(key))
            {
                UsersData.Add(key, data);
            }

            if(!countrys.Contains(data.GetCountry()))
                countrys.Add(data.GetCountry());
        }

        public UserData GetUserData(String key)
        {
            if (UsersData.ContainsKey(key))
            {
                return UsersData[key];
            }
            throw new KeyNotFoundException("User ID not exist");
        }

        public void initFeatureVectors()
        {
            foreach (UserData u in UsersData.Values)
            {
                u.initFeatureVector(countrys);
            }
        }
    }
}
