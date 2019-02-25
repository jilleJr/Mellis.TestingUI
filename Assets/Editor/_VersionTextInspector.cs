using System.Reflection;
using Mellis.Core.Interfaces;
using Mellis.Lang.Python3;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(VersionText)), CanEditMultipleObjects]
    public class _VersionTextInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("{0} UI version\n" +
                                    "{1} Mellis version\n" +
                                    "{2} Python3 module version", MessageType.Info, true);

            GUI.enabled = false;
            EditorGUILayout.Space();

            Assembly mellis = Assembly.GetAssembly(typeof(IScriptType));
            Assembly python3 = Assembly.GetAssembly(typeof(PyCompiler));

            EditorGUILayout.HelpBox(mellis + "\n" + python3, MessageType.Info, true);
            GUI.enabled = true;
        }
    }
}
