CREATE USER service_user WITH PASSWORD 'service_password';
CREATE USER auth_user WITH PASSWORD 'auth_password';

CREATE DATABASE platform_business_db;
CREATE DATABASE platform_search_db;
CREATE DATABASE platform_orders_db;
CREATE DATABASE platform_notify_db;
CREATE DATABASE platform_analytics_db;
CREATE DATABASE auth_service_db;
CREATE DATABASE auth_business_db;

GRANT ALL PRIVILEGES ON DATABASE platform_business_db TO service_user;
GRANT ALL PRIVILEGES ON DATABASE platform_search_db TO service_user;
GRANT ALL PRIVILEGES ON DATABASE platform_orders_db TO service_user;
GRANT ALL PRIVILEGES ON DATABASE platform_notify_db TO service_user;
GRANT ALL PRIVILEGES ON DATABASE platform_analytics_db TO service_user;
GRANT ALL PRIVILEGES ON DATABASE auth_service_db TO auth_user;
GRANT ALL PRIVILEGES ON DATABASE auth_business_db TO auth_user;
