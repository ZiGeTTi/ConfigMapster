using ConfigMapster.API.ApplicationService.Services.Interfaces;
using ConfigurationApi.Models.Requests;
using ConfigurationApi.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguraitonService _configService;

        public ConfigController(IConfiguraitonService configService)
        {
            _configService = configService;
        }

        [HttpPost("insert")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateConfigurationRecordResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create([FromBody] CreateConfigurationRecordRequest request, CancellationToken token)
        {
            var result = await _configService.CreateConfiguraitonAsync(request, token);
            return Created($"api/v1/configurations/{result.Id}", result);
        }
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateConfigurationRecordResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Update([FromBody] UpdateConfigurationRecordRequest request, CancellationToken token)
        {
            return Ok(await _configService.UpdateConfiguraitonAsync(request, token));
        }
        [HttpDelete("delete")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _configService.DeleteConfiguraitonAsync(id, cancellationToken);
            return NoContent();
        }
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueryConfigurationRecordsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> List([FromQuery] string applicationName, CancellationToken token)
        {
            var result = await _configService.ListConfigurations(applicationName, token);
            return Ok(result);
        }
    }
}