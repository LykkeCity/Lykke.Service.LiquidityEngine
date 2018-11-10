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
                .InclusiveBetween(0, 1)
                .WithMessage("Markup should be between zero and one");
        }
    }
}
