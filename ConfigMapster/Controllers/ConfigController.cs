using ConfigMapster.API.ApplicationService.Services.Interfaces;
using ConfigurationApi.Entities;
using ConfigurationApi.Events;
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
      private readonly  IConfiguraitonService _configService;

        public ConfigController(IConfiguraitonService configService)
        {
            _configService = configService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateConfigurationRecordResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultExceptionDto))]
        public async Task<IActionResult> Create([FromBody] CreateConfigurationRecordRequest request,CancellationToken token )
        {
            if (!IsValidValue(input.Value, input.Type))
            {
                throw new ValidationException("Invalid value or type provided");
            }

           var response = await _configService.CreateConfiguraitonAsync(request, token);


            return Created($"api/v1/configurations/{config.IdentityObject.Id}", new CreateConfigurationRecordResponse { Id = config.IdentityObject.Id });

        }
    }
}
