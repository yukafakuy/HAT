using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using UnityEngine;

public class PlatformDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== ImageSourceDebugger START ===");

        // Check what ImageSourceProvider gives us
        var imageSource = ImageSourceProvider.ImageSource;
        Debug.Log($"ImageSourceProvider.ImageSource type: {imageSource?.GetType()?.Name ?? "NULL"}");

        if (imageSource != null)
        {
            Debug.Log($"ImageSource isPrepared: {imageSource.isPrepared}");
            Debug.Log($"ImageSource isPlaying: {imageSource.isPlaying}");
            Debug.Log($"ImageSource sourceName: {imageSource.sourceName ?? "NULL"}");

            // If it's a WebCamSource, let's check its state
            if (imageSource is WebCamSource webCamSource)
            {
                Debug.Log("=== WebCamSource Details ===");
                Debug.Log($"WebCamSource textureWidth: {webCamSource.textureWidth}");
                Debug.Log($"WebCamSource textureHeight: {webCamSource.textureHeight}");
                Debug.Log($"WebCamSource sourceCandidateNames: {string.Join(", ", webCamSource.sourceCandidateNames ?? new string[0])}");

                // Try to start it manually for debugging
                Debug.Log("Attempting to start WebCamSource manually...");
                StartCoroutine(TestWebCamSource(webCamSource));
            }
        }
        else
        {
            Debug.LogError("ImageSourceProvider.ImageSource is NULL!");

            // Check if we can create one manually
            Debug.Log("Attempting to create WebCamSource manually...");
            try
            {
                var resolutions = new ImageSource.ResolutionStruct[]
                {
                    new ImageSource.ResolutionStruct(1920, 1080, 30),
                    new ImageSource.ResolutionStruct(1280, 720, 30),
                    new ImageSource.ResolutionStruct(640, 480, 30)
                };

                var manualWebCamSource = new WebCamSource(1280, resolutions);
                Debug.Log("Manual WebCamSource created successfully");
                StartCoroutine(TestWebCamSource(manualWebCamSource));
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create manual WebCamSource: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
            }
        }
    }

    private System.Collections.IEnumerator TestWebCamSource(WebCamSource webCamSource)
    {
        Debug.Log("=== Testing WebCamSource ===");

        var playCoroutine = webCamSource.Play(); // get IEnumerator

        // Yield outside the try/catch
        while (true)
        {
            bool done = false;
            try
            {
                if (!playCoroutine.MoveNext())
                {
                    done = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"WebCamSource Play() failed: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
                yield break; // exit coroutine
            }

            if (done) break;

            yield return playCoroutine.Current;
        }

        Debug.Log($"WebCamSource Play() completed - isPrepared: {webCamSource.isPrepared}, isPlaying: {webCamSource.isPlaying}");

        if (webCamSource.isPrepared)
        {
            Debug.Log($"WebCamSource successfully started: {webCamSource.textureWidth}x{webCamSource.textureHeight}");
            var texture = webCamSource.GetCurrentTexture();
            Debug.Log($"Current texture: {texture?.width ?? 0}x{texture?.height ?? 0}");
        }
        else
        {
            Debug.LogError("WebCamSource failed to prepare");
        }
    }
}