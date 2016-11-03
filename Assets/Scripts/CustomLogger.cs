using UnityEngine;
using System.Collections;
using System;

public class CustomLogHandler : ILogHandler {

	private static ILogger _loggerInstance = null;
	public static ILogger Logger
	{
		get {
			if (_loggerInstance == null) {
				_loggerInstance = new Logger(new CustomLogHandler());
			}
			return _loggerInstance;
		}
	}

	public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) {
		Debug.logger.logHandler.LogFormat(logType, context, format, args);
	}

	public void LogException(Exception exception, UnityEngine.Object context) {
		Debug.logger.LogException(exception, context);
	}
}

