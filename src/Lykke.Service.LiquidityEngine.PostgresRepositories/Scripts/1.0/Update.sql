DO $$
BEGIN
	IF NOT EXISTS(SELECT * FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'db_version') THEN
		ALTER TABLE positions 
			ALTER COLUMN price TYPE numeric(19,10),
			ALTER COLUMN volume TYPE numeric(19,10),
			ALTER COLUMN close_price TYPE numeric(19,10),
			ALTER COLUMN pnl TYPE numeric(19,10),
			ALTER COLUMN cross_ask TYPE numeric(19,10) ,
			ALTER COLUMN cross_bid TYPE numeric(19,10),
			ALTER COLUMN trade_avg_price TYPE numeric(19,10);

		ALTER TABLE internal_trades 
			ALTER COLUMN price TYPE numeric(19,10),
			ALTER COLUMN volume TYPE numeric(19,10),
			ALTER COLUMN remaining_volume TYPE numeric(19,10),
			ALTER COLUMN opposite_volume TYPE numeric(19,10);

		ALTER TABLE internal_trades 
			ALTER COLUMN price TYPE numeric(19,10),
			ALTER COLUMN volume TYPE numeric(19,10);

		CREATE TABLE db_version (
			major int NOT NULL,
			minor int NOT NULL,
			"timestamp" timestamp with time zone NOT NULL
		);

		ALTER TABLE ONLY db_version
			ADD CONSTRAINT pk_db_version PRIMARY KEY (major, minor);
		
		INSERT INTO db_version (major, minor, "timestamp")
			VALUES (1, 0, NOW());
	END IF;
END$$;