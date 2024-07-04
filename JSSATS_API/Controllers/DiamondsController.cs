﻿using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondsController : ControllerBase
    {
        private readonly IDiamondService _diamond;

        public DiamondsController(IDiamondService diamond)
        {
            _diamond = diamond;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DiamondResponse>>> GetDiamond()
        {
            try
            {
                var diamonds = await _diamond.GetAllDiamond();
                if (diamonds == null)
                {
                    return NotFound("there are no diamond");
                }

                return Ok(diamonds);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CreateDiamondWithPrice")]
        [Authorize(Roles ="Manager")]
        public async Task<IActionResult> CreateDiamondWithPrice([FromBody] DiamondRequest request)
        {
            try
            {
                var response = await _diamond.CreateDiamondWithPriceAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}