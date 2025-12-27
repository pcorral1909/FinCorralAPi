using System;
using System.Collections.Generic;
using System.Text;
using FinCorralApi.Shared.Results;
using MediatR;

namespace FinCorralApi.Application.Command.CreateSample
{



    public sealed record CreateSampleCommand(string Name)
    : IRequest<Result<Guid>>;
}
