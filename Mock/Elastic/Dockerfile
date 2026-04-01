FROM mcr.microsoft.com/mssql/server:2019-latest

USER root

COPY mssql-schemas/elasticDatabaseSetup.sql /docker-entrypoint-initdb.d/
COPY setupUserDatabase.sh /docker-entrypoint-initdb.d/
COPY entrypointsql.sh /docker-entrypoint-initdb.d/

RUN chmod +x /docker-entrypoint-initdb.d/entrypointsql.sh
RUN chmod +x /docker-entrypoint-initdb.d/setupUserDatabase.sh
RUN chmod +x /docker-entrypoint-initdb.d/elasticDatabaseSetup.sql

ENTRYPOINT ["/docker-entrypoint-initdb.d/entrypointsql.sh"]
