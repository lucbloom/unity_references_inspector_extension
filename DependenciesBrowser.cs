﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DependenciesBrowser : EditorWindow
{
    public class Ref
    {
        public string name;
        public Object obj;
        public Ref(string n, Object o) { name = n; obj = o; }
    };


    List<Ref> m_ReferencingObjects;
    List<Ref> m_ReferencedObjects;
    Vector2 m_ScrollPos;
    static bool m_ReferencedOpen = true;
    static bool m_ReferencingOpen = false;

    [MenuItem("Window/General/Dependencies Browser")]
    static void Init()
    {
        DependenciesBrowser window = (DependenciesBrowser)EditorWindow.GetWindow(typeof(DependenciesBrowser));
        window.Show();

        Selection.selectionChanged += window.UpdateMe;
        window.UpdateMe();
    }

    public static List<Object> GetSubObjectsToCheck(Object obj)
    {
        List<Object> objectsToCheck = new List<Object>();
        objectsToCheck.Add(obj);
        if (obj is GameObject)
        {
            Component[] components = (obj as GameObject).GetComponents<Component>();
            foreach (Component co in components)
            {
                objectsToCheck.Add(co);
            }
        }
        return objectsToCheck;
    }

    public static List<Ref> GatherRefsForList(object[] objects, List<Object> objectsToCheck)
    {
        List<Ref> refs = new List<Ref>();
        foreach (GameObject go in objects)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component co in components)
            {
                if (co)
                {
                    var sp = new SerializedObject(co).GetIterator();
                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType == SerializedPropertyType.ObjectReference &&
                            (objectsToCheck == null || objectsToCheck.Contains(sp.objectReferenceValue)))
                        {
                            Object subject = objectsToCheck == null ? sp.objectReferenceValue : go;
                            refs.Add(new Ref(string.Concat(sp.propertyPath, " (", sp.displayName, ")"), subject));
                        }
                    }
                }
            }
        }
        return refs;
    }

    public static List<Ref> GatherRefsForSelection<T>(IEnumerable<T> selection)
    {
        List<Object> objectsToCheck = new List<Object>();
        foreach (object obj in selection)
        {
            objectsToCheck.Add(obj as Object);
            objectsToCheck.AddRange(GetSubObjectsToCheck(obj as Object));
        }
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        return GatherRefsForList(allObjects, objectsToCheck);
    }

    void UpdateMe()
    {
        m_ReferencingObjects = GatherRefsForSelection(Selection.objects);
        m_ReferencedObjects = GatherRefsForList(Selection.objects, null);

        m_ReferencedObjects.Sort((a, b) => string.Compare(a.name, b.name));
        m_ReferencingObjects.Sort((a, b) => string.Compare(a.name, b.name));
    }

    static public void DrawInspector(List<DependenciesBrowser.Ref> refs, bool header)
    {
        foreach (Ref r in refs)
        {
            if (header)
            {
                Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.SelectableLabel(position, r.name);
                EditorGUILayout.ObjectField(r.obj, typeof(Object));
            }
            else
            {
                EditorGUILayout.ObjectField(r.obj, typeof(Object));
                Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.SelectableLabel(position, r.name);
            }
        }
    }

    void OnGUI()
    {
        if (m_ReferencingObjects != null)
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            m_ReferencedOpen = EditorGUILayout.BeginFoldoutHeaderGroup(m_ReferencedOpen, "Referenced by (" + m_ReferencingObjects.Count + ")");
            if (m_ReferencedOpen)
            {
                DrawInspector(m_ReferencingObjects, false);
            }
            EditorGUI.EndFoldoutHeaderGroup();
            m_ReferencingOpen = EditorGUILayout.BeginFoldoutHeaderGroup(m_ReferencingOpen, "This is referencing (" + m_ReferencedObjects.Count + ")");
            if (m_ReferencingOpen)
            {
                DrawInspector(m_ReferencedObjects, true);
            }
            EditorGUI.EndFoldoutHeaderGroup();
            EditorGUILayout.EndScrollView();
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnFocus()
    {
        Selection.selectionChanged += UpdateMe;
        UpdateMe();
    }
}
