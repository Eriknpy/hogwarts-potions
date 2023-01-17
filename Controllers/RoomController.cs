using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data.Interfaces;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomController(IRoomService context)
        {
            _service = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _service.GetAllRooms();
            if (rooms != null)
            {
                return StatusCode(StatusCodes.Status200OK, rooms);
            }
            return StatusCode(StatusCodes.Status404NotFound, $"No rooms found!");
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRoom([FromBody] Room room)
        {
            var rooms = await _service.GetAllRooms();
            if (!rooms.Any(r => r.Id == room.Id))
            {
                await _service.AddRoom(room);
                return StatusCode(StatusCodes.Status201Created, room);
            }
            if (room.Id == 0)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
            return StatusCode(StatusCodes.Status406NotAcceptable, "Room already exists!");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(long id)
        {
            var room = await _service.GetRoomById(id);
            if (room != null)
            {
                return StatusCode(StatusCodes.Status200OK, room);
            }
            return StatusCode(StatusCodes.Status404NotFound, $"Room by {id} doesn't exist!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoomById(long id, [FromBody] Room updatedRoom)
        {
            await _service.UpdateRoomById(id, updatedRoom);
            return StatusCode(StatusCodes.Status200OK, updatedRoom);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomById(long id)
        {
            var room = await _service.GetRoomById(id);
            if (room != null)
            {
                await _service.DeleteRoomById(id);
                return StatusCode(StatusCodes.Status202Accepted, $"{id}. room deleted");
            }
            return StatusCode(StatusCodes.Status404NotFound, $"Room by id: {id} doesn't exist!");
        }

        [HttpGet("rat-owners")]
        public async Task<IActionResult> GetRoomsForRatOwners()
        {
            var ratSafeRooms = await _service.GetRoomsForRatOwners();
            if (ratSafeRooms != null)
            {
                return StatusCode(StatusCodes.Status200OK, ratSafeRooms);
            }
            return StatusCode(StatusCodes.Status404NotFound, "No rat safe rooms found!");
        }
    }
}