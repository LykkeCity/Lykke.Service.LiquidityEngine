CREATE TABLE [dbo].[LEOrderBookUpdateReports](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	CONSTRAINT PK_LEOrderBookUpdateReports_Id PRIMARY KEY CLUSTERED (Id),
	[IterationId] [float] NOT NULL,
	[IterationDateTime] [datetime] NOT NULL,
	[AssetPair] [nvarchar](16) NOT NULL,
	[FirstQuoteAsk] [decimal](36, 18) NOT NULL,
	[FirstQuoteBid] [decimal](36, 18) NOT NULL,
	[SecondQuoteAsk] [decimal](36, 18) NOT NULL,
	[SecondQuoteBid] [decimal](36, 18) NOT NULL,
	[QuoteDateTime] [datetime] NOT NULL,
	[GlobalMarkup] [decimal](36, 18) NOT NULL,
	[NoFreshQuoteMarkup] [decimal](36, 18) NOT NULL,
	[PnLStopLossMarkup] [decimal](36, 18) NOT NULL,
	[FiatEquityMarkup] [decimal](36, 18) NOT NULL
)
GO

CREATE TABLE [dbo].[LEPlacedOrderReports](
	[Id] [uniqueidentifier] NOT NULL,
	CONSTRAINT [FK_LEPlacedOrderReports_LEOrderBookUpdateReports] FOREIGN KEY([OrderBookUpdateReportId])
	REFERENCES [dbo].[LEOrderBookUpdateReports] ([Id])
	ON UPDATE CASCADE
	ON DELETE CASCADE,
	[OrderBookUpdateReportId] [bigint] NOT NULL,
	[Type] [nvarchar](8) NOT NULL,
	[Price] [decimal](36, 18) NOT NULL,
	[Volume] [decimal](36, 18) NOT NULL,
	[LevelMarkup] [decimal](36, 18) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LEPlacedOrderReports] CHECK CONSTRAINT [FK_LEPlacedOrderReports_LEOrderBookUpdateReports]
GO