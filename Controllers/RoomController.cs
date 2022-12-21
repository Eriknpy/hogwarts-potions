using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Data;
using HogwartsPotions.Data.Services;
using HogwartsPotions.Models;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/room")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomController(IRoomService context)
        {
            _service = context;
        }

        [HttpGet("all")]
        public async Task<List<Room>> GetAllRooms()
        {
            return await _service.GetAllRooms();
        }

        [HttpPost("add")]
        public async Task AddRoom([FromBody] Room room)
        {
            await _service.AddRoom(room);
        }

        [HttpGet("{id}")]
        public async Task<Room> GetRoomById(long id)
        {
            return await _service.GetRoomById(id);
        }

        [HttpPut("{id}")]
        public void UpdateRoomById(long id, [FromBody] Room updatedRoom)
        {
            _service.UpdateRoomById(id,updatedRoom);
        }

        [HttpDelete("{id}")]
        public async Task DeleteRoomById(long id)
        {
            await _service.DeleteRoomById(id);
        }

        [HttpGet("rat-owners")]
        public async Task<List<Room>> GetRoomsForRatOwners()
        {
            return await _service.GetRoomsForRatOwners();
        }
    }
}
