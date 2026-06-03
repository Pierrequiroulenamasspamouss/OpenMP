using UnityEngine;
using UnityEditor;

public class FixMissingNormals
{
    [MenuItem("Kampai/Utilities/Fix Missing Normals")]
    public static void FixNormals()
    {
        int count = 0;
        
        // Fix models (FBX, OBJ, etc.)
        string[] guids = AssetDatabase.FindAssets("t:Model");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null && importer.importNormals == ModelImporterNormals.None)
            {
                importer.importNormals = ModelImporterNormals.Calculate;
                AssetDatabase.ImportAsset(path);
                count++;
            }
        }

        // Fix raw Mesh assets (.asset files)
        guids = AssetDatabase.FindAssets("t:Mesh");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Mesh mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;
            if (mesh != null && (mesh.normals == null || mesh.normals.Length == 0) && mesh.vertexCount > 0)
            {
                mesh.RecalculateNormals();
                EditorUtility.SetDirty(mesh);
                count++;
            }
        }
        
        if (count > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log("Successfully generated missing normals for " + count + " meshes/models.");
        }
        else
        {
            Debug.Log("No meshes with missing normals found.");
        }
    }
}
