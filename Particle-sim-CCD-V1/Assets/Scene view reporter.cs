using UnityEditor;
using UnityEngine;

public class SceneViewReporter : EditorWindow
{
    [MenuItem("Tools/Scene View Reporter")]
    static void ShowWindow() => GetWindow<SceneViewReporter>();

    void OnGUI()
    {
        if (SceneView.lastActiveSceneView == null)
        {
            GUILayout.Label("No Scene view found.");
            return;
        }

        var camTr = SceneView.lastActiveSceneView.camera.transform;
        GUILayout.Label($"Position: {camTr.position}");
        GUILayout.Label($"Rotation: {camTr.rotation.eulerAngles}");
    }
}
