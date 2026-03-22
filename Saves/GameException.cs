using System;

namespace ConsoleApp46
{
    /// <summary>
    /// Собственный класс исключений для игры
    /// </summary>
    public class GameException : Exception
    {
        public string ErrorCode { get; set; }
        public DateTime ErrorTime { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string Component { get; set; }

        public GameException() : base()
        {
            ErrorTime = DateTime.Now;
            Severity = ErrorSeverity.Medium;
            Component = "Unknown";
        }

        public GameException(string message) : base(message)
        {
            ErrorTime = DateTime.Now;
            Severity = ErrorSeverity.Medium;
            Component = "Unknown";
        }

        public GameException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
            ErrorTime = DateTime.Now;
            Severity = ErrorSeverity.Medium;
            Component = "Unknown";
        }

        public GameException(string message, string errorCode, string component) : base(message)
        {
            ErrorCode = errorCode;
            ErrorTime = DateTime.Now;
            Severity = ErrorSeverity.Medium;
            Component = component;
        }

        public GameException(string message, string errorCode, string component, ErrorSeverity severity) : base(message)
        {
            ErrorCode = errorCode;
            ErrorTime = DateTime.Now;
            Severity = severity;
            Component = component;
        }

        // Конструктор с внутренним исключением (без severity)
        public GameException(string message, string errorCode, string component, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorTime = DateTime.Now;
            Severity = ErrorSeverity.High;
            Component = component;
        }

        // Конструктор с внутренним исключением и severity
        public GameException(string message, string errorCode, string component, ErrorSeverity severity, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorTime = DateTime.Now;
            Severity = severity;
            Component = component;
        }

        public GameException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorTime = DateTime.Now;
            Severity = ErrorSeverity.High;
            Component = "Unknown";
        }

        public override string ToString()
        {
            string severityStr;
            switch (Severity)
            {
                case ErrorSeverity.Low:
                    severityStr = "Низкая";
                    break;
                case ErrorSeverity.Medium:
                    severityStr = "Средняя";
                    break;
                case ErrorSeverity.High:
                    severityStr = "Высокая";
                    break;
                case ErrorSeverity.Critical:
                    severityStr = "Критическая";
                    break;
                default:
                    severityStr = "Неизвестно";
                    break;
            }

            return $"[{ErrorTime:yyyy-MM-dd HH:mm:ss}] Компонент: {Component} | Код: {ErrorCode ?? "N/A"} | Уровень: {severityStr}\nСообщение: {Message}";
        }

        public string GetShortMessage()
        {
            return $"[{ErrorCode}] {Message}";
        }
    }

    public enum ErrorSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
}