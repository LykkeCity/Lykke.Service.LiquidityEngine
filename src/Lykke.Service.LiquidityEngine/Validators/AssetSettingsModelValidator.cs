using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.AssetSettings;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class AssetSettingsModelValidator : AbstractValidator<AssetSettingsModel>
    {
        public AssetSettingsModelValidator()
        {
            RuleFor(o => o.AssetId)
                .NotEmpty()
                .WithMessage("Asset id required");

            RuleFor(o => o.LykkeAssetId)
                .NotEmpty()
                .WithMessage("Lykke asset id required");

            RuleFor(o => o.QuoteSource)
                .NotEmpty()
                .WithMessage("Quote source required");

            RuleFor(o => o.ExternalAssetPairId)
                .NotEmpty()
                .WithMessage("External asset pair id required");

            RuleFor(o => o.DisplayAccuracy)
                .InclusiveBetween(1, 8)
                .WithMessage("Display accuracy should be between 1 and 8");
        }
    }
}
