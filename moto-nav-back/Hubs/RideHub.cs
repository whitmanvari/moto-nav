using Microsoft.AspNetCore.SignalR;
using MotoNav.Application.DTOs.Rides;
using MotoNav.Application.DTOs.Sos;
using MotoNav.Application.Interfaces.Repositories;
using MotoNav.Domain.Entities.Rides;
using NetTopologySuite.Geometries;

namespace moto_nav_back.Hubs;

public class RideHub(
    IGroupRideRepository rideRepository,
    ISosAlertRepository sosRepository) : Hub
{
    private readonly IGroupRideRepository _rideRepository = rideRepository;
    private readonly ISosAlertRepository _sosRepository = sosRepository;


    /// Sürücünün belirli bir grup sürüşü odasına katılması
    public async Task JoinRideGroup(string rideId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, rideId);
        await Clients.Group(rideId).SendAsync("UserJoinedGroup", Context.ConnectionId);
    }

    /// Sürücünün gruptan ayrılması
    public async Task LeaveRideGroup(string rideId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, rideId);
        await Clients.Group(rideId).SendAsync("UserLeftGroup", Context.ConnectionId);
    }

    /// Sürüş esnasında canlı GPS verisini gruptaki diğer sürücülere anlık yayınlar ve DB'ye yazar
    public async Task SendLocationUpdate(UpdateRideLocationDto dto)
    {
        // 1. Veritabanında son konumu güncelle
        var location = new GroupRideLocation
        {
            GroupRideId = dto.GroupRideId,
            UserId = dto.UserId,
            CurrentLocation = new Point(dto.Longitude, dto.Latitude) { SRID = 4326 },
            CurrentSpeedKmh = dto.CurrentSpeedKmh,
            IsLaggingBehind = dto.IsLaggingBehind,
            Timestamp = DateTime.UtcNow
        };

        await _rideRepository.UpdateMemberLocationAsync(location);

        // 2. O gruptaki tüm aktif sürücülere WebSocket üzerinden fırlat
        await Clients.Group(dto.GroupRideId.ToString()).SendAsync("ReceiveLocationUpdate", new
        {
            dto.UserId,
            dto.Latitude,
            dto.Longitude,
            dto.CurrentSpeedKmh,
            dto.IsLaggingBehind,
            Timestamp = DateTime.UtcNow
        });
    }

    /// Acil durum (SOS) sinyali yayınlandığında tüm bağlı sürücülere anlık alarm düşürür
    public async Task BroadcastSosAlert(CreateSosAlertDto dto)
    {
        await Clients.All.SendAsync("ReceiveSosAlert", new
        {
            dto.UserId,
            dto.Type,
            dto.Latitude,
            dto.Longitude,
            dto.Note,
            Timestamp = DateTime.UtcNow
        });
    }
}