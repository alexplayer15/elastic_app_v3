FROM mcr.microsoft.com/mssql/server:2019-latest

USER root

COPY mssql-schemas/userDataSampleSchema.sql /docker-entrypoint-initdb.d/
COPY setupUserDatabase.sh /docker-entrypoint-initdb.d/
COPY entrypointsql.sh /docker-entrypoint-initdb.d/

RUN chmod +x /docker-entrypoint-initdb.d/entrypointsql.sh
RUN chmod +x /docker-entrypoint-initdb.d/setupUserDatabase.sh
RUN chmod +x /docker-entrypoint-initdb.d/userDataSampleSchema.sql

ENTRYPOINT ["/docker-entrypoint-initdb.d/entrypointsql.sh"]
