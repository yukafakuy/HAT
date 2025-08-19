using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownSelect : MonoBehaviour
{
    public TMP_Dropdown OptionsDropdown;
    private List<string> options;
    public Toggle GraphToggle, TableToggle, NotesToggle;

    public void UpdateDropdownOptions()
    {
        // Clear existing options
        OptionsDropdown.ClearOptions();

        if (GraphToggle.isOn)
        {
            options = new List<string>() { "PDF Document"};
        }
        else
        {
            options = new List<string>() { "CSV Spreadsheet"};
        }
        // Add the new options to the dropdown
        OptionsDropdown.AddOptions(options);
        //RefreshOptions(OptionsDropdown);
    }

    public void RefreshOptions(TMP_Dropdown dropdown)
    {
        dropdown.enabled = false;
        dropdown.enabled = true;
        dropdown.Show();
    }
}
