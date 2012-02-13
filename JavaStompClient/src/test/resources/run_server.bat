call classpath.bat
SET JAVA_OPTS=-Xdebug -Xrunjdwp:transport=dt_socket,address=8000,server=y,suspend=n
java %JAVA_OPTS% -classpath %CLASSPATH% com.carestreamhealth.pas.notification.SimpleServerTest