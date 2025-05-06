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

public class PopulateSeriesLeft : MonoBehaviour
{
    //data
    private string filePath;
    public static List<string> dates = new List<string>();
    private List<float> thumbMCPmin = new List<float>();
    private List<float> thumbMCPmax = new List<float>();
    private List<float> thumbIPmin = new List<float>();
    private List<float> thumbIPmax = new List<float>();

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

    private List<float> therapyType = new List<float>();

    //Series
    public static E2ChartData.Series series1 = new E2ChartData.Series();
    public static E2ChartData.Series series2 = new E2ChartData.Series();
    public static E2ChartData.Series series3 = new E2ChartData.Series();
    public static E2ChartData.Series series4 = new E2ChartData.Series();
    public static E2ChartData.Series series5 = new E2ChartData.Series();
    public static E2ChartData.Series series6 = new E2ChartData.Series();
    public static E2ChartData.Series series7 = new E2ChartData.Series();
    public static E2ChartData.Series series8 = new E2ChartData.Series();
    public static E2ChartData.Series series9 = new E2ChartData.Series();
    public static E2ChartData.Series series10 = new E2ChartData.Series();
    public static E2ChartData.Series series11 = new E2ChartData.Series();
    public static E2ChartData.Series series12 = new E2ChartData.Series();
    public static E2ChartData.Series series13 = new E2ChartData.Series();
    public static E2ChartData.Series series14 = new E2ChartData.Series();
    public static E2ChartData.Series series15 = new E2ChartData.Series();
    public static E2ChartData.Series series16 = new E2ChartData.Series();
    public static E2ChartData.Series series17 = new E2ChartData.Series();
    public static E2ChartData.Series series18 = new E2ChartData.Series();
    public static E2ChartData.Series series19 = new E2ChartData.Series();
    public static E2ChartData.Series series20 = new E2ChartData.Series();
    public static E2ChartData.Series series21 = new E2ChartData.Series();
    public static E2ChartData.Series series22 = new E2ChartData.Series();
    public static E2ChartData.Series series23 = new E2ChartData.Series();
    public static E2ChartData.Series series24 = new E2ChartData.Series();
    public static E2ChartData.Series series25 = new E2ChartData.Series();
    public static E2ChartData.Series series26 = new E2ChartData.Series();
    public static E2ChartData.Series series27 = new E2ChartData.Series();
    public static E2ChartData.Series series28 = new E2ChartData.Series();
    public static E2ChartData.Series series29 = new E2ChartData.Series();
    public static E2ChartData.Series series30 = new E2ChartData.Series();

    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void ExtractSeries()
    {
        filePath = Application.dataPath + "/" + LogIn.PatientID + "_dataFile_Left.txt";

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
                thumbMCPmax.Add(-1f * parseString(tokens[1]));
                thumbMCPmin.Add(-1f * parseString(tokens[2]));
                thumbIPmax.Add(-1f * parseString(tokens[3]));
                thumbIPmin.Add(-1f * parseString(tokens[4]));

                indexMCPmax.Add(-1f * parseString(tokens[5]));
                indexMCPmin.Add(-1f * parseString(tokens[6]));
                indexPIPmax.Add(-1f * parseString(tokens[7]));
                indexPIPmin.Add(-1f * parseString(tokens[8]));
                indexDIPmax.Add(-1f * parseString(tokens[9]));
                indexDIPmin.Add(-1f * parseString(tokens[10]));

                middleMCPmax.Add(-1f * parseString(tokens[11]));
                middleMCPmin.Add(-1f * parseString(tokens[12]));
                middlePIPmax.Add(-1f * parseString(tokens[13]));
                middlePIPmin.Add(-1f * parseString(tokens[14]));
                middleDIPmax.Add(-1f * parseString(tokens[15]));
                middleDIPmin.Add(-1f * parseString(tokens[16]));

                ringMCPmax.Add(-1f * parseString(tokens[17]));
                ringMCPmin.Add(-1f * parseString(tokens[18]));
                ringPIPmax.Add(-1f * parseString(tokens[19]));
                ringPIPmin.Add(-1f * parseString(tokens[20]));
                ringDIPmax.Add(-1f * parseString(tokens[21]));
                ringDIPmin.Add(-1f * parseString(tokens[22]));

                pinkyMCPmax.Add(-1f * parseString(tokens[23]));
                pinkyMCPmin.Add(-1f * parseString(tokens[24]));
                pinkyPIPmax.Add(-1f * parseString(tokens[25]));
                pinkyPIPmin.Add(-1f * parseString(tokens[26]));
                pinkyDIPmax.Add(-1f * parseString(tokens[27]));
                pinkyDIPmin.Add(-1f * parseString(tokens[28]));

                wristExtend.Add(-1f * parseString(tokens[29]));
                wristFlex.Add(-1f * parseString(tokens[30]));

                therapyType.Add(parseString(tokens[31]));
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("An error occured {ex.Message}");
        }

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
        series5.name = "Extension";
        series5.dataY = new List<float>();
        series5.dateTimeString = new List<string>();
        series5.dateTimeTick = new List<long>();
        series5.dataY.AddRange(indexMCPmax);
        series5.dateTimeString.AddRange(dates);
        series6.name = "Flexion";
        series6.dataY = new List<float>();
        series6.dateTimeString = new List<string>();
        series6.dateTimeTick = new List<long>();
        series6.dataY.AddRange(indexMCPmin);
        series6.dateTimeString.AddRange(dates);

        //Index PIP
        series7.name = "Extension";
        series7.dataY = new List<float>();
        series7.dateTimeString = new List<string>();
        series7.dateTimeTick = new List<long>();
        series7.dataY.AddRange(indexPIPmax);
        series7.dateTimeString.AddRange(dates);
        series8.name = "Flexion";
        series8.dataY = new List<float>();
        series8.dateTimeString = new List<string>();
        series8.dateTimeTick = new List<long>();
        series8.dataY.AddRange(indexPIPmin);
        series8.dateTimeString.AddRange(dates);

        //Index DIP
        series9.name = "Extension";
        series9.dataY = new List<float>();
        series9.dateTimeString = new List<string>();
        series9.dateTimeTick = new List<long>();
        series9.dataY.AddRange(indexDIPmax);
        series9.dateTimeString.AddRange(dates);
        series10.name = "Flexion";
        series10.dataY = new List<float>();
        series10.dateTimeString = new List<string>();
        series10.dateTimeTick = new List<long>();
        series10.dataY.AddRange(indexDIPmin);
        series10.dateTimeString.AddRange(dates);

        //middle MCP
        series11.name = "Extension";
        series11.dataY = new List<float>();
        series11.dateTimeString = new List<string>();
        series11.dateTimeTick = new List<long>();
        series11.dataY.AddRange(middleMCPmax);
        series11.dateTimeString.AddRange(dates);
        series12.name = "Flexion";
        series12.dataY = new List<float>();
        series12.dateTimeString = new List<string>();
        series12.dateTimeTick = new List<long>();
        series12.dataY.AddRange(middleMCPmin);
        series12.dateTimeString.AddRange(dates);

        //middle PIP
        series13.name = "Extension";
        series13.dataY = new List<float>();
        series13.dateTimeString = new List<string>();
        series13.dateTimeTick = new List<long>();
        series13.dataY.AddRange(middlePIPmax);
        series13.dateTimeString.AddRange(dates);
        series14.name = "Flexion";
        series14.dataY = new List<float>();
        series14.dateTimeString = new List<string>();
        series14.dateTimeTick = new List<long>();
        series14.dataY.AddRange(middlePIPmin);
        series14.dateTimeString.AddRange(dates);

        //middle DIP
        series15.name = "Extension";
        series15.dataY = new List<float>();
        series15.dateTimeString = new List<string>();
        series15.dateTimeTick = new List<long>();
        series15.dataY.AddRange(middleDIPmax);
        series15.dateTimeString.AddRange(dates);
        series16.name = "Flexion";
        series16.dataY = new List<float>();
        series16.dateTimeString = new List<string>();
        series16.dateTimeTick = new List<long>();
        series16.dataY.AddRange(middleDIPmin);
        series16.dateTimeString.AddRange(dates);

        //ring MCP
        series17.name = "Extension";
        series17.dataY = new List<float>();
        series17.dateTimeString = new List<string>();
        series17.dateTimeTick = new List<long>();
        series17.dataY.AddRange(ringMCPmax);
        series17.dateTimeString.AddRange(dates);
        series18.name = "Flexion";
        series18.dataY = new List<float>();
        series18.dateTimeString = new List<string>();
        series18.dateTimeTick = new List<long>();
        series18.dataY.AddRange(ringMCPmin);
        series18.dateTimeString.AddRange(dates);

        //ring PIP
        series19.name = "Extension";
        series19.dataY = new List<float>();
        series19.dateTimeString = new List<string>();
        series19.dateTimeTick = new List<long>();
        series19.dataY.AddRange(ringPIPmax);
        series19.dateTimeString.AddRange(dates);
        series20.name = "Flexion";
        series20.dataY = new List<float>();
        series20.dateTimeString = new List<string>();
        series20.dateTimeTick = new List<long>();
        series20.dataY.AddRange(ringPIPmin);
        series20.dateTimeString.AddRange(dates);


        //ring DIP
        series21.name = "Extension";
        series21.dataY = new List<float>();
        series21.dateTimeString = new List<string>();
        series21.dateTimeTick = new List<long>();
        series21.dataY.AddRange(ringDIPmax);
        series21.dateTimeString.AddRange(dates);
        series22.name = "Flexion";
        series22.dataY = new List<float>();
        series22.dateTimeString = new List<string>();
        series22.dateTimeTick = new List<long>();
        series22.dataY.AddRange(ringDIPmin);
        series22.dateTimeString.AddRange(dates);

        //pinky MCP
        series23.name = "Extension";
        series23.dataY = new List<float>();
        series23.dateTimeString = new List<string>();
        series23.dateTimeTick = new List<long>();
        series23.dataY.AddRange(pinkyMCPmax);
        series23.dateTimeString.AddRange(dates);
        series24.name = "Flexion";
        series24.dataY = new List<float>();
        series24.dateTimeString = new List<string>();
        series24.dateTimeTick = new List<long>();
        series24.dataY.AddRange(pinkyMCPmin);
        series24.dateTimeString.AddRange(dates);

        //pinky PIP
        series25.name = "Extension";
        series25.dataY = new List<float>();
        series25.dateTimeString = new List<string>();
        series25.dateTimeTick = new List<long>();
        series25.dataY.AddRange(pinkyPIPmax);
        series25.dateTimeString.AddRange(dates);
        series26.name = "Flexion";
        series26.dataY = new List<float>();
        series26.dateTimeString = new List<string>();
        series26.dateTimeTick = new List<long>();
        series26.dataY.AddRange(pinkyPIPmin);
        series26.dateTimeString.AddRange(dates);

        //pinky DIP
        series27.name = "Extension";
        series27.dataY = new List<float>();
        series27.dateTimeString = new List<string>();
        series27.dateTimeTick = new List<long>();
        series27.dataY.AddRange(pinkyDIPmax);
        series27.dateTimeString.AddRange(dates);
        series28.name = "Flexion";
        series28.dataY = new List<float>();
        series28.dateTimeString = new List<string>();
        series28.dateTimeTick = new List<long>();
        series28.dataY.AddRange(pinkyDIPmin);
        series28.dateTimeString.AddRange(dates);

        //wrist
        series29.name = "Extension";
        series29.dataY = new List<float>();
        series29.dateTimeString = new List<string>();
        series29.dateTimeTick = new List<long>();
        series29.dataY.AddRange(wristExtend);
        series29.dateTimeString.AddRange(dates);
        series30.name = "Flexion";
        series30.dataY = new List<float>();
        series30.dateTimeString = new List<string>();
        series30.dateTimeTick = new List<long>();
        series30.dataY.AddRange(wristFlex);
        series30.dateTimeString.AddRange(dates);
    }
    private float parseString(string token)
    {
        if (float.TryParse(token, out float result)) { };

        return result;

    }
    // Update is called once per frame
    void Update()
    {

    }
}
