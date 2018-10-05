using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.MarketMaker;

namespace Lykke.Service.LiquidityEngine.Validators
{
    [UsedImplicitly]
    public class MarketMakerModelValidator : AbstractValidator<MarketMakerStateUpdateModel>
    {
        public MarketMakerModelValidator()
        {
            RuleFor(o => o.Status)
                .Must(o => o == MarketMakerStatus.Active || o == MarketMakerStatus.Disabled)
                .WithMessage($"Status must be '{MarketMakerStatus.Active}' or '{MarketMakerStatus.Disabled}'.");

            RuleFor(o => o.UserId)
                .NotEmpty()
                .WithMessage("User id is required.");
        }
    }
}
