using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data.Enums;
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
               .ToListAsync();
        }

        public async Task AddRoom(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task<Room> GetRoomById(long id)
        {
            return await _context.Rooms.FirstOrDefaultAsync(room => room.ID == id);
        }

        public async Task UpdateRoomById(long id, Room updatedRoom)
        {
            EntityEntry entityEntry = _context.Entry(updatedRoom);
            entityEntry.State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomById(long id)
        {
            //var entity = await GetRoomById(id);
            //EntityEntry entityEntry = _context.Entry(entity);
            //entityEntry.State = EntityState.Deleted;
            //await _context.SaveChangesAsync();

            Room room = _context.Rooms.Find(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Room>> GetRoomsForRatOwners()
        {
            //List<Room> rooms = new List<Room>();
            //var dbRooms = await _context.Rooms
            //    .Include(r => r.Residents)
            //    .ToListAsync();
            //foreach (var room in dbRooms)
            //{
            //    int bannedPetCounter = 0;
            //    foreach (var student in room.Residents)
            //    {

            //        if (student.PetType == PetType.Owl || student.PetType == PetType.Cat)
            //        {
            //            bannedPetCounter += 1;
            //        }

            //    }

            //    if (bannedPetCounter == 0)
            //    {
            //        rooms.Add(room);
            //    }
            //}
            //return rooms;
            return await _context.Rooms
                .Include(r => r.Residents)
                .Where(room => !room.Residents
                    .Any(resident =>
                         resident.PetType == PetType.Cat ||
                         resident.PetType == PetType.Owl))
                .ToListAsync();
        }
    }
}
