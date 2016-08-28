#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ZoneEditor : EditorWindow
{
    private string _zoneName;

    private bool _isError;

    [MenuItem("Tools/HitZones/EditZones")]
    public static void Init()
    {
        var window = (ZoneEditor)GetWindow(typeof(ZoneEditor));
        window._isError = false;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("You can add a new zone. \nNew zone name:");
        _zoneName = EditorGUILayout.TextArea(_zoneName);

        if (_isError)
        {
            EditorGUILayout.HelpBox("Already contains this zone.", MessageType.Error);
        }
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Add"))
        {
            AddEndScript(ReadEnum(), _zoneName);
            AssetDatabase.Refresh();
        }
        GUILayout.EndVertical();
    }

    private void AddEndScript(string origin, string substring)
    {
        if (origin.Contains(substring))
        {
            _isError = true;
            return;
        }
        _isError = false;
        var index = origin.LastIndexOf(",", StringComparison.Ordinal);
        Debug.LogError(index);

        if (index == -1)
            index = origin.LastIndexOf("{", StringComparison.Ordinal);

        origin = origin.Insert(index + 1, "\n   " + substring + ",");
        File.WriteAllText(GetPath(), origin);
    }

    private string ReadEnum()
    {
        var path = GetPath();

        if (!File.Exists(path))
        {
            Debug.LogError("File HitZones.cs not found.");

            using (StreamWriter outfile = new StreamWriter(path))
            {
                outfile.WriteLine("public enum " + "HitZones");
                outfile.WriteLine("{");
                outfile.WriteLine("}");
            }
            Debug.Log("HitZones.cs has been created.");
        }

        return File.ReadAllText(path);
    }

    private string GetPath()
    {
        var script = MonoScript.FromScriptableObject(this);
        var path = AssetDatabase.GetAssetPath(script);
        var name = GetType().FullName + ".cs";
        int index = path.IndexOf(name, StringComparison.InvariantCulture);
        path = (index < 0)
            ? path
            : path.Remove(index, name.Length);
        path = path + "HitZones" + ".cs";
        return path;
    }
}
#endif