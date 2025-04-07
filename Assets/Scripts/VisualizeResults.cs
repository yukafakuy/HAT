using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using E2C;
using UnityEngine.UI;
using Unity;
using TMPro;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class VisualizeResults : MonoBehaviour
{
    public E2Chart myChart;
    public Button ThumbButton, IndexButton, MiddleButton, RingButton, PinkyButton, WristButton;
    public Button forwardButton, backButton, returnToTaskSelectionButton, SignOutButton, exportToCSVButton;
    private bool thumbFlag, indexFlag, middleFlag, ringFlag, pinkyFlag, wristFlag = false;
    private List<string> jointList = new List<string>();
    public TMP_Text chartTitle, successText;
    private int jointNum = 0;

    public static bool returnToTaskSelected = false;

    //data
    private string filePath;
    private List<string> dates = new List<string>();
    private List<float> thumbMCPmin = new List<float>();
    private List<float> thumbMCPmax = new List<float>();
    private List<float> thumbIPmin = new List<float>();
    private List<float> thumbIPmax = new List<float>();
    private List<float> thumbCMCxmin = new List<float>();
    private List<float> thumbCMCxmax = new List<float>();
    private List<float> thumbCMCymin = new List<float>();
    private List<float> thumbCMCymax = new List<float>();
    private List<float> thumbCMCzmin = new List<float>();
    private List<float> thumbCMCzmax = new List<float>();

    private List<float> indexMCPmin = new List<float>();
    private List<float> indexMCPmax = new List<float>();
    private List<float> indexPIPmin = new List<float>();
    private List<float> indexPIPmax = new List<float>();
    private List<float> indexDIPmin = new List<float>();
    private List<float> indexDIPmax = new List<float>();

    private List<float> middleMCPmin = new List<float>();
    private List<float> middleMCPmax = new List<float>();
    private List<float> middlePIPmin = new List<float>();
    private List<float> middlePIPmax = new List<float>();
    private List<float> middleDIPmin = new List<float>();
    private List<float> middleDIPmax = new List<float>();

    private List<float> ringMCPmin = new List<float>();
    private List<float> ringMCPmax = new List<float>();
    private List<float> ringPIPmin = new List<float>();
    private List<float> ringPIPmax = new List<float>();
    private List<float> ringDIPmin = new List<float>();
    private List<float> ringDIPmax = new List<float>();

    private List<float> pinkyMCPmin = new List<float>();
    private List<float> pinkyMCPmax = new List<float>();
    private List<float> pinkyPIPmin = new List<float>();
    private List<float> pinkyPIPmax = new List<float>();
    private List<float> pinkyDIPmin = new List<float>();
    private List<float> pinkyDIPmax = new List<float>();

    private List<float> wristFlex = new List<float>();
    private List<float> wristExtend = new List<float>();

    //Series
    private E2ChartData.Series series1 = new E2ChartData.Series();
    private E2ChartData.Series series2 = new E2ChartData.Series();
    private E2ChartData.Series series3 = new E2ChartData.Series();
    private E2ChartData.Series series4 = new E2ChartData.Series();
    private E2ChartData.Series series5 = new E2ChartData.Series();
    private E2ChartData.Series series6 = new E2ChartData.Series();
    private E2ChartData.Series series7 = new E2ChartData.Series();
    private E2ChartData.Series series8 = new E2ChartData.Series();
    private E2ChartData.Series series9 = new E2ChartData.Series();
    private E2ChartData.Series series10 = new E2ChartData.Series();
    private E2ChartData.Series series11 = new E2ChartData.Series();
    private E2ChartData.Series series12 = new E2ChartData.Series();
    private E2ChartData.Series series13 = new E2ChartData.Series();
    private E2ChartData.Series series14 = new E2ChartData.Series();
    private E2ChartData.Series series15 = new E2ChartData.Series();
    private E2ChartData.Series series16 = new E2ChartData.Series();
    private E2ChartData.Series series17 = new E2ChartData.Series();
    private E2ChartData.Series series18 = new E2ChartData.Series();
    private E2ChartData.Series series19 = new E2ChartData.Series();
    private E2ChartData.Series series20 = new E2ChartData.Series();
    private E2ChartData.Series series21 = new E2ChartData.Series();
    private E2ChartData.Series series22 = new E2ChartData.Series();
    private E2ChartData.Series series23 = new E2ChartData.Series();
    private E2ChartData.Series series24 = new E2ChartData.Series();
    private E2ChartData.Series series25 = new E2ChartData.Series();
    private E2ChartData.Series series26 = new E2ChartData.Series();
    private E2ChartData.Series series27 = new E2ChartData.Series();
    private E2ChartData.Series series28 = new E2ChartData.Series();
    private E2ChartData.Series series29 = new E2ChartData.Series();
    private E2ChartData.Series series30 = new E2ChartData.Series();
    private E2ChartData.Series series31 = new E2ChartData.Series();
    private E2ChartData.Series series32 = new E2ChartData.Series();
    private E2ChartData.Series series33 = new E2ChartData.Series();
    private E2ChartData.Series series34 = new E2ChartData.Series();
    private E2ChartData.Series series35 = new E2ChartData.Series();
    private E2ChartData.Series series36 = new E2ChartData.Series();

    //circles
    public Button circle1, circle2, circle3, circle4, circle5;

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

    // Start is called before the first frame update
    void Start()
    {

        successText.enabled = false;

        DateFromField.onValueChanged.AddListener(LockDateFrom);
        DateToField.onValueChanged.AddListener(LockDateTo);

        if (LogIn.TodaysDate == null)
        {
            LogIn.TodaysDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        //extract and save data
        extractData();

        //load the data AND flip the signs on each of them
        //(Extension is positive, hyper extensive is negative)
        filePath = Application.dataPath + "/dataFile.txt";

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            bool firstLine = true;

            foreach (string line in lines)
            {
                if (firstLine)
                {
                    firstLine = false;
                    continue;
                }

                string[] tokens = line.Split(new[] { ' ', '\t' });

                dates.Add(tokens[0]);
                thumbMCPmax.Add(-1f*parseString(tokens[1]));
                thumbMCPmin.Add(-1f * parseString(tokens[2]));
                thumbIPmax.Add(-1f * parseString(tokens[3]));
                thumbIPmin.Add(-1f * parseString(tokens[4]));
                /*
                thumbCMCxmax.Add(parseString(tokens[5]));
                thumbCMCxmin.Add(parseString(tokens[6]));
                thumbCMCymax.Add(parseString(tokens[7]));
                thumbCMCymin.Add(parseString(tokens[8]));
                thumbCMCzmax.Add(parseString(tokens[9]));
                thumbCMCzmin.Add(parseString(tokens[10]));
                */

                indexMCPmax.Add(-1f * parseString(tokens[11]));
                indexMCPmin.Add(-1f * parseString(tokens[12]));
                indexPIPmax.Add(-1f * parseString(tokens[13]));
                indexPIPmin.Add(-1f * parseString(tokens[14]));
                indexDIPmax.Add(-1f * parseString(tokens[15]));
                indexDIPmin.Add(-1f * parseString(tokens[16]));

                middleMCPmax.Add(-1f*parseString(tokens[17]));
                middleMCPmin.Add(-1f*parseString(tokens[18]));
                middlePIPmax.Add(-1f*parseString(tokens[19]));
                middlePIPmin.Add(-1f*parseString(tokens[20]));
                middleDIPmax.Add(-1f*parseString(tokens[21]));
                middleDIPmin.Add(-1f*parseString(tokens[22]));

                ringMCPmax.Add(-1f*parseString(tokens[23]));
                ringMCPmin.Add(-1f*parseString(tokens[24]));
                ringPIPmax.Add(-1f*parseString(tokens[25]));
                ringPIPmin.Add(-1f*parseString(tokens[26]));
                ringDIPmax.Add(-1f*parseString(tokens[27]));
                ringDIPmin.Add(-1f*parseString(tokens[28]));

                pinkyMCPmax.Add(-1f*parseString(tokens[29]));
                pinkyMCPmin.Add(-1f*parseString(tokens[30]));
                pinkyPIPmax.Add(-1f*parseString(tokens[31]));
                pinkyPIPmin.Add(-1f*parseString(tokens[32]));
                pinkyDIPmax.Add(-1f*parseString(tokens[33]));
                pinkyDIPmin.Add(-1f*parseString(tokens[34]));

                wristExtend.Add(-1f * parseString(tokens[35]));
                wristFlex.Add(-1f * parseString(tokens[36]));
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("An error occured {ex.Message}");
        }

        DateFrom = dates[0];
        DateTo = dates[dates.Count-1];

        //buttons
        ThumbButton.onClick.AddListener(ThumbButtonOnClick);
        IndexButton.onClick.AddListener(IndexButtonOnClick);
        MiddleButton.onClick.AddListener(MiddleButtonOnClick);
        RingButton.onClick.AddListener(RingButtonOnClick);
        PinkyButton.onClick.AddListener(PinkyButtonOnClick);
        WristButton.onClick.AddListener(WristButtonOnClick);
        forwardButton.onClick.AddListener(ForwardButtonOnClick);
        backButton.onClick.AddListener(BackButtonOnClick);
        returnToTaskSelectionButton.onClick.AddListener(returnToTaskSelection);
        SignOutButton.onClick.AddListener(signOut);
        exportToCSVButton.onClick.AddListener(ExportToCSV);

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

        //create new series
        //Thumb MCP
        series1.name = "Extension";
        series1.dataY = new List<float>();
        series1.dateTimeString = new List<string>();
        series1.dateTimeTick = new List<long>();
        series1.dataY.AddRange(thumbMCPmax);
        series1.dateTimeString.AddRange(dates);       
        series2.name = "Flexion";
        series2.dataY = new List<float>();
        series2.dateTimeString = new List<string>();
        series2.dateTimeTick = new List<long>();
        series2.dataY.AddRange(thumbMCPmin);
        series2.dateTimeString.AddRange(dates);

        //Thumb IP
        series3.name = "Extension";
        series3.dataY = new List<float>();
        series3.dateTimeString = new List<string>();
        series3.dateTimeTick = new List<long>();
        series3.dataY.AddRange(thumbIPmax);
        series3.dateTimeString.AddRange(dates);
        series4.name = "Flexion";
        series4.dataY = new List<float>();
        series4.dateTimeString = new List<string>();
        series4.dateTimeTick = new List<long>();
        series4.dataY.AddRange(thumbIPmin);
        series4.dateTimeString.AddRange(dates);

        //Index MCP
        series11.name = "Extension";
        series11.dataY = new List<float>();
        series11.dateTimeString = new List<string>();
        series11.dateTimeTick = new List<long>();
        series11.dataY.AddRange(indexMCPmax);
        series11.dateTimeString.AddRange(dates);
        series12.name = "Flexion";
        series12.dataY = new List<float>();
        series12.dateTimeString = new List<string>();
        series12.dateTimeTick = new List<long>();
        series12.dataY.AddRange(indexMCPmin);
        series12.dateTimeString.AddRange(dates);

        //Index PIP
        series13.name = "Extension";
        series13.dataY = new List<float>();
        series13.dateTimeString = new List<string>();
        series13.dateTimeTick = new List<long>();
        series13.dataY.AddRange(indexPIPmax);
        series13.dateTimeString.AddRange(dates);
        series14.name = "Flexion";
        series14.dataY = new List<float>();
        series14.dateTimeString = new List<string>();
        series14.dateTimeTick = new List<long>();
        series14.dataY.AddRange(indexPIPmin);
        series14.dateTimeString.AddRange(dates);

        //Index DIP
        series15.name = "Extension";
        series15.dataY = new List<float>();
        series15.dateTimeString = new List<string>();
        series15.dateTimeTick = new List<long>();
        series15.dataY.AddRange(indexDIPmax);
        series15.dateTimeString.AddRange(dates);
        series16.name = "Flexion";
        series16.dataY = new List<float>();
        series16.dateTimeString = new List<string>();
        series16.dateTimeTick = new List<long>();
        series16.dataY.AddRange(indexDIPmin);
        series16.dateTimeString.AddRange(dates);

        //middle MCP
        series17.name = "Extension";
        series17.dataY = new List<float>();
        series17.dateTimeString = new List<string>();
        series17.dateTimeTick = new List<long>();
        series17.dataY.AddRange(middleMCPmax);
        series17.dateTimeString.AddRange(dates);
        series18.name = "Flexion";
        series18.dataY = new List<float>();
        series18.dateTimeString = new List<string>();
        series18.dateTimeTick = new List<long>();
        series18.dataY.AddRange(middleMCPmin);
        series18.dateTimeString.AddRange(dates);

        //middle PIP
        series19.name = "Extension";
        series19.dataY = new List<float>();
        series19.dateTimeString = new List<string>();
        series19.dateTimeTick = new List<long>();
        series19.dataY.AddRange(middlePIPmax);
        series19.dateTimeString.AddRange(dates);
        series20.name = "Flexion";
        series20.dataY = new List<float>();
        series20.dateTimeString = new List<string>();
        series20.dateTimeTick = new List<long>();
        series20.dataY.AddRange(middlePIPmin);
        series20.dateTimeString.AddRange(dates);

        //middle DIP
        series21.name = "Extension";
        series21.dataY = new List<float>();
        series21.dateTimeString = new List<string>();
        series21.dateTimeTick = new List<long>();
        series21.dataY.AddRange(middleDIPmax);
        series21.dateTimeString.AddRange(dates);
        series22.name = "Flexion";
        series22.dataY = new List<float>();
        series22.dateTimeString = new List<string>();
        series22.dateTimeTick = new List<long>();
        series22.dataY.AddRange(middleDIPmin);
        series22.dateTimeString.AddRange(dates);

        //ring MCP
        series23.name = "Extension";
        series23.dataY = new List<float>();
        series23.dateTimeString = new List<string>();
        series23.dateTimeTick = new List<long>();
        series23.dataY.AddRange(ringMCPmax);
        series23.dateTimeString.AddRange(dates);
        series24.name = "Flexion";
        series24.dataY = new List<float>();
        series24.dateTimeString = new List<string>();
        series24.dateTimeTick = new List<long>();
        series24.dataY.AddRange(ringMCPmin);
        series24.dateTimeString.AddRange(dates);

        //ring PIP
        series25.name = "Extension";
        series25.dataY = new List<float>();
        series25.dateTimeString = new List<string>();
        series25.dateTimeTick = new List<long>();
        series25.dataY.AddRange(ringPIPmax);
        series25.dateTimeString.AddRange(dates);
        series26.name = "Flexion";
        series26.dataY = new List<float>();
        series26.dateTimeString = new List<string>();
        series26.dateTimeTick = new List<long>();
        series26.dataY.AddRange(ringPIPmin);
        series26.dateTimeString.AddRange(dates);


        //ring DIP
        series27.name = "Extension";
        series27.dataY = new List<float>();
        series27.dateTimeString = new List<string>();
        series27.dateTimeTick = new List<long>();
        series27.dataY.AddRange(ringDIPmax);
        series27.dateTimeString.AddRange(dates);
        series28.name = "Flexion";
        series28.dataY = new List<float>();
        series28.dateTimeString = new List<string>();
        series28.dateTimeTick = new List<long>();
        series28.dataY.AddRange(ringDIPmin);
        series28.dateTimeString.AddRange(dates);

        //pinky MCP
        series29.name = "Extension";
        series29.dataY = new List<float>();
        series29.dateTimeString = new List<string>();
        series29.dateTimeTick = new List<long>();
        series29.dataY.AddRange(pinkyMCPmax);
        series29.dateTimeString.AddRange(dates);
        series30.name = "Flexion";
        series30.dataY = new List<float>();
        series30.dateTimeString = new List<string>();
        series30.dateTimeTick = new List<long>();
        series30.dataY.AddRange(pinkyMCPmin);
        series30.dateTimeString.AddRange(dates);

        //pinky PIP
        series31.name = "Extension";
        series31.dataY = new List<float>();
        series31.dateTimeString = new List<string>();
        series31.dateTimeTick = new List<long>();
        series31.dataY.AddRange(pinkyPIPmax);
        series31.dateTimeString.AddRange(dates);
        series32.name = "Flexion";
        series32.dataY = new List<float>();
        series32.dateTimeString = new List<string>();
        series32.dateTimeTick = new List<long>();
        series32.dataY.AddRange(pinkyPIPmin);
        series32.dateTimeString.AddRange(dates);

        //pinky DIP
        series33.name = "Extension";
        series33.dataY = new List<float>();
        series33.dateTimeString = new List<string>();
        series33.dateTimeTick = new List<long>();
        series33.dataY.AddRange(pinkyDIPmax);
        series33.dateTimeString.AddRange(dates);
        series34.name = "Flexion";
        series34.dataY = new List<float>();
        series34.dateTimeString = new List<string>();
        series34.dateTimeTick = new List<long>();
        series34.dataY.AddRange(pinkyDIPmin);
        series34.dateTimeString.AddRange(dates);

        //wrist
        series35.name = "Extension";
        series35.dataY = new List<float>();
        series35.dateTimeString = new List<string>();
        series35.dateTimeTick = new List<long>();
        series35.dataY.AddRange(wristExtend);
        series35.dateTimeString.AddRange(dates);
        series36.name = "Flexion";
        series36.dataY = new List<float>();
        series36.dateTimeString = new List<string>();
        series36.dateTimeTick = new List<long>();
        series36.dataY.AddRange(wristFlex);
        series36.dateTimeString.AddRange(dates);

        //hide some data
        //series1.dataY[1] = float.NaN;

        ThumbButtonOnClick();
    }

    private void signOut()
    {
        SceneManager.LoadScene(0);
    }

    private void DeleteLastLine()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        int numLines = lines.Count();
        string lastLine = lines[numLines - 1];
        string firstValue = lastLine.Split(new[] { ' ', '\t' })[0];

        if(firstValue == LogIn.TodaysDate)
        {
            File.WriteAllLines(filePath, lines[..^1]);
        }

        SceneManager.LoadScene(1);
    }

    private void returnToTaskSelection()
    {
        DeleteLastLine();
    }

    private void LockDateFrom(string s)
    {
        DateFrom = s;
        dateFromChangedFlag = true;
    }

    private void LockDateTo(string s)
    {
        DateTo = s;
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

    private float parseString(string token)
    {
        if (float.TryParse(token, out float result)) { } ;

        return result;
        
    }

    private void extractData()
    {
        string[] dataExtracted = Enumerable.Repeat("null", 37).ToArray();

        List<bool> filesExist = new List<bool> {false,false, false, false, false, false, false, false};

        for (int i = 1; i <9; i++)
        {
            string path = Application.dataPath + "/rightHand" + i.ToString() + ".txt";
            if (File.Exists(path))
            {
                UnityEngine.Debug.Log(path);

                UnityEngine.Debug.Log(i.ToString());
                filesExist[i - 1] = true;

                dataExtracted[0] = LogIn.TodaysDate;
                //open files
                string[] lines = File.ReadAllLines(path);
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
                    dataExtracted[11] = maxValue.ToString();
                    maxValue = columnArrays[3].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[13] = maxValue.ToString();
                    maxValue = columnArrays[4].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[15] = maxValue.ToString();

                    maxValue = columnArrays[5].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[17] = maxValue.ToString();
                    maxValue = columnArrays[6].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[19] = maxValue.ToString();
                    maxValue = columnArrays[7].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[21] = maxValue.ToString();

                    maxValue = columnArrays[8].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[23] = maxValue.ToString();
                    maxValue = columnArrays[9].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[25] = maxValue.ToString();
                    maxValue = columnArrays[10].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[27] = maxValue.ToString();

                    maxValue = columnArrays[11].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[29] = maxValue.ToString();
                    maxValue = columnArrays[12].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[31] = maxValue.ToString();
                    maxValue = columnArrays[13].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[33] = maxValue.ToString();

                    File.Delete(path);
                }

                else if (i == 2)
                {
                    float minValue = columnArrays[2].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[12] = minValue.ToString();

                    minValue = columnArrays[5].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[18] = minValue.ToString();

                    minValue = columnArrays[8].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[24] = minValue.ToString();

                    minValue = columnArrays[11].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[30] = minValue.ToString();

                    minValue = columnArrays[3].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[14] = minValue.ToString();
                    minValue = columnArrays[4].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[16] = minValue.ToString();

                    minValue = columnArrays[6].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[20] = minValue.ToString();
                    minValue = columnArrays[7].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[22] = minValue.ToString();

                    minValue = columnArrays[9].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[26] = minValue.ToString();
                    minValue = columnArrays[10].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[28] = minValue.ToString();

                    minValue = columnArrays[12].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[32] = minValue.ToString();
                    minValue = columnArrays[13].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[34] = minValue.ToString();


                    File.Delete(path);
                }

                /*
                else if (i == 3)
                {
                    float minValue = columnArrays[6].Min();
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

                    minValue = columnArrays[15].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[32] = minValue.ToString();
                    minValue = columnArrays[16].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[34] = minValue.ToString();

                    File.Delete(path);
                }
                */
                /*
                else if (i == 4)
                {
                    float maxValue = columnArrays[0].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[5] = maxValue.ToString();
                    maxValue = columnArrays[1].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[7] = maxValue.ToString();
                    maxValue = columnArrays[2].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[9] = maxValue.ToString();

                    File.Delete(path);
                }
                */

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

                    File.Delete(path);
                }

                /*
                else if (i == 6)
                {
                    float minValue = columnArrays[0].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[6] = minValue.ToString();
                    minValue = columnArrays[2].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[10] = minValue.ToString();

                    File.Delete(path);
                }
                */

                else if (i == 7)
                {
                    float minValue = columnArrays[0].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[2] = minValue.ToString();
                    minValue = columnArrays[1].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[4] = minValue.ToString();

                    File.Delete(path);
                }

                else if (i == 8)
                {
                    float maxValue = columnArrays[14].Max();
                    maxValue = MathF.Round(maxValue);
                    dataExtracted[35] = maxValue.ToString();
                    float minValue = columnArrays[14].Min();
                    minValue = MathF.Round(minValue);
                    dataExtracted[36] = minValue.ToString();

                    File.Delete(path);
                }

            }
            else
            {
                filesExist[i - 1] = false;
            }
        }

        //UnityEngine.Debug.Log(dataExtracted[0]);

        bool hasTrue = false;
        foreach (bool value in filesExist)
        {
            if (value)
            {
                hasTrue = true;
                break;
            }
        }

        string dataSavePath = Application.dataPath + "/dataFile.txt";

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
                    string header = "Date" + "\t" + "ThumbMCP Extend" + "\t" + "ThumbMCP Flex" + "\t" +
                        "ThumbIP Extend" + "\t" + "ThumbIP Flex" + "\t" +
                        "IndexMCP Ext" + "\t" + "IndexMCP Flex" + "\t" + "IndexPIP Ext" + "\t" + "IndexPIP Flex" + "\t" + "IndexDIP Ext" + "\t" + "IndexDIP Flex" + "\t" +
                        "MiddleMCP Ext" + "\t" + "MiddleMCP Flex" + "\t" + "MiddlePIP Ext" + "\t" + "MiddlePIP Flex" + "\t" + "MiddleDIP Ext" + "\t" + "MiddleDIP Flex" + "\t" +
                        "RingMCP Ext" + "\t" + "RingMCP Flex" + "\t" + "RingPIP Ext" + "\t" + "RingPIP Flex" + "\t" + "RingDIP Ext" + "\t" + "RingDIP Flex" + "\t" +
                        "LittleMCP Ext" + "\t" + "LittleMCP Flex" + "\t" + "LittlePIP Ext" + "\t" + "LittlePIP Flex" + "\t" + "LittleDIP Ext" + "\t" + "LittleDIP Flex" + "\t" +
                        "Wrist Ext" + "\t" + "Wrist Flex";

                    writer.WriteLine(header);
                    writer.WriteLine(lineWrite);
                }
            }
        }
       
    }

    private void updateGraph()
    {
        if (thumbFlag)
        {
            if (jointList[jointNum] == "MCP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbMCP;
                //add series into series list
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series1);
                myChart.chartData.series.Add(series2);
                myChart.UpdateChart();
            }
            
            else if (jointList[jointNum] == "IP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbIP;
                //add series into series list
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series3);
                myChart.chartData.series.Add(series4);
                myChart.UpdateChart();
            }
            /*
            else if (jointList[jointNum] == "CMCx")
            {
                //add series into series list
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series5);
                myChart.chartData.series.Add(series6);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "CMCy")
            {
                //add series into series list
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series7);
                myChart.chartData.series.Add(series8);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "CMCz")
            {
                //add series into series list
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series9);
                myChart.chartData.series.Add(series10);
                myChart.UpdateChart();
            }
            */
        }
        else if (indexFlag)
        {
            if(jointList[jointNum] == "MCP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexMCP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series11);
                myChart.chartData.series.Add(series12);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "PIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexPIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series13);
                myChart.chartData.series.Add(series14);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "DIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexDIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series15);
                myChart.chartData.series.Add(series16);
                myChart.UpdateChart();
            }
        }

        else if (middleFlag)
        {
            if (jointList[jointNum] == "MCP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleMCP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series17);
                myChart.chartData.series.Add(series18);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "PIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddlePIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series19);
                myChart.chartData.series.Add(series20);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "DIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleDIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series21);
                myChart.chartData.series.Add(series22);
                myChart.UpdateChart();
            }
        }
        else if (ringFlag)
        {
            if (jointList[jointNum] == "MCP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingMCP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series23);
                myChart.chartData.series.Add(series24);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "PIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingPIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series25);
                myChart.chartData.series.Add(series26);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "DIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingDIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series27);
                myChart.chartData.series.Add(series28);
                myChart.UpdateChart();
            }
        }
        else if (pinkyFlag)
        {
            if (jointList[jointNum] == "MCP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyMCP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series29);
                myChart.chartData.series.Add(series30);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "PIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyPIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series31);
                myChart.chartData.series.Add(series32);
                myChart.UpdateChart();
            }
            else if (jointList[jointNum] == "DIP")
            {
                HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyDIP;
                myChart.chartData.series = new List<E2ChartData.Series>();
                myChart.chartData.series.Add(series33);
                myChart.chartData.series.Add(series34);
                myChart.UpdateChart();
            }  
        }
        else if (wristFlag)
        {
            HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageWrist;
            myChart.chartData.series = new List<E2ChartData.Series>();
            myChart.chartData.series.Add(series35);
            myChart.chartData.series.Add(series36);
            myChart.UpdateChart();
        }
    }

    private void changeDateRange()
    {
        if (dateFromChangedFlag || dateToChangedFlag)
        {
            myChart.chartOptions.xAxis.autoAxisRange = false;
            myChart.chartOptions.xAxis.rangeDateTimeStringFormat = "yyyy-MM-dd";
            myChart.chartOptions.xAxis.minDateTimeString = DateFrom;
            myChart.chartOptions.xAxis.maxDateTimeString = DateTo;
            myChart.UpdateChart();
            dateFromChangedFlag = false;
            dateToChangedFlag = false;
       
        }
    }

    private void changeCircleColors()
    {
        if (thumbFlag)
        {
            if (jointNum == 0)
            {
                circle1.image.color = Color.gray;
                //circle2.image.color = Color.grey;
                //circle3.image.color = Color.white;
                //circle4.image.color = Color.white;
                circle5.image.color = Color.white;
            }
            else if (jointNum == 1)
            {
                circle1.image.color = Color.white;
                //circle2.image.color = Color.white;
                //circle3.image.color = Color.grey;
                //circle4.image.color = Color.white;
                circle5.image.color = Color.grey;
            }
            /*
            else if (jointNum == 2)
            {
                circle1.image.color = Color.white;
                circle2.image.color = Color.white;
                circle3.image.color = Color.grey;
                circle4.image.color = Color.white;
                circle5.image.color = Color.white;
            }
            else if (jointNum == 3)
            {
                circle1.image.color = Color.white;
                circle2.image.color = Color.white;
                circle3.image.color = Color.white;
                circle4.image.color = Color.grey;
                circle5.image.color = Color.white;
            }
            else if (jointNum == 4)
            {
                circle1.image.color = Color.white;
                circle2.image.color = Color.white;
                circle3.image.color = Color.white;
                circle4.image.color = Color.white;
                circle5.image.color = Color.grey;
            }
            */
        }
        else if (wristFlag)
        {
            circle1.image.color = Color.white;
            circle2.image.color = Color.white;
            circle3.image.color = Color.grey;
            circle4.image.color = Color.white;
            circle5.image.color = Color.white;
        }
        else
        {
            if (jointNum == 0)
            {
                circle2.image.color = Color.grey;
                circle3.image.color = Color.white;
                circle4.image.color = Color.white;
            }
            else if (jointNum == 1)
            {
                circle2.image.color = Color.white;
                circle3.image.color = Color.grey;
                circle4.image.color = Color.white;
            }
            else if (jointNum == 2)
            {
                circle2.image.color = Color.white;
                circle3.image.color = Color.white;
                circle4.image.color = Color.grey;
            }
        }
    }

    private void ForwardButtonOnClick()
    {
        jointNum = jointNum + 1;

        if (jointNum < jointList.Count)
        {
            chartTitle.text = jointList[jointNum];
            changeCircleColors();
        }
        else
        {
            jointNum = jointList.Count-1;
        }

        updateGraph();
    }

    private void BackButtonOnClick()
    {
        jointNum = jointNum - 1;
        if (jointNum >= 0)
        {
            chartTitle.text = jointList[jointNum];
            changeCircleColors();
        }
        else
        {
            jointNum = 0;
        }

        updateGraph();
    }

    private void ThumbButtonOnClick()
    {
        HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageThumbMCP;

        thumbFlag = true;
        indexFlag = false;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = false;

        jointList = new List<string>();
        jointList.Add("MCP");
        jointList.Add("IP");
        /*
        jointList.Add("CMCx");
        jointList.Add("CMCy");
        jointList.Add("CMCz");
        */

        jointNum = 0;
        chartTitle.text = jointList[jointNum];

        //add series into series list
        updateGraph();

        ThumbButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        circle1.gameObject.SetActive(true);
        circle2.gameObject.SetActive(false);
        circle3.gameObject.SetActive(false);
        circle4.gameObject.SetActive(false);
        circle5.gameObject.SetActive(true);
        changeCircleColors();
    }

    private void IndexButtonOnClick()
    {
        HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageIndexMCP;

        thumbFlag = false;
        indexFlag = true;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = false;

        jointList = new List<string>();
        jointList.Add("MCP");
        jointList.Add("PIP");
        jointList.Add("DIP");

        jointNum = 0;
        chartTitle.text = jointList[jointNum];

        updateGraph();

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        circle1.gameObject.SetActive(false);
        circle2.gameObject.SetActive(true);
        circle3.gameObject.SetActive(true);
        circle4.gameObject.SetActive(true);
        circle5.gameObject.SetActive(false);
        changeCircleColors();
    }

    private void MiddleButtonOnClick()
    {
        HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageMiddleMCP;

        thumbFlag = false;
        indexFlag = false;
        middleFlag = true;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = false;

        jointList = new List<string>();
        jointList.Add("MCP");
        jointList.Add("PIP");
        jointList.Add("DIP");

        jointNum = 0;
        chartTitle.text = jointList[jointNum];

        updateGraph();

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        circle1.gameObject.SetActive(false);
        circle2.gameObject.SetActive(true);
        circle3.gameObject.SetActive(true);
        circle4.gameObject.SetActive(true);
        circle5.gameObject.SetActive(false);
        changeCircleColors();
    }

    private void RingButtonOnClick()
    {
        HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageRingMCP;

        thumbFlag = false;
        indexFlag = false;
        middleFlag = false;
        ringFlag = true;
        pinkyFlag = false;
        wristFlag = false;

        jointList = new List<string>();
        jointList.Add("MCP");
        jointList.Add("PIP");
        jointList.Add("DIP");

        jointNum = 0;
        chartTitle.text = jointList[jointNum];

        updateGraph();

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        PinkyButton.image.color = Color.white;
        WristButton.image.color = Color.white;

        circle1.gameObject.SetActive(false);
        circle2.gameObject.SetActive(true);
        circle3.gameObject.SetActive(true);
        circle4.gameObject.SetActive(true);
        circle5.gameObject.SetActive(false);
        changeCircleColors();
    }

    private void PinkyButtonOnClick()
    {
        HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imagePinkyMCP;

        thumbFlag = false;
        indexFlag = false;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = true;
        wristFlag = false;

        jointList = new List<string>();
        jointList.Add("MCP");
        jointList.Add("PIP");
        jointList.Add("DIP");

        jointNum = 0;
        chartTitle.text = jointList[jointNum];

        updateGraph();

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = new Color(160f/255f, 223f/255f, 255f/255f, 1f);
        WristButton.image.color = Color.white;

        circle1.gameObject.SetActive(false);
        circle2.gameObject.SetActive(true);
        circle3.gameObject.SetActive(true);
        circle4.gameObject.SetActive(true);
        circle5.gameObject.SetActive(false);
        changeCircleColors();
    }

    private void WristButtonOnClick()
    {
        HandMapImage.GetComponent<UnityEngine.UI.Image>().sprite = imageWrist;

        thumbFlag = false;
        indexFlag = false;
        middleFlag = false;
        ringFlag = false;
        pinkyFlag = false;
        wristFlag = true;

        jointList = new List<string>();
        jointList.Add("Flex/Extend");

        jointNum = 0;
        chartTitle.text = jointList[jointNum];

        updateGraph();

        ThumbButton.image.color = Color.white;
        IndexButton.image.color = Color.white;
        MiddleButton.image.color = Color.white;
        RingButton.image.color = Color.white;
        PinkyButton.image.color = Color.white;
        WristButton.image.color = new Color(160f / 255f, 223f / 255f, 255f / 255f, 1f);

        circle1.gameObject.SetActive(false);
        circle2.gameObject.SetActive(false);
        circle3.gameObject.SetActive(true);
        circle4.gameObject.SetActive(false);
        circle5.gameObject.SetActive(false);
        changeCircleColors();
    }

    private void ExportToCSV()
    {
        string csvFilePath = Application.dataPath + "/" + LogIn.PatientID + "_" +
            LogIn.TodaysDate + "_jointROM.csv";
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.Log("Text file not found: " + filePath);
            return;
        }

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            using (StreamWriter writer = new StreamWriter(csvFilePath, false))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(new[] { ',', '\t' });
                    writer.WriteLine(string.Join("\t", values));
                }
            }
            UnityEngine.Debug.Log("Export successful! CSV saved at: " + csvFilePath);
            successText.text = "Success!";
            successText.color = new Color(4f / 255f, 173f / 255f, 0f / 255f);
            successText.enabled = true;
        }

        catch (IOException e)
        {
            UnityEngine.Debug.Log("File operation failed: " + e.Message);
            successText.text = "Export Failed.";
            successText.color = new Color(197f / 255f, 61f / 255f, 61f / 255f);
            successText.enabled = true;
        }

        StartCoroutine(WaitAndExecute());
    }

    IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(2f);

        successText.enabled = false;    }

    private void FixedUpdate()
    {
        //UnityEngine.Debug.Log(DateTo);
        changeDateRange();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}
    
