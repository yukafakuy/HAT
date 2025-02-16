using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using System.IO;
using System;
using System.Linq;
using System.Globalization;

public class fingerExtensions : MonoBehaviour
{
    public Toggle FingerExtensions, MCPFlexions, PIPDIPFlexions, ThumbIn, ThumbOut, ThumbFront, ThumbLast;
    public TMP_Text instructions;
    public Button RedoButton, ProceedButton;
    public GameObject PopupWindow;
    public TMP_Text text_patientID, text_diagnosis, text_provider, text_date;

    //Hand tracking
    public GameObject HandDetectionBar;
    public TMP_Text HandDetectionStatus;
    public LeapServiceProvider LeapServiceProvider;
    private string path_leapRight;
    private string path_leapLeft;
    private StreamWriter rawLeapRight;
    private StreamWriter rawLeapLeft;
    private Hand _leftHand, _rightHand;
    private string rawDataRight;
    public GameObject RthumbCMC, RthumbMCP, RthumbIP, RindexMCP, RindexPIP, RindexDIP, RmiddleMCP, RmiddlePIP, RmiddleDIP,
        RringMCP, RringPIP, RringDIP, RpinkyMCP, RpinkyPIP, RpinkyDIP;

    //Audio
    public AudioSource audioSource;
    public AudioClip Clip1, Clip2;
    private int audioNumber = 0;

    //Stopwatch
    private float maxTime = 2000;
    private Stopwatch stopWatch;
    private bool stopWatchFlag = false;
    public TMP_Text timer;

    //to switch between scenes but use the same script
    private bool FingerExtensionsFlag, MCPFlexionsFlag, PIPDIPFlexionsFlag, ThumbInFlag, ThumbOutFlag, ThumbFrontFlag, ThumbLastFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        int scene = currentScene.buildIndex;

        if (scene == 1)
        {
            FingerExtensionsFlag = true;
        }
        else if(scene == 2)
        {
            MCPFlexionsFlag = true;
        }
        else if(scene == 3)
        {
            PIPDIPFlexionsFlag = true;
        }
        else if(scene == 4)
        {
            ThumbInFlag = true;
        }
        else if(scene == 5)
        {
            ThumbOutFlag = true;
        }
        else if (scene == 6)
        {
            ThumbFrontFlag = true;
        }
        else if(scene == 7)
        {
            ThumbLastFlag = true;
        }



        text_patientID.text = main.patientID.ToString();
        text_date.text = main.date;
        text_diagnosis.text = main.diagnosis;
        text_provider.text = main.provider;

        PopupWindow.SetActive(false);

        RedoButton.onClick.AddListener(RedoButtonOnClick);
        ProceedButton.onClick.AddListener(ProceedButtonOnClick);

        if (main.taskList[0] == 1)
        {
            FingerExtensions.isOn = true;
        }
        if (main.taskList[1] == 1)
        {
            MCPFlexions.isOn = true;
        }
        if (main.taskList[2] == 1)
        {
            PIPDIPFlexions.isOn = true;
        }
        if (main.taskList[3] == 1)
        {
            ThumbIn.isOn = true;
        }
        if (main.taskList[4] == 1)
        {
            ThumbOut.isOn = true;
        }
        if (main.taskList[5] == 1)
        {
            ThumbFront.isOn = true;
        }
        if (main.taskList[6] == 1)
        {
            ThumbLast.isOn = true;
        }

        if (FingerExtensionsFlag)
        {
            instructions.text = "Open the hand as much as you can.";
        }
        else if (MCPFlexionsFlag)
        {
            instructions.text = "Show the side of your hand to the camera with the palm open.";
        }
        else if (PIPDIPFlexionsFlag || ThumbInFlag || ThumbFrontFlag || ThumbLastFlag || ThumbOutFlag)
        {
            instructions.text = "Show the palm of your hand to the camera.";
        }
        
        audioSource.Play(0);
        audioNumber = audioNumber + 1;

        stopWatch = new Stopwatch();
    }

    void RedoButtonOnClick()
    {
        PopupWindow.SetActive(false);

        if (FingerExtensionsFlag)
        {
            instructions.text = "Open the hand as much as you can.";
        }
        else if (MCPFlexionsFlag)
        {
            instructions.text = "Show the side of your hand to the camera with the palm open.";
        }
        else if (PIPDIPFlexionsFlag || ThumbInFlag || ThumbFrontFlag || ThumbLastFlag || ThumbOutFlag)
        {
            instructions.text = "Show the palm of your hand to the camera.";
        }

        audioSource.clip = Clip1;
        audioSource.Play();
        audioNumber = 1;
    }

    void ProceedButtonOnClick()
    {
        PopupWindow.SetActive(false);

        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();
        int scene = currentScene.buildIndex;
        main.taskCompleteList[scene-1] = 1;

        FingerExtensionsFlag = false;
        MCPFlexionsFlag = false;
        PIPDIPFlexionsFlag = false;
        ThumbInFlag = false;
        ThumbOutFlag = false;
        ThumbFrontFlag = false;
        ThumbLastFlag = false;

        for (int i = 0; i < 7; i++)
        {
            if (main.taskList[i] == 1 && main.taskCompleteList[i] == 0)
            {
                UnityEngine.Debug.Log(main.taskList[i] + "\t" + main.taskCompleteList[i]);
                SceneManager.LoadScene(i + 1);
                break;
            }
            else
            {
                SceneManager.LoadScene(8);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (audioNumber == 1)
        {
            if (!audioSource.isPlaying)
            {
                if (FingerExtensionsFlag)
                {
                    instructions.text = "Place the hand with the palm facing the camera and hold for 2 seconds.";
                }
                else if (MCPFlexionsFlag)
                {
                    instructions.text = "Make a fist and hold for 2 seconds.";
                }
                else if (PIPDIPFlexionsFlag)
                {
                    instructions.text = "Bend your fingers at a 90deg as shown and hold for 2 seconds. Make sure not to close your fingers into a fist.";
                }
                
                audioSource.clip = Clip2;
                audioSource.Play();

                audioNumber = audioNumber + 1;
            }
        }
        else if (audioNumber == 2)
        {
            if (!audioSource.isPlaying)
            {
                //create a txt file
                if (FingerExtensionsFlag)
                {
                    path_leapRight = Application.dataPath + "\\rightHand1.txt";
                }
                else if (MCPFlexionsFlag)
                {
                    path_leapRight = Application.dataPath + "\\rightHand2.txt";
                }
                else if (PIPDIPFlexionsFlag)
                {
                    path_leapRight = Application.dataPath + "\\rightHand3.txt";
                }

                else if (ThumbInFlag)
                {
                    path_leapRight = Application.dataPath + "\\rightHand4.txt";
                }
                else if (ThumbOutFlag)
                {
                    path_leapRight = Application.dataPath + "\\rightHand5.txt";
                }
                else if (ThumbFrontFlag)
                {
                    path_leapRight = Application.dataPath + "\\rightHand6.txt";
                }
                else if (ThumbLastFlag)
                {
                    path_leapRight = Application.dataPath + "\\rightHand7.txt";
                }
                rawLeapRight = File.CreateText(path_leapRight);

                stopWatch.Start();
                stopWatchFlag = true;
                audioNumber = audioNumber + 1;
            }
        }

        if (stopWatchFlag)
        {
            double timeLeft = (double)stopWatch.ElapsedMilliseconds / 1000.0;
            timer.text = "REC " + timeLeft.ToString();

            _rightHand = null;
            foreach (var hand in LeapServiceProvider.CurrentFrame.Hands)
            {
                if (hand.IsRight)
                {
                    _rightHand = hand;
                }
            }
            if (_rightHand != null)
            {
                HandDetectionBar.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 255, 0);
                HandDetectionStatus.text = "HAND DETECTED";
                collectDataLeapVer2();
            }
            else
            {
                HandDetectionBar.GetComponent<UnityEngine.UI.Image>().color = new Color(255, 0, 0);
                HandDetectionStatus.text = "HAND NOT DETECTED";
            }

            if (stopWatch.ElapsedMilliseconds > maxTime)
            {
                stopWatch.Reset();
                stopWatchFlag = false;
                PopupWindow.SetActive(true);

                rawLeapRight.Close();


                //display the max for one of the values
                float[,] signalData = readTxt("Assets/rightHand1.txt");
                float maxVal = calculateMax(signalData, 0);
            }
        }
    }

    private float[,] readTxt(string input)
    {
        string[] fileLines = File.ReadAllLines(input);
        float[,] map = new float[fileLines.Length, fileLines[0].Split('\t').Length];
        for (int i = 0; i < fileLines.Length; ++i)
        {
            string line = fileLines[i];
            for (int j = 0; j < map.GetLength(1); ++j)
            {
                string[] split = line.Split('\t');
                map[i, j] = float.Parse(split[j], CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        return map;
    }

    private float calculateMax(float[,] handData, int signal)
    {
        var lineCount = handData.GetLength(0);
        float[] array = new float[lineCount];
        for (int i = 0; i < lineCount; i++)
        {
            array[i] = handData[i, signal];
        }

        float maxSignal  = array.Max(); 

        return maxSignal;
    }

    private void collectDataLeapVer2()
    {
        if (_rightHand != null)
        {
            //thumb
            float CMC_x = RthumbCMC.transform.localEulerAngles.x;
            float CMC_y = RthumbCMC.transform.localEulerAngles.y;
            float CMC_z = RthumbCMC.transform.localEulerAngles.z;
            float thumb_MCP = RthumbMCP.transform.localEulerAngles.z;
            float thumb_IP = 360 - RthumbIP.transform.localEulerAngles.z;

            //index
            float index_MCP = 360 - RindexMCP.transform.localEulerAngles.z;
            float index_PIP = 360 - RindexPIP.transform.localEulerAngles.z;
            float index_DIP = 360 - RindexDIP.transform.localEulerAngles.z;

            //middle
            float middle_MCP = 360 - RmiddleMCP.transform.localEulerAngles.z;
            float middle_PIP = 360 - RmiddlePIP.transform.localEulerAngles.z;
            float middle_DIP = 360 - RmiddleDIP.transform.localEulerAngles.z;

            //ring
            float ring_MCP = 360 - RringMCP.transform.localEulerAngles.z;
            float ring_PIP = 360 - RringPIP.transform.localEulerAngles.z;
            float ring_DIP = 360 - RringDIP.transform.localEulerAngles.z;

            //pinky
            float pinky_MCP = 360 - RpinkyMCP.transform.localEulerAngles.z;
            float pinky_PIP = 360 - RpinkyPIP.transform.localEulerAngles.z;
            float pinky_DIP = 360 - RpinkyDIP.transform.localEulerAngles.z;

            rawDataRight = CMC_x + "\t" + CMC_y + "\t" + CMC_z + "\t" + thumb_MCP + "\t" + thumb_IP + "\t" +
                index_MCP + "\t" + index_PIP + "\t" + index_DIP + "\t" +
                middle_MCP + "\t" + middle_PIP + "\t" + middle_DIP + "\t" +
                ring_MCP + "\t" + ring_PIP + "\t" + ring_DIP + "\t" +
                pinky_MCP + "\t" + pinky_PIP + "\t" + pinky_DIP;

            //UnityEngine.Debug.Log(index_PIP);

            rawLeapRight.WriteLine(rawDataRight);

        }
    }

    private void collectDataLeap()
    {
        //erase everything inside the rawDataRight and rawDataLeft
        //rawDataRight = null;

        if (_rightHand != null)
        {
            Finger _thumb = _rightHand.GetThumb();
            Finger _index = _rightHand.GetIndex();
            Finger _middle = _rightHand.GetMiddle();
            Finger _ring = _rightHand.GetRing();
            Finger _pinky = _rightHand.GetPinky();

            Bone _indexMetacarpal = _index.Bone(Bone.BoneType.TYPE_METACARPAL);
            Bone _indexProximal = _index.Bone(Bone.BoneType.TYPE_PROXIMAL);
            Bone _indexIntermediate = _index.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
            Bone _indexDistal = _index.Bone(Bone.BoneType.TYPE_DISTAL);

            Bone _middleMetacarpal = _middle.Bone(Bone.BoneType.TYPE_METACARPAL);
            Bone _middleProximal = _middle.Bone(Bone.BoneType.TYPE_PROXIMAL);
            Bone _middleIntermediate = _middle.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
            Bone _middleDistal = _middle.Bone(Bone.BoneType.TYPE_DISTAL);

            Bone _ringMetacarpal = _ring.Bone(Bone.BoneType.TYPE_METACARPAL);
            Bone _ringProximal = _ring.Bone(Bone.BoneType.TYPE_PROXIMAL);
            Bone _ringIntermediate = _ring.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
            Bone _ringDistal = _ring.Bone(Bone.BoneType.TYPE_DISTAL);

            Bone _pinkyMetacarpal = _pinky.Bone(Bone.BoneType.TYPE_METACARPAL);
            Bone _pinkyProximal = _pinky.Bone(Bone.BoneType.TYPE_PROXIMAL);
            Bone _pinkyIntermediate = _pinky.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
            Bone _pinkyDistal = _pinky.Bone(Bone.BoneType.TYPE_DISTAL);

            Bone _thumbMetacarpal = _thumb.Bone(Bone.BoneType.TYPE_METACARPAL);
            Bone _thumbProximal = _thumb.Bone(Bone.BoneType.TYPE_PROXIMAL);
            Bone _thumbDistal = _thumb.Bone(Bone.BoneType.TYPE_DISTAL);

            //INDEX DATA
            Vector3 index_MCP1 = _indexMetacarpal.Direction;
            Vector3 index_MCP2 = _indexProximal.Direction;
            float index_MCP = Vector3.Angle(index_MCP1,index_MCP2);

            Vector3 index_PIP1 = _indexProximal.Direction;
            Vector3 index_PIP2 = _indexIntermediate.Direction;
            float index_PIP = Vector3.Angle(index_PIP1, index_PIP2);

            Vector3 index_DIP1 = _indexIntermediate.Direction;
            Vector3 index_DIP2 = _indexDistal.Direction;
            float index_DIP = Vector3.Angle(index_DIP1, index_DIP2);

            //MIDDLE DATA
            Vector3 middle_MCP1 = _middleMetacarpal.Direction;
            Vector3 middle_MCP2 = _middleProximal.Direction;
            float middle_MCP = Vector3.Angle(middle_MCP1, middle_MCP2);

            Vector3 middle_PIP1 = _middleProximal.Direction;
            Vector3 middle_PIP2 = _middleIntermediate.Direction;
            float middle_PIP = Vector3.Angle(middle_PIP1, middle_PIP2);

            Vector3 middle_DIP1 = _middleIntermediate.Direction;
            Vector3 middle_DIP2 = _middleDistal.Direction;
            float middle_DIP = Vector3.Angle(middle_DIP1, middle_DIP2);

            //RING DATA
            Vector3 ring_MCP1 = _ringMetacarpal.Direction;
            Vector3 ring_MCP2 = _ringProximal.Direction;
            float ring_MCP = Vector3.Angle(ring_MCP1, ring_MCP2);

            Vector3 ring_PIP1 = _ringProximal.Direction;
            Vector3 ring_PIP2 = _ringIntermediate.Direction;
            float ring_PIP = Vector3.Angle(ring_PIP1, ring_PIP2);

            Vector3 ring_DIP1 = _ringIntermediate.Direction;
            Vector3 ring_DIP2 = _ringDistal.Direction;
            float ring_DIP = Vector3.Angle(ring_DIP1, ring_DIP2);

            //PINKY DATA
            Vector3 pinky_MCP1 = _pinkyMetacarpal.Direction;
            Vector3 pinky_MCP2 = _pinkyProximal.Direction;
            float pinky_MCP = Vector3.Angle(pinky_MCP1, pinky_MCP2);

            Vector3 pinky_PIP1 = _pinkyProximal.Direction;
            Vector3 pinky_PIP2 = _pinkyIntermediate.Direction;
            float pinky_PIP = Vector3.Angle(pinky_PIP1, pinky_PIP2);

            Vector3 pinky_DIP1 = _pinkyIntermediate.Direction;
            Vector3 pinky_DIP2 = _pinkyDistal.Direction;
            float pinky_DIP = Vector3.Angle(pinky_DIP1, pinky_DIP2);


            //THUMB DATA
            Vector3 thumb_CMCabd1 = _thumbMetacarpal.Direction;
            Vector3 thumb_CMCabd2 = _indexMetacarpal.Direction;
            float thumb_CMCabd = Vector3.Angle(thumb_CMCabd1, thumb_CMCabd2);

            Vector3 thumb_MCP1 = _thumbMetacarpal.Direction;
            Vector3 thumb_MCP2 = _thumbProximal.Direction;
            float thumb_MCP = Vector3.Angle(thumb_MCP1, thumb_MCP2);

            Vector3 thumb_IP1 = _thumbProximal.Direction;
            Vector3 thumb_IP2 = _thumbDistal.Direction;
            float thumb_IP = Vector3.Angle(thumb_IP1,thumb_IP2);



            rawDataRight = thumb_CMCabd + "\t" + thumb_MCP + "\t" + thumb_IP +
                index_MCP + "\t" + index_PIP + "\t" + index_DIP + "\t" +
                middle_MCP + "\t" + middle_PIP + "\t" + middle_DIP + "\t" +
                ring_MCP + "\t" + ring_PIP + "\t" + ring_DIP + "\t" +
                pinky_MCP + "\t" + pinky_PIP + "\t" + pinky_DIP;

            //UnityEngine.Debug.Log(index_PIP);

            rawLeapRight.WriteLine(rawDataRight);

        }
    }
}