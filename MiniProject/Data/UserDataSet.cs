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

        public UserDataSet()
        {
            UsersData = new Dictionary<string,UserData>();
        }

        public void AddUser(String key, UserData data)
        {
            if (!UsersData.ContainsKey(key))
            {
                UsersData.Add(key, data);
            }
        }

        public UserData GetUserData(String key)
        {
            if (UsersData.ContainsKey(key))
            {
                return UsersData[key];
            }
            throw new KeyNotFoundException("User ID not exist");
        }
    }
}
