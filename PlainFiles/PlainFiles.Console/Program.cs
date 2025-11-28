using PlainFiles.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlainFiles.ConsoleApp
{
    class Program
    {

        static List<User> users = new List<User>();
        static List<Person> persons = new List<Person>();

        static string usersFile = "Users.txt";
        static string personsFile = "Persons.txt";
        static string currentUser = "";

        static void Main(string[] args)
        {
            LoadData(); 


            if (!DoLogin())
            {
                return; 
            }


            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine($"=== SISTEMA CONTABLE (Usuario: {currentUser}) ===");
                Console.WriteLine("1. Mostrar informe (Subtotales por Ciudad)");
                Console.WriteLine("2. Agregar persona");
                Console.WriteLine("3. Editar persona");
                Console.WriteLine("4. Borrar persona");
                Console.WriteLine("5. Guardar cambios");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");

                string op = Console.ReadLine();
                switch (op)
                {
                    case "1": ShowReport_ControlBreak(); break; 
                    case "2": AddPerson(); break;
                    case "3": EditPerson(); break;
                    case "4": DeletePerson(); break;
                    case "5":
                        SaveChanges();
                        Console.WriteLine("¡Datos guardados correctamente!");
                        Console.ReadKey();
                        break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
        }
        static void LoadData()
        {
            var userLines = SimpleTextFile.ReadAllLines(usersFile);
            if (userLines.Count == 0)
            {
                users.Add(new User { Username = "jzuluaga", Password = "P@ssw0rd123!", IsActive = true });
                users.Add(new User { Username = "mbedoya", Password = "S0yS3gur02025*", IsActive = false });
                SimpleTextFile.SaveAllLines(usersFile, users.Select(ManualCsvHelper.UserToLine).ToList());
            }
            else
            {
                foreach (var line in userLines)
                {
                    var u = ManualCsvHelper.LineToUser(line);
                    if (u != null) users.Add(u);
                }
            }

            var personLines = SimpleTextFile.ReadAllLines(personsFile);
            foreach (var line in personLines)
            {
                var p = ManualCsvHelper.LineToPerson(line);
                if (p != null) persons.Add(p);
            }
        }

        static bool DoLogin()
        {
            int attempts = 0;
            while (attempts < 3)
            {
                Console.Clear();
                Console.WriteLine("LOGIN REQUERIDO");
                Console.Write("Usuario: ");
                string uName = Console.ReadLine();
                Console.Write("Contraseña: ");
                string uPass = Console.ReadLine();

                var userObj = users.FirstOrDefault(u => u.Username == uName);

                if (userObj == null)
                {
                    Console.WriteLine("El usuario no existe.");
                }
                else if (!userObj.IsActive)
                {
                    Console.WriteLine("USUARIO BLOQUEADO. Contacte al administrador.");
                    Console.ReadKey();
                    return false;
                }
                else if (userObj.Password == uPass)
                {
                    currentUser = userObj.Username;
                    LogWriter.Log("Inicio de sesión exitoso", currentUser);
                    return true;
                }
                else
                {
                    attempts++;
                    Console.WriteLine("Contraseña incorrecta.");
                }

                if (attempts < 3)
                {
                    Console.WriteLine($"Intentos restantes: {3 - attempts}");
                    Console.ReadKey();
                }

                if (attempts >= 3 && userObj != null)
                {
                    userObj.IsActive = false;
                    SimpleTextFile.SaveAllLines(usersFile, users.Select(ManualCsvHelper.UserToLine).ToList());
                    LogWriter.Log($"Usuario {uName} bloqueado por intentos fallidos", "SYSTEM");
                    Console.WriteLine("HAS SIDO BLOQUEADO POR SEGURIDAD.");
                    Console.ReadKey();
                }
            }
            return false;
        }

        static void ShowReport_ControlBreak()
        {
            Console.Clear();
            Console.WriteLine("INFORME DE SALDOS POR CIUDAD\n");

            if (persons.Count == 0)
            {
                Console.WriteLine("No hay datos para mostrar.");
                Console.ReadKey();
                return;
            }

            var sortedList = persons.OrderBy(p => p.City).ToList();

            string currentCity = sortedList[0].City;
            decimal cityTotal = 0;
            decimal grandTotal = 0;

            Console.WriteLine($"Ciudad: {currentCity}");
            Console.WriteLine("ID\tNombres\t\tApellidos\tSaldo");
            Console.WriteLine("--\t-------\t\t---------\t-----");

            foreach (var p in sortedList)
            {
                if (p.City != currentCity)
                {
                    Console.WriteLine("\t\t\t\t\t========");
                    Console.WriteLine($"Total: {currentCity}\t\t\t\t{cityTotal:N2}\n");

                    currentCity = p.City;
                    cityTotal = 0;

                    Console.WriteLine($"Ciudad: {currentCity}");
                    Console.WriteLine("ID\tNombres\t\tApellidos\tSaldo");
                    Console.WriteLine("--\t-------\t\t---------\t-----");
                }

                Console.WriteLine($"{p.Id}\t{p.FirstName}\t\t{p.LastName}\t\t{p.Balance:N2}");
                cityTotal += p.Balance;
                grandTotal += p.Balance;
            }

            Console.WriteLine("\t\t\t\t\t========");
            Console.WriteLine($"Total: {currentCity}\t\t\t\t{cityTotal:N2}\n");
            Console.WriteLine("\t\t\t\t\t========");
            Console.WriteLine($"Total General:\t\t\t\t{grandTotal:N2}");

            LogWriter.Log("Generó reporte de saldos", currentUser);
            Console.ReadKey();
        }

        static void AddPerson()
        {
            Console.WriteLine("\n--- Agregar Persona ---");
            int id;
            while (true)
            {
                Console.Write("ID: ");
                if (int.TryParse(Console.ReadLine(), out id) && !persons.Any(p => p.Id == id)) break;
                Console.WriteLine("ID inválido o repetido.");
            }

            Console.Write("Nombres: "); string nom = Console.ReadLine();
            Console.Write("Apellidos: "); string ape = Console.ReadLine();
            Console.Write("Teléfono: "); string tel = Console.ReadLine();
            Console.Write("Ciudad: "); string ciu = Console.ReadLine();

            decimal saldo;
            while (true)
            {
                Console.Write("Saldo: ");
                if (decimal.TryParse(Console.ReadLine(), out saldo) && saldo >= 0) break;
                Console.WriteLine("Saldo inválido.");
            }

            persons.Add(new Person { Id = id, FirstName = nom, LastName = ape, Phone = tel, City = ciu, Balance = saldo });
            LogWriter.Log($"Agregó persona {id}", currentUser);
        }

        static void EditPerson()
        {
            Console.Write("ID a editar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var p = persons.FirstOrDefault(x => x.Id == id);
                if (p != null)
                {
                    Console.WriteLine("(Enter para mantener valor actual)");
                    Console.Write($"Nombre [{p.FirstName}]: ");
                    string s = Console.ReadLine();
                    if (!string.IsNullOrEmpty(s)) p.FirstName = s;

                    Console.Write($"Apellido [{p.LastName}]: ");
                    s = Console.ReadLine();
                    if (!string.IsNullOrEmpty(s)) p.LastName = s;

                    Console.Write($"Ciudad [{p.City}]: ");
                    s = Console.ReadLine();
                    if (!string.IsNullOrEmpty(s)) p.City = s;

                    Console.Write($"Saldo [{p.Balance}]: ");
                    s = Console.ReadLine();
                    if (!string.IsNullOrEmpty(s) && decimal.TryParse(s, out decimal val)) p.Balance = val;

                    LogWriter.Log($"Editó persona {id}", currentUser);
                }
            }
        }

        static void DeletePerson()
        {
            Console.Write("ID a borrar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var p = persons.FirstOrDefault(x => x.Id == id);
                if (p != null)
                {
                    Console.Write("¿Confirmar? (S/N): ");
                    if (Console.ReadLine().ToUpper() == "S")
                    {
                        persons.Remove(p);
                        LogWriter.Log($"Eliminó persona {id}", currentUser);
                        Console.WriteLine("Eliminado.");
                    }
                }
            }
        }

        static void SaveChanges()
        {
            var listStrings = new List<string>();
            foreach (var p in persons)
            {
                listStrings.Add(ManualCsvHelper.PersonToLine(p));
            }
            SimpleTextFile.SaveAllLines(personsFile, listStrings);
            LogWriter.Log("Guardó cambios en disco", currentUser);
        }
    }
}
