using Prometheus;

namespace Lykke.Service.LiquidityEngine.DomainServices.Metrics
{
    public static class PrometheusMetrics
    {
        public static readonly Gauge B2C2OrderBookLatency = Prometheus.Metrics
            .CreateGauge("b2c2_orderbook_duration_milliseconds", "Gauge of B2C2 OrderBook processing durations.");

        public static readonly Gauge MarketMakingLatency = Prometheus.Metrics
            .CreateGauge("market_making_duration_milliseconds", "Gauge of Market Making durations.");

        public static readonly Gauge MarketMakingAssetPairLatency = Prometheus.Metrics
            .CreateGauge("market_making_asset_pair_duration_milliseconds", "Gauge of Asset Pair Market Making durations.");

        public static readonly Gauge MarketMakingMeRequestLatency = Prometheus.Metrics
            .CreateGauge("market_making_me_request_duration_milliseconds", "Gauge of Market Making ME request durations.");

        public static readonly Gauge HedgeTotalLatency = Prometheus.Metrics
            .CreateGauge("total_hedge_duration_milliseconds", "Gauge of Total Hedging durations.");

        public static readonly Gauge HedgeAssetPairLatency = Prometheus.Metrics
            .CreateGauge("asset_pair_hedge_duration_milliseconds", "Gauge of Asset Par Hedging durations.");

        public static readonly Gauge HedgeB2C2OrderRequestLatency = Prometheus.Metrics
            .CreateGauge("hedge_order_request_duration_milliseconds", "Gauge of Hedging Order request durations.");
    }
}
