using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectTasks : MonoBehaviour
{
    public TMP_Text patientID, providersName, todaysDate;
    public Toggle FingerExtensions, MCPFlexions, PIPFlexions, ThumbOut, ThumbLast, WristFlexExtend;
    public TMP_Text buttonText;
    private int taskNumber = 0;

    public Button PastResult, SignOut, BeginTask;

    public static float[] taskList = new float[6];
    public static float[] taskCompleteList = new float[6];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < taskCompleteList.Length; i++)
        {
            taskCompleteList[i] = 0;
        }

        patientID.text = LogIn.PatientID;
        providersName.text = LogIn.ProvidersName;
        todaysDate.text = LogIn.TodaysDate;

        BeginTask.onClick.AddListener(BeginTaskOnClick);
        SignOut.onClick.AddListener(SignOutOnClick);
        PastResult.onClick.AddListener(PastResultOnClick);
    }

    void BeginTaskOnClick()
    {
        for (int i = 0; i <taskList.Length; i++)
        {
            if(taskList[i] == 1)
            {
                if (SetUp.webcamFlag == false)
                {
                    SceneManager.LoadScene(i + 2);
                    break;
                }
                else if (SetUp.webcamFlag == true)
                {
                    UnityEngine.Debug.Log("loading scene " + (i + 8).ToString());
                    SceneManager.LoadScene(i + 8);
                    break;
                }
            }
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

    void CountTasks()
    {
        taskNumber = 0;
        if (FingerExtensions.isOn)
        {
            taskNumber = taskNumber + 1;
            taskList[0] = 1;
        }
        else
        {
            taskList[0] = 0;
        }
        if (MCPFlexions.isOn)
        {
            taskNumber = taskNumber + 1;
            taskList[1] = 1;
        }
        else
        {
            taskList[1] = 0;
        }
        if (PIPFlexions.isOn)
        {
            taskNumber = taskNumber + 1;
            taskList[2] = 1;
        }
        else
        {
            taskList[2] = 0;
        }

        if (ThumbOut.isOn)
        {
            taskNumber = taskNumber + 1;
            taskList[3] = 1;
        }
        else
        {
            taskList[3] = 0;
        }
        if (ThumbLast.isOn)
        {
            taskNumber = taskNumber + 1;
            taskList[4] = 1;
        }
        else
        {
            taskList[4] = 0;
        }
        if (WristFlexExtend.isOn)
        {
            taskNumber = taskNumber + 1;
            taskList[5] = 1;
        }
        else
        {
            taskList[5] = 0;
        }


        buttonText.text = "Begin " + taskNumber.ToString() + " task(s)";

        if(taskNumber == 0)
        {
            BeginTask.interactable = false;
        }
        else
        {
            BeginTask.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CountTasks();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
