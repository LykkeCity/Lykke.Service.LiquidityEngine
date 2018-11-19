using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.CrossRateInstruments;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class CrossRateInstrumentModelValidator : AbstractValidator<CrossRateInstrumentModel>
    {
        public CrossRateInstrumentModelValidator()
        {
            RuleFor(o => o.AssetPairId)
                .NotEmpty()
                .WithMessage("Asset pair id required");

            RuleFor(o => o.QuoteSource)
                .NotEmpty()
                .WithMessage("Quote source required");

            RuleFor(o => o.ExternalAssetPairId)
                .NotEmpty()
                .WithMessage("External asset pair id required");
        }
    }
}
