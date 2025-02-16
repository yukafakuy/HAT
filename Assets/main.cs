using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using System.Globalization;
using TMPro;
using UnityEngine.UIElements;

public class main : MonoBehaviour
{
    public UnityEngine.UI.Toggle FingerExtensions, MCPFlexions, PIPDIPFlexions, ThumbIn, ThumbOut, ThumbFront, ThumbLast;
    public UnityEngine.UI.Button StartButton;
    public static float[] taskList = new float[7];
    public static float[] taskCompleteList = new float[7];


    public TMP_InputField input_patientID, input_diagnosis, input_provider, input_date;
    public static int patientID;
    public static string date, diagnosis, provider;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < taskCompleteList.Length; i++)
        {
            taskCompleteList[i] = 0;
        }
        StartButton.onClick.AddListener(StartButtonOnClick);

        input_patientID.onEndEdit.AddListener(LockPatientID);
        input_diagnosis.onEndEdit.AddListener(LockDiagnosis);
        input_provider.onEndEdit.AddListener(LockProvider);
        input_date.onEndEdit.AddListener(LockDate);
    }

    void LockPatientID(string s)
    {
        patientID = int.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
    }
    void LockDiagnosis(string s)
    {
        diagnosis = s;
    }
    void LockProvider(string s)
    {
        provider = s;
    }
    void LockDate(string s)
    {
        date = s;
    }


    void StartButtonOnClick()
    {
        if (FingerExtensions.isOn)
        {
            taskList[0] = 1;
        }
        else
        {
            taskList[0] = 0;
        }
        if (MCPFlexions.isOn)
        {
            taskList[1] = 1;
        }
        else
        {
            taskList[1] = 0;
        }
        if (PIPDIPFlexions.isOn)
        {
            taskList[2] = 1;
        }
        else
        {
            taskList[2] = 0;
        }
        if (ThumbIn.isOn)
        {
            taskList[3] = 1;
        }
        else
        {
            taskList[3] = 0;
        }
        if (ThumbOut.isOn)
        {
            taskList[4] = 1;
        }
        else
        {
            taskList[4] = 0;
        }
        if (ThumbFront.isOn)
        {
            taskList[5] = 1;
        }
        else
        {
            taskList[5] = 0;
        }
        if (ThumbLast.isOn)
        {
            taskList[6] = 1;
        }
        else
        {
            taskList[6] = 0;
        }

        for (int i = 0; i < taskList.Length; i++)
        {
            if (taskList[i] == 1)
            {
                SceneManager.LoadScene(i + 1);
                break;
            }
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
