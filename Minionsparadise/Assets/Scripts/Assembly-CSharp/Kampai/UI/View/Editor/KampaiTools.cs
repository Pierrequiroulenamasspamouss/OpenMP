using UnityEngine;
using UnityEditor;
using strange.extensions.mediation.impl;

namespace Kampai.Editor
{
    public class KampaiTools : EditorWindow
    {
        private const string SETTINGS_PREFAB_PATH = "Assets/Resources/content/shared/shared_ui/ui_prefabs/ui_common/hud/screen_HUD_Panel_Settings_Menu.prefab";

        [MenuItem("Kampai/UI/Repair Settings Prefab (Mediators + Scaling)")]
        public static void RepairSettingsPrefab()
        {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(SETTINGS_PREFAB_PATH);
            if (prefabAsset == null)
            {
                Debug.LogError("[KampaiTools] Could not find Settings prefab at: " + SETTINGS_PREFAB_PATH);
                return;
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
            if (instance == null) return;

            // 1. Fix Dimension Scaling (Stretch to full screen)
            RectTransform rect = instance.GetComponent<RectTransform>();
            if (rect != null)
            {
                Undo.RecordObject(rect, "Fix Scaling");
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                Debug.Log("[KampaiTools] Fixed root RectTransform scaling.");
            }

            // 2. Remove redundant Mediators (avoid duplicate injection crashes)
            int removedCount = 0;
            Mediator[] mediators = instance.GetComponentsInChildren<Mediator>(true);
            foreach (Mediator m in mediators)
            {
                Undo.DestroyObjectImmediate(m);
                removedCount++;
            }

            // 3. Save changes
            PrefabUtility.SaveAsPrefabAsset(instance, SETTINGS_PREFAB_PATH);
            DestroyImmediate(instance);

            Debug.Log(string.Format("<b>[KampaiTools] SUCCESS: Removed {0} redundant mediators and repaired scaling.</b>", removedCount));
        }
    }
}
