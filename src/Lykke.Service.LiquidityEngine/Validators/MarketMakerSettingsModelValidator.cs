using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Settings;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class MarketMakerSettingsModelValidator : AbstractValidator<MarketMakerSettingsModel>
    {
        public MarketMakerSettingsModelValidator()
        {
            RuleFor(o => o.LimitOrderPriceMaxDeviation)
                .InclusiveBetween(0, 1)
                .WithMessage("Value should be greater than or equal to 0 and less than or equal to 1");

            RuleFor(o => o.LimitOrderPriceMarkup)
                .InclusiveBetween(0, 1)
                .WithMessage("Value should be greater than or equal to 0 and less than or equal to 1");
        }
    }
}
