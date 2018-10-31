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

            RuleFor(o => o.PnLThreshold)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Pnl threshold should be greater than or equal to zero");

            RuleFor(o => o.InventoryThreshold)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Inventory threshold should be greater than or equal to zero");

            RuleFor(o => o.VolumeAccuracy)
                .InclusiveBetween(1, 8)
                .WithMessage("Volume accuracy should be greater than or equal to 1 and less than o equal to 8");

            RuleFor(o => o.MinVolume)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Min volume should be greater than or equal to zero");

            RuleFor(o => o.HalfLifePeriod)
                .GreaterThan(0)
                .WithMessage("Half life period should be greater than zero");
        }
    }
}
