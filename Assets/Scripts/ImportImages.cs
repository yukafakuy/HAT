using Cdm.Figma;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImportImages : MonoBehaviour
{
    public static Camera chartCamera;
    public static RenderTexture renderTexture;
    public static string[] screenshotPaths = new string[30];
    public static string pdfToolPath = @"C:\Program Files\ImageMagick-7.1.1-Q16-HDRI\magick.exe";

    void Awake()
    {
        chartCamera = GetComponent<Camera>();

        string newFolderGUID = Path.Combine(Application.persistentDataPath, "Screenshots");

        if (!Directory.Exists(newFolderGUID))
        {
            Directory.CreateDirectory(newFolderGUID);
        }
    }

    public static void CombineImagesToPDF(string[] imagePaths, string outputPDFPath, string screenshotsFolder, string externalToolPath)
    {
        string inputFiles = string.Join(" ", imagePaths.Select(f => $"\"{f}\""));
        string args = $"{inputFiles} \"{outputPDFPath}\"";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = externalToolPath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(psi))
        {
            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            UnityEngine.Debug.Log($"ImageMagick output:\n{output}");
            if (!string.IsNullOrEmpty(errors))
                UnityEngine.Debug.LogWarning($"ImageMagick errors:\n{errors}");
        }

        UnityEngine.Debug.Log($"PDF created at: {outputPDFPath}");

        // Delete the folder and all its contents
        string pathToScreenshots = "Assets/Screenshots";
        try
        {
            Directory.Delete(pathToScreenshots, true); // true = recursive delete
            Console.WriteLine($"Deleted folder: {pathToScreenshots}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting folder: {ex.Message}");
        }
    }

    public void StartCaptureAllImages()
    {
        StartCoroutine(CaptureAllImagesCoroutine());
    }

    public static IEnumerator CaptureAllImagesCoroutine()
    {
        string[] paths = new string[30];
        int i = 0;

        // RIGHT HAND
        //THUMB
        paths[i++] = "Assets/Screenshots/rightHand_thumb_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, true, false, false, false, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_thumb_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, true, false, false, false, false, false, false, true, false, paths[i - 1]));

        //INDEX
        paths[i++] = "Assets/Screenshots/rightHand_index_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, true, false, false, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_index_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, true, false, false, false, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_index_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, true, false, false, false, false, false, false, true, paths[i - 1]));

        //MIDDLE
        paths[i++] = "Assets/Screenshots/rightHand_middle_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, true, false, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_middle_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, true, false, false, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_middle_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, true, false, false, false, false, false, true, paths[i - 1]));

        //RING
        paths[i++] = "Assets/Screenshots/rightHand_ring_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, false, true, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_ring_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, false, true, false, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_ring_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, false, true, false, false, false, false, true, paths[i - 1]));

        //LITTLE
        paths[i++] = "Assets/Screenshots/rightHand_little_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, false, false, true, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_little_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, false, false, true, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/rightHand_little_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, false, false, true, false, false, false, true, paths[i - 1]));

        //WRIST
        paths[i++] = "Assets/Screenshots/rightHand_wrist.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(true, false, false, false, false, false, true, false, false, false, paths[i - 1]));

        // LEFT HAND
        //THUMB
        paths[i++] = "Assets/Screenshots/leftHand_thumb_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, true, false, false, false, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_thumb_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, true, false, false, false, false, false, false, true, false, paths[i - 1]));

        //INDEX
        paths[i++] = "Assets/Screenshots/leftHand_index_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, true, false, false, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_index_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, true, false, false, false, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_index_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, true, false, false, false, false, false, false, true, paths[i - 1]));

        //MIDDLE
        paths[i++] = "Assets/Screenshots/leftHand_middle_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, true, false, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_middle_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, true, false, false, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_middle_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, true, false, false, false, false, false, true, paths[i - 1]));

        //RING
        paths[i++] = "Assets/Screenshots/leftHand_ring_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, false, true, false, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_ring_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, false, true, false, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_ring_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, false, true, false, false, false, false, true, paths[i - 1]));

        //LITTLE
        paths[i++] = "Assets/Screenshots/leftHand_little_MCP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, false, false, true, false, true, false, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_little_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, false, false, true, false, false, true, false, paths[i - 1]));
        paths[i++] = "Assets/Screenshots/leftHand_little_IP.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, false, false, true, false, false, false, true, paths[i - 1]));

        //WRIST
        paths[i++] = "Assets/Screenshots/leftHand_wrist.png";
        yield return CoroutineRunner.Instance.StartCoroutine(IterateImages(false, false, false, false, false, false, true, false, false, false, paths[i - 1]));

        screenshotPaths = paths;

        string pathToPDF = Application.dataPath + "/" + LogIn.PatientID + "_" + LogIn.TodaysDate + "_data.pdf";
        string screenshotsFolder = "Assets/Screenshots";
        CombineImagesToPDF(screenshotPaths, pathToPDF, screenshotsFolder, pdfToolPath);
    }



    public static IEnumerator IterateImages(bool rightHandFlag, bool thumbFlag, bool indexFlag,
    bool middleFlag, bool ringFlag, bool pinkyFlag, bool wristFlag,
    bool MCPFlag, bool PIPFlag, bool DIPFlag, string path)
    {
        // Set all the flags
        VisualizeResults.rightHandFlag = rightHandFlag;
        VisualizeResults.thumbFlag = thumbFlag;
        VisualizeResults.indexFlag = indexFlag;
        VisualizeResults.middleFlag = middleFlag;
        VisualizeResults.ringFlag = ringFlag;
        VisualizeResults.pinkyFlag = pinkyFlag;
        VisualizeResults.wristFlag = wristFlag;
        VisualizeResults.MCPFlag = MCPFlag;
        VisualizeResults.PIPFlag = PIPFlag;
        VisualizeResults.DIPFlag = DIPFlag;

        // Wait one frame to allow UI to update
        yield return new WaitForEndOfFrame();

        // Capture the screenshot
        CaptureChartScreenshot(path);
    }

    public static void CaptureChartScreenshot(string filePath)
    {
        renderTexture = new RenderTexture(1920, 1080, 24);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        chartCamera.targetTexture = renderTexture;
        chartCamera.Render();

        // Convert top-right to bottom-left coordinate system
        int captureWidth = 1150;
        int captureHeight = 1000;
        int x = renderTexture.width - captureWidth;
        int y = renderTexture.height - captureHeight;

        Rect captureRect = new Rect(x, y, captureWidth, captureHeight);
        Texture2D image = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        image.ReadPixels(captureRect, 0, 0);

        //Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        //image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        RenderTexture.active = currentRT;
        chartCamera.targetTexture = null;

        UnityEngine.Debug.Log("Chart screenshot saved to " + filePath);
    }

}
