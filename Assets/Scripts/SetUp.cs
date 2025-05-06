using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.Search;
using Leap.Unity;
using System.Reflection;
using Unity.VisualScripting;

public class SetUp : MonoBehaviour
{
    public TMP_Text patientID, providersName, todaysDate;
    public Toggle RightHand, LeftHand, BothHands, PreTherapy, PostTherapy, Webcam, LeapController;
    public static bool rightHandFlag, bothHandFlag, bothHandFlagAbsolute, preTherapyFlag, webcamFlag = false;

    public Button NextButton, PastResults, SignOut;

    public GameObject PopUpObject;
    public Button RightHandButton, LeftHandButton;

    public LeapServiceProvider LeapServiceProvider;
    public TMP_Text title;

    // Start is called before the first frame update
    void Start()
    {
        patientID.text = LogIn.PatientID;
        providersName.text = LogIn.ProvidersName;
        todaysDate.text = LogIn.TodaysDate;

        NextButton.onClick.AddListener(NextButtonOnClick);
        SignOut.onClick.AddListener(SignOutOnClick);
        PastResults.onClick.AddListener(PastResultsOnClick);

        PopUpObject.SetActive(false);
        RightHandButton.onClick.AddListener(RightHandOnClick);
        LeftHandButton.onClick.AddListener(LeftHandOnClick);

    }

    void SignOutOnClick()
    {
        SceneManager.LoadScene(0);
    }

    void PastResultsOnClick()
    {
        SceneManager.LoadScene("New Result");
    }
    void NextButtonOnClick()
    {
        // Set flags based on UI toggles
        preTherapyFlag = PreTherapy.isOn;
        webcamFlag = Webcam.isOn;

        // Check Leap connection if LeapController is selected
        if (LeapController.isOn)
        {
            webcamFlag = false;

            if (!LeapServiceProvider.IsConnected())
            {
                title.text = "Leap is not connected. Check your connection and try again!";
                return; // Stop here if Leap is not connected
            }
            else
            {
                title.text = "Leap is connected! You may proceed.";
            }
        }

        // Only proceed to scenes if we reach this point
        if (RightHand.isOn)
        {
            rightHandFlag = true;
            SceneManager.LoadScene("Select Task - Right Hand");
        }
        else if (LeftHand.isOn)
        {
            rightHandFlag = false;
            SceneManager.LoadScene("Select Task - Left Hand");
        }
        else if (BothHands.isOn)
        {
            bothHandFlag = true;
            bothHandFlagAbsolute = true;
            PopUpObject.SetActive(true); // or handle both hands accordingly
        }
    }

    void RightHandOnClick()
    {
        rightHandFlag = true;
        SceneManager.LoadScene("Select Task - Right Hand");
    }

    void LeftHandOnClick()
    {
        rightHandFlag = false;
        SceneManager.LoadScene("Select Task - Left Hand");
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
