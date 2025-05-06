using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Mediapipe.Unity.Sample.HandLandmarkDetection;

public class MediaPipeTask : MonoBehaviour
{
    public TMP_Text patientID, providersName, todaysDate, handIndication;
    public Button PastResult, SignOut, Exit;
    public TMP_Text title, instruction;
    public GameObject HandDetectionBar;
    public GameObject InstructionalImage;

    //MediaPipe
    public HandLandmarkerRunner _handLandmarkerRunner;

    //Audio
    public AudioSource audioSource;
    public AudioClip ClipOrientation1, ClipOrientation2,
        ClipPosition1, ClipPosition2, ClipPosition3, ClipPosition4,
        ClipPosition5, ClipPosition6, ClipPosition7,
        ClipPosition8, ClipPosition9, beepSound;
    private int audioNumber = 0;

    //Stopwatch
    private float maxTime = 2000;
    private Stopwatch stopWatch;
    private bool stopWatchFlag = false;
    public TMP_Text task1Text;

    //Images
    public Sprite OrientationImage1, OrientationImage2, PositionImage1,
        PositionImage2, PositionImage3, PositionImage5,
        PositionImage7, PositionImage8, PositionImage9;
    public Sprite OrientationImage1_left, OrientationImage2_left, PositionImage1_left,
        PositionImage2_left, PositionImage3_left, PositionImage5_left,
        PositionImage7_left, PositionImage8_left, PositionImage9_left;

    //Progress Bar
    public Slider slider;
    public TMP_Text sliderText;

    //popup
    public GameObject PopupWindow;
    public Button NextTaskButton, RetakeButton;
    public GameObject PopupWindowOtherHand;
    public Button SkipButton, ProceedButton;

    //task list
    private bool task1Flag, task2Flag, task3Flag, task4Flag, task5Flag, task6Flag, task7Flag = false;
    private string taskText;
    private string[] taskNames = { "Finger Extension", "MCP Flexion", "PIP/DIP Extension", "Thumb Out", "Thumb In", "Wrist Flexion/Extension" };
    private string lineText;

    //Data saving
    private string path_leapRight, path_leapLeft;
    private StreamWriter rawLeapRight, rawLeapLeft;
    private string rawDataRight, rawDataLeft;

    //Inactivity
    public float inactivtiyThreshold = 5f;
    private float inactivityTimer = 0f;
    private bool popupFlag = false;
    public TMP_Text timer;

    //Camera
    //public Camera Camera2;


    // Start is called before the first frame update
    void Start()
    {
        //camera stuff
        //Matrix4x4 mat = Camera2.projectionMatrix;
        ////mat.m00 *= -1;
        //mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        //Camera2.projectionMatrix = mat;

        if (SetUp.rightHandFlag)
        {
            handIndication.text = "Right Hand";
        }
        else
        {
            handIndication.text = "Left Hand";
        }

        audioSource.clip = beepSound;
        audioSource.Play();

        patientID.text = LogIn.PatientID;
        providersName.text = LogIn.ProvidersName;
        todaysDate.text = LogIn.TodaysDate;

        SignOut.onClick.AddListener(SignOutOnClick);
        PastResult.onClick.AddListener(PastResultOnClick);
        Exit.onClick.AddListener(ExitOnClick);
        NextTaskButton.onClick.AddListener(NextTaskOnClick);
        RetakeButton.onClick.AddListener(RetakeButtonOnClick);
        SkipButton.onClick.AddListener(SkipButtonOnClick);
        ProceedButton.onClick.AddListener(ProceedButtonOnClick);

        instruction.text = "Place the palm facing the camera.";

        stopWatch = new Stopwatch();
        sliderText.text = "";

        PopupWindow.SetActive(false);

        Scene currentScene = SceneManager.GetActiveScene();
        int scene = currentScene.buildIndex;

        if (scene == 8)
        {
            task1Flag = true;
            taskText = "<b>Finger Extension</b>";
            title.text = "Finger Extension";
            if (SetUp.rightHandFlag)
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1;
                path_leapRight = Application.dataPath + "/rightHand1.txt";
                rawLeapRight = File.CreateText(path_leapRight);
            }
            else
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1_left;
                path_leapLeft = Application.dataPath + "/leftHand1.txt";
                rawLeapLeft = File.CreateText(path_leapLeft);
            }

            audioSource.clip = ClipOrientation1;
        }
        else if (scene == 9)
        {
            task2Flag = true;
            taskText = "<b>MCP Flexion</b>";
            title.text = "MCP Flexion";
            if (SetUp.rightHandFlag)
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage2;
                path_leapRight = Application.dataPath + "/rightHand2.txt";
                rawLeapRight = File.CreateText(path_leapRight);
            }
            else
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage2_left;
                path_leapLeft = Application.dataPath + "/leftHand2.txt";
                rawLeapLeft = File.CreateText(path_leapLeft);
            }
            audioSource.clip = ClipOrientation2;
        }
        else if (scene == 10)
        {
            task3Flag = true;
            taskText = "<b>PIP/DIP Flexion</b>";
            title.text = "PIP/DIP Flexion";
            if (SetUp.rightHandFlag)
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1;
                path_leapRight = Application.dataPath + "/rightHand3.txt";
                rawLeapRight = File.CreateText(path_leapRight);
            }
            else
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1_left;
                path_leapLeft = Application.dataPath + "/leftHand3.txt";
                rawLeapLeft = File.CreateText(path_leapLeft);
            }
            audioSource.clip = ClipOrientation1;
        }
        else if (scene == 11)
        {
            task4Flag = true;
            taskText = "<b>Thumb Out</b>";
            title.text = "Thumb Out";
            if (SetUp.rightHandFlag)
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1;
                path_leapRight = Application.dataPath + "/rightHand5.txt";
                rawLeapRight = File.CreateText(path_leapRight);
            }
            else
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1_left;
                path_leapLeft = Application.dataPath + "/leftHand5.txt";
                rawLeapLeft = File.CreateText(path_leapLeft);
            }
            audioSource.clip = ClipOrientation1;
        }
        else if (scene == 12)
        {
            task5Flag = true;
            taskText = "<b>Thumb In</b>";
            title.text = "Thumb In";
            if (SetUp.rightHandFlag)
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1;
                path_leapRight = Application.dataPath + "/rightHand7.txt";
                rawLeapRight = File.CreateText(path_leapRight);
            }
            else
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage1_left;
                path_leapLeft = Application.dataPath + "/leftHand7.txt";
                rawLeapLeft = File.CreateText(path_leapLeft);
            }
            audioSource.clip = ClipOrientation1;
        }
        else if (scene == 13)
        {
            task6Flag = true;
            taskText = "<b>Wrist Flexion/Extension</b>";
            title.text = "Wrist Flexion/Extension";
            if (SetUp.rightHandFlag)
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage2;
                path_leapRight = Application.dataPath + "/rightHand8.txt";
                rawLeapRight = File.CreateText(path_leapRight);
            }
            else
            {
                InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = OrientationImage2_left;
                path_leapLeft = Application.dataPath + "/leftHand8.txt";
                rawLeapLeft = File.CreateText(path_leapLeft);
            }
            audioSource.clip = ClipOrientation2;
        }

        //populate the list
        for (int i = 0; i < 6; i++)
        {
            if (SelectTasks.taskList[i] == 1 && scene != i + 8)
            {
                lineText = lineText + "<br>" + taskNames[i];
            }
            else if (SelectTasks.taskList[i] == 1 && scene == i + 8)
            {
                lineText = lineText + "<br>" + taskText;
            }
        }

        task1Text.text = lineText;
    }

    void SkipButtonOnClick()
    {
        SceneManager.LoadScene("New Result");
    }

    void ProceedButtonOnClick()
    {
        if (SetUp.rightHandFlag)
        {
            SetUp.rightHandFlag = false;
            SceneManager.LoadScene("Select Task - Left Hand");
        }
        else
        {
            SetUp.rightHandFlag = true;
            SceneManager.LoadScene("Select Task - Right Hand");
        }
    }

    void RetakeButtonOnClick()
    {
        PopupWindow.SetActive(false);
        popupFlag = false;

        rawLeapRight.BaseStream.SetLength(0);
        rawLeapRight.Flush();
        UnityEngine.Debug.Log("File content deleted successfully!");

        if (task7Flag)
        {
            task7Flag = false;
            task6Flag = true;
            audioNumber = 1;
        }
        else
        {
            audioNumber = 1;
        }
        timer.text = "Next Task";
    }


    void NextTaskOnClick()
    {
        if (SetUp.rightHandFlag)
        {
            rawLeapRight.Close();
        }
        else
        {
            rawLeapLeft.Close();
        }
        PopupWindow.SetActive(false);
        popupFlag = false;

        Scene currentScene = SceneManager.GetActiveScene();
        int scene = currentScene.buildIndex;
        SelectTasks.taskCompleteList[scene - 8] = 1;

        for (int i = 0; i < 6; i++)
        {
            if (SelectTasks.taskList[i] == 1 && SelectTasks.taskCompleteList[i] == 0)
            {
                if (!SetUp.webcamFlag)
                {
                    SceneManager.LoadScene(i + 2);
                    return;
                }
                else
                {
                    SceneManager.LoadScene(i + 8);
                    return;
                }
            }
        }

        // If we reached this point, all selected tasks are completed
        if (SetUp.bothHandFlag)
        {
            SetUp.bothHandFlag = false;
            PopupWindowOtherHand.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("New Result");
        }
    }


    void SignOutOnClick()
    {
        SceneManager.LoadScene(0);
    }

    void PastResultOnClick()
    {
        SceneManager.LoadScene("New Result");
    }

    void ExitOnClick()
    {
        SceneManager.LoadScene("Log-In");
    }

    void HandDetection()
    {
        if (SetUp.rightHandFlag)
        {
            //if (_handLandmarkerRunner != null && _handLandmarkerRunner.handJoints.All(joint => !float.IsNaN(joint)))
            if (_handLandmarkerRunner.handLabel == "Right")
            {
                stopWatch.Start();
                stopWatchFlag = true;
            }
            else
            {
                stopWatch.Reset();
                stopWatchFlag = false;
                slider.value = 0;
            }
        }
        else
        {
            if (_handLandmarkerRunner.handLabel == "Left")
            {
                stopWatch.Start();
                stopWatchFlag = true;
            }
            else
            {
                stopWatch.Reset();
                stopWatchFlag = false;
                slider.value = 0;
            }
        }
        

        if (stopWatchFlag)
        {
            collectDataLeap();
            sliderText.text = "hold steady";
            slider.value = stopWatch.ElapsedMilliseconds / 1000.0f;
            if (stopWatch.ElapsedMilliseconds > maxTime)
            {
                stopWatch.Reset();
                stopWatchFlag = false;
                sliderText.text = "";
                if (task6Flag)
                {
                    task6Flag = false;
                    task7Flag = true;
                    audioNumber = 1;
                }
                else
                {
                    //rawLeapRight.Close();
                    PopupWindow.SetActive(true);
                    popupFlag = true;
                    audioNumber = audioNumber + 1;
                }

            }
        }
    }


    void collectDataLeap()
    {
        if (SetUp.rightHandFlag)
        {
            if (_handLandmarkerRunner.handLabel == "Right" && _handLandmarkerRunner != null && _handLandmarkerRunner.handJoints.All(joint => !float.IsNaN(joint)))
            {

                // Regular way of extracting data from the virtual hand

                //thumb
                float thumb_MCP = _handLandmarkerRunner.handJoints[0];
                float thumb_IP = _handLandmarkerRunner.handJoints[1];

                //index
                float index_MCP = _handLandmarkerRunner.handJoints[2];
                float index_PIP = _handLandmarkerRunner.handJoints[3];
                float index_DIP = _handLandmarkerRunner.handJoints[4];

                //middle
                float middle_MCP = _handLandmarkerRunner.handJoints[5];
                float middle_PIP = _handLandmarkerRunner.handJoints[6];
                float middle_DIP = _handLandmarkerRunner.handJoints[7];

                //ring
                float ring_MCP = _handLandmarkerRunner.handJoints[8];
                float ring_PIP = _handLandmarkerRunner.handJoints[9];
                float ring_DIP = _handLandmarkerRunner.handJoints[10];

                //pinky
                float pinky_MCP = _handLandmarkerRunner.handJoints[11];
                float pinky_PIP = _handLandmarkerRunner.handJoints[12];
                float pinky_DIP = _handLandmarkerRunner.handJoints[13];

                //wrist
                float wrist_flex = _handLandmarkerRunner.handJoints[14];

                //Vector3 palm_normal = rightHand.PalmNormal;
                //Vector3 palm_direction = rightHand.Direction;
                //float wrist_flex = (float)(Math.Atan2(palm_direction.x, palm_direction.z) * 180 / Math.PI);

                rawDataRight = thumb_MCP + "\t" + thumb_IP + "\t" +
                    index_MCP + "\t" + index_PIP + "\t" + index_DIP + "\t" +
                    middle_MCP + "\t" + middle_PIP + "\t" + middle_DIP + "\t" +
                    ring_MCP + "\t" + ring_PIP + "\t" + ring_DIP + "\t" +
                    pinky_MCP + "\t" + pinky_PIP + "\t" + pinky_DIP + "\t" + wrist_flex +
                    "\t" + SetUp.preTherapyFlag.ToString() + "\t" + SetUp.webcamFlag.ToString();

                //UnityEngine.Debug.Log(index_PIP);

                rawLeapRight.WriteLine(rawDataRight);
            }
        }
        else
        {
            if (_handLandmarkerRunner.handLabel == "Left" && _handLandmarkerRunner != null && _handLandmarkerRunner.handJoints.All(joint => !float.IsNaN(joint)))
            {

                // Regular way of extracting data from the virtual hand

                //thumb
                float thumb_MCP = -1f*_handLandmarkerRunner.handJoints[0];
                float thumb_IP = -1f * _handLandmarkerRunner.handJoints[1];

                //index
                float index_MCP = -1f * _handLandmarkerRunner.handJoints[2];
                float index_PIP = -1f * _handLandmarkerRunner.handJoints[3];
                float index_DIP = -1f * _handLandmarkerRunner.handJoints[4];

                //middle
                float middle_MCP = -1f * _handLandmarkerRunner.handJoints[5];
                float middle_PIP = -1f * _handLandmarkerRunner.handJoints[6];
                float middle_DIP = -1f * _handLandmarkerRunner.handJoints[7];

                //ring
                float ring_MCP = -1f * _handLandmarkerRunner.handJoints[8];
                float ring_PIP = -1f * _handLandmarkerRunner.handJoints[9];
                float ring_DIP = -1f * _handLandmarkerRunner.handJoints[10];

                //pinky
                float pinky_MCP = -1f * _handLandmarkerRunner.handJoints[11];
                float pinky_PIP = -1f * _handLandmarkerRunner.handJoints[12];
                float pinky_DIP = -1f * _handLandmarkerRunner.handJoints[13];

                //wrist
                float wrist_flex = -1f * _handLandmarkerRunner.handJoints[14];

                //Vector3 palm_normal = rightHand.PalmNormal;
                //Vector3 palm_direction = rightHand.Direction;
                //float wrist_flex = (float)(Math.Atan2(palm_direction.x, palm_direction.z) * 180 / Math.PI);

                rawDataLeft = thumb_MCP + "\t" + thumb_IP + "\t" +
                    index_MCP + "\t" + index_PIP + "\t" + index_DIP + "\t" +
                    middle_MCP + "\t" + middle_PIP + "\t" + middle_DIP + "\t" +
                    ring_MCP + "\t" + ring_PIP + "\t" + ring_DIP + "\t" +
                    pinky_MCP + "\t" + pinky_PIP + "\t" + pinky_DIP + "\t" + wrist_flex +
                    "\t" + SetUp.preTherapyFlag + "\t" + SetUp.webcamFlag;


                rawLeapLeft.WriteLine(rawDataLeft);
            }
        }
        
    }

    void ResetInactivityTimer()
    {
        inactivityTimer = 0f;
    }


    // Update is called once per frame
    void Update()
    {

        if (audioNumber == 0)
        {
            audioSource.Play(0);

            Thread.Sleep(1000);
            audioNumber = audioNumber + 1;
        }
        else if (audioNumber == 1)
        {
            if (!audioSource.isPlaying)
            {
                if (task1Flag)
                {
                    if (SetUp.rightHandFlag)
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage1;
                    }
                    else
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage1_left;
                    }
                    instruction.text = "Open your fingers as much as you can and hold this pose for 2 seconds.";
                    audioSource.clip = ClipPosition1;
                }
                else if (task2Flag)
                {
                    if (SetUp.rightHandFlag)
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage2;
                    }
                    else
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage2_left;
                    }
                    instruction.text = "Make a fist and hold for 2 seconds.";
                    audioSource.clip = ClipPosition2;
                }
                else if (task3Flag)
                {
                    if (SetUp.rightHandFlag)
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage3;
                    }
                    else
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage3_left;
                    }
                    instruction.text = "Curl your fingers at the middle knuckles to a 90-degree angle, bringing your 4 fingertips down. Hold for 2 seconds.";
                    audioSource.clip = ClipPosition3;
                }
                else if (task4Flag)
                {
                    if (SetUp.rightHandFlag)
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage5;
                    }
                    else
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage5_left;
                    }
                    instruction.text = "Open and extend your thumb. Hold for 2 seconds.";
                    audioSource.clip = ClipPosition5;
                }
                else if (task5Flag)
                {
                    if (SetUp.rightHandFlag)
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage7;
                    }
                    else
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage7_left;
                    }
                    instruction.text = "Bend your thumb towards your palm and hold for 2 seconds.";
                    audioSource.clip = ClipPosition7;
                }
                else if (task6Flag)
                {
                    if (SetUp.rightHandFlag)
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage8;
                    }
                    else
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage8_left;
                    }
                    instruction.text = "Bend your wrist forward and hold for 2 seconds.";
                    audioSource.clip = ClipPosition8;
                }
                else if (task7Flag)
                {
                    if (SetUp.rightHandFlag)
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage9;
                    }
                    else
                    {
                        InstructionalImage.GetComponent<UnityEngine.UI.Image>().sprite = PositionImage9_left;
                    }
                    instruction.text = "Bend your wrist backward and hold for 2 seconds.";
                    audioSource.clip = ClipPosition9;
                }

                audioSource.Play();


                Thread.Sleep(1000);

                audioNumber = audioNumber + 1;
            }
        }
        else if (audioNumber == 2)
        {
            if (!audioSource.isPlaying)
            {
                //create some txt files
                HandDetection();
            }
        }

        if (popupFlag)
        {
            if (Input.anyKey || Input.touchCount > 0)
            {
                ResetInactivityTimer();
            }

            inactivityTimer += Time.deltaTime;
            float timeLeft = inactivtiyThreshold - inactivityTimer;
            timeLeft = Mathf.Ceil(timeLeft);
            timer.text = "Next Task " + timeLeft.ToString();

            if (inactivityTimer >= inactivtiyThreshold)
            {
                timer.text = "Next Task";
                NextTaskOnClick();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }
}

