using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Settlements;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class SettlementModelValidator : AbstractValidator<SettlementModel>
    {
        public SettlementModelValidator()
        {
            RuleFor(o => o.AssetId)
                .NotEmpty()
                .WithMessage("Asset id required");
        }
    }
}
