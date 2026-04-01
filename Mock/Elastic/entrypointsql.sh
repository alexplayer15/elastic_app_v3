#!/bin/bash

# Start SQL server, start creating database and tables
/opt/mssql/bin/sqlservr | /docker-entrypoint-initdb.d/setupUserDatabase.sh
