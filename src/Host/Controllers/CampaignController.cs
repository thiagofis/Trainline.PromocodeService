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
            if (!IsMandatoryHeaderValid())
            {
                return BadRequest("Mandatory headers are not valid.");
            }

            // TODO - Campaign service

            return Ok(new Campaign());
        }

        [HttpPost("/new")]
        public async Task<IActionResult> NewCampaign([FromBody] NewCampaign request)
        {
            if (!IsMandatoryHeaderValid())
            {
                return BadRequest("Mandatory headers are not valid.");
            }

            // TODO - Campaign service
            return Ok();
        }

        [HttpPost("/update")]
        public async Task<IActionResult> UpdateCampaign([FromBody] UpdateCampaign request)
        {
            if (!IsMandatoryHeaderValid())
            {
                return BadRequest("Mandatory headers are not valid.");
            }

            // TODO - Campaign service
            return Ok();
        }

        [HttpPost("/delete")]
        public async Task<IActionResult> DeleteCampaign([FromHeader] Guid campaignId)
        {
            if (!IsMandatoryHeaderValid())
            {
                return BadRequest("Mandatory headers are not valid.");
            }

            // TODO - Campaign service
            return Ok();
        }

        private bool IsMandatoryHeaderValid()
        {
            var userAgent = Request.Headers["User-Agent"];
            var traceId = Request.Headers["TraceId"];

            return !string.IsNullOrWhiteSpace(userAgent) &&
                   !string.IsNullOrWhiteSpace(traceId);
        }
    }
}
