using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Functions : MonoBehaviour
{
    private static Functions _instance;
    public static void ConvertCSVtoPDF(string csvPath, string sofficePath, string outputDir)
    {
        if (!File.Exists(csvPath))
        {
            UnityEngine.Debug.LogError("CSV file not found: " + csvPath);
            return;
        }

        if (!File.Exists(sofficePath))
        {
            UnityEngine.Debug.LogError("LibreOffice executable not found: " + sofficePath);
            return;
        }

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = sofficePath,
            Arguments = $"--headless --convert-to pdf \"{csvPath}\" --outdir \"{outputDir}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            UnityEngine.Debug.Log("LibreOffice output:\n" + output);
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogWarning("LibreOffice error:\n" + error);
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error running LibreOffice: " + e.Message);
        }
    }

    public static Functions Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("FunctionsHelper");
                _instance = go.AddComponent<Functions>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public static void SaveCSV(string csvFilePath, string dataFilePath)
    {
        if (!File.Exists(dataFilePath))
        {
            UnityEngine.Debug.Log("Text file not found: " + dataFilePath);
            return;
        }

        try
        {
            using (StreamReader reader = new StreamReader(dataFilePath))
            using (StreamWriter writer = new StreamWriter(csvFilePath, false))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(new[] { ',', '\t' });
                    writer.WriteLine(string.Join("\t", values));
                }
            }
            UnityEngine.Debug.Log("Export successful! CSV saved at: " + csvFilePath);
        }
        catch (IOException e)
        {
            UnityEngine.Debug.Log("File operation failed: " + e.Message);
        }

        // Start coroutine on instance
        Instance.StartCoroutine(Instance.WaitAndExecute());
    }

    private IEnumerator WaitAndExecute()
    {
        UnityEngine.Debug.Log("Waiting...");
        yield return new WaitForSeconds(2f);
        UnityEngine.Debug.Log("Wait complete!");
    }

    public static void MergePDFs(string[] inputFiles, string outputFile, string ghostscriptPath)
    {
        if (!File.Exists(ghostscriptPath))
        {
            UnityEngine.Debug.LogError("Ghostscript executable not found at: " + ghostscriptPath);
            return;
        }

        foreach (string file in inputFiles)
        {
            if (!File.Exists(file))
            {
                UnityEngine.Debug.LogError("Missing input file: " + file);
                return;
            }
        }

        // Wrap file paths with quotes and join
        string inputArgs = string.Join(" ", inputFiles.Select(f => $"\"{f}\""));

        // Build command-line arguments
        string arguments = $"-dBATCH -dNOPAUSE -q -sDEVICE=pdfwrite -sOutputFile=\"{outputFile}\" {inputArgs}";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ghostscriptPath,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        try
        {
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                    UnityEngine.Debug.LogWarning("Ghostscript error output:\n" + error);

                UnityEngine.Debug.Log("PDF merge completed: " + outputFile);

            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Error running Ghostscript: " + ex.Message);
        }
    }
}
