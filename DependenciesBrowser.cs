#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Linq;

public class DependenciesBrowser : EditorWindow
{
    public class Ref
    {
        public string script;
        public string component;
        public string memberName;
        public string displayName;
        public Object obj;
        public Ref(string s, string mn, string dn, string c, Object o)
        {
            script = s;
            memberName = mn;
            displayName = dn;
            component = c;
            obj = o;
        }
    };


    //static Object m_SelectedObject;
    List<Ref> m_ReferencingObjects;
    List<Ref> m_ReferencedObjects;
    Vector2 m_ScrollPos;
    static bool m_ReferencedOpen = true;
    static bool m_ReferencingOpen = false;

    [MenuItem("Window/General/Dependencies Browser")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DependenciesBrowser window = (DependenciesBrowser)GetWindow(typeof(DependenciesBrowser));
        window.Show();

        Selection.selectionChanged += window.UpdateMe;
        window.UpdateMe();
    }

    public static List<Object> GetSubObjectsToCheck(Object obj)
    {
        var objectsToCheck = new List<Object>{obj};
        if (obj is GameObject)
        {
            objectsToCheck.AddRange((obj as GameObject).GetComponents<Component>());
        }
        return objectsToCheck;
    }

    public static List<Ref> GatherRefsForList(List<GameObject> objects, List<Object> objectsToCheck)
    {
        var refs = new List<Ref>();
        foreach (var go in objects)
        {
            var components = go.GetComponents<Component>();
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
                            refs.Add(new Ref(co.GetType().ToString(), sp.propertyPath, sp.displayName, sp.objectReferenceValue.GetType().ToString(), subject));
                        }
                    }
                }
            }
        }
        return refs;
    }

    public static List<Ref> GatherRefsForSelection<T>(IEnumerable<T> selection)
    {
        var objectsToCheck = new List<Object>();
        foreach (object obj in selection)
        {
            objectsToCheck.Add(obj as Object);
            objectsToCheck.AddRange(GetSubObjectsToCheck(obj as Object));
        }

        var allObjects = FindObjectsOfType<GameObject>().ToList();
        if (StageUtility.GetCurrentStageHandle() != StageUtility.GetMainStageHandle())
        {
            allObjects.AddRange(StageUtility.GetCurrentStageHandle().FindComponentsOfType<Transform>().Select(t => t.gameObject));
        }
        return GatherRefsForList(allObjects, objectsToCheck);
    }

    void UpdateMe()
    {
        m_ReferencingObjects = GatherRefsForSelection(Selection.objects);
        m_ReferencedObjects = GatherRefsForList(Selection.objects.OfType<GameObject>().ToList(), null);

        m_ReferencedObjects.Sort((a, b) => string.Compare(a.displayName, b.displayName));
        m_ReferencingObjects.Sort((a, b) => string.Compare(a.displayName, b.displayName));
    }

    static public void DrawInspector(List<DependenciesBrowser.Ref> refs, bool header)
    {
        foreach (Ref r in refs)
        {
            if (!header)
            {
                EditorGUILayout.ObjectField(r.obj, typeof(Object));
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Class Member", $"{r.script}.{r.memberName}", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("Copy", GUILayout.Width(50)))
            {
                StoryGiant.Extensions.SystemExtensions.SetTextOnClipboard(r.memberName);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.TextField("Display Name", r.displayName, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.TextField("Referenced Component", r.component, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));

            if (header)
            {
                EditorGUILayout.ObjectField(r.obj, typeof(Object));
            }

            if (refs.IndexOf(r) != refs.Count-1)
            {
                EditorGUILayout.Space(); EditorGUILayout.Space();
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
#endif // UNITY_EDITOR
