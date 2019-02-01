using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossSettings;

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

            RuleFor(o => o.Interval)
                .NotEmpty()
                .WithMessage("Interval required.");

            RuleFor(o => o.Threshold)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Threshold should be greater than or equal to zero.");

            RuleFor(o => o.Markup)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Markup should be greater than or equal to zero.");
        }
    }
}
