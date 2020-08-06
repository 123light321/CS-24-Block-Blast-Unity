using UnityEngine;
using UnityEditor;

public class ResetEditor : EditorWindow {

    [MenuItem("Window/Reset")]
    public static void ResetSaveData () {
        PlayerPrefs.DeleteAll();
        SkinLocker.DeleteFile();
        Debug.Log("All Record has been reset");
    }

    private void OnGUI() {
        
    }

}
