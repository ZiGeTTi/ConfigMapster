using ConfigurationApi.Models.Requests;
using ConfigurationApi.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigMapster.API.ApplicationService.Services.Interfaces
{
    public interface IConfiguraitonService
    {
        Task<CreateConfigurationRecordResponse> CreateConfiguraitonAsync(CreateConfigurationRecordRequest request, CancellationToken token);
        Task<UpdateConfigurationRecordResponse> UpdateConfiguraitonAsync(UpdateConfigurationRecordRequest request, CancellationToken token);
        Task DeleteConfiguraitonAsync(Guid id, CancellationToken token);
        Task <QueryConfigurationRecordsResponse> ListConfigurations(string applicationName, CancellationToken token);
    }
}
