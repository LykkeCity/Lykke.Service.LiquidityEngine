using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Settlements;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class SettlementModelValidator : AbstractValidator<SettlementOperationModel>
    {
        public SettlementModelValidator()
        {
            RuleFor(o => o.AssetId)
                .NotEmpty()
                .WithMessage("Asset id required");
            
            RuleFor(o => o.Comment)
                .NotEmpty()
                .WithMessage("Comment required");

            RuleFor(o => o.UserId)
                .NotEmpty()
                .WithMessage("User id required");
        }
    }
}
