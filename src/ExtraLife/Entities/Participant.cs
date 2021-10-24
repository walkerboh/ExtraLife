using System;

namespace ExtraLife.Entities
{
    public class Participant
    {
        public string AvatarImageUrl { get; set; }

        public DateTime CreatedDateUtc { get; set; }

        public string DisplayName { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public decimal FundraisingGoal { get; set; }

        public bool IsTeamCaptain { get; set; }

        public Links Links { get; set; }

        public int NumDonations { get; set; }

        public int ParticipantId { get; set; }

        public bool StreamIsLive { get; set; }

        public decimal SumDonations { get; set; }

        public decimal? SumPledges { get; set; }

        public int? TeamId { get; set; }

        public string TeamName { get; set; }
    }

    public class Links
    {
        public string Donate { get; set; }

        public string Page { get; set; }

        public string Stream { get; set; }
    }
}