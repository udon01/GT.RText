using System;
using System.IO;

namespace GT.Shared.Logging
{
    public class FileWriter : ILogWriter, IDisposable
    {
        private static StreamWriter sw;

        public FileWriter(string fileName = "log.txt", bool append = false)
        {
            sw = new StreamWriter(fileName, append)
            {
                AutoFlush = true
            };
        }

        public void WriteLine(string message)
        {
            sw?.WriteLine(message);
        }

        public void WriteLine(string message, params object[] parameters)
        {
            sw?.WriteLine(message, parameters);
        }

        public void WriteLine(Exception exception)
        {
            sw?.WriteLine(exception);
        }

        public void Dispose()
        {
            sw?.Dispose();
        }
    }
}
