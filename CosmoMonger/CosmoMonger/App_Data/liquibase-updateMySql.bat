@echo off
echo Output redirected to liquibase.log
liquibase.bat --defaultsFile=liquibase-mysql.properties update > liquibase.log