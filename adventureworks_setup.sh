#!/bin/bash

download_error=$(mktemp)
backup_path="/backup/AdventureWorks2022.bak"

if ! test -f "$backup_path"; then
    echo "[*] Downloading AdventureWorks2022 backup file"
    wget \
      https://github.com/Microsoft/sql-server-samples/releases/download/adventureworks/AdventureWorks2022.bak \
      -O $backup_path >/dev/null 2>"$download_error"
    
    if [ "$?" -ne 0 ]; then
      echo "[X] An error has occured while downloading backup file"
      cat $download_error
      exit 1
    fi
    
    chown -R mssql:mssql /backup
    echo -e "[!] AdventureWorks2022.bak has been downloaded successfully:\n$(ls -l $backup_path)\n"
else
  echo "[*] $backup_path found, omitting backup download"
fi

setup_sql=$(cat << EOF
  RESTORE DATABASE AdventureWorks2022
  FROM DISK = '$backup_path'
  WITH
    MOVE 'AdventureWorks2022' TO '/var/opt/mssql/data/AdventureWorks2022.mdf',
    MOVE 'AdventureWorks2022_log' TO '/var/opt/mssql/data/AdventureWorks2022_log.ldf',
    REPLACE;
  GO
  
  USE AdventureWorks2022
  GO
  
  IF NOT EXISTS (
      SELECT 1
      FROM sys.server_principals
      WHERE name = '$DB_SETUP_LOGIN'
  )
  BEGIN
      CREATE LOGIN submission
      WITH PASSWORD = '$DB_SETUP_PASSWORD';
      
      CREATE USER $DB_SETUP_USER
      FOR LOGIN $DB_SETUP_LOGIN;
          
      ALTER ROLE db_datareader
      ADD MEMBER $DB_SETUP_USER;
  END
  GO
    
EOF
)
  
/opt/mssql-tools18/bin/sqlcmd \
  -S "${DB_HOST}" \
  -U sa \
  -P "${DB_SA_PASSWORD}" \
  -C \
  -Q "${setup_sql}"
