using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1.Data
{
    interface IItem<T, K, M>
    {
        T GetUserID();
        M GetShearedItemID();
        K GetUniqueItemID();
        double GetRating(); 
    }
}
