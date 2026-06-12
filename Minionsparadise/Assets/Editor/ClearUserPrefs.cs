using UnityEngine;
using UnityEditor;
using System.IO;

public class ClearUserPrefs : EditorWindow
{
    [MenuItem("Kampai/Debug/Clear User Preferences")]
    public static void ClearAll()
    {
        // 1. Empêcher l'exécution si le jeu tourne
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Action Impossible", 
                "Vous devez arrêter le mode Play avant de supprimer les préférences, sinon le moteur IoC va planter.", "OK");
            return;
        }

        if (EditorUtility.DisplayDialog("Clear User Preferences", 
            "This will delete all PlayerPrefs and all files in the Persistent Data Path. Are you sure?", 
            "Yes, Clear Everything", "Cancel"))
        {
            // 2. Clear PlayerPrefs
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("<color=green>[ClearUserPrefs]</color> PlayerPrefs cleared.");

            // 3. Clear Persistent Data Path
            string path = Application.persistentDataPath;
            try 
            {
                if (Directory.Exists(path))
                {
                    // Une méthode plus propre pour vider sans supprimer le dossier racine lui-même
                    string[] files = Directory.GetFiles(path);
                    foreach (string file in files) File.Delete(file);
                    
                    string[] dirs = Directory.GetDirectories(path);
                    foreach (string dir in dirs) Directory.Delete(dir, true);
                    
                    Debug.Log("<color=green>[ClearUserPrefs]</color> Persistent data path cleared: " + path);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("[ClearUserPrefs] Error clearing path: " + e.Message);
            }
            
            EditorUtility.DisplayDialog("Success", "All user preferences and local data have been cleared.", "OK");
        }
    }

    [MenuItem("Kampai/Debug/Open Persistent Data Path")]
    public static void OpenPersistentDataPath()
    {
        // On s'assure que le dossier existe avant d'essayer de l'ouvrir
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}