using System;

namespace ExtraLife.Entities
{
    public class Donation
    {
        public decimal? Amount { get; set; }

        public string AvatarImageUrl { get; set; }

        public DateTime CreatedDateUtc { get; set; }

        public string DisplayName { get; set; }

        public string DonationId { get; set; }

        public string DonorId { get; set; }

        public int EventId { get; set; }

        public string IncentiveId { get; set; }

        public string Message { get; set; }

        public int? ParticipantId { get; set; }

        public int? TeamId { get; set; }
    }
}