using System;
using System.Diagnostics;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Утилитарный класс для логирования с дополнительной информацией
    /// </summary>
    public static class LoggingUtils
    {
        /// <summary>
        /// Уровни логирования
        /// </summary>
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Exception
        }

        /// <summary>
        /// Логирование с указанием уровня
        /// </summary>
        /// <param name="level">Уровень логирования</param>
        /// <param name="message">Сообщение</param>
        /// <param name="context">Контекст (объект)</param>
        public static void Log(LogLevel level, string message, UnityEngine.Object context = null)
        {
            string formattedMessage = FormatLogMessage(level, message);

            switch (level)
            {
                case LogLevel.Debug:
                    UnityEngine.Debug.Log(formattedMessage, context);
                    break;
                case LogLevel.Info:
                    UnityEngine.Debug.Log(formattedMessage, context);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(formattedMessage, context);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(formattedMessage, context);
                    break;
                case LogLevel.Exception:
                    UnityEngine.Debug.LogError(formattedMessage, context);
                    break;
            }
        }

        /// <summary>
        /// Логирование информационного сообщения
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="context">Контекст (объект)</param>
        public static void LogInfo(string message, UnityEngine.Object context = null)
        {
            Log(LogLevel.Info, message, context);
        }

        /// <summary>
        /// Логирование предупреждения
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="context">Контекст (объект)</param>
        public static void LogWarning(string message, UnityEngine.Object context = null)
        {
            Log(LogLevel.Warning, message, context);
        }

        /// <summary>
        /// Логирование ошибки
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="context">Контекст (объект)</param>
        public static void LogError(string message, UnityEngine.Object context = null)
        {
            Log(LogLevel.Error, message, context);
        }

        /// <summary>
        /// Логирование исключения
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="additionalInfo">Дополнительная информация</param>
        /// <param name="context">Контекст (объект)</param>
        public static void LogException(Exception ex, string additionalInfo = null, UnityEngine.Object context = null)
        {
            string message = string.IsNullOrEmpty(additionalInfo) 
                ? $"Exception: {ex.Message}\nStackTrace: {ex.StackTrace}" 
                : $"{additionalInfo}\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";

            Log(LogLevel.Exception, message, context);
        }

        /// <summary>
        /// Форматирует сообщение для логирования
        /// </summary>
        /// <param name="level">Уровень логирования</param>
        /// <param name="message">Сообщение</param>
        /// <returns>Отформатированное сообщение</returns>
        private static string FormatLogMessage(LogLevel level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string callerInfo = GetCallerInfo();

            return $"[{timestamp}] [{level}] {callerInfo} - {message}";
        }

        /// <summary>
        /// Получает информацию о вызывающем методе
        /// </summary>
        /// <returns>Строка с информацией о вызывающем методе</returns>
        private static string GetCallerInfo()
        {
            StackTrace stackTrace = new StackTrace(true);

            // Пропускаем фреймы логгера и берем первый пользовательский фрейм
            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                string className = frame.GetMethod()?.DeclaringType?.Name;

                // Пропускаем фреймы внутри LoggingUtils
                if (className != nameof(LoggingUtils))
                {
                    string methodName = frame.GetMethod()?.Name;
                    return $"{className}.{methodName}";
                }
            }

            return "<unknown>";
        }
    }
}
