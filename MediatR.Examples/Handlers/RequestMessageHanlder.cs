using MediatR.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Examples.Handlers
{
    public class ConcreteARequestMessageHanlder : IRequestHandler<RequestMessage<ConcreteClassA>, string>
    {
        public Task<string> Handle(RequestMessage<ConcreteClassA> request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.AbstractClass.GetType().ToString());
        }
    }
    public class ConcreteBRequestMessageHanlder : IRequestHandler<RequestMessage<ConcreteClassB>, string>
    {
        public Task<string> Handle(RequestMessage<ConcreteClassB> request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.AbstractClass.GetType().ToString());
        }
    }
}
