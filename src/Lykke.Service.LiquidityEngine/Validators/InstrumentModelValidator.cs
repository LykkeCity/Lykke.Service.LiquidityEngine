using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class InstrumentModelValidator : AbstractValidator<InstrumentModel>
    {
        public InstrumentModelValidator()
        {
            RuleFor(o => o.AssetPairId)
                .NotEmpty()
                .WithMessage("Asset pair required");

            RuleFor(o => o.Mode)
                .Must((instance, value) => value != InstrumentMode.None)
                .WithMessage("Mode can not be unspecified");
            
            RuleFor(o => o.Markup)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Markup should be greater than or equal to zero");
        }
    }
}
