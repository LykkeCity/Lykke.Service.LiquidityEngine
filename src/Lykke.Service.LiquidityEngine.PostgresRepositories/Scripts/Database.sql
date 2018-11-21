CREATE DATABASE liquidity_engine
    WITH 
    OWNER = lykkex
    ENCODING = 'UTF8'
    LC_COLLATE = 'English_United States.1252'
    LC_CTYPE = 'English_United States.1252'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

COMMENT ON DATABASE liquidity_engine
    IS 'The database used by Liquidity Engine

https://github.com/LykkeCity/Lykke.Service.LiquidityEngine';