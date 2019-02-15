using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.InternalExchange.Client;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.InternalTrader
{
    public class InternalTraderService : IInternalTraderService
    {
        private readonly IInternalOrderRepository _internalOrderRepository;
        private readonly IInstrumentService _instrumentService;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly IBalanceService _balanceService;
        private readonly ISettingsService _settingsService;
        private readonly ILykkeExchangeService _lykkeExchangeService;
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly IPositionService _positionService;
        private readonly ILog _log;

        public InternalTraderService(
            IInternalOrderRepository internalOrderRepository,
            IInstrumentService instrumentService,
            IAssetsServiceWithCache assetsServiceWithCache,
            IBalanceService balanceService,
            ISettingsService settingsService,
            ILykkeExchangeService lykkeExchangeService,
            IExternalExchangeService externalExchangeService,
            IPositionService positionService,
            ILogFactory logFactory)
        {
            _internalOrderRepository = internalOrderRepository;
            _instrumentService = instrumentService;
            _assetsServiceWithCache = assetsServiceWithCache;
            _balanceService = balanceService;
            _settingsService = settingsService;
            _lykkeExchangeService = lykkeExchangeService;
            _externalExchangeService = externalExchangeService;
            _positionService = positionService;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteAsync()
        {
            IReadOnlyCollection<InternalOrder> internalOrders =
                await _internalOrderRepository.GetByStatusAsync(InternalOrderStatus.New);

            foreach (InternalOrder internalOrder in internalOrders)
            {
                try
                {
                    if (!await ValidateInstrumentAsync(internalOrder))
                        continue;

                    if (!await ValidateBalanceAsync(internalOrder))
                        continue;

                    if (!await ReserveFundsAsync(internalOrder))
                    {
                        if (internalOrder.Status == InternalOrderStatus.Failed)
                            await ReleaseReservedFundsAsync(internalOrder);

                        continue;
                    }

                    if (!await ExecuteAsync(internalOrder))
                    {
                        if (internalOrder.Status == InternalOrderStatus.Failed)
                            await ReleaseReservedFundsAsync(internalOrder);

                        continue;
                    }

                    if (!await TransferFundsAsync(internalOrder))
                        continue;

                    if (!await TransferRemainingFundsAsync(internalOrder))
                        continue;

                    await CompleteAsync(internalOrder);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "An unexpected error occurred while processing internal order",
                        internalOrder);
                }
            }
        }

        private async Task<bool> ValidateInstrumentAsync(InternalOrder internalOrder)
        {
            Instrument instrument = await _instrumentService.TryGetByAssetPairIdAsync(internalOrder.AssetPairId);

            string error = null;

            if (instrument == null)
                error = Errors.AssetPairNotSupported;
            else if (Math.Round(internalOrder.Volume, instrument.VolumeAccuracy) < instrument.MinVolume)
                error = Errors.TooSmallVolume;
            else if (Math.Abs(internalOrder.Volume) % 1 * (decimal) Math.Pow(10, instrument.VolumeAccuracy) % 1 != 0)
                error = Errors.InvalidVolume;

            if (!string.IsNullOrEmpty(error))
            {
                internalOrder.Status = InternalOrderStatus.Rejected;
                internalOrder.RejectReason = error;

                await _internalOrderRepository.UpdateAsync(internalOrder);

                return false;
            }

            return true;
        }

        private async Task<bool> ValidateBalanceAsync(InternalOrder internalOrder)
        {
            Instrument instrument = await _instrumentService.TryGetByAssetPairIdAsync(internalOrder.AssetPairId);

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            Asset asset;
            decimal amount;

            if (internalOrder.Type == LimitOrderType.Sell)
            {
                asset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.QuotingAssetId);
                amount = (internalOrder.Volume * internalOrder.Price).TruncateDecimalPlaces(asset.Accuracy, true);
            }
            else
            {
                asset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.BaseAssetId);
                amount = internalOrder.Volume;
            }

            Balance balance = await _balanceService.GetByAssetIdAsync(ExchangeNames.Lykke, asset.Id);

            if (balance.Amount < amount)
            {
                internalOrder.Status = InternalOrderStatus.Rejected;
                internalOrder.RejectReason = Errors.NotEnoughLiquidity;

                await _internalOrderRepository.UpdateAsync(internalOrder);

                return false;
            }

            return true;
        }

        private async Task<bool> ReserveFundsAsync(InternalOrder internalOrder)
        {
            Instrument instrument = await _instrumentService.TryGetByAssetPairIdAsync(internalOrder.AssetPairId);

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            string assetId;
            decimal amount;

            if (internalOrder.Type == LimitOrderType.Sell)
            {
                assetId = assetPair.BaseAssetId;
                amount = internalOrder.Volume;
            }
            else
            {
                Asset asset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.QuotingAssetId);

                assetId = assetPair.QuotingAssetId;
                amount = (internalOrder.Volume * internalOrder.Price).TruncateDecimalPlaces(asset.Accuracy, true);
            }

            _log.InfoWithDetails("Reserve funds for internal trade",
                new {OrderId = internalOrder.Id, Asset = assetId, Amount = amount});

            string error = null;

            string walletId = await _settingsService.GetWalletIdAsync();

            try
            {
                await _lykkeExchangeService.TransferAsync(internalOrder.WalletId, walletId, assetId, amount);
            }
            catch (NotEnoughFundsException)
            {
                error = Errors.NotEnoughFunds;
            }
            catch (Exception)
            {
                error = "An unexpected error occurred during reserving funds";
            }

            if (!string.IsNullOrEmpty(error))
            {
                internalOrder.Status = InternalOrderStatus.Rejected;
                internalOrder.RejectReason = error;

                await _internalOrderRepository.UpdateAsync(internalOrder);

                return false;
            }

            internalOrder.Status = InternalOrderStatus.Reserved;
            await _internalOrderRepository.UpdateAsync(internalOrder);

            return true;
        }

        private async Task<bool> ExecuteAsync(InternalOrder internalOrder)
        {
            ExternalTrade externalTrade = null;

            string error = null;

            try
            {
                externalTrade = await _externalExchangeService.ExecuteLimitOrderAsync(internalOrder.AssetPairId,
                    internalOrder.Volume, internalOrder.Price, internalOrder.Type);
            }
            catch (NotEnoughLiquidityException)
            {
                error = Errors.NotEnoughLiquidity;
            }
            catch (Exception)
            {
                error = "An unexpected error occurred while executing order";
            }

            if (!string.IsNullOrEmpty(error))
            {
                internalOrder.Status = InternalOrderStatus.Failed;
                internalOrder.RejectReason = error;

                await _internalOrderRepository.UpdateAsync(internalOrder);

                return false;
            }

            // ReSharper disable once PossibleNullReferenceException
            internalOrder.ExecutedPrice = externalTrade.Price;
            internalOrder.ExecutedVolume = externalTrade.Volume;
            internalOrder.TradeId = externalTrade.Id;
            internalOrder.Status = InternalOrderStatus.Executed;

            await _internalOrderRepository.UpdateAsync(internalOrder);

            await _positionService.CloseAsync(internalOrder, externalTrade);

            return true;
        }

        private async Task ReleaseReservedFundsAsync(InternalOrder internalOrder)
        {
            Instrument instrument = await _instrumentService.TryGetByAssetPairIdAsync(internalOrder.AssetPairId);

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            string assetId;
            decimal amount;

            if (internalOrder.Type == LimitOrderType.Sell)
            {
                assetId = assetPair.BaseAssetId;
                amount = internalOrder.Volume;
            }
            else
            {
                Asset asset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.QuotingAssetId);

                assetId = assetPair.QuotingAssetId;
                amount = (internalOrder.Volume * internalOrder.Price).TruncateDecimalPlaces(asset.Accuracy, true);
            }

            _log.InfoWithDetails("Releasing reserved funds for internal trade",
                new {OrderId = internalOrder.Id, Asset = assetId, Amount = amount});

            string walletId = await _settingsService.GetWalletIdAsync();

            try
            {
                await _lykkeExchangeService.TransferAsync(walletId, internalOrder.WalletId, assetId, amount);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "Can not transfer back reserved funds", internalOrder);
            }

            internalOrder.Status = InternalOrderStatus.Cancelled;
            await _internalOrderRepository.UpdateAsync(internalOrder);
        }

        private async Task<bool> TransferFundsAsync(InternalOrder internalOrder)
        {
            if (internalOrder.ExecutedVolume == null || internalOrder.ExecutedPrice == null)
                throw new InvalidOperationException("The executed volume and price not specified");

            Instrument instrument = await _instrumentService.TryGetByAssetPairIdAsync(internalOrder.AssetPairId);

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            string assetId;
            decimal amount;

            if (internalOrder.Type == LimitOrderType.Sell)
            {
                Asset asset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.QuotingAssetId);

                assetId = assetPair.QuotingAssetId;
                amount = (internalOrder.ExecutedVolume.Value * internalOrder.ExecutedPrice.Value)
                    .TruncateDecimalPlaces(asset.Accuracy, true);
            }
            else
            {
                assetId = assetPair.BaseAssetId;
                amount = internalOrder.ExecutedVolume.Value;
            }

            _log.InfoWithDetails("Transfer funds according internal trade",
                new {OrderId = internalOrder.Id, Asset = assetId, Amount = amount});

            string walletId = await _settingsService.GetWalletIdAsync();

            try
            {
                await _lykkeExchangeService.TransferAsync(walletId, internalOrder.WalletId, assetId, amount);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "Can not transfer funds to destination wallet", internalOrder);

                return false;
            }

            internalOrder.Status = InternalOrderStatus.Transferred;
            await _internalOrderRepository.UpdateAsync(internalOrder);

            return true;
        }

        private async Task<bool> TransferRemainingFundsAsync(InternalOrder internalOrder)
        {
            if (internalOrder.ExecutedVolume == null || internalOrder.ExecutedPrice == null)
                throw new InvalidOperationException("The executed volume and price not specified");

            if (internalOrder.Type != LimitOrderType.Buy)
                return true;

            Instrument instrument = await _instrumentService.TryGetByAssetPairIdAsync(internalOrder.AssetPairId);

            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(instrument.AssetPairId);

            Asset asset = await _assetsServiceWithCache.TryGetAssetAsync(assetPair.QuotingAssetId);

            decimal originalAmount = internalOrder.Volume * internalOrder.Price;

            decimal executedAmount = (internalOrder.ExecutedVolume.Value * internalOrder.ExecutedPrice.Value)
                .TruncateDecimalPlaces(asset.Accuracy, true);

            decimal amount = originalAmount - executedAmount;

            amount = Math.Round(amount, asset.Accuracy);

            if (amount > 0)
            {
                _log.InfoWithDetails("Transfer remaining funds according internal trade",
                    new {OrderId = internalOrder.Id, Asset = asset.Id, Amount = amount});

                string walletId = await _settingsService.GetWalletIdAsync();

                try
                {
                    await _lykkeExchangeService.TransferAsync(walletId, internalOrder.WalletId, asset.Id, amount);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "Can not transfer remaining funds to destination wallet",
                        internalOrder);

                    return false;
                }
            }

            return true;
        }

        private async Task CompleteAsync(InternalOrder internalOrder)
        {
            internalOrder.Status = InternalOrderStatus.Completed;

            await _internalOrderRepository.UpdateAsync(internalOrder);
        }
    }
}
