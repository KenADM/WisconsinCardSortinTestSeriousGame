using UnityEngine;
using Serilog;
using System.IO;
using System;

public class PlayerLog
{
    public static void Initialize(string FileName)
    {
        // Assicurati che il logger venga configurato una sola volta
        if (Log.Logger.GetType().Name == "SilentLogger") 
        {
            // Percorso sicuro per salvare i dati su qualsiasi piattaforma (Windows, Mac, Android, iOS)
            // Application.persistentDataPath per salvataggio log su cartella apposita
            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Logs");
            string logFilePath = Path.Combine(logDirectory, $"{FileName}.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: logFilePath, 
                    outputTemplate: "[{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            Log.Information($"Test paziente {FileName}\n");
        }
    }

    public static void CloseLogger()
    {
        // Importante per assicurarsi che tutti i log in coda vengano scritti sul file prima che il gioco si chiuda
        Log.CloseAndFlush();
    }
}