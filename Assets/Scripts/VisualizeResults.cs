using E2C;
using Leap.Unity.Attributes;
using LeapInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using Unity;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static E2C.E2ChartData;

public class VisualizeResults : MonoBehaviour
{
    public E2Chart myChart;
    public Button ThumbButton, IndexButton, MiddleButton, RingButton, PinkyButton, WristButton;
    public Button RightHand, LeftHand;
    public Button MCPButton, PIPButton, DIPButton;
    public Button returnToTaskSelectionButton, SignOutButton, exportToCSVButton;
    public static bool thumbFlag, indexFlag, middleFlag, ringFlag, pinkyFlag, wristFlag = false;
    private List<string> jointList = new List<string>();
    public TMP_Text chartTitle, successText;
    private GraphState previousState;
    public TMP_Text patientName, providerName, date;

    public static bool returnToTaskSelected = false;

    private string filePathRight, filePathLeft, filePath;

    // Hand and joint flags
    public static bool rightHandFlag = false;
    public static bool MCPFlag, PIPFlag, DIPFlag = false;

    //Date Range selector
    public TMP_InputField DateFromField, DateToField;
    private string DateTo, DateFrom;
    private bool dateFromChangedFlag, dateToChangedFlag = false;

    //Hand Map
    public GameObject HandMapImage;
    public Sprite imageThumbMCP, imageThumbIP, imageIndexMCP, imageIndexPIP,
        imageIndexDIP, imageMiddleMCP, imageMiddlePIP, imageMiddleDIP,
        imageRingMCP, imageRingPIP, imageRingDIP, imagePinkyMCP,
        imagePinkyPIP, imagePinkyDIP, imageWrist;
    public Sprite imageThumbMCP_left, imageThumbIP_left, imageIndexMCP_left, imageIndexPIP_left,
        imageIndexDIP_left, imageMiddleMCP_left, imageMiddlePIP_left, imageMiddleDIP_left,
        imageRingMCP_left, imageRingPIP_left, imageRingDIP_left, imagePinkyMCP_left,
        imagePinkyPIP_left, imagePinkyDIP_left, imageWrist_left;

    //Exporting stuff
    public Toggle GraphToggle, TableToggle;
    public TMP_Dropdown FormatDropdown;

    //Comments section
    public TMP_InputField NotesInput;
    public Button AddNotesButton;
    private string notesPath;
    public TMP_Text PreviousNotes;

    //Return to Task
    public GameObject returnToTaskSelectionPopup;
    public Button AgreedToProceedButton, DeclinedToProceedButton;

    public TMP_Text JointDescription;

    //CSV to PDF conversion
    private string sofficePath = @"C:\Program Files\LibreOffice\program\soffice.exe";
    private string ghostscriptPath = @"C:\Program Files\gs\gs10.05.1\bin\gswin64.exe";

    // Start is called before the first frame update
    void Start()
    {
        successText.text = "";
        DateFromField.onValueChanged.AddListener(LockDateFrom);
        DateToField.onValueChanged.AddListener(LockDateTo);

        if (LogIn.TodaysDate == null)
        {
            LogIn.TodaysDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        patientName.text = LogIn.PatientID;
        providerName.text = LogIn.ProvidersName;
        date.text = LogIn.TodaysDate;

        notesPath = Application.dataPath + "/" + LogIn.PatientID + "_notes.csv";
        if (File.Exists(notesPath))
        {
            string previousNotesString = File.ReadAllText(notesPath);
            PreviousNotes.text = previousNotesString;
            PreviousNotes.alignment = TextAlignmentOptions.Left;
        }

        //extract and save data
        string pathRight = Application.dataPath + "/rightHand";
        string pathSaveRight = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_right.txt";
        extractData(pathRight, pathSaveRight);
        string pathLeft = Application.dataPath + "/leftHand";
        string pathSaveLeft = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_left.txt";
        extractData(pathLeft, pathSaveLeft);

        var populateRight = GetComponent<PopulateSeriesRight>();
        populateRight.ExtractSeries();
        var populateLeft = GetComponent<PopulateSeriesLeft>();
        populateLeft.ExtractSeries();

        //buttons
        ThumbButton.onClick.AddListener(ThumbButtonOnClick);
        IndexButton.onClick.AddListener(IndexButtonOnClick);
        MiddleButton.onClick.AddListener(MiddleButtonOnClick);
        RingButton.onClick.AddListener(RingButtonOnClick);
        PinkyButton.onClick.AddListener(PinkyButtonOnClick);
        WristButton.onClick.AddListener(WristButtonOnClick);
        RightHand.onClick.AddListener(RightHandOnClick);
        LeftHand.onClick.AddListener(LeftHandOnClick);
        MCPButton.onClick.AddListener(MCPButtonOnClick);
        PIPButton.onClick.AddListener(PIPButtonOnClick);
        DIPButton.onClick.AddListener(DIPButtonOnClick);
        returnToTaskSelectionButton.onClick.AddListener(returnToTaskSelection);
        SignOutButton.onClick.AddListener(signOut);
        exportToCSVButton.onClick.AddListener(ExportToCSV);
        AddNotesButton.onClick.AddListener(AddNotes);
        AgreedToProceedButton.onClick.AddListener(AgreedToProceedOnClick);
        DeclinedToProceedButton.onClick.AddListener(DeclineToProceedOnClick);
        FormatDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        //add chart options
        myChart.chartOptions = myChart.gameObject.AddComponent<E2ChartOptions>();
        myChart.chartOptions.title.enableTitle = false;
        myChart.chartOptions.title.enableSubTitle = false;
        myChart.chartOptions.yAxis.enableTitle = true;
        myChart.chartOptions.xAxis.enableTitle = true;
        myChart.chartOptions.label.enable = true;
        myChart.chartOptions.legend.enable = true;
        myChart.chartOptions.chartStyles.lineChart.lineWidth = 2;
        myChart.chartOptions.xAxis.type = E2ChartOptions.AxisType.DateTime;
        myChart.chartOptions.yAxis.autoAxisRange = true;
        myChart.chartOptions.xAxis.labelNumericFormat = "MM-dd";
        //myChart.chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
        myChart.chartOptions.yAxis.enableGridLine = false;
        myChart.chartOptions.xAxis.enableGridLine = false;
        myChart.chartOptions.yAxis.titleTextOption.fontSize = 18;
        myChart.chartOptions.xAxis.titleTextOption.fontSize = 18;
        myChart.chartOptions.legend.textOption.fontSize = 16;
        myChart.chartOptions.yAxis.labelTextOption.fontSize = 14;
        myChart.chartOptions.xAxis.labelTextOption.fontSize = 14;
        myChart.chartOptions.chartStyles.lineChart.lineWidth = 7;
        myChart.chartOptions.xAxis.minPadding = 10;
        myChart.chartOptions.xAxis.maxPadding = 20;
        myChart.chartOptions.label.enable = false;
        myChart.chartOptions.chartStyles.lineChart.pointOutline = true;
        myChart.chartOptions.chartStyles.lineChart.pointSize = 12;
        myChart.chartOptions.chartStyles.lineChart.pointOutlineColor = new Color(76f / 255f, 76f / 255f, 76f / 255f);

        //color data
        Color Color1 = new Color(255f / 255f, 54f / 255f, 118f / 255f);
        Color Color2 = new Color(18f / 255f, 204f / 255f, 255f / 255f);
        Color[] colorPalette = {Color1, Color2};
        myChart.chartOptions.plotOptions.seriesColors = colorPalette;

        //add chart data
        myChart.chartData = myChart.gameObject.AddComponent<E2ChartData>();
        myChart.chartData.yAxisTitle = "Joint Angle (deg)";
        myChart.chartData.xAxisTitle = "Date";
        myChart.chartData.dateTimeStringFormat = "yyyy-MM-dd";

        RightHandOnClick();
        MCPButtonOnClick();
        ThumbButtonOnClick();
    }

    private void RightHandOnClick()
    {
        rightHandFlag = true;
        RightHand.image.color = new Color(160f / 255f, 223f / 255f, 255f / 255f, 1f);
        LeftHand.image.color = Color.white;
    }

    private void LeftHandOnClick()
    {
        rightHandFlag = false;
        RightHand.image.color = Color.white;
        LeftHand.image.color = new Color(160f / 255f, 223f / 255f, 255f / 255f, 1f);
    }

    private void MCPButtonOnClick()
    {
        MCPFlag = true;
        PIPFlag = false;
        DIPFlag = false;

        JointDescription.text = "Metacarpophalangeal Joint";

        MCPButton.image.color = new Color(160f / 255f, 223f / 255f, 255f / 255f, 1f);
        PIPButton.image.color = Color.white;
        DIPButton.image.color = Color.white;
    }

    private void PIPButtonOnClick()
    {
        MCPFlag = false;
        PIPFlag = true;
        DIPFlag = false;

        if (thumbFlag)
        {
            JointDescription.text = "Interphalangeal Joint";
        }
        else
        {
            JointDescription.text = "Proximal Interphalangeal Joint";
        }

        MCPButton.image.color = Color.white;
        PIPButton.image.color = new Color(160f / 255f, 223f / 255f, 255f / 255f, 1f);
        DIPButton.image.color = Color.white;
    }

    private void DIPButtonOnClick()
    {
        MCPFlag = false;
        PIPFlag = false;
        DIPFlag = true;

        JointDescription.text = "Distal Interphalangeal Joint";

        MCPButton.image.color = Color.white;
        PIPButton.image.color = Color.white;
        DIPButton.image.color = new Color(160f / 255f, 223f / 255f, 255f / 255f, 1f);
    }

    private void signOut()
    {
        LogIn.PatientID = "";
        LogIn.ProvidersName = "";
        LogIn.TodaysDate = "";
        SceneManager.LoadScene(0);
    }

    private void DeleteLastLine()
    {
        if (SetUp.rightHandFlag && SetUp.bothHandFlagAbsolute == false)
        {
            filePath = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_right.txt";
            DeleteLastLine2(filePath,null,false);
            returnToTaskSelectionPopup.SetActive(false);
            SceneManager.LoadScene("Select Task - Right Hand");
        }
        else if (SetUp.rightHandFlag == false && SetUp.bothHandFlagAbsolute == false)
        {
            filePath = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_left.txt";
            DeleteLastLine2(filePath,null,false);
            returnToTaskSelectionPopup.SetActive(false);
            SceneManager.LoadScene("Select Task - Left Hand");
        }
        else if (SetUp.bothHandFlagAbsolute == true)
        {
            filePath = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_right.txt";
            DeleteLastLine2(filePath, null, false);
            filePath = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_left.txt";
            DeleteLastLine2(filePath, null, false);
            returnToTaskSelectionPopup.SetActive(false);
            SetUp.bothHandFlag = true;
            SetUp.rightHandFlag = true;
            SceneManager.LoadScene("Select Task - Right Hand");
        }

    }

    private void DeleteLastLine2(string path1,string path2, bool bothHands)
    {
        if (bothHands == false)
        {
            if (!File.Exists(path1))
            {
                UnityEngine.Debug.LogError("File not found: " + path1);
                return;
            }

            string[] lines = File.ReadAllLines(path1);
            int numLines = lines.Count();
            string lastLine = lines[numLines - 1];
            string firstValue = lastLine.Split(new[] { ' ', '\t' })[0];

            if (firstValue == LogIn.TodaysDate)
            {
                File.WriteAllLines(path1, lines[..^1]);
            }

            SceneManager.LoadScene(1);
        }
        else
        {
            if (!File.Exists(path1))
            {
                UnityEngine.Debug.LogError("File not found: " + path1);
                return;
            }
            else
            {
                string[] lines = File.ReadAllLines(path1);
                int numLines = lines.Count();
                string lastLine = lines[numLines - 1];
                string firstValue = lastLine.Split(new[] { ' ', '\t' })[0];

                if (firstValue == LogIn.TodaysDate)
                {
                    File.WriteAllLines(path1, lines[..^1]);
                }
            }


            if (!File.Exists(path2))
            {
                UnityEngine.Debug.LogError("File not found: " + path2);
                return;
            }
            else
            {
                string[] lines = File.ReadAllLines(path2);
                int numLines = lines.Count();
                string lastLine = lines[numLines - 1];
                string firstValue = lastLine.Split(new[] { ' ', '\t' })[0];

                if (firstValue == LogIn.TodaysDate)
                {
                    File.WriteAllLines(path2, lines[..^1]);
                }
            }
            SceneManager.LoadScene(1);
        }
    }

    private void returnToTaskSelection()
    {
        returnToTaskSelectionPopup.SetActive(true);
    }

    private void AgreedToProceedOnClick()
    {
        DeleteLastLine();
    }

    private void DeclineToProceedOnClick()
    {
        returnToTaskSelectionPopup.SetActive(false);
    }

    private void LockDateFrom(string s)
    {
        if (s != null)
        {
            DateFrom = s;
        }
        else
        {
            if (rightHandFlag)
            {
                DateTo = PopulateSeriesRight.dates[0];
            }
            else
            {
                DateTo = PopulateSeriesLeft.dates[0];
            }
        }
        dateFromChangedFlag = true;
    }

    private void LockDateTo(string s)
    {
        if (s != null)
        {
            DateTo = s;
        }
        else
        {
            if (rightHandFlag)
            {
                DateTo = PopulateSeriesRight.dates[PopulateSeriesRight.dates.Count - 1];
            }
            else
            {
                DateTo = PopulateSeriesLeft.dates[PopulateSeriesLeft.dates.Count - 1];
            }
        }
        dateToChangedFlag = true;
    }

    private List<float> extractNulls(string token, List<float> joint)
    {
        if (token == null)
        {
            joint.Add(float.NaN);
        }
        else if (token != null)
        {
            joint.Add(float.Parse(token));
        }

        return joint;
    }

    private void extractData(string path, string pathSave)
    {
        string[] dataExtracted = Enumerable.Repeat("null", 33).ToArray();

        List<bool> filesExist = new List<bool> {false,false, false, false, false, false, false, false};

        for (int i = 1; i <9; i++)
        {
            string path_new = path + i.ToString() + ".txt";
            
            if (File.Exists(path_new))
            {
                filesExist[i - 1] = true;

                dataExtracted[0] = LogIn.TodaysDate;
                //open files
                string[] lines = File.ReadAllLines(path_new);
                List < List<float>> columns = new List<List<float>>();

                foreach (string line in lines)
                {
                    string[] tokens = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    //UnityEngine.Debug.Log(tokens.Length);

                    for (int j = 0; j < tokens.Length; j++)
                    {
                        if (columns.Count <= j)
                        {
                            columns.Add(new List<float>());
                        }

                        //UnityEngine.Debug.Log(j);
                        if (float.TryParse(tokens[j], out float parsedValue))
                        {
                            columns[j].Add(parsedValue);
                        }
                   }
                }
                //convert columns to arrays
                float[][] columnArrays = new float[columns.Count][];
                for (int j = 0; j < columnArrays.Length; j++)
                {
                    columnArrays[j] = columns[j].ToArray();
                }

                if (i == 1)
                {
                    //extract data
                    float maxValue = columnArrays[2].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[5] = maxValue.ToString();
                    maxValue = columnArrays[3].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[7] = maxValue.ToString();
                    maxValue = columnArrays[4].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[9] = maxValue.ToString();

                    maxValue = columnArrays[5].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[11] = maxValue.ToString();
                    maxValue = columnArrays[6].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[13] = maxValue.ToString();
                    maxValue = columnArrays[7].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[15] = maxValue.ToString();

                    maxValue = columnArrays[8].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[17] = maxValue.ToString();
                    maxValue = columnArrays[9].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[19] = maxValue.ToString();
                    maxValue = columnArrays[10].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[21] = maxValue.ToString();

                    maxValue = columnArrays[11].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[23] = maxValue.ToString();
                    maxValue = columnArrays[12].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[25] = maxValue.ToString();
                    maxValue = columnArrays[13].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[27] = maxValue.ToString();

                    File.Delete(path_new);
                }

                else if (i == 2)
                {
                    float minValue = columnArrays[2].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[6] = minValue.ToString();

                    minValue = columnArrays[5].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[12] = minValue.ToString();

                    minValue = columnArrays[8].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[18] = minValue.ToString();

                    minValue = columnArrays[11].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[24] = minValue.ToString();

                    dataExtracted[31] = columnArrays[15].ToString();
                    dataExtracted[32] = columnArrays[16].ToString();

                    File.Delete(path_new);
                }

                else if (i == 3)
                {
                    float minValue = columnArrays[3].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[8] = minValue.ToString();
                    minValue = columnArrays[4].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[10] = minValue.ToString();

                    minValue = columnArrays[6].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[14] = minValue.ToString();
                    minValue = columnArrays[7].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[16] = minValue.ToString();

                    minValue = columnArrays[9].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[20] = minValue.ToString();
                    minValue = columnArrays[10].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[22] = minValue.ToString();

                    minValue = columnArrays[12].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[26] = minValue.ToString();
                    minValue = columnArrays[13].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[28] = minValue.ToString();

                    File.Delete(path_new);
                }

                else if (i == 5)
                {
                    //float minValue = columnArrays[1].Min();
                    //minValue = MathF.Round(minValue);
                    //dataExtracted[8] = minValue.ToString();

                    float maxValue = columnArrays[0].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[1] = maxValue.ToString();

                    maxValue = columnArrays[1].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[3] = maxValue.ToString();

                    File.Delete(path_new);
                }


                else if (i == 7)
                {
                    float minValue = columnArrays[0].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[2] = minValue.ToString();
                    minValue = columnArrays[1].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[4] = minValue.ToString();

                    File.Delete(path_new);
                }

                else if (i == 8)
                {
                    float maxValue = columnArrays[14].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[29] = maxValue.ToString();
                    float minValue = columnArrays[14].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[30] = minValue.ToString();

                    File.Delete(path_new);
                }

            }
            else
            {
                filesExist[i - 1] = false;
            }
        }


        bool hasTrue = false;
        foreach (bool value in filesExist)
        {
            if (value)
            {
                hasTrue = true;
                break;
            }
        }

        string dataSavePath = pathSave;

        string lineWrite = string.Join("\t",dataExtracted);
        
        if (dataExtracted != null && dataExtracted.Length > 0 && hasTrue)
        {
            if (File.Exists(dataSavePath))
            {
                using (StreamWriter writer = new StreamWriter(dataSavePath, append: true))
                {
                    writer.WriteLine(lineWrite);
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(dataSavePath, append: true))
                {
                    string header = "Date" + "\t" + "ThumbMCP Ext" + "\t" + "ThumbMCP Flex" + "\t" +
                        "ThumbIP Ext" + "\t" + "ThumbIP Flex" + "\t" +
                        "IndexMCP Ext" + "\t" + "IndexMCP Flex" + "\t" + "IndexPIP Ext" + "\t" + "IndexPIP Flex" + "\t" + "IndexDIP Ext" + "\t" + "IndexDIP Flex" + "\t" +
                        "MiddleMCP Ext" + "\t" + "MiddleMCP Flex" + "\t" + "MiddlePIP Ext" + "\t" + "MiddlePIP Flex" + "\t" + "MiddleDIP Ext" + "\t" + "MiddleDIP Flex" + "\t" +
                        "RingMCP Ext" + "\t" + "RingMCP Flex" + "\t" + "RingPIP Ext" + "\t" + "RingPIP Flex" + "\t" + "RingDIP Ext" + "\t" + "RingDIP Flex" + "\t" +
                        "LittleMCP Ext" + "\t" + "LittleMCP Flex" + "\t" + "LittlePIP Ext" + "\t" + "LittlePIP Flex" + "\t" + "LittleDIP Ext" + "\t" + "LittleDIP Flex" + "\t" +
                        "Wrist Ext" + "\t" + "Wrist Flex" + "\t" + "Webcam Flag" + "\t" + "Provider";

                    writer.WriteLine(header);
                    writer.WriteLine(lineWrite);
                }
            }
        }
       
    }
    public void updateGraph()
    {
        GraphState currentState = new GraphState
        {
            thumbFlag = thumbFlag,
            indexFlag = indexFlag,
            middleFlag = middleFlag,
            ringFlag = ringFlag,
            pinkyFlag = pinkyFlag,
            wristFlag = wristFlag,
            MCPFlag = MCPFlag,
            PIPFlag = PIPFlag,
            DIPFlag = DIPFlag,
            rightHandFlag = rightHandFlag
        };

        if (currentState.Equals(previousState))
        {
            // No relevant flag has changed, skip update
            return;
        }

        previousState = currentState; // Save new state

        if (thumbFlag)
        {
            if (MCPFlag)
            {
                //add series into series list
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbMCP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series1);
                    myChart.chartData.series.Add(PopulateSeriesRight.series2);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbMCP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series1);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series2);
                }
                myChart.UpdateChart();
            }
            
            else if (PIPFlag)
            {
                //add series into series list
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series3);
                    myChart.chartData.series.Add(PopulateSeriesRight.series4);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series3);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series4);
                }
                myChart.UpdateChart();
            }
        }
        else if (indexFlag)
        {
            if(MCPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexMCP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series5);
                    myChart.chartData.series.Add(PopulateSeriesRight.series6);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexMCP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series5);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series6);
                }
                myChart.UpdateChart();
            }
            else if (PIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexPIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series7);
                    myChart.chartData.series.Add(PopulateSeriesRight.series8);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexPIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series7);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series8);
                }
                myChart.UpdateChart();
            }
            else if (DIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexDIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series9);
                    myChart.chartData.series.Add(PopulateSeriesRight.series10);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexDIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series9);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series10);
                }
                myChart.UpdateChart();
            }
        }

        else if (middleFlag)
        {
            if (MCPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleMCP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series11);
                    myChart.chartData.series.Add(PopulateSeriesRight.series12);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleMCP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series11);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series12);
                }
                myChart.UpdateChart();
            }
            else if (PIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddlePIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series13);
                    myChart.chartData.series.Add(PopulateSeriesRight.series14);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddlePIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series13);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series14);
                }
                myChart.UpdateChart();
            }
            else if (DIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleDIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series15);
                    myChart.chartData.series.Add(PopulateSeriesRight.series16);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleDIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series15);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series16);
                }
                myChart.UpdateChart();
            }
        }
        else if (ringFlag)
        {
            if (MCPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingMCP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series17);
                    myChart.chartData.series.Add(PopulateSeriesRight.series18);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingMCP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series17);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series18);
                }
                myChart.UpdateChart();
            }
            else if (PIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingPIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series19);
                    myChart.chartData.series.Add(PopulateSeriesRight.series20);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingPIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series19);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series20);
                }
                myChart.UpdateChart();
            }
            else if (DIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingDIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series21);
                    myChart.chartData.series.Add(PopulateSeriesRight.series22);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingDIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series21);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series22);
                }
                myChart.UpdateChart();
            }
        }
        else if (pinkyFlag)
        {
            if (MCPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyMCP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series23);
                    myChart.chartData.series.Add(PopulateSeriesRight.series24);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyMCP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series23);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series24);
                }
                myChart.UpdateChart();
            }
            else if (PIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyPIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series25);
                    myChart.chartData.series.Add(PopulateSeriesRight.series26);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyPIP_left ;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series25);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series26);
                }
                myChart.UpdateChart();
            }
            else if (DIPFlag)
            {
                myChart.chartData.series = new List<E2ChartData.Series>();
                if (rightHandFlag)
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyDIP;
                    myChart.chartData.series.Add(PopulateSeriesRight.series27);
                    myChart.chartData.series.Add(PopulateSeriesRight.series28);
                }
                else
                {
                    HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyDIP_left;
                    myChart.chartData.series.Add(PopulateSeriesLeft.series27);
                    myChart.chartData.series.Add(PopulateSeriesLeft.series28);
                }
                myChart.UpdateChart();
            }  
        }
        else if (wristFlag)
        {
            myChart.chartData.series = new List<E2ChartData.Series>();
            if (rightHandFlag)
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageWrist;
                myChart.chartData.series.Add(PopulateSeriesRight.series29);
                myChart.chartData.series.Add(PopulateSeriesRight.series30);
            }
            else
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageWrist_left;
                myChart.chartData.series.Add(PopulateSeriesLeft.series29);
                myChart.chartData.series.Add(PopulateSeriesLeft.series30);
            }
            myChart.UpdateChart();
        }
    }

    private void changeDateRange()
    {
        if (dateFromChangedFlag || dateToChangedFlag)
        {
            if (DateTo == null)
            {
                if (rightHandFlag)
                {
                    DateTo = PopulateSeriesRight.dates[PopulateSeriesRight.dates.Count - 1];
                }
                else
                {
                    DateTo = PopulateSeriesLeft.dates[PopulateSeriesLeft.dates.Count - 1];
                }
            }
            if (DateFrom == null)
            {
                if (rightHandFlag)
                {
                    DateTo = PopulateSeriesRight.dates[0];
                }
                else
                {
                    DateTo = PopulateSeriesLeft.dates[0];
                }
            }
            myChart.chartOptions.xAxis.autoAxisRange = false;
            myChart.chartOptions.xAxis.rangeDateTimeStringFormat = "yyyy-MM-dd";
            myChart.chartOptions.xAxis.minDateTimeString = DateFrom;
            myChart.chartOptions.xAxis.maxDateTimeString = DateTo;
            myChart.UpdateChart();
            dateFromChangedFlag = false;
            dateToChangedFlag = false;
       
        }
    }

    private void ThumbButtonOnClick()
    {
        if (rightHandFlag)
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbMCP;
        }
        else
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbMCP_left;
        }       

        thumbFlag = true;
        indexFlag = false;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = false;

        ThumbButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        //hide one of the buttons (DIP)
        DIPButton.interactable = false;
        PIPButton.interactable = true;
        //rename the PIP button to IP
        PIPButton.GetComponentInChildren<TextMeshProUGUI>().text = "IP";
        MCPButton.GetComponentInChildren<TextMeshProUGUI>().text = "MCP";
    }

    private void IndexButtonOnClick()
    {
        if (rightHandFlag)
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexMCP;
        }
        else
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexMCP_left;
        }
        

        thumbFlag = false;
        indexFlag = true;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = false;

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        //unhide one of the buttons (DIP)
        DIPButton.interactable = true;
        PIPButton.interactable = true;
        //rename the PIP button to IP
        PIPButton.GetComponentInChildren<TextMeshProUGUI>().text = "PIP";
        MCPButton.GetComponentInChildren<TextMeshProUGUI>().text = "MCP";
    }

    private void MiddleButtonOnClick()
    {
        if (rightHandFlag)
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleMCP;
        }
        else
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleMCP_left;
        }

        thumbFlag = false;
        indexFlag = false;
        middleFlag = true;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = false;

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        //unhide one of the buttons (DIP)
        DIPButton.interactable = true;
        PIPButton.interactable = true;
        //rename the PIP button to IP
        PIPButton.GetComponentInChildren<TextMeshProUGUI>().text = "PIP";
        MCPButton.GetComponentInChildren<TextMeshProUGUI>().text = "MCP";
    }

    private void RingButtonOnClick()
    {
        if (rightHandFlag)
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingMCP;
        }
        else
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingMCP_left;
        }

        thumbFlag = false;
        indexFlag = false;
        middleFlag = false;
        ringFlag = true;
        pinkyFlag = false;
        wristFlag = false;

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        //unhide one of the buttons (DIP)
        DIPButton.interactable = true;
        PIPButton.interactable = true;
        //rename the PIP button to IP
        PIPButton.GetComponentInChildren<TextMeshProUGUI>().text = "PIP";
        MCPButton.GetComponentInChildren<TextMeshProUGUI>().text = "MCP";
    }

    private void PinkyButtonOnClick()
    {
        if (rightHandFlag)
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyMCP;
        }
        else
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyMCP_left;
        }       

        thumbFlag = false;
        indexFlag = false;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = true;
        wristFlag = false;

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        WristButton.image.color = Color.white;

        //unhide one of the buttons (DIP)
        DIPButton.interactable = true;
        PIPButton.interactable = true;
        //rename the PIP button to IP
        PIPButton.GetComponentInChildren<TextMeshProUGUI>().text = "PIP";
        MCPButton.GetComponentInChildren<TextMeshProUGUI>().text = "MCP";
    }
    private void WristButtonOnClick()
    {
        if (rightHandFlag)
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageWrist;
        }
        else
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageWrist_left;
        }

        thumbFlag = false;
        indexFlag = false;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = true;

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = new Color(160f / 255f, 223f / 255f, 255f / 255f, 1f);

        //hide one of the buttons (DIP)
        DIPButton.interactable = false;
        PIPButton.interactable = false;
        //rename the PIP button to IP
        MCPButton.GetComponentInChildren<TextMeshProUGUI>().text = "Flex/Ext";
    }
    private void updateChartTitle()
    {
        if (rightHandFlag)
        {
            if (thumbFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Right Hand - Thumb - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Right Hand - Thumb - IP Joint";
                }
            }
            else if (indexFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Right Hand - Index Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Right Hand - Index Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Right Hand - Index Finger - DIP Joint";
                }
            }
            else if (middleFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Right Hand - Middle Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Right Hand - Middle Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Right Hand - Middle Finger - DIP Joint";
                }
            }
            else if (ringFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Right Hand - Ring Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Right Hand - Ring Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Right Hand - Ring Finger - DIP Joint";
                }
            }
            else if (pinkyFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Right Hand - Pinky Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Right Hand - Pinky Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Right Hand - Pinky Finger - DIP Joint";
                }
            }
            else if (wristFlag)
            {
                chartTitle.text = "Right Hand - Wrist - Flex/Ext";
            }
        }
        else
        {
            if (thumbFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Left Hand - Thumb - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Left Hand - Thumb - IP Joint";
                }
            }
            else if (indexFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Left Hand - Index Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Left Hand - Index Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Left Hand - Index Finger - DIP Joint";
                }
            }
            else if (middleFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Left Hand - Middle Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Left Hand - Middle Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Left Hand - Middle Finger - DIP Joint";
                }
            }
            else if (ringFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Left Hand - Ring Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Left Hand - Ring Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Left Hand - Ring Finger - DIP Joint";
                }
            }
            else if (pinkyFlag)
            {
                if (MCPFlag)
                {
                    chartTitle.text = "Left Hand - Pinky Finger - MCP Joint";
                }
                else if (PIPFlag)
                {
                    chartTitle.text = "Left Hand - Pinky Finger - PIP Joint";
                }
                else if (DIPFlag)
                {
                    chartTitle.text = "Left Hand - Pinky Finger - DIP Joint";
                }
            }
            else if (wristFlag)
            {
                chartTitle.text = "Left Hand - Wrist - Flex/Ext";
            }
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        string selectedOption = FormatDropdown.options[index].text;
    }
    private void ExportToCSV()
    {
        //CSV of the table values
        if (TableToggle.isOn)
        {
            string csvFilePath_right = Application.dataPath + "/" + LogIn.PatientID + "_" +
            LogIn.TodaysDate + "_jointROM_right.csv";
            string dataFilePath_right = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_right.txt";
            string csvFilePath_left = Application.dataPath + "/" + LogIn.PatientID + "_" +
               LogIn.TodaysDate + "_jointROM_left.csv";
            string dataFilePath_left = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_left.txt";
            Functions.SaveCSV(csvFilePath_right, dataFilePath_right);
            Functions.SaveCSV(csvFilePath_left, dataFilePath_left);

            StartCoroutine(WaitAndCheckFile(csvFilePath_left));
        }
        //pdf of the plot
        else if (GraphToggle.isOn)
        {
            CoroutineRunner.Instance.StartCoroutine(ImportImages.CaptureAllImagesCoroutine());

            string pdfFile = Directory.GetCurrentDirectory() + "/Assets/" + LogIn.PatientID + "_" + LogIn.TodaysDate + "_data.pdf";
            StartCoroutine(WaitAndCheckFile(pdfFile));
        }
    }
    IEnumerator WaitAndCheckFile(string filePath)
    {
        yield return new WaitForSeconds(3f); // Wait 5 seconds

        if (File.Exists(filePath))
        {
            successText.text = "Export was successful!";
            successText.color = Color.green;
        }
        else
        {
            successText.text = "Export was not successful!";
            successText.color = Color.red;
        }

        StartCoroutine(HideTextAfterDelay(2f)); // Hide after 2 more seconds
    }

    IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        successText.text = "";
    }

    public struct GraphState
    {
        public bool thumbFlag, indexFlag, middleFlag, ringFlag, pinkyFlag, wristFlag;
        public bool MCPFlag, PIPFlag, DIPFlag;
        public bool rightHandFlag;

        public override bool Equals(object obj)
        {
            if (!(obj is GraphState)) return false;
            var other = (GraphState)obj;
            return thumbFlag == other.thumbFlag &&
                   indexFlag == other.indexFlag &&
                   middleFlag == other.middleFlag &&
                   ringFlag == other.ringFlag &&
                   pinkyFlag == other.pinkyFlag &&
                   wristFlag == other.wristFlag &&
                   MCPFlag == other.MCPFlag &&
                   PIPFlag == other.PIPFlag &&
                   DIPFlag == other.DIPFlag &&
                   rightHandFlag == other.rightHandFlag;
        }

        public override int GetHashCode()
        {
            return (thumbFlag, indexFlag, middleFlag, ringFlag, pinkyFlag, wristFlag, MCPFlag, PIPFlag, DIPFlag, rightHandFlag).GetHashCode();
        }
    }

    private void AddNotes()
    {
        string notesText = LogIn.TodaysDate + "\t" + NotesInput.text;
        notesPath = Directory.GetCurrentDirectory() + "/Assets/" + LogIn.PatientID + "_notes.csv";

        if (!string.IsNullOrEmpty(notesText))
        {
            File.AppendAllText(notesPath, notesText + "\n");
            UnityEngine.Debug.Log("Saved: " + notesText);
            NotesInput.text = ""; // clear after saving
        }
    }

    //My own Toggle Group function
    private void ToggleGroup()
    {
        if (GraphToggle.isOn)
        {
            TableToggle.isOn = false;
        }
        else if (TableToggle.isOn)
        {
            GraphToggle.isOn = false;
        }
    }

    private void FixedUpdate()
    {
        //UnityEngine.Debug.Log(DateTo);
        changeDateRange();
        updateChartTitle();
        updateGraph();
        ToggleGroup();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}
    
