using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMapster.API.Domain.Events
{
    public class BaseEvent
    {
        public Guid ConfigurationRecordId { get; set; }
    }
}
