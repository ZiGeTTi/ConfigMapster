using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMapster.API.Infrastructure.Exceptions
{
    public interface ICustomException
    {
        public object CustomMessage { get; set; }
    }
    public sealed class ObjectNotFoundException<TObject> : ObjectNotFoundException
    {
        public ObjectNotFoundException(object key, object customMessage = null)
            : base($"Object '{typeof(TObject).Name}' with key '{key}' was not found.", customMessage)
        {
        }
    }
    public abstract class ObjectNotFoundException : Exception, ICustomException
    {
        public object CustomMessage { get; set; }

        protected ObjectNotFoundException(string message, object customMessage = null)
            : base(message)
        {
            CustomMessage = customMessage;
        }
    }

    public class RecordAlreadyExistException : Exception, ICustomException
    {
        public object CustomMessage { get; set; }

        public RecordAlreadyExistException(string message, Exception innerException = null, object customMessage = null)
            : base(message, innerException)
        {
            CustomMessage = customMessage;
        }
    }
}
