using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Model;

namespace Trainline.PromocodeService.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampaignController : ControllerBase
    {
        [HttpPost("/get")]
        public async Task<IActionResult> GetCampaign([FromHeader] Guid campaignId)
        {
            // TODO - Campaign service
            var campaing = new Campaign();

            return Ok(campaing);
        }

        [HttpPost("/new")]
        public async Task<IActionResult> NewCampaign([FromBody] NewCampaign request)
        {
            // TODO - Campaign service
            return Ok();
        }

        [HttpPost("/update")]
        public async Task<IActionResult> UpdateCampaign([FromBody] UpdateCampaign request)
        {
            // TODO - Campaign service
            return Ok();
        }

        [HttpPost("/delete")]
        public async Task<IActionResult> DeleteCampaign([FromHeader] Guid campaignId)
        {
            // TODO - Campaign service
            return Ok();
        }
    }
}
