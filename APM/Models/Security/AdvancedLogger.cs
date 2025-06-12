using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web; 
namespace APM.Models.Security
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }

    public interface ILogWriter
    {
        Task WriteAsync(LogEntry entry);
    }

    public class LogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public Exception Exception { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }

    // پیاده‌سازی برای ذخیره در فایل
    public class FileLogWriter : ILogWriter
    {
        private readonly string _filePath;

        public FileLogWriter(string filePath = ("logs/log.txt"))
        { 
            _filePath = filePath;
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
        }
         
        public async Task WriteAsync(LogEntry entry)
        {
            var logMessage = $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.Level}] {entry.Category} - {entry.Message}";

            if (entry.Exception != null)
            {
                logMessage += $"\nException: {entry.Exception}\n";
            }

            using (var fileStream = new FileStream(
                _filePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true))
            {
                var bytes = Encoding.UTF8.GetBytes(logMessage + Environment.NewLine);
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }

    // پیاده‌سازی برای ذخیره در دیتابیس
    public class DatabaseLogWriter : ILogWriter
    {
        private readonly string _connectionString;

        public DatabaseLogWriter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task WriteAsync(LogEntry entry)
        {
           //await Referral.DBRegistry.InsertAsync("Logs_APMRegistry"
           //                                        , new string[] { "RegistryDate", "RegistryTime", "LevelInfo", "Category", "Message", "Exception", "AdditionalData" }
           //                                        , new object[] { entry.Timestamp, entry.Timestamp, entry.Level.ToString(), entry.Category, entry.Message, entry.Exception?.ToString(), SerializeAdditionalData(entry.AdditionalData) });
      
        }

        private static string SerializeAdditionalData(Dictionary<string, object> data)
        {
            // پیاده‌سازی سریالایزیشن بر اساس نیاز شما
            return System.Text.Json.JsonSerializer.Serialize(data);
        }
    }
     
    public static class AdvancedLogger
    {
        private static readonly List<ILogWriter> _writers = new List<ILogWriter>();
        private static LogLevel _minimumLevel = LogLevel.Information;

        public static void Configure(Action<LoggingConfiguration> config)
        {
            var configuration = new LoggingConfiguration();
            config(configuration);

            _writers.Clear();
            _writers.AddRange(configuration.Writers);
            _minimumLevel = configuration.MinimumLevel;
        }

        public static async Task LogAsync(
            LogLevel level,
            string category,
            string message,
            Exception exception = null,
            Dictionary<string, object> additionalData = null)
        {
            if (level < _minimumLevel) return;

            var entry = new LogEntry
            {
                Timestamp = DateTimeOffset.UtcNow,
                Level = level,
                Category = category,
                Message = message,
                Exception = exception,
                AdditionalData = additionalData ?? new Dictionary<string, object>()
            };

            foreach (var writer in _writers)
            {
                try
                {
                    await writer.WriteAsync(entry);
                }
                catch (Exception ex)
                { 
                    Console.WriteLine($"Failed to write log: {ex.Message}");
                }
            }
        }
         
        public static Task LogInformationAsync(this string category, string message)
            => LogAsync(LogLevel.Information, category, message);

        public static Task LogErrorAsync(this string category, string message, Exception ex)
            => LogAsync(LogLevel.Error, category, message, ex);
        public static Task LogErrorAsync(this string category, string message)
            => LogAsync(LogLevel.Error, category, message);
    }
     
    public class LoggingConfiguration
    {
        public List<ILogWriter> Writers { get; } = new List<ILogWriter>();
        public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

        public LoggingConfiguration AddFileWriter(string filePath = "logs/log.txt")
        {
            Writers.Add(new FileLogWriter(filePath));
            return this;
        }

        public LoggingConfiguration AddDatabaseWriter(string connectionString)
        {
            Writers.Add(new DatabaseLogWriter(connectionString));
            return this;
        }

        public LoggingConfiguration SetMinimumLevel(LogLevel level)
        {
            MinimumLevel = level;
            return this;
        }
    } 
}