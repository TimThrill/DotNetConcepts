using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediatR.Examples.Models
{
    public class RequestMessage<TClass> : IRequest<string> where TClass : AbstractClass
    {
        public TClass AbstractClass { get; set; }
    }
}
