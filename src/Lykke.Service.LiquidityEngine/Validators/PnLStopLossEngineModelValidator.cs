using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class PnLStopLossEngineModelValidator : AbstractValidator<PnLStopLossEngineModel>
    {
        public PnLStopLossEngineModelValidator()
        {
            RuleFor(o => o.AssetPairId)
                .NotEmpty()
                .WithMessage("Asset pair required.");

            RuleFor(o => o.Interval)
                .NotEmpty()
                .WithMessage("Interval required.");

            RuleFor(o => o.Threshold)
                .LessThanOrEqualTo(0)
                .WithMessage("Threshold should be greater than or equal to zero.");

            RuleFor(o => o.Markup)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Markup should be greater than or equal to zero.");
        }
    }
}
