using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class InstrumentLevelModelValidator : AbstractValidator<InstrumentLevelModel>
    {
        public InstrumentLevelModelValidator()
        {
            RuleFor(o => o.Number)
                .GreaterThan(0)
                .WithMessage("Number should be greater than zero");

            RuleFor(o => o.Volume)
                .GreaterThan(0)
                .WithMessage("Volume should be greater than zero");
            
            RuleFor(o => o.Markup)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Markup should be greater than or equal to zero");
        }
    }
}
