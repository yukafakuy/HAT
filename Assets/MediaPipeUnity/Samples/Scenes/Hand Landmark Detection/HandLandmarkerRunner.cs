// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System.Diagnostics.Tracing;
using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;
using UnityEngine.Rendering;
using static Mediapipe.ImageFormat.Types;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{

  public class HandLandmarkerRunner : VisionTaskApiRunner<HandLandmarker>
  {
        //[SerializeField] public Transform[] handJoints; 
    public float[] handJoints;
        public string handLabel;
    [SerializeField] private HandLandmarkerResultAnnotationController _handLandmarkerResultAnnotationController;

    private Experimental.TextureFramePool _textureFramePool;

    public readonly HandLandmarkDetectionConfig config = new HandLandmarkDetectionConfig();

    public override void Stop()
    {
      base.Stop();
      _textureFramePool?.Dispose();
      _textureFramePool = null;
    }

    protected override IEnumerator Run()
    {
      Debug.Log($"Delegate = {config.Delegate}");
      Debug.Log($"Image Read Mode = {config.ImageReadMode}");
      Debug.Log($"Running Mode = {config.RunningMode}");
      Debug.Log($"NumHands = {config.NumHands}");
      Debug.Log($"MinHandDetectionConfidence = {config.MinHandDetectionConfidence}");
      Debug.Log($"MinHandPresenceConfidence = {config.MinHandPresenceConfidence}");
      Debug.Log($"MinTrackingConfidence = {config.MinTrackingConfidence}");

      yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

      var options = config.GetHandLandmarkerOptions(config.RunningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnHandLandmarkDetectionOutput : null);
      taskApi = HandLandmarker.CreateFromOptions(options, GpuManager.GpuResources);
      var imageSource = ImageSourceProvider.ImageSource;

      yield return imageSource.Play();

      if (!imageSource.isPrepared)
      {
        Debug.LogError("Failed to start ImageSource, exiting...");
        yield break;
      }

      // Use RGBA32 as the input format.
      // TODO: When using GpuBuffer, MediaPipe assumes that the input format is BGRA, so maybe the following code needs to be fixed.
      _textureFramePool = new Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);

      // NOTE: The screen will be resized later, keeping the aspect ratio.
      screen.Initialize(imageSource);

      SetupAnnotationController(_handLandmarkerResultAnnotationController, imageSource);

      var transformationOptions = imageSource.GetTransformationOptions();
      var flipHorizontally = transformationOptions.flipHorizontally;
      var flipVertically = transformationOptions.flipVertically;
      var imageProcessingOptions = new Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);

      AsyncGPUReadbackRequest req = default;
      var waitUntilReqDone = new WaitUntil(() => req.done);
      var waitForEndOfFrame = new WaitForEndOfFrame();
      var result = HandLandmarkerResult.Alloc(options.numHands);

      // NOTE: we can share the GL context of the render thread with MediaPipe (for now, only on Android)
      var canUseGpuImage = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 && GpuManager.GpuResources != null;
      using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;

      while (true)
      {
        if (isPaused)
        {
          yield return new WaitWhile(() => isPaused);
        }

        if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
        {
          yield return new WaitForEndOfFrame();
          continue;
        }

        // Build the input Image
        Image image;
        switch (config.ImageReadMode)
        {
          case ImageReadMode.GPU:
            if (!canUseGpuImage)
            {
              throw new System.Exception("ImageReadMode.GPU is not supported");
            }
            textureFrame.ReadTextureOnGPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
            image = textureFrame.BuildGPUImage(glContext);
            // TODO: Currently we wait here for one frame to make sure the texture is fully copied to the TextureFrame before sending it to MediaPipe.
            // This usually works but is not guaranteed. Find a proper way to do this. See: https://github.com/homuler/MediaPipeUnityPlugin/pull/1311
            yield return waitForEndOfFrame;
            break;
          case ImageReadMode.CPU:
            yield return waitForEndOfFrame;
            textureFrame.ReadTextureOnCPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
            image = textureFrame.BuildCPUImage();
            textureFrame.Release();
            break;
          case ImageReadMode.CPUAsync:
          default:
            req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
            yield return waitUntilReqDone;

            if (req.hasError)
            {
              Debug.LogWarning($"Failed to read texture from the image source");
              continue;
            }
            image = textureFrame.BuildCPUImage();
            textureFrame.Release();
            break;
        }

        switch (taskApi.runningMode)
        {
          case Tasks.Vision.Core.RunningMode.IMAGE:
            if (taskApi.TryDetect(image, imageProcessingOptions, ref result))
            {
              _handLandmarkerResultAnnotationController.DrawNow(result);
            }
            else
            {
              _handLandmarkerResultAnnotationController.DrawNow(default);
            }
            break;
          case Tasks.Vision.Core.RunningMode.VIDEO:
            if (taskApi.TryDetectForVideo(image, GetCurrentTimestampMillisec(), imageProcessingOptions, ref result))
            {
              _handLandmarkerResultAnnotationController.DrawNow(result);
            }
            else
            {
              _handLandmarkerResultAnnotationController.DrawNow(default);
            }
            break;
          case Tasks.Vision.Core.RunningMode.LIVE_STREAM:
            taskApi.DetectAsync(image, GetCurrentTimestampMillisec(), imageProcessingOptions);
            break;
        }
      }
    }

    private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Image image, long timestamp)
    {
      _handLandmarkerResultAnnotationController.DrawLater(result);

            if (result.handLandmarks != null && result.handLandmarks.Count > 0)
            {
                for (int i = 0; i < result.handLandmarks.Count; i++)
                {
                    // ✅ Get handedness for the same hand index
                    if (result.handedness != null && i < result.handedness.Count)
                    {
                        var handedness = result.handedness[i];
                        handLabel = null;
                        if (handedness.categories != null && handedness.categories.Count > 0)
                        {
                            string label = handedness.categories[0].categoryName; // "Left" or "Right"
                            if (label == "Left")
                            {
                                handLabel = "Right";
                            }
                            else if (label == "Right")
                            {
                                handLabel = "Left";
                            }
                        }
                    }
                    else
                    {
                        handLabel = null;
                    }


                    //Debug.Log($"Hand {i + 1} Landmarks:");
                    handJoints = new float[15];

                    var landmarks = result.handLandmarks[i];
                    for (int j = 0; j < landmarks.landmarks.Count; j++)
                    {
                        var landmark = landmarks.landmarks[j];
                        Vector3 jointPosition = new Vector3(landmark.x, landmark.y, landmark.z);
                        //Debug.Log($"Landmark {j}: (x: {landmark.x}, y: {landmark.y}, z: {landmark.z})");
                    }

                    float thumbMCPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[1].x, landmarks.landmarks[1].y, landmarks.landmarks[1].z),  // Index MCP
                        new Vector3(landmarks.landmarks[2].x, landmarks.landmarks[2].y, landmarks.landmarks[2].z),  // Index PIP
                        new Vector3(landmarks.landmarks[3].x, landmarks.landmarks[3].y, landmarks.landmarks[3].z)   // Index PIP
                    );

                    float thumbIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[2].x, landmarks.landmarks[2].y, landmarks.landmarks[2].z),  // Index MCP
                        new Vector3(landmarks.landmarks[3].x, landmarks.landmarks[3].y, landmarks.landmarks[3].z),  // Index PIP
                        new Vector3(landmarks.landmarks[4].x, landmarks.landmarks[4].y, landmarks.landmarks[4].z)   // Index PIP
                    );

                    handJoints[0] = thumbMCPAngle;
                    handJoints[1] = thumbIPAngle;

                    float indexMCPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[0].x, landmarks.landmarks[0].y, landmarks.landmarks[0].z),  // Index MCP
                        new Vector3(landmarks.landmarks[5].x, landmarks.landmarks[5].y, landmarks.landmarks[5].z),  // Index PIP
                        new Vector3(landmarks.landmarks[6].x, landmarks.landmarks[6].y, landmarks.landmarks[6].z)   // Index PIP
                    );

                    Debug.Log(indexMCPAngle);

                    float indexPIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[5].x, landmarks.landmarks[5].y, landmarks.landmarks[5].z),  // Index MCP
                        new Vector3(landmarks.landmarks[6].x, landmarks.landmarks[6].y, landmarks.landmarks[6].z),  // Index PIP
                        new Vector3(landmarks.landmarks[7].x, landmarks.landmarks[7].y, landmarks.landmarks[7].z)   // Index PIP
                    );

                    float indexDIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[6].x, landmarks.landmarks[6].y, landmarks.landmarks[6].z),  // Index MCP
                        new Vector3(landmarks.landmarks[7].x, landmarks.landmarks[7].y, landmarks.landmarks[7].z),  // Index PIP
                        new Vector3(landmarks.landmarks[8].x, landmarks.landmarks[8].y, landmarks.landmarks[8].z)   // Index PIP
                    );

                    handJoints[2] = indexMCPAngle;
                    handJoints[3] = indexPIPAngle;
                    handJoints[4] = indexDIPAngle;

                    float middleMCPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[0].x, landmarks.landmarks[0].y, landmarks.landmarks[0].z),  // middle MCP
                        new Vector3(landmarks.landmarks[9].x, landmarks.landmarks[9].y, landmarks.landmarks[9].z),  // middle PIP
                        new Vector3(landmarks.landmarks[10].x, landmarks.landmarks[10].y, landmarks.landmarks[10].z)   // middle PIP
                    );

                    float middlePIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[9].x, landmarks.landmarks[9].y, landmarks.landmarks[9].z),  // middle MCP
                        new Vector3(landmarks.landmarks[10].x, landmarks.landmarks[10].y, landmarks.landmarks[10].z),  // middle PIP
                        new Vector3(landmarks.landmarks[11].x, landmarks.landmarks[11].y, landmarks.landmarks[11].z)   // middle PIP
                    );

                    float middleDIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[10].x, landmarks.landmarks[10].y, landmarks.landmarks[10].z),  // middle MCP
                        new Vector3(landmarks.landmarks[11].x, landmarks.landmarks[11].y, landmarks.landmarks[11].z),  // middle PIP
                        new Vector3(landmarks.landmarks[12].x, landmarks.landmarks[12].y, landmarks.landmarks[12].z)   // middle PIP
                    );

                    handJoints[5] = middleMCPAngle;
                    handJoints[6] = middlePIPAngle;
                    handJoints[7] = middleDIPAngle;

                    float ringMCPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[0].x, landmarks.landmarks[0].y, landmarks.landmarks[0].z),  // ring MCP
                        new Vector3(landmarks.landmarks[13].x, landmarks.landmarks[13].y, landmarks.landmarks[13].z),  // ring PIP
                        new Vector3(landmarks.landmarks[14].x, landmarks.landmarks[14].y, landmarks.landmarks[14].z)   // ring PIP
                    );

                    float ringPIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[13].x, landmarks.landmarks[13].y, landmarks.landmarks[13].z),  // ring MCP
                        new Vector3(landmarks.landmarks[14].x, landmarks.landmarks[14].y, landmarks.landmarks[14].z),  // ring PIP
                        new Vector3(landmarks.landmarks[15].x, landmarks.landmarks[15].y, landmarks.landmarks[15].z)   // ring PIP
                    );

                    float ringDIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[14].x, landmarks.landmarks[14].y, landmarks.landmarks[14].z),  // ring MCP
                        new Vector3(landmarks.landmarks[15].x, landmarks.landmarks[15].y, landmarks.landmarks[15].z),  // ring PIP
                        new Vector3(landmarks.landmarks[16].x, landmarks.landmarks[16].y, landmarks.landmarks[16].z)   // ring PIP
                    );

                    handJoints[8] = ringMCPAngle;
                    handJoints[9] = ringPIPAngle;
                    handJoints[10] = ringDIPAngle;

                    float pinkyMCPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[0].x, landmarks.landmarks[0].y, landmarks.landmarks[0].z),  // pinky MCP
                        new Vector3(landmarks.landmarks[17].x, landmarks.landmarks[17].y, landmarks.landmarks[17].z),  // pinky PIP
                        new Vector3(landmarks.landmarks[18].x, landmarks.landmarks[18].y, landmarks.landmarks[18].z)   // pinky PIP
                    );

                    float pinkyPIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[17].x, landmarks.landmarks[17].y, landmarks.landmarks[17].z),  // pinky MCP
                        new Vector3(landmarks.landmarks[18].x, landmarks.landmarks[18].y, landmarks.landmarks[18].z),  // pinky PIP
                        new Vector3(landmarks.landmarks[19].x, landmarks.landmarks[19].y, landmarks.landmarks[19].z)   // pinky PIP
                    );

                    float pinkyDIPAngle = CalculateJointAngle(
                        new Vector3(landmarks.landmarks[18].x, landmarks.landmarks[18].y, landmarks.landmarks[18].z),  // pinky MCP
                        new Vector3(landmarks.landmarks[19].x, landmarks.landmarks[19].y, landmarks.landmarks[19].z),  // pinky PIP
                        new Vector3(landmarks.landmarks[20].x, landmarks.landmarks[20].y, landmarks.landmarks[20].z)   // pinky PIP
                    );

                    handJoints[11] = pinkyMCPAngle;
                    handJoints[12] = pinkyPIPAngle;
                    handJoints[13] = pinkyDIPAngle;

                    float wristAngle = CalculateWristAngle(
                        new Vector3(landmarks.landmarks[0].x, landmarks.landmarks[0].y, landmarks.landmarks[0].z),  // wrist
                        new Vector3(landmarks.landmarks[5].x, landmarks.landmarks[5].y, landmarks.landmarks[5].z),  // index MCP
                        new Vector3(landmarks.landmarks[17].x, landmarks.landmarks[17].y, landmarks.landmarks[17].z) // pinky MCP
                        );

                    handJoints[14] = wristAngle;
                }
            }
            else
            {
                handLabel = null;
            }
    }

    private float CalculateWristAngle(Vector3 wrist, Vector3 indexMCP, Vector3 pinkyMCP)
    {
        // Compute Forearm Vector (Wrist to midpoint of MCP joints)
        Vector3 forearmMid = (indexMCP + pinkyMCP) / 2;
        Vector3 vForearm = wrist - forearmMid;

        // Compute Hand Dorsal Vector (Cross product to get normal to the hand plane)
        Vector3 vHand = Vector3.Cross(indexMCP - wrist, pinkyMCP - wrist);

        // Normalize vectors
        vForearm.Normalize();
        vHand.Normalize();

        // Compute wrist flexion-extension angle (in degrees)
        float angleRad = Mathf.Acos(Mathf.Clamp(Vector3.Dot(vForearm, vHand), -1.0f, 1.0f));
        float angleDeg = angleRad * Mathf.Rad2Deg;

        return angleDeg;
    }

    private float CalculateJointAngle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        // Convert landmarks to vectors
        Vector3 vector1 = new Vector3(pointA.x - pointB.x, pointA.y - pointB.y, pointA.z - pointB.z);
        Vector3 vector2 = new Vector3(pointC.x - pointB.x, pointC.y - pointB.y, pointC.z - pointB.z);

        // Calculate the dot product and magnitudes
        float dotProduct = Vector3.Dot(vector1, vector2);
        float magnitude1 = vector1.magnitude;
        float magnitude2 = vector2.magnitude;
    
        // Calculate the angle in radians and convert to degrees
        float angleRad = Mathf.Acos(Mathf.Clamp(dotProduct / (magnitude1 * magnitude2),-1f,1f));
        float angleDeg = 180-(angleRad * Mathf.Rad2Deg);

        return angleDeg;
    }
  }
}
