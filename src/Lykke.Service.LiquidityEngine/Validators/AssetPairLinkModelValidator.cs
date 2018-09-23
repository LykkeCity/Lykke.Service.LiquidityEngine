using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class AssetPairLinkModelValidator : AbstractValidator<AssetPairLinkModel>
    {
        public AssetPairLinkModelValidator()
        {
            RuleFor(o => o.AssetPairId)
                .NotEmpty()
                .WithMessage("Asset pair id required");

            RuleFor(o => o.ExternalAssetPairId)
                .NotEmpty()
                .WithMessage("External asset pair id required");
        }
    }
}
