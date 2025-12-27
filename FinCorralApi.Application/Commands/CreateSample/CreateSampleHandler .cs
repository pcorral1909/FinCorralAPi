using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using FinCorralApi.Application.Common.Features;
using FinCorralApi.Shared.Results;
using FinCorralApi.Shared.Features;

namespace FinCorralApi.Application.Command.CreateSample
{

    public sealed class CreateSampleHandler
       : IRequestHandler<CreateSampleCommand, Result<Guid>>
    {
        private readonly IFeatureFlagService _featureFlags;

        public CreateSampleHandler(IFeatureFlagService featureFlags)
        {
            _featureFlags = featureFlags;
        }

        public async Task<Result<Guid>> Handle(
            CreateSampleCommand request,
            CancellationToken cancellationToken)
        {
            return _featureFlags.IsEnabled(FeatureFlags.NewSampleFlow)
                ? await HandleV2(request, cancellationToken)
                : await HandleV1(request, cancellationToken);
        }

        private Task<Result<Guid>> HandleV1(
            CreateSampleCommand request,
            CancellationToken ct)
        {
            // lógica estable
            var id = Guid.NewGuid();
            return Task.FromResult(Result<Guid>.Success(id));
        }

        private Task<Result<Guid>> HandleV2(
            CreateSampleCommand request,
            CancellationToken ct)
        {
            // nueva lógica mejorada
            var id = Guid.NewGuid();
            return Task.FromResult(Result<Guid>.Success(id));
        }
    }



}
