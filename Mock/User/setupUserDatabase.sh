#!/bin/bash

# Wait until SQL server is running and then start creating Database and tables
echo "Creating Database and Tables"

for i in {1..50};
do
echo $SQLHOST

if [ -s /run/secrets/MSSQL_SA_PASSWORD ]; then
  PASSWORD=$(cat /run/secrets/MSSQL_SA_PASSWORD)
  echo "Using password from secret file."
else
  PASSWORD=$MSSQL_SA_PASSWORD
  echo "Using password from environment variable."
fi

/opt/mssql-tools18/bin/sqlcmd -C -U sa -P "$PASSWORD" -S "$SQLHOST" -i /docker-entrypoint-initdb.d/userDataSampleSchema.sql

    if [ $? -eq 0 ]
    then
        echo "Successfully created Database and Tables."
        break
    else
        echo "..."
        sleep 1
    fi

done
