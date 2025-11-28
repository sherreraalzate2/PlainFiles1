using System;

namespace PlainFiles.Core
{
    public static class ManualCsvHelper
    {
        // --- Convertir Línea de texto a Objeto USER ---
        public static User LineToUser(string line)
        {
            // Formato esperado: usuario,password,activo
            var parts = line.Split(',');
            if (parts.Length < 3) return null;

            return new User
            {
                Username = parts[0],
                Password = parts[1],
                IsActive = bool.Parse(parts[2])
            };
        }

        // --- Convertir Objeto USER a Línea de texto ---
        public static string UserToLine(User u)
        {
            return $"{u.Username},{u.Password},{u.IsActive}";
        }


        // --- Convertir Línea de texto a Objeto PERSON ---
        public static Person LineToPerson(string line)
        {
            // Formato esperado: Id;Nombre;Apellido;Tel;Ciudad;Saldo
            var parts = line.Split(';');
            if (parts.Length < 6) return null;

            return new Person
            {
                Id = int.Parse(parts[0]),
                FirstName = parts[1],
                LastName = parts[2],
                Phone = parts[3],
                City = parts[4],
                Balance = decimal.Parse(parts[5])
            };
        }

        // --- Convertir Objeto PERSON a Línea de texto ---
        public static string PersonToLine(Person p)
        {
            return $"{p.Id};{p.FirstName};{p.LastName};{p.Phone};{p.City};{p.Balance}";
        }
    }
}