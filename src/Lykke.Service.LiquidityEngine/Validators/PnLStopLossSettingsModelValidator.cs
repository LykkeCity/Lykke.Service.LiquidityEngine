using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLosses;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class PnLStopLossSettingsModelValidator : AbstractValidator<PnLStopLossSettingsModel>
    {
        public PnLStopLossSettingsModelValidator()
        {
            RuleFor(o => o.Id)
                .Empty()
                .WithMessage("Id must be empty.");

            RuleFor(o => o.AssetPairId)
                .NotEmpty()
                .WithMessage("Asset pair required.");

            RuleFor(o => o.Interval)
                .NotEmpty()
                .WithMessage("Interval required.");

            RuleFor(o => o.Markup)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Markup should be greater than or equal to zero.");
        }
    }
}
