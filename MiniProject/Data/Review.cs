using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Assignment1.Data
{
    class Review : IItem<string, string, string>
    {
        [JsonProperty(PropertyName = "user_id")]
        private string userId;
        [JsonProperty(PropertyName = "review_id")]
        private string reviewId;
        [JsonProperty(PropertyName = "business_id")]
        private string businessId;
        [JsonProperty(PropertyName = "stars")]
        private double stars;

        public Review(string userId, string reviewId, string businessId, string stars)
        {
            this.userId = userId;
            this.reviewId = reviewId;
            this.stars = Double.Parse(stars);
            this.businessId = businessId;
        }

        public string GetUserID()
        {
            return this.userId;
        }

        public string GetUniqueItemID()
        {
            return this.reviewId;
        }

        public string GetShearedItemID()
        {
            return this.businessId;
        }

        public double GetRating()
        {
            return this.stars;
        }
    }
}
