using System.Collections;
using System.Collections.Generic;
using PM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputColorTheCodeYo : MonoBehaviour
{
    //public TMP_InputField input;
    public TMP_Text output;

    public void UpdateColoredOutput(string text)
    {
        output.text = IDEColorCoding.RunColorCode(text);
    }
}
