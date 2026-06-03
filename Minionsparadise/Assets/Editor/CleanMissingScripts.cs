using UnityEngine;
using UnityEditor;

public class CleanMissingScripts
{
    [MenuItem("Kampai/Utilities/Clean Missing Scripts In Scene")]
    static void CleanScene()
    {
        GameObject[] gos = Resources.FindObjectsOfTypeAll<GameObject>();
        int count = 0;
        foreach (GameObject go in gos)
        {
            if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                continue;

            int localCount = 0;
            Component[] components = go.GetComponents<Component>();
            SerializedObject so = new SerializedObject(go);
            SerializedProperty prop = so.FindProperty("m_Component");

            if (prop == null) continue;

            int r = 0;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    prop.DeleteArrayElementAtIndex(i - r);
                    r++;
                    localCount++;
                }
            }

            if (localCount > 0)
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(go);
                count += localCount;
            }
        }
        Debug.Log("Cleaned " + count + " missing script(s) from the scene GameObjects.");
    }
}
