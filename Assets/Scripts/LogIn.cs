using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class LogIn : MonoBehaviour
{
    public TMP_InputField PatiendIDField, ProvidersNameField, TodaysDateField;
    public static string ProvidersName, TodaysDate, PatientID;
    public UnityEngine.UI.Button EnterButton;
    public TMP_Text instructions;

    // Start is called before the first frame update
    void Start()
    {
        PatiendIDField.onEndEdit.AddListener(LockPatientID);
        ProvidersNameField.onEndEdit.AddListener(LockProvidersName);
        TodaysDateField.onEndEdit.AddListener(LockTodaysDate);

        EnterButton.onClick.AddListener(EnterButtonOnClick);
        EnterButton.interactable = false;

        instructions.text = "Please enter the required information in each field.";
    }

    void EnterButtonOnClick()
    {
        SceneManager.LoadScene(1);
    }

    void CheckConditions()
    {
        if (string.IsNullOrEmpty(PatientID) ||
            string.IsNullOrEmpty(ProvidersName) ||
            string.IsNullOrEmpty(TodaysDate) || IsValidDate(TodaysDate) == false)
        {
            EnterButton.interactable = false;
        }
        else
        {
            EnterButton.interactable = true;
        }
    }

    void LockPatientID(string s)
    {
        PatientID = s;
    }

    void LockProvidersName(string s)
    {
        ProvidersName = s;
    }

    void LockTodaysDate(string s)
    {
        TodaysDate = s;
        if (IsValidDate(TodaysDate) == false)
        {
            instructions.text = "Please verify that the date entered in the correct format (yyyy-mm-dd)";
        }
        else
        {
            instructions.text = "Please enter the required information in each field.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckConditions();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private bool IsValidDate(string input)
    {
        string pattern = @"^\d{4}-\d{2}-\d{2}$"; // YYYY-MM-DD
        return Regex.IsMatch(input, pattern);
    }
}
