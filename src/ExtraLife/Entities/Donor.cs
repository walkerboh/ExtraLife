using System;

namespace ExtraLife.Entities
{
    public class Donor
    {
        public string AvatarImageUrl { get; set; }

        public string DisplayName { get; set; }

        public string DonorId { get; set; }

        public DateTime ModifiedDateUtc { get; set; }

        public int NumDonations { get; set; }

        public decimal SumDonations { get; set; }
    }
}