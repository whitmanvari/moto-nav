using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace moto_nav_back.Hubs;

[Authorize]
public class RideHub : Hub
{
    // Bir grup sürüşü odasına katılma
    public async Task JoinRideGroup(string rideId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, rideId);

        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.ConnectionId;
        await Clients.Group(rideId).SendAsync("UserJoined", userId);
    }

    // Gruptan ayrılma
    public async Task LeaveRideGroup(string rideId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, rideId);

        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.ConnectionId;
        await Clients.Group(rideId).SendAsync("UserLeft", userId);
    }

    // Anlık konum yayını (Sürücü hareket ettikçe haritada güncelleme)
    public async Task SendLocationUpdate(string rideId, double latitude, double longitude, double speed, double heading)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        // O gruptaki diğer tüm sürücülere bu konumu canlı yayınla
        await Clients.OthersInGroup(rideId).SendAsync("ReceiveLocationUpdate", new
        {
            UserId = userId,
            Latitude = latitude,
            Longitude = longitude,
            Speed = speed,
            Heading = heading,
            Timestamp = DateTime.UtcNow
        });
    }
}