-- Initialize Firmeza Database
-- This script runs automatically when the PostgreSQL container starts for the first time

-- The database is already created by POSTGRES_DB environment variable
-- This script can be used for additional initialization if needed

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE "FirmezaDB" TO postgres;

-- Log initialization
DO $$
BEGIN
    RAISE NOTICE 'Firmeza database initialized successfully';
END $$;
