using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlHand : MonoBehaviour
{
    public GameObject wrist, indexMCP, indexPIP, indexDIP, indexTip,
          middleMCP, middlePIP, middleDIP, middleTip,
          ringMCP, ringPIP, ringDIP, ringTip,
          pinkyMCP, pinkyPIP, pinkyDIP, pinkyTip,
          thumbCMC, thumbMCP, thumbIP, thumbTip;

    public HandLandmarkerRunner _handLandmarkerRunner; 

    // Start is called before the first frame update
    void Start()
    {
        if (_handLandmarkerRunner != null && _handLandmarkerRunner.handJoints != null)
        {
            // Access the handJoints array and log their positions
            for (int i = 0; i < _handLandmarkerRunner.handJoints.Length; i++)
            {
                Debug.Log($"Hand joint {i}: {_handLandmarkerRunner.handJoints[i]}");
            }
        }
        else
        {
            Debug.LogWarning("HandLandmarkerRunner or handJoints is not initialized.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_handLandmarkerRunner != null && _handLandmarkerRunner.handJoints != null)
        {
            //Debug.Log(_handLandmarkerRunner.handJoints[0]);
            Vector3 currentRotation = indexMCP.transform.localEulerAngles;
            currentRotation.z = -1f*_handLandmarkerRunner.handJoints[0];
            indexMCP.transform.localEulerAngles = currentRotation;

            currentRotation = indexPIP.transform.localEulerAngles;
            currentRotation.z = -1f * _handLandmarkerRunner.handJoints[0];
            indexPIP.transform.localEulerAngles = currentRotation;

            currentRotation = indexDIP.transform.localEulerAngles;
            currentRotation.z = -1f * _handLandmarkerRunner.handJoints[0];
            indexDIP.transform.localEulerAngles = currentRotation;

            /*
            indexMCP.transform.localPosition = handJoints[5].localPosition;
            indexPIP.transform.localPosition = handJoints[6].localPosition;
            indexDIP.transform.localPosition = handJoints[7].localPosition;
            indexTip.transform.localPosition = handJoints[8].localPosition;
            middleMCP.transform.localPosition = handJoints[5].localPosition;
            middlePIP.transform.localPosition = handJoints[6].localPosition;
            middleDIP.transform.localPosition = handJoints[7].localPosition;
            middleTip.transform.localPosition = handJoints[8].localPosition;
            ringMCP.transform.localPosition = handJoints[5].localPosition;
            ringPIP.transform.localPosition = handJoints[6].localPosition;
            ringDIP.transform.localPosition = handJoints[7].localPosition;
            ringTip.transform.localPosition = handJoints[8].localPosition;
            pinkyMCP.transform.localPosition = handJoints[5].localPosition;
            pinkyPIP.transform.localPosition = handJoints[6].localPosition;
            pinkyDIP.transform.localPosition = handJoints[7].localPosition;
            pinkyTip.transform.localPosition = handJoints[8].localPosition;
            */
        }
                
    }
}
