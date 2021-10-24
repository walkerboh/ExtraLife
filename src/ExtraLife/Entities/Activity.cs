using System;

namespace ExtraLife.Entities
{
    public class Activity
    {
        public decimal? Amount { get; set; }

        public DateTime CreatedDateUtc { get; set; }

        public string ImageUrl { get; set; }

        public bool? IsIncentive { get; set; }

        public string Message { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }
    }
}