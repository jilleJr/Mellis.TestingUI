using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnableEmbeddedResources : MonoBehaviour
{
    [MenuItem("Build/Web GL/Enable Embedded Resources")]
    public static void EnableErrorMessageTesting()
    {
        PlayerSettings.SetPropertyBool("useEmbeddedResources", true, BuildTargetGroup.WebGL);
    }
}
