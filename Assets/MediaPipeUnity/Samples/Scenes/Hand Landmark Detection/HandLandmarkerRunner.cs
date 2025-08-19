// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Leap;
using Mediapipe.Tasks.Vision.HandLandmarker;
using System.Collections;
using System.Diagnostics.Tracing;
using System.Reflection;
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
                    // Get handedness (your existing code)
                    if (result.handedness != null && i < result.handedness.Count)
                    {
                        var handedness = result.handedness[i];
                        handLabel = null;
                        if (handedness.categories != null && handedness.categories.Count > 0)
                        {
                            string label = handedness.categories[0].categoryName;
                            handLabel = (label == "Left") ? "Right" : "Left";
                        }
                    }
                    else
                    {
                        handLabel = null;
                    }

                    handJoints = new float[15];
                    var landmarks = result.handLandmarks[i];

                    // Calculate hand normal for consistent orientation
                    Vector3 wrist = new Vector3(landmarks.landmarks[0].x, landmarks.landmarks[0].y, landmarks.landmarks[0].z);
                    Vector3 indexMCP = new Vector3(landmarks.landmarks[5].x, landmarks.landmarks[5].y, landmarks.landmarks[5].z);
                    Vector3 pinkyMCP = new Vector3(landmarks.landmarks[17].x, landmarks.landmarks[17].y, landmarks.landmarks[17].z);
                    Vector3 handNormal = Vector3.Cross(indexMCP - wrist, pinkyMCP - wrist).normalized;

                    // Thumb angles
                    float thumbMCPAngle = CalculateFingerJointAngle(
                        new Vector3(landmarks.landmarks[1].x, landmarks.landmarks[1].y, landmarks.landmarks[1].z),
                        new Vector3(landmarks.landmarks[2].x, landmarks.landmarks[2].y, landmarks.landmarks[2].z),
                        new Vector3(landmarks.landmarks[3].x, landmarks.landmarks[3].y, landmarks.landmarks[3].z),
                        handNormal
                    );

                    float thumbIPAngle = CalculateFingerJointAngle(
                        new Vector3(landmarks.landmarks[2].x, landmarks.landmarks[2].y, landmarks.landmarks[2].z),
                        new Vector3(landmarks.landmarks[3].x, landmarks.landmarks[3].y, landmarks.landmarks[3].z),
                        new Vector3(landmarks.landmarks[4].x, landmarks.landmarks[4].y, landmarks.landmarks[4].z),
                        handNormal
                    );

                    handJoints[0] = thumbMCPAngle;
                    handJoints[1] = thumbIPAngle;

                    // Index finger angles
                    float indexMCPAngle = CalculateFingerJointAngle(wrist, indexMCP,
                        new Vector3(landmarks.landmarks[6].x, landmarks.landmarks[6].y, landmarks.landmarks[6].z),
                        handNormal
                    );

                    float indexPIPAngle = CalculateFingerJointAngle(
                        indexMCP,
                        new Vector3(landmarks.landmarks[6].x, landmarks.landmarks[6].y, landmarks.landmarks[6].z),
                        new Vector3(landmarks.landmarks[7].x, landmarks.landmarks[7].y, landmarks.landmarks[7].z),
                        handNormal
                    );

                    float indexDIPAngle = CalculateFingerJointAngle(
                        new Vector3(landmarks.landmarks[6].x, landmarks.landmarks[6].y, landmarks.landmarks[6].z),
                        new Vector3(landmarks.landmarks[7].x, landmarks.landmarks[7].y, landmarks.landmarks[7].z),
                        new Vector3(landmarks.landmarks[8].x, landmarks.landmarks[8].y, landmarks.landmarks[8].z),
                        handNormal
                    );        

                    handJoints[2] = indexMCPAngle;
                    handJoints[3] = indexPIPAngle;
                    //handJoints[4] = indexDIPAngle;
                    handJoints[4] = GetRobustDIPAngle(indexDIPAngle);

                    // Middle finger angles
                    Vector3 middleMCP = new Vector3(landmarks.landmarks[9].x, landmarks.landmarks[9].y, landmarks.landmarks[9].z);

                    float middleMCPAngle = CalculateFingerJointAngle(
                        wrist,
                        middleMCP,
                        new Vector3(landmarks.landmarks[10].x, landmarks.landmarks[10].y, landmarks.landmarks[10].z),
                        handNormal
                    );

                    float middlePIPAngle = CalculateFingerJointAngle(
                        middleMCP,
                        new Vector3(landmarks.landmarks[10].x, landmarks.landmarks[10].y, landmarks.landmarks[10].z),
                        new Vector3(landmarks.landmarks[11].x, landmarks.landmarks[11].y, landmarks.landmarks[11].z),
                        handNormal
                    );

                    float middleDIPAngle = CalculateFingerJointAngle(
                        new Vector3(landmarks.landmarks[10].x, landmarks.landmarks[10].y, landmarks.landmarks[10].z),
                        new Vector3(landmarks.landmarks[11].x, landmarks.landmarks[11].y, landmarks.landmarks[11].z),
                        new Vector3(landmarks.landmarks[12].x, landmarks.landmarks[12].y, landmarks.landmarks[12].z),
                        handNormal
                    );

                    handJoints[5] = middleMCPAngle;
                    handJoints[6] = middlePIPAngle;
                    //handJoints[7] = middleDIPAngle;
                    handJoints[7] = GetRobustDIPAngle(middleDIPAngle);

                    // Ring finger angles
                    Vector3 ringMCP = new Vector3(landmarks.landmarks[13].x, landmarks.landmarks[13].y, landmarks.landmarks[13].z);

                    float ringMCPAngle = CalculateFingerJointAngle(
                        wrist,
                        ringMCP,
                        new Vector3(landmarks.landmarks[14].x, landmarks.landmarks[14].y, landmarks.landmarks[14].z),
                        handNormal
                    );

                    float ringPIPAngle = CalculateFingerJointAngle(
                        ringMCP,
                        new Vector3(landmarks.landmarks[14].x, landmarks.landmarks[14].y, landmarks.landmarks[14].z),
                        new Vector3(landmarks.landmarks[15].x, landmarks.landmarks[15].y, landmarks.landmarks[15].z),
                        handNormal
                    );

                    float ringDIPAngle = CalculateFingerJointAngle(
                        new Vector3(landmarks.landmarks[14].x, landmarks.landmarks[14].y, landmarks.landmarks[14].z),
                        new Vector3(landmarks.landmarks[15].x, landmarks.landmarks[15].y, landmarks.landmarks[15].z),
                        new Vector3(landmarks.landmarks[16].x, landmarks.landmarks[16].y, landmarks.landmarks[16].z),
                        handNormal
                    );

                    handJoints[8] = ringMCPAngle;
                    handJoints[9] = ringPIPAngle;
                    //handJoints[10] = ringDIPAngle;
                    handJoints[10] = GetRobustDIPAngle(ringDIPAngle);


                    // Pinky finger angles
                    float pinkyMCPAngle = CalculateFingerJointAngle(
                        wrist,
                        pinkyMCP,
                        new Vector3(landmarks.landmarks[18].x, landmarks.landmarks[18].y, landmarks.landmarks[18].z),
                        handNormal
                    );

                    float pinkyPIPAngle = CalculateFingerJointAngle(
                        pinkyMCP,
                        new Vector3(landmarks.landmarks[18].x, landmarks.landmarks[18].y, landmarks.landmarks[18].z),
                        new Vector3(landmarks.landmarks[19].x, landmarks.landmarks[19].y, landmarks.landmarks[19].z),
                        handNormal
                    );

                    float pinkyDIPAngle = CalculateFingerJointAngle(
                        new Vector3(landmarks.landmarks[18].x, landmarks.landmarks[18].y, landmarks.landmarks[18].z),
                        new Vector3(landmarks.landmarks[19].x, landmarks.landmarks[19].y, landmarks.landmarks[19].z),
                        new Vector3(landmarks.landmarks[20].x, landmarks.landmarks[20].y, landmarks.landmarks[20].z),
                        handNormal
                    );

                    handJoints[11] = pinkyMCPAngle;
                    handJoints[12] = pinkyPIPAngle;
                    //handJoints[13] = pinkyDIPAngle;
                    handJoints[13] = GetRobustDIPAngle(pinkyDIPAngle);

                    // Wrist angle
                    //float wristAngle = CalculateWristAngle(wrist, indexMCP, pinkyMCP);
                    handJoints[14] = 0.0f;
                }
            }
            else
            {
                handLabel = null;
            }
        }

        // Alternative finger angle calculation with more explicit sign handling
        private float CalculateFingerJointAngle(Vector3 proximal, Vector3 joint, Vector3 distal, Vector3 handNormal)
        {
            // Create vectors from the joint to adjacent points
            Vector3 v1 = (proximal - joint).normalized;
            Vector3 v2 = (distal - joint).normalized;

            // Calculate the unsigned angle
            float dotProduct = Vector3.Dot(v1, v2);
            float angleRad = Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f));
            float angleDeg = angleRad * Mathf.Rad2Deg;

            // Determine the bending direction using the hand normal
            Vector3 bendDirection = Vector3.Cross(v1, v2);
            float bendSign = Vector3.Dot(bendDirection, handNormal) > 0 ? 1f : -1f;

            // Convert to signed angle (0° = straight, negative = flexed, positive = hyperextended)
            return (180f - angleDeg) * bendSign;
        }

        private float CalculateJointAngle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            // Create vectors from the joint (pointB) to the adjacent points
            Vector3 vector1 = (pointA - pointB).normalized;
            Vector3 vector2 = (pointC - pointB).normalized;

            // Calculate the dot product for the angle
            float dotProduct = Vector3.Dot(vector1, vector2);

            // Calculate the unsigned angle
            float angleRad = Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f));
            float angleDeg = angleRad * Mathf.Rad2Deg;

            // To determine the sign, we need a reference plane
            // For fingers, we can use the hand's normal vector or a consistent reference
            // Here's a simple approach using the cross product to determine orientation
            Vector3 crossProduct = Vector3.Cross(vector1, vector2);

            // The sign depends on the orientation of the cross product
            // You may need to adjust this based on your coordinate system
            // For MediaPipe's coordinate system, positive Z typically points toward the camera
            float sign = crossProduct.z > 0 ? 1f : -1f;

            // Return signed angle (positive for extension, negative for flexion)
            return (180f - angleDeg) * sign;
        }

        private float CalculateWristAngle(Vector3 wrist, Vector3 indexMCP, Vector3 pinkyMCP)
        {
            // Compute hand plane normal (palm normal)
            Vector3 handPlaneNormal = Vector3.Cross(indexMCP - wrist, pinkyMCP - wrist).normalized;

            // Compute forearm direction (from wrist toward the midpoint of MCP joints)
            Vector3 forearmMid = (indexMCP + pinkyMCP) / 2;
            Vector3 forearmDirection = (forearmMid - wrist).normalized;

            // For wrist flexion/extension, we want the angle between the forearm and hand plane
            // The dot product with the hand normal gives us the flexion/extension component
            float flexionComponent = Vector3.Dot(forearmDirection, handPlaneNormal);

            // Calculate the angle
            float angleRad = Mathf.Asin(Mathf.Clamp(flexionComponent, -1f, 1f));
            float angleDeg = angleRad * Mathf.Rad2Deg;

            return angleDeg;
        }

        private float GetRobustDIPAngle(float pipAngle)
        {
            float estimatedDIP;
            if (MediaPipeTask.task3Flag)
            {
                // Use biomechanical model when confidence is low
                estimatedDIP = EstimateDIPFromPIP(pipAngle);
            }
            else
            {
                estimatedDIP = pipAngle;
            }
            return estimatedDIP;
        }

        // Apply anatomical constraints to DIP joint estimation
        private float EstimateDIPFromPIP(float pipAngle)
        {
            // DIP typically moves 60-80% of PIP angle
            float dipRatio = 0.7f;

            // Anatomical limits: DIP typically 0-90° flexion
            float estimatedDIP = Mathf.Clamp(pipAngle * dipRatio, -90f, 30f);

            // Apply coupling constraint (DIP can't extend much when PIP is flexed)
            if (pipAngle < -60f)
            {
                estimatedDIP = Mathf.Max(estimatedDIP, pipAngle - 30f);
            }

            return estimatedDIP;
        }
   }
}
