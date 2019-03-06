using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputBuiltinField : MonoBehaviour
{
    public TMP_InputField field;
    public GameObject gameObjectToActivate;

    public void Show()
    {
        gameObjectToActivate.SetActive(true);
        field.text = string.Empty;
    }

    public void Hide()
    {
        gameObjectToActivate.SetActive(false);
    }

    public void OnFieldSubmit()
    {
        CompilerController.ResolveYield(field.text);
        Hide();
    }
}
