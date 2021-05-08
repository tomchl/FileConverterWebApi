using System;

namespace FileConverterWebApi.Utils
{
    public class Logger : ILogger
    {
        public Logger() { }

        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now}: {message}");
        }
    }

    public interface ILogger
    {
        public void Log(string message);
    }
}