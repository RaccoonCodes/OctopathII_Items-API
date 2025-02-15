using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OctopathII_Items.DTO;
using OctopathII_Items.Models;
using OctopathII_Items.Models.Interfaces;

namespace OctopathII_Items.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
            => _equipmentService = equipmentService;

        [HttpGet]
        public async Task<ActionResult<RestDTO<Equipment[]>>> GetEquipment([FromQuery]RequestDTO<Equipment> requestDTO)
        {
            RestDTO<Equipment[]> results = await _equipmentService.GetEquipmentAsync(requestDTO, Url.Action(
                null, "Equipment", null, Request.Scheme)!, "Self", "GET");

            if (!results.Data.Any())
            {
                return results.Message != null
                    ? Ok(results.Message)
                    : BadRequest("Invalid pagination parameters. Ensure 'pageIndex' >= 0 and 'pageSize' > 0.");
            }

            return Ok(results);
        }

        [HttpPut]
        public async Task<ActionResult<Equipment>> PutEquipment([FromBody]Equipment equipment)
        {
            Equipment? result = await _equipmentService.PutEquipmentAsync(equipment);

            if (result == null)
            {
                return NotFound("Error: Name of the item is not found!");
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Equipment>> GetInfo(string name)
        {
            Equipment? result = await _equipmentService.GetInfoEquipment(name);

            if (result == null)
            {
                return NotFound(new { Message = "Item not found!" });
            }
            return Ok(result);
        }
    }
}
