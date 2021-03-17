using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(Transform), true)]
[CanEditMultipleObjects]
public class CustomTransformInspector : Editor
{
    Editor m_DefaultEditor;

    void OnEnable()
    {
        m_DefaultEditor = Editor.CreateEditor(targets, System.Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
    }

    void OnDisable()
    {
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
                refs.Sort((a, b) => string.Compare(a.name, b.name));
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
    Editor m_DefaultEditor;

    void OnEnable()
    {
        m_DefaultEditor = CreateEditor(targets, Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.RectTransformEditor"));
    }

    void OnDisable()
    {
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
                refs.Sort((a, b) => string.Compare(a.name, b.name));
                DependenciesBrowser.DrawInspector(refs, false);
            }
        }
        EditorGUI.EndFoldoutHeaderGroup();
    }
}
