CREATE TABLE positions (
    id uuid NOT NULL,
    asset_pair_id varchar(12) NOT NULL,
    type smallint NOT NULL,
    "timestamp" timestamp with time zone NOT NULL,
    price decimal NOT NULL,
    volume decimal NOT NULL,
    close_date timestamp with time zone NOT NULL,
    close_price decimal NOT NULL,
    pnl decimal NOT NULL,
    cross_asset_pair_id varchar(12) NULL,
    cross_ask decimal NULL,
    cross_bid decimal NULL,
    trade_asset_pair_id varchar(12) NOT NULL,
    trade_avg_price decimal NOT NULL,
    internal_trade_id uuid NULL,
    external_trade_id uuid NULL
);

CREATE TABLE internal_trades (
    id uuid NOT NULL,
    limit_order_id uuid NOT NULL,
    exchange_order_id uuid NOT NULL,
    asset_pair_id varchar(12) NOT NULL,
    type smallint NOT NULL,
    "timestamp" timestamp with time zone NOT NULL,
    price decimal NOT NULL,
    volume decimal NOT NULL,
    remaining_volume decimal NOT NULL,
    status smallint NOT NULL,
    opposite_volume decimal NOT NULL,
    opposite_client_id uuid NOT NULL,
    opposite_limit_order_id uuid NOT NULL
);

CREATE TABLE external_trades (
    id uuid NOT NULL,
    limit_order_id uuid NOT NULL,
    asset_pair_id varchar(12) NOT NULL,
    type smallint NOT NULL,
    "timestamp" timestamp with time zone NOT NULL,
    price decimal NOT NULL,
    volume decimal NOT NULL,
    request_id uuid NOT NULL
);



ALTER TABLE ONLY positions
    ADD CONSTRAINT pk_positions PRIMARY KEY (id);
    
ALTER TABLE ONLY internal_trades
    ADD CONSTRAINT pk_internal_trades PRIMARY KEY (id);

ALTER TABLE ONLY external_trades
    ADD CONSTRAINT pk_external_trades PRIMARY KEY (id);



ALTER TABLE positions
    ADD CONSTRAINT fk_positions_internal_trades FOREIGN KEY (internal_trade_id)
    REFERENCES internal_trades (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
    
ALTER TABLE positions
    ADD CONSTRAINT fk_positions_external_trades FOREIGN KEY (external_trade_id)
    REFERENCES external_trades (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;



CREATE INDEX ix_internal_trades_asset_pair_id
    ON internal_trades USING btree (asset_pair_id ASC);

CREATE INDEX ix_internal_trades_timestamp
    ON internal_trades USING btree ("timestamp" DESC);

CREATE INDEX ix_internal_trades_opposite_client_id
    ON internal_trades USING btree ("opposite_client_id" ASC);