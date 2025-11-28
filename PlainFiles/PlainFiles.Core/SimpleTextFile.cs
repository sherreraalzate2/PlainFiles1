using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlainFiles.Core
{
    public static class SimpleTextFile
    {
        public static void SaveAllLines(string filePath, List<string> lines)
        {
            File.WriteAllLines(filePath, lines);
        }

        public static List<string> ReadAllLines(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<string>();
            }
            // Convierte el array de líneas a una Lista para manipularla más fácil
            return File.ReadAllLines(filePath).ToList();
        }
    }
}