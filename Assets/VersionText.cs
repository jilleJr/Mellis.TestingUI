using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mellis.Core.Interfaces;
using Mellis.Lang.Python3;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    public TMP_Text text;
    public TextAsset dependenciesFile;

    public string mellisSearchRegex = @"""Mellis\.Core\/(\d+(?:\.\d+){0,2})""";
    public string python3SearchRegex = @"""Mellis\.Lang\.Python3\/(\d+(?:\.\d+){0,2})""";

    [Space] [TextArea] public string format;

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetText();
    }
#endif

    private void Start()
    {
        SetText();
    }

    private void SetText()
    {
        if (!text) return;
        if (!dependenciesFile) return;

        string mellis;
        string python3;
        try
        {
            string json = dependenciesFile.text;

            mellis = Regex.Match(json, mellisSearchRegex).Groups[1].Value;
            python3 = Regex.Match(json, python3SearchRegex).Groups[1].Value;
        }
        catch (Exception)
        {
            text.text = $"<i><color=red>Failed to parse \"{dependenciesFile.name}\" file.</color></i>";
            throw;
        }

        try
        {
            text.text = string.Format(format,
                Application.version,
                mellis,
                python3);
        }
        catch (FormatException)
        {
            text.text = "<i><color=red>Invalid version format.</color></i>";
        }
    }

    private static DependenciesFile ParseFile(string json)
    {
        return JsonUtility.FromJson<DependenciesFile>(json);
    }

    [Serializable]
    public struct DependenciesFile
    {
        public Dictionary<string, Library> old_libraries;
        public string[] libraries;

        public Dictionary<string, string> GetVersions()
        {
            return old_libraries?.ToDictionary(o => o.Key.Substring(0, o.Key.IndexOf('/')),
                o => o.Key.Substring(o.Key.IndexOf('/') + 1));
        }

        [Serializable]
        public struct Library
        {
            public LibraryType type;
            public bool serviceable;
            public string sha512;
            public string path;
            public string hashPath;

            public enum LibraryType
            {
                Project,
                Package
            }
        }
    }
}