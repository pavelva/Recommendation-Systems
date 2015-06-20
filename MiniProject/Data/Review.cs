using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MiniProject.Data
{
    class Review : IItem<string, string, string>
    {
        [JsonProperty(PropertyName = "user_id")]
        private string userId;
        [JsonProperty(PropertyName = "review_id")]
        private string reviewId;
        [JsonProperty(PropertyName = "track_id")]
        private string trackId;

        public Review(string userId, string reviewId, string trackId)
        {
            this.userId = userId;
            this.reviewId = reviewId;
            this.trackId = trackId;
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
            return this.trackId;
        }

        public double GetRating()
        {
            return 1.0;
        }
    }
}
