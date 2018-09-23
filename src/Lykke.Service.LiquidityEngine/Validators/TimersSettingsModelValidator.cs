using System;
using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Settings;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class TimersSettingsModelValidator : AbstractValidator<TimersSettingsModel>
    {
        public TimersSettingsModelValidator()
        {
            RuleFor(o => o.Balances)
                .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1))
                .WithMessage("Balances timer interval should be greater than or equal to one second.");

            RuleFor(o => o.MarketMaker)
                .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1))
                .WithMessage("Market maker timer interval should be greater than or equal to one second.");
        }
    }
}
