using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class CrossInstrumentModelValidator : AbstractValidator<CrossInstrumentModel>
    {
        public CrossInstrumentModelValidator()
        {
            RuleFor(o => o.AssetPairId)
                .NotEmpty()
                .WithMessage("Asset pair id required");

            RuleFor(o => o.QuoteSource)
                .NotEmpty()
                .WithMessage("Quote source required");

            RuleFor(o => o.AssetPairId)
                .NotEmpty()
                .WithMessage("External asset pair id required");
        }
    }
}
