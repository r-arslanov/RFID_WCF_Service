@echo off
REM Проверить путь до исполняемого файла
set PROG=".\RFIDWCFService\RFIDWCFService\bin\RFIDWCFService.exe"
REM Проверка версии .NET
IF EXIST %SystemRoot%"\Microsoft.NET\Framework64\v4.0.30319" (
	echo -------------------------------------------
	echo ------------- .NET v4.0.30319 -------------
	SET INSTALLUTIL=%SystemRoot%"\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe" 
	GOTO install 
	)
IF EXIST %SystemRoot%"\Microsoft.NET\Framework64\v2.0.50727" (
	echo -------------------------------------------
	echo ------------- .NET v2.0.50727 -------------
	SET INSTALLUTIL=%SystemRoot%"\Microsoft.NET\Framework64\v2.0.50727\InstallUtil.exe" 
	GOTO install
	)

:install
	echo ------------- Start installing ------------
	echo -------------------------------------------
	%INSTALLUTIL% %PROG%
	echo -------------------------------------------
	echo ----------- Service installed -------------
	echo -------------------------------------------