using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.LiquidityEngine.DomainServices.MarketMaker
{
    public class StateOperationContext
    {
        public StateOperationContext(
            MarketMakerStatus currentStatus, 
            MarketMakerStatus targetStatus, 
            string comment, 
            string userId,
            MarketMakerError error = MarketMakerError.None,
            string errorMessage = null)
        {
            CurrentStatus = currentStatus;
            TargetStatus = targetStatus;
            Error = error;
            ErrorMessage = errorMessage;
            Comment = comment;
            UserId = userId;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public MarketMakerStatus CurrentStatus { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MarketMakerStatus TargetStatus { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MarketMakerError Error { get; }

        public string ErrorMessage { get; }

        public string Comment { get; }

        public string UserId { get; }
    }
}
