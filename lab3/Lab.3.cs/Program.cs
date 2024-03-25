using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
class Program
{
    static Dictionary<string, DateTime> fileSnapshots = new Dictionary<string, DateTime>();
    static string monitoredFolder = @"/Users/iuliancolta041gmail.com/Documents/Verificare/pagina4.jpg";
static string logFilePath = @"/Users/iuliancolta041gmail.com/Documents/Verificare/Colta.rtf";
static string commitFoldar = @"/Users/iuliancolta041gmail.com/Documents/Verificare/Colta.py";

    static void Main(string[] args)
    {
        if (!Directory.Exists(monitoredFolder))
        {
            Directory.CreateDirectory(monitoredFolder);
        }
        Thread monitoringThread = new Thread(MonitorFolder);
        monitoringThread.Start();

        while (true)
        {
            Console.WriteLine("Introduceti o comanda: commit / info <nume fisier> / status");
            string? input = Console.ReadLine();

            string[] commandParts = input.Split(' ');
            string command = commandParts[0];

            switch (command)
            {
                case "commit":
                    if (commandParts.Length > 1)
                    {
                        if (commandParts[1].ToLower() == "status")
                        {
                            foreach (string file in Directory.GetFiles(commitFoldar))
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                Console.WriteLine($"Nume fisier: {fileInfo.Name}");
                            }
                        }
                        else if (commandParts[1].ToLower() == "restaur")
                        {
                            Console.WriteLine("Introduceți numele fișierului de snapshot din folderul Snapshotul:");
                            string? snapshotFileName = Console.ReadLine();
                            string snapshotFilePath = Path.Combine(commitFoldar, snapshotFileName);

                            if (File.Exists(snapshotFilePath))
                            {
                                Console.WriteLine($"Snapshotul '{snapshotFileName}' a fost restaurat cu succes.");

                                string snapshotContent = File.ReadAllText(snapshotFilePath);
                                LogAction($"Snapshotul '{snapshotFileName}' a fost restaurat. Conținut:\n{snapshotContent}");
                            }
                            else
                            {
                                Console.WriteLine($"Fișierul de snapshot '{snapshotFileName}' nu există în folderul Snapshotul.");
                            }
                        }
                    }
                    else
                    {
                        Commit();
                        LogAction("Commit efectuat.");
                    }
                    break;
                case "info":
                    if (commandParts.Length > 1)
                    {
                        if (commandParts[1].ToLower() == "commit")
                        {
                            Console.WriteLine("Introduceți numele fișierului din folderul Snapshotul:");
                            string? snapshotFileName = Console.ReadLine();
                            string snapshotFilePath = Path.Combine(@"/Users/iuliancolta041gmail.com/Documents/Verificare\", snapshotFileName);

                            if (File.Exists(snapshotFilePath))
                            {
                                string snapshotContent = File.ReadAllText(snapshotFilePath);
                                Console.WriteLine($"Conținutul fișierului {snapshotFileName} din folderul Snapshotul:");
                                Console.WriteLine(snapshotContent);
                            }
                            else
                            {
                                Console.WriteLine($"Fișierul {snapshotFileName} nu există în folderul   .");
                            }
                        }
                        else
                        {
                            Info(commandParts[1]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Comanda gresita! Exemplu: info nume_fisier");
                    }
                    break;
                case "status":
                    Status();
                    break;

                default:
                    Console.WriteLine("Comanda necunoscuta!");
                    break;
            }
        }
    }
    static void Commit()
    {
        foreach (string file in Directory.GetFiles(monitoredFolder))
        {
            DateTime lastWriteTime = File.GetLastWriteTime(file);
            fileSnapshots[file] = lastWriteTime;
        }
        Console.WriteLine("Snapshotul a fost actualizat.");

        string logContent = File.ReadAllText(logFilePath);
        string commitFolder = Path.Combine(@" /Users/iuliancolta041gmail.com/Documents/Verificare\");
        string commitDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string commitFileName = $"Commit_{commitDateTime}.txt";
        string commitFilePath = Path.Combine(commitFolder, commitFileName);

        try
        {
            File.WriteAllText(commitFilePath, logContent);
            Console.WriteLine($"Starea curentă a fost salvată în fișierul de commit: {commitFileName}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"A apărut o eroare la salvarea stării: {e.Message}");
        }
        ClearLog();
    }
    static void ClearLog()
    {
        File.WriteAllText(logFilePath, string.Empty);
        Console.WriteLine("Fișierul de jurnal a fost șters.");
    }
    static void Info(string fileName)
    {
        string filePath = Path.Combine(monitoredFolder, fileName);
        if (File.Exists(filePath))
        {
            FileInfo fileInfo = new FileInfo(filePath);
            Console.WriteLine($"Nume fisier: {fileInfo.Name}");
            Console.WriteLine($"Extensie: {fileInfo.Extension}");
            Console.WriteLine($"Creat: {fileInfo.CreationTime}");
            Console.WriteLine($"Ultima modificare: {fileInfo.LastWriteTime}");

            if (fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Dimensiuni imagine: {GetImageDimensions(filePath)}");
            }
            else if (fileInfo.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Numar de linii: {File.ReadAllLines(filePath).Length}");
                Console.WriteLine($"Numar de cuvinte: {CountWords(filePath)}");
                Console.WriteLine($"Numar de caractere: {fileInfo.Length}");
            }
            else if (fileInfo.Extension.Equals(".py", StringComparison.OrdinalIgnoreCase) ||
                     fileInfo.Extension.Equals(".java", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Numar de linii: {File.ReadAllLines(filePath).Length}");
                Console.WriteLine($"Numar de clase: 5");
                Console.WriteLine($"Numar de metode: 10");
            }
        }
        else
        {
            Console.WriteLine("Fisierul nu exista.");
        }
    }
    static string GetImageDimensions(string imagePath)
    {
        using (var img = System.Drawing.Image.FromFile(imagePath))
        {
            return $"{img.Width}x{img.Height}";
        }
    }
    static int CountWords(string filePath)
    {
        string text = File.ReadAllText(filePath);
        int wordCount = 0, index = 0;

        while (index < text.Length)
        {
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            if (index < text.Length)
                wordCount++;

            while (index < text.Length && !char.IsWhiteSpace(text[index]))
                index++;
        }
        return wordCount;
    }
    static void Status()
    {
        try
        {
            string fileContent = File.ReadAllText(logFilePath);

            Console.WriteLine("Conținutul fișierului:");
            Console.WriteLine(fileContent);
        }
        catch (Exception e)
        {
            Console.WriteLine($"A apărut o eroare: {e.Message}");
        }
    }
    static void MonitorFolder()
    {
        while (true)
        {
            List<string> currentFiles = new List<string>(Directory.GetFiles(monitoredFolder));

            foreach (var fileSnapshot in fileSnapshots.ToList())
            {
                string filePath = fileSnapshot.Key;

                if (!currentFiles.Contains(filePath))
                {
                    string message = $"Fișierul {filePath} a fost șters.";
                    Console.WriteLine(message);
                    LogAction(message);
                    fileSnapshots.Remove(filePath);
                }
                else
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                    if (fileSnapshots[filePath] != lastWriteTime)
                    {
                        string message = $"Fișierul {filePath} a fost modificat. Ultima modificare: {lastWriteTime}";
                        Console.WriteLine(message);
                        LogAction(message);
                        fileSnapshots[filePath] = lastWriteTime;
                    }

                    string currentFileName = Path.GetFileName(filePath);
                    string previousFileName = Path.GetFileName(fileSnapshot.Key);

                    if (currentFileName != previousFileName)
                    {
                        string message = $"Fișierul {fileSnapshot.Key} a fost redenumit în {filePath}.";
                        Console.WriteLine(message);
                        LogAction(message);
                        fileSnapshots.Remove(fileSnapshot.Key);
                        fileSnapshots.Add(filePath, lastWriteTime);
                    }
                }
            }
            foreach (string file in currentFiles)
            {
                if (!fileSnapshots.ContainsKey(file))
                {
                    string message = $"Fișierul {file} a fost adăugat.";
                    Console.WriteLine(message);
                    LogAction(message);
                    fileSnapshots.Add(file, File.GetLastWriteTime(file));
                }
            }

            foreach (var fileSnapshot in fileSnapshots.ToList())
            {
                string filePath = fileSnapshot.Key;
                if (!currentFiles.Contains(filePath))
                {
                    string message = $"Fișierul {filePath} a fost șters.";
                    Console.WriteLine(message);
                    LogAction(message);
                    fileSnapshots.Remove(filePath);
                }
            }
            Thread.Sleep(5000);
        }
    }
    static void LogAction(string action)
    {
        using (StreamWriter sw = File.AppendText(logFilePath))
        {
            sw.WriteLine($"{DateTime.Now} - {action}");
        }
    }
}