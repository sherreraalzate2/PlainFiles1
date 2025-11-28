using System;
using System.IO;

namespace PlainFiles.Core
{
    public static class LogWriter
    {
        private static string logPath = "log.txt";

        public static void Log(string message, string currentUser)
        {
            // Escribir: Fecha - Usuario - Mensaje
            string line = $"{DateTime.Now} - Usuario: [{currentUser}] - {message}";

            // AppendAllText es nativo de .NET, no es librería externa
            File.AppendAllText(logPath, line + Environment.NewLine);
        }
    }
}
