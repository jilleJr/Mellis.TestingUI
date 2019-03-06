using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnableEmbeddedResources : MonoBehaviour
{
    [MenuItem("Build/Web GL/Enable Embedded Resources")]
    public static void EnableErrorMessageTesting()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        PlayerSettings.SetPropertyBool("useEmbeddedResources", true, BuildTargetGroup.WebGL);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
