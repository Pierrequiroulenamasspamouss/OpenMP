using UnityEditor;
using UnityEngine;
using Kampai.UI.View;
using System.IO;

public class SetRotateIconMask : EditorWindow
{
    [MenuItem("Kampai/UI/Set Rotate Button Icon")]
    public static void SetIcon()
    {
        // 1. Find the MoveBuildingModal prefab (or instance)
        GameObject rotateBtn = GameObject.Find("RotateButton");
        
        // If not in scene, we search for the prefab
        if (rotateBtn == null)
        {
            string[] guids = AssetDatabase.FindAssets("screen_MoveBuilding t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    // Find RotateButton in prefab
                    Transform rotateTransform = FindDeepChild(prefab.transform, "RotateButton");
                    if (rotateTransform != null)
                    {
                        ProcessIcon(rotateTransform.gameObject);
                        return;
                    }
                }
            }
        }
        else
        {
            ProcessIcon(rotateBtn);
            return;
        }

        Debug.LogError("Could not find 'RotateButton' in the scene or 'screen_MoveBuilding' prefab!");
    }

    private static void ProcessIcon(GameObject rotateBtn)
    {
        Transform iconTransform = rotateBtn.transform.Find("icon");
        if (iconTransform == null)
        {
            Debug.LogError("Could not find 'icon' child under RotateButton!");
            return;
        }

        KampaiImage kImage = iconTransform.GetComponent<KampaiImage>();
        if (kImage == null)
        {
            Debug.LogError("KampaiImage component missing on RotateButton/icon!");
            return;
        }

        // Load the refresh mask
        Sprite refreshMask = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/content/shared/shared_ui/ui_textures/ui_resources_common/textures/img_marketplace_refresh_mask.png");
        
        if (refreshMask == null)
        {
            Debug.LogError("Could not find img_marketplace_refresh_mask.png at the expected path!");
            return;
        }

        // Apply the mask
        kImage.maskSprite = refreshMask;
        
        // Mark dirty to save changes
        EditorUtility.SetDirty(kImage);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Successfully set the RotateButton icon mask to the Refresh Icon!");
    }

    private static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
