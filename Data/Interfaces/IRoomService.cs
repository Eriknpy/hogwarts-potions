using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;

namespace HogwartsPotions.Data.Interfaces
{
    public interface IRoomService
    {
        Task<List<Room>> GetAllRooms();
        Task AddRoom(Room room);
        Task<Room> GetRoomById(long id);
        Task UpdateRoomById(long id, Room updatedRoom);
        Task DeleteRoomById(long id);
        Task<List<Room>> GetRoomsForRatOwners();
    }
}
