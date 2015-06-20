using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniProject.Utils;

namespace MiniProject.Data
{
    //T - User ID Type.
    //K - Unique Item ID Type.
    //M - Shared Item ID Type.
    //I - Item Type.
    class DataSet<T, K, M, I> where I:IItem<T, K, M>
    {
        private Dictionary<K, I> Items;
        private Dictionary<T, Dictionary<M, I>> RatingMatrix;
        private Dictionary<M, Dictionary<T, I>> UsersByItems;
        //private Dictionary<T, Dictionary<T, List<I>>> SharedItems;
        //private Dictionary<T, Dictionary<T, List<I>>> JoinedItems;
        private Dictionary<T, double> UsersAverage;
        private Dictionary<M, double> ItemsAverage;
        private int ItemCounter;
        private int UserCounter;
        private double RatingAverage;
        private UserDataSet UserDS;

        public DataSet()
        {
            this.Items = new Dictionary<K, I>();
            this.RatingMatrix = new Dictionary<T, Dictionary<M, I>>();
            this.UsersByItems = new Dictionary<M, Dictionary<T, I>>();
            this.UsersAverage = new Dictionary<T, double>();
            this.ItemsAverage = new Dictionary<M, double>();
            //this.SharedItems = new Dictionary<T, Dictionary<T, List<I>>>();
            //this.JoinedItems = new Dictionary<T, Dictionary<T, List<I>>>();
            this.ItemCounter = 0;
            this.UserCounter = 0;
            this.RatingAverage = Double.MinValue;
        }

        public void AddItem(I item)
        {
            try
            {
                Items.Add(item.GetUniqueItemID(), item);
                ItemCounter++;

                if (RatingMatrix.ContainsKey(item.GetUserID()))
                {
                    RatingMatrix[item.GetUserID()].Add(item.GetShearedItemID(), item);
                }
                else
                {
                    RatingMatrix.Add(item.GetUserID(), new Dictionary<M, I>());
                    RatingMatrix[item.GetUserID()].Add(item.GetShearedItemID(), item);
                    UserCounter++;
                }

                if (UsersByItems.ContainsKey(item.GetShearedItemID()))
                {
                    UsersByItems[item.GetShearedItemID()].Add(item.GetUserID(), item);
                }
                else
                {
                    UsersByItems.Add(item.GetShearedItemID(), new Dictionary<T, I>());
                    UsersByItems[item.GetShearedItemID()].Add(item.GetUserID(), item);
                }
            }
            catch (ArgumentException) { }
        }

        public DataSet<T, K, M, I> splitDataSet(double splitSize)
        {
            //spliting the dataset reset the users avarege ratings and shared items.
            this.UsersAverage = new Dictionary<T, double>();
            //this.SharedItems = new Dictionary<T, Dictionary<T, List<I>>>();
            //this.JoinedItems = new Dictionary<T, Dictionary<T, List<I>>>();
            this.RatingAverage = Double.MinValue;

            Random randomGen = new Random();
            DataSet<T, K, M, I> splitDataSet = new DataSet<T, K, M, I>();
            //the number to extract.
            int remains = (int)(ItemCounter * splitSize);

            //generate random user generator.
            List<T> userIds = new List<T>(RatingMatrix.Keys);
            RandomUniqeGen<T> userRandomUniqeGen = new RandomUniqeGen<T>(userIds);

            while (remains > 0)
            {
                //select user from which the data will be removed.
                T userID = userRandomUniqeGen.next();
                //the number of reviews to extract from selected user.
                int itemsToExtract = randomGen.Next(0, RatingMatrix[userID].Count - 1);
                //Console.WriteLine("{0}/{1}", itemsToExtract, RatingMatrix[userID].Count);
                //the reviews of the user to remove.
                Dictionary<M, I> userItems = RatingMatrix[userID];

                //generate random reviews generator.
                List<M> sharedItemIDs = new List<M>(RatingMatrix[userID].Keys);
                RandomUniqeGen<M> itemRandomUniqeGen = new RandomUniqeGen<M>(sharedItemIDs);
                
                //remove reviews from dataset and add them to the new dataset.
                //do this while there is reviews to remove and there is reviews to
                //remove from the user.
                while (remains > 0 && itemsToExtract > 0)
                {
                    M sharedItemID = itemRandomUniqeGen.next();
                    I deletedItem = this.DeleteItem(userID, sharedItemID);
                    splitDataSet.AddItem(deletedItem);
                    remains--;
                    itemsToExtract--;
                }
            }

            return splitDataSet;
        }

        private I DeleteItem(T userID, M sharedItemID)
        {
            //if not exist return null and exit.
            if (!RatingMatrix.ContainsKey(userID))
                throw new KeyNotFoundException("Can't remove item");
            if (!RatingMatrix[userID].ContainsKey(sharedItemID))
                throw new KeyNotFoundException("Can't remove item");

            //if not exist return null and exit.
            if (!UsersByItems.ContainsKey(sharedItemID))
                throw new KeyNotFoundException("Can't remove item");
            if (!UsersByItems[sharedItemID].ContainsKey(userID))
                throw new KeyNotFoundException("Can't remove item");

            I toDelete = RatingMatrix[userID][sharedItemID];

            if (!Items.ContainsKey(toDelete.GetUniqueItemID()))
                throw new KeyNotFoundException("Can't remove item");

            Items.Remove(toDelete.GetUniqueItemID());
            RatingMatrix[userID].Remove(sharedItemID);
            UsersByItems[sharedItemID].Remove(userID);

            if (RatingMatrix[userID].Count == 0)
            {
                RatingMatrix.Remove(userID);
                UserCounter--;
            }
            if (UsersByItems[sharedItemID].Count == 0)
            {
                UsersByItems.Remove(sharedItemID);
            }
            ItemCounter--;

            return toDelete;
        }

        public double GetRating(T userID, M sharedItemID)
        {
            if (RatingMatrix.ContainsKey(userID))
            {
                if (RatingMatrix[userID].ContainsKey(sharedItemID))
                {
                    return RatingMatrix[userID][sharedItemID].GetRating();
                }
            }
            return 0.0;
        }

        public double GetUserAverage(T userId)
        {
            if (UsersAverage.ContainsKey(userId))
            {
                return UsersAverage[userId];
            }
            else if (RatingMatrix.ContainsKey(userId))
            {
                return CalculateUserAverage(userId);
            }
            else
            {
                return -1;
            }
        }

        private double CalculateUserAverage(T userId)
        {
            return ((double) RatingMatrix[userId].Count) / UsersByItems.Count;
        }

        public List<M> getSharedItems(T userID1, T userID2)
        {
            //return RatingMatrix[userID1].Keys.Intersect(RatingMatrix[userID2].Keys).ToList();
            List<M> sharedItems = new List<M>();

            foreach (I item in RatingMatrix[userID1].Values)
            {
                if (RatingMatrix[userID2].ContainsKey(item.GetShearedItemID()))
                {
                    sharedItems.Add(item.GetShearedItemID());
                }
            }
            return sharedItems;
        }

        public List<M> getJoinedItems(T userID1, T userID2)
        {
            return RatingMatrix[userID1].Keys.Union(RatingMatrix[userID2].Keys).ToList();
        }

        public Dictionary<T, Dictionary<M, I>>.KeyCollection GetUserIDs()
        {
            return RatingMatrix.Keys;
        }

        public Dictionary<K, I>.ValueCollection GetItems()
        {
            return Items.Values;
        }

        public IEnumerable<I> GetItems(T userID)
        {
            return RatingMatrix[userID].Values;
        }

        public double getItemAverage()
        {
            return UsersByItems.Keys.Select(si => getItemAverage(si)).Average();
        }

        public double getItemAverage(M shearedItemID)
        {
            return ((double) UsersByItems[shearedItemID].Count) / RatingMatrix.Count;
        }

        public Dictionary<M, Dictionary<T, I>> getUsersByItems()
        {
            return UsersByItems;
        }

        public bool UserContainsItem(T UID, M IID)
        {
            if(RatingMatrix.ContainsKey(UID))
                return RatingMatrix[UID].ContainsKey(IID);
            return false;
        }

        public Dictionary<T, Dictionary<M, I>> GetMatrixRating()
        {
            return RatingMatrix;
        }

        public void setUserDataSet(UserDataSet userDataSet)
        {
            UserDS = userDataSet;
        }

        public UserData GetUserData(String UID)
        {
            return UserDS.GetUserData(UID);
        }
    }
}
