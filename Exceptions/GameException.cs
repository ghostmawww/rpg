using System;

namespace ConsoleApp46
{
    /// <summary>
    /// Собственный класс исключений для игры
    /// </summary>
    public class GameException : Exception
    {
        private string _errorCode;
        private DateTime _errorTime;
        private ErrorSeverity _severity;
        private string _component;

        /// <summary>
        /// Уникальный код ошибки
        /// </summary>
        public string ErrorCode
        {
            get => _errorCode;
            set => _errorCode = value;
        }

        /// <summary>
        /// Время возникновения ошибки
        /// </summary>
        public DateTime ErrorTime
        {
            get => _errorTime;
            set => _errorTime = value;
        }

        /// <summary>
        /// Уровень критичности ошибки
        /// </summary>
        public ErrorSeverity Severity
        {
            get => _severity;
            set => _severity = value;
        }

        /// <summary>
        /// Название компонента, в котором произошла ошибка
        /// </summary>
        public string Component
        {
            get => _component;
            set => _component = value;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public GameException() : base()
        {
            _errorTime = DateTime.Now;
            _severity = ErrorSeverity.Medium;
            _component = "Unknown";
        }

        /// <summary>
        /// Конструктор с сообщением об ошибке
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public GameException(string message) : base(message)
        {
            _errorTime = DateTime.Now;
            _severity = ErrorSeverity.Medium;
            _component = "Unknown";
        }

        /// <summary>
        /// Конструктор с сообщением и кодом ошибки
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="errorCode">Код ошибки</param>
        public GameException(string message, string errorCode) : base(message)
        {
            _errorCode = errorCode;
            _errorTime = DateTime.Now;
            _severity = ErrorSeverity.Medium;
            _component = "Unknown";
        }

        /// <summary>
        /// Конструктор с сообщением, кодом ошибки и компонентом
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="errorCode">Код ошибки</param>
        /// <param name="component">Компонент, вызвавший ошибку</param>
        public GameException(string message, string errorCode, string component) : base(message)
        {
            _errorCode = errorCode;
            _errorTime = DateTime.Now;
            _severity = ErrorSeverity.Medium;
            _component = component;
        }

        /// <summary>
        /// Конструктор с сообщением, кодом ошибки, компонентом и уровнем критичности
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="errorCode">Код ошибки</param>
        /// <param name="component">Компонент, вызвавший ошибку</param>
        /// <param name="severity">Уровень критичности</param>
        public GameException(string message, string errorCode, string component, ErrorSeverity severity) : base(message)
        {
            _errorCode = errorCode;
            _errorTime = DateTime.Now;
            _severity = severity;
            _component = component;
        }

        /// <summary>
        /// Конструктор с сообщением, кодом ошибки, компонентом и внутренним исключением
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="errorCode">Код ошибки</param>
        /// <param name="component">Компонент, вызвавший ошибку</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public GameException(string message, string errorCode, string component, Exception innerException)
            : base(message, innerException)
        {
            _errorCode = errorCode;
            _errorTime = DateTime.Now;
            _severity = ErrorSeverity.High;
            _component = component;
        }

        /// <summary>
        /// Конструктор с сообщением, кодом ошибки, компонентом, уровнем критичности и внутренним исключением
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="errorCode">Код ошибки</param>
        /// <param name="component">Компонент, вызвавший ошибку</param>
        /// <param name="severity">Уровень критичности</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public GameException(string message, string errorCode, string component, ErrorSeverity severity, Exception innerException)
            : base(message, innerException)
        {
            _errorCode = errorCode;
            _errorTime = DateTime.Now;
            _severity = severity;
            _component = component;
        }

        /// <summary>
        /// Конструктор с сообщением и внутренним исключением
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public GameException(string message, Exception innerException) : base(message, innerException)
        {
            _errorTime = DateTime.Now;
            _severity = ErrorSeverity.High;
            _component = "Unknown";
        }

        /// <summary>
        /// Возвращает форматированное сообщение об ошибке
        /// </summary>
        /// <returns>Строка с подробной информацией об ошибке</returns>
        public override string ToString()
        {
            string severityText;
            switch (_severity)
            {
                case ErrorSeverity.Low:
                    severityText = "Низкая";
                    break;
                case ErrorSeverity.Medium:
                    severityText = "Средняя";
                    break;
                case ErrorSeverity.High:
                    severityText = "Высокая";
                    break;
                case ErrorSeverity.Critical:
                    severityText = "Критическая";
                    break;
                default:
                    severityText = "Неизвестно";
                    break;
            }

            return $"[{_errorTime:yyyy-MM-dd HH:mm:ss}] Компонент: {_component ?? "N/A"} | Код: {_errorCode ?? "N/A"} | Уровень: {severityText}\nСообщение: {Message}";
        }

        /// <summary>
        /// Возвращает краткое сообщение об ошибке
        /// </summary>
        /// <returns>Строка в формате [код] сообщение</returns>
        public string GetShortMessage()
        {
            return $"[{_errorCode}] {Message}";
        }
    }

    /// <summary>
    /// Уровни критичности ошибок
    /// </summary>
    public enum ErrorSeverity
    {
        /// <summary>Низкая - ошибка, которую можно проигнорировать</summary>
        Low,
        /// <summary>Средняя - ошибка, требующая внимания</summary>
        Medium,
        /// <summary>Высокая - ошибка, которая может нарушить работу</summary>
        High,
        /// <summary>Критическая - аварийное завершение работы</summary>
        Critical
    }
}