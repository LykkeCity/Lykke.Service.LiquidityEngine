using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Credits;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class CreditModelValidator : AbstractValidator<CreditModel>
    {
        public CreditModelValidator()
        {
            RuleFor(o => o.AssetId)
                .NotEmpty()
                .WithMessage("Asset id required");
        }
    }
}
