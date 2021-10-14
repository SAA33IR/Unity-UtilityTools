#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class UtilityTools
{


    #region Clear Player Prefs
    [MenuItem("Utility/Clear Player Prefs &z", false, 0)]
    public static void ClearPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

    #region Capture Screenshot
    private static string path;
    private static string fileName;

    [MenuItem("Utility/Capture Screenshot &s", false, 1)]
    public static void Screenshot()
    {
        path = string.Format("Screenshots\\{0}x{1}", Screen.width, Screen.height);
        fileName = "Screenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-ms-ms") + ".png";

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        ScreenCapture.CaptureScreenshot(Path.Combine(path, fileName));
    }
    #endregion

    #region Snap To Ground
    [MenuItem("Utility/Snap To Ground &g", false, 2)]
    public static void SnapToGround()
    {
        foreach (var trans in Selection.transforms)
        {
            var hits = Physics.RaycastAll(trans.position + Vector3.up, Vector3.down, 10f);
            foreach (var hit in hits)
            {
                if(hit.collider.gameObject == trans.gameObject)
                {
                    continue;
                }
                trans.position = hit.point;
                break;
            }
        }
    }
    #endregion

    #region Component Copier
    private static Component[] copiedComponents;

    [MenuItem("Utility/Copy All Components &c", false, 3)]
    public static void Copy()
    {
        if (Selection.activeGameObject == null)
            return;

        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }

    [MenuItem("Utility/Paste All Components &v", false, 4)]
    private static void Paste()
    {
        if (copiedComponents == null)
        {
            Debug.LogError("Nothing to paste!");
            return;
        }

        foreach (GameObject targetGameObject in Selection.gameObjects)
        {
            if (!targetGameObject)
                continue;

            Undo.RegisterCompleteObjectUndo(targetGameObject, targetGameObject.name + ": Paste all components");

            foreach (Component copiedComponent in copiedComponents)
            {
                if (!copiedComponent)
                    continue;

                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);

                Component targetComponent = targetGameObject.GetComponent(copiedComponent.GetType());

                if (targetComponent)
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComponent))
                    {
                        Debug.Log(copiedComponent.GetType() + " Pasted.");
                    }
                    else
                    {
                        Debug.LogError("Failed to copy " + copiedComponent.GetType());
                    }
                }
                else
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject))
                    {
                        Debug.Log(copiedComponent.GetType() + " Pasted.");
                    }
                    else
                    {
                        Debug.LogError("Failed to copy: " + copiedComponent.GetType());
                    }
                }
            }
        }

        copiedComponents = null;
    }
    #endregion

}
#endif