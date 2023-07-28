using System.Collections.Generic;
using System.Threading.Tasks;
using ExtraLife.Entities;

namespace ExtraLife
{
    public interface IExtraLifeApiClient
    {
        Task<IEnumerable<Participant>> GetParticipantsAsync(int limit, int page);

        Task<IEnumerable<Participant>> GetAllParticipantsAsync();

        Task<Participant> GetParticipantAsync(int participantId);

        Task<IEnumerable<Donation>> GetParticipantDonationsAsync(int participantId, int limit, int page);

        Task<IEnumerable<Donation>> GetAllParticipantDonationsAsync(int participantId);

        Task<IEnumerable<Donor>> GetParticipantDonorsAsync(int participantId, int limit, int page);

        Task<IEnumerable<Donor>> GetAllParticipantDonorsAsync(int participantId);

        Task<IEnumerable<Activity>> GetAllParticipantActivitiesAsync(int participantId);
    }
}