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
                null, "Items", null, Request.Scheme)!, "Self", "GET");

            if (!results.Data.Any())
            {
                return results.Message != null
                    ? Ok(results.Message)
                    : BadRequest("Invalid pagination parameters. Ensure 'pageIndex' >= 0 and 'pageSize' > 0.");
            }

            return Ok(results);
        }
    }
}
