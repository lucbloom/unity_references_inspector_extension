using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(Transform), true)]
[CanEditMultipleObjects]
public class CustomTransformInspector : Editor
{
    // Unity's built-in editor
    Editor m_DefaultEditor;

    void OnEnable()
    {
        // When this inspector is created, also create the built-in inspector
        m_DefaultEditor = Editor.CreateEditor(targets, System.Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
    }

    void OnDisable()
    {
        // When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
        // Also, make sure to call any required methods like OnDisable
        MethodInfo disableMethod = m_DefaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (disableMethod != null)
        {
            disableMethod.Invoke(m_DefaultEditor, null);
        }
        DestroyImmediate(m_DefaultEditor);
    }

    static bool m_ShowReferences = false;
    public override void OnInspectorGUI()
    {
        m_DefaultEditor.OnInspectorGUI();

        Rect prev = GUILayoutUtility.GetLastRect();
        m_ShowReferences = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowReferences, "Referenced by");
        if (m_ShowReferences)
        {
            List<Object> gos = new List<Object>();
            foreach (Component tr in targets) { gos.Add(tr.gameObject); }
            List<DependenciesBrowser.Ref> refs = DependenciesBrowser.GatherRefsForSelection(gos);
            if (refs.Count > 0)
            {
                refs.Sort((a, b) => string.Compare(a.displayName, b.displayName));
                DependenciesBrowser.DrawInspector(refs, false);
            }
        }
        EditorGUI.EndFoldoutHeaderGroup();
    }
}

[CustomEditor(typeof(RectTransform), true)]
[CanEditMultipleObjects]
public class CustomRectTransformInspector : Editor
{
    // Unity's built-in editor
    Editor m_DefaultEditor;

    void OnEnable()
    {
        // When this inspector is created, also create the built-in inspector
        m_DefaultEditor = CreateEditor(targets, Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.RectTransformEditor"));
    }

    void OnDisable()
    {
        // When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
        // Also, make sure to call any required methods like OnDisable
        MethodInfo disableMethod = m_DefaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (disableMethod != null)
        {
            disableMethod.Invoke(m_DefaultEditor, null);
        }
        DestroyImmediate(m_DefaultEditor);
    }

    static bool m_ShowReferences = false;
    public override void OnInspectorGUI()
    {
        m_DefaultEditor.OnInspectorGUI();

        Rect prev = GUILayoutUtility.GetLastRect();
        m_ShowReferences = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowReferences, "Referenced by");
        if (m_ShowReferences)
        {
            List<Object> gos = new List<Object>();
            foreach (Component tr in targets) { gos.Add(tr.gameObject); }
            List<DependenciesBrowser.Ref> refs = DependenciesBrowser.GatherRefsForSelection(gos);
            if (refs.Count > 0)
            {
                refs.Sort((a, b) => string.Compare(a.displayName, b.displayName));
                DependenciesBrowser.DrawInspector(refs, false);
            }
        }
        EditorGUI.EndFoldoutHeaderGroup();
    }
}
