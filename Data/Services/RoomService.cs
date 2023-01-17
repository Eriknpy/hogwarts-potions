using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data.Enums;
using HogwartsPotions.Data.Interfaces;
using HogwartsPotions.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HogwartsPotions.Data.Services
{
    public class RoomService : IRoomService
    {
        private readonly HogwartsContext _context;

        public RoomService(HogwartsContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await _context.Rooms
                .Include(p => p.Residents)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddRoom(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task<Room> GetRoomById(long id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task UpdateRoomById(long id, Room updatedRoom)
        {
            updatedRoom.Id = id;
            EntityEntry entityEntry = _context.Entry(updatedRoom);
            entityEntry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomById(long id)
        {
            Room room = _context.Rooms.Find(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Room>> GetRoomsForRatOwners()
        {
            return await _context.Rooms
                .Include(r => r.Residents)
                .Where(room => !room.Residents
                    .Any(resident =>
                         resident.PetType == PetType.Cat ||
                         resident.PetType == PetType.Owl))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}