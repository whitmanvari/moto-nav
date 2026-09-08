using MotoNav.Domain.Entities.Rides;

namespace MotoNav.Application.Interfaces.Repositories;

public interface IGroupRideRepository : IGenericRepository<GroupRide>
{
    Task<GroupRide?> GetByJoinCodeAsync(string joinCode);
    Task<GroupRide?> GetRideWithMembersAsync(Guid rideId);
    Task<bool> JoinRideAsync(string joinCode, Guid userId);
    Task UpdateMemberLocationAsync(GroupRideLocation location);
    Task<IEnumerable<GroupRideLocation>> GetRideLiveLocationsAsync(Guid rideId);
}