using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OctopathII_Items.DTO;
using OctopathII_Items.Models;
using OctopathII_Items.Models.Interfaces;


namespace OctopathII_Items.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;

        public ItemsController(IItemsService itemsService)
            => _itemsService = itemsService;

        //READ
        [HttpGet]
        public async Task<ActionResult<RestDTO<Item[]>>> GetItems([FromQuery] RequestDTO<Item> requestDTO)
        {
            RestDTO<Item[]> results = await _itemsService.GetItemsAsync(requestDTO, Url.Action(
                null, "Items", null, Request.Scheme)!, "Self", "GET");

            if (!results.Data.Any())
            {
                return results.Message != null
                    ? Ok(results.Message)
                    : BadRequest("Invalid pagination parameters. Ensure 'pageIndex' >= 0 and 'pageSize' > 0.");
            }

            return Ok(results);

        }

        //UPDATE
        [HttpPut]
        public async Task <ActionResult<Item>> PutItem([FromBody]Item item)
        {
            Item? result = await _itemsService.PutItemAsync(item);

            if(result == null)
            {
                return NotFound("Error: Name of the item is not found!");
            }
            return Ok(result);
        }

        //READ BY INFO
        [HttpGet]
        public async Task<ActionResult<Item>> GetInfo(string name)
        {
            Item? result = await _itemsService.GetInfoAsync(name);

            if (result == null)
            {
                return NotFound(new { Message = "Item not found!" });
            }
            return Ok(result);

        }

    }
}
