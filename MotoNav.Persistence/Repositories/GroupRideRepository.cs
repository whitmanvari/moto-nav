using Microsoft.EntityFrameworkCore;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Rides;
using MotoNav.Persistence.Context;

namespace MotoNav.Persistence.Repositories;

public class GroupRideRepository(MotoNavDbContext context) : GenericRepository<GroupRide>(context), IGroupRideRepository
{
    public async Task<GroupRide?> GetByJoinCodeAsync(string joinCode)
    {
        return await _context.GroupRides
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.JoinCode.ToUpper() == joinCode.Trim().ToUpper() && g.IsActive);
    }

    public async Task<GroupRide?> GetRideWithMembersAsync(Guid rideId)
    {
        return await _context.GroupRides
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == rideId);
    }

    public async Task<bool> JoinRideAsync(string joinCode, Guid userId)
    {
        var ride = await GetByJoinCodeAsync(joinCode);
        if (ride == null) return false;

        var alreadyMember = ride.Members.Any(m => m.UserId == userId);
        if (alreadyMember) return true;

        ride.Members.Add(new GroupRideMember
        {
            GroupRideId = ride.Id,
            UserId = userId,
            IsOnline = true
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task UpdateMemberLocationAsync(GroupRideLocation location)
    {
        var existing = await _context.GroupRideLocations
            .FirstOrDefaultAsync(l => l.GroupRideId == location.GroupRideId && l.UserId == location.UserId);

        if (existing != null)
        {
            existing.CurrentLocation = location.CurrentLocation;
            existing.CurrentSpeedKmh = location.CurrentSpeedKmh;
            existing.Timestamp = DateTime.UtcNow;
            existing.IsLaggingBehind = location.IsLaggingBehind;
        }
        else
        {
            await _context.GroupRideLocations.AddAsync(location);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GroupRideLocation>> GetRideLiveLocationsAsync(Guid rideId)
    {
        return await _context.GroupRideLocations
            .Where(l => l.GroupRideId == rideId)
            .ToListAsync();
    }
}