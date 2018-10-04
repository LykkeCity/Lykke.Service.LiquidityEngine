using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Credits;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class CreditOperationModelValidator : AbstractValidator<CreditOperationModel>
    {
        public CreditOperationModelValidator()
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
