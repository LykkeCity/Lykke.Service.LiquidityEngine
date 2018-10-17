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
            RuleFor(o => o.LykkeBalances)
                .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1))
                .WithMessage("Lykke balances timer interval should be greater than or equal to one second.");

            RuleFor(o => o.Hedging)
                .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1))
                .WithMessage("Hedging timer interval should be greater than or equal to one second.");
            
            RuleFor(o => o.ExternalBalances)
                .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1))
                .WithMessage("External balances timer interval should be greater than or equal to one second.");

            RuleFor(o => o.MarketMaker)
                .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1))
                .WithMessage("Market maker timer interval should be greater than or equal to one second.");
        }
    }
}
