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
        }
    }
}
