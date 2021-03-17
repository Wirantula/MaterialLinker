using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MLEditorWindow : EditorWindow
{
    string materialName;
    string objectTag;
    Texture _MLmainTex;
    Texture _MLdetailMask;
    Texture _MLnormalMap;
    Texture _MLalbedoMap;
    Texture _MLemissionMap;
    Texture _MLdetailNormalMap;
    Texture _MLocclusionMap;
    Texture _MLmetallicGlossMap;
    Texture _MLparallaxMap;
    
    Material matToEdit;
    CreateMaterial _cm = new CreateMaterial();

    public List<GameObject> allGameObjects = new List<GameObject>();

    [MenuItem("Window/Material Linker")]
    public static void ShowWindow()
    {
        GetWindow<MLEditorWindow>("Material Linker");
    }

    private void OnGUI()
    {

        GUILayout.BeginHorizontal();
        //window

        GUILayout.BeginVertical(GUILayout.MinWidth(100f));
        //main textures
        _MLmainTex = (Texture)EditorGUILayout.ObjectField("Main Texture", _MLmainTex, typeof(Texture));
        _MLnormalMap = (Texture)EditorGUILayout.ObjectField("Normal Map", _MLnormalMap, typeof(Texture));
        _MLemissionMap = (Texture)EditorGUILayout.ObjectField("Emission Map", _MLemissionMap, typeof(Texture));
        _MLocclusionMap = (Texture)EditorGUILayout.ObjectField("Occlusion Map", _MLocclusionMap, typeof(Texture));
        _MLmetallicGlossMap = (Texture)EditorGUILayout.ObjectField("Metallic Map", _MLmetallicGlossMap, typeof(Texture));
        _MLparallaxMap = (Texture)EditorGUILayout.ObjectField("Parallax Map", _MLparallaxMap, typeof(Texture));

        //detail textures
        _MLdetailMask = (Texture)EditorGUILayout.ObjectField("Detail Mask", _MLdetailMask, typeof(Texture));
        _MLalbedoMap = (Texture)EditorGUILayout.ObjectField("Detail Main Tex", _MLalbedoMap, typeof(Texture));
        _MLdetailNormalMap = (Texture)EditorGUILayout.ObjectField("Detail Normal Map", _MLdetailNormalMap, typeof(Texture));
        GUILayout.EndVertical();


        GUILayout.BeginVertical(GUILayout.MinWidth(200f));
        materialName = EditorGUILayout.TextField("Material Name", materialName);
        
        matToEdit = (Material)EditorGUILayout.ObjectField("Material", matToEdit, typeof(Material));
        
        if(GUILayout.Button("Create Material"))
        {
            matToEdit = _cm.CM(materialName);
        }

        if(GUILayout.Button("Edit Material"))
        {
            _cm._matToEdit = matToEdit;
            _cm.EditMaterial(_MLmainTex, _MLdetailMask, _MLnormalMap, _MLalbedoMap, _MLdetailNormalMap, _MLemissionMap, _MLocclusionMap,_MLmetallicGlossMap, _MLparallaxMap);
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.MinWidth(200f));
        objectTag = EditorGUILayout.TextField("Object Tag", objectTag, GUILayout.MinWidth(100f));
        if(GUILayout.Button("Link to objects with tag"))
        {
            RefreshDatabase();
            foreach(GameObject o in allGameObjects)
            {
                if (o.name.Contains(objectTag))
                {
                    o.GetComponent<Renderer>().material = matToEdit;
                }
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();


        void RefreshDatabase()
        {
            allGameObjects = FindAllObjectsOfTypeInAssetDatabase<GameObject>();
        }

        List<T> FindAllObjectsOfTypeInAssetDatabase<T>() where T : Object
        {
            Debug.Log("Looking for type: " + typeof(T).ToString());
            List<T> results = new List<T>();
            var guids = AssetDatabase.FindAssets("t:Prefab");
            foreach(var s in guids)
            {
                T foundObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(T)) as T;
                if(foundObject != null)
                {
                    results.Add(foundObject);
                    Debug.Log(typeof(T).ToString() + " Added: " + foundObject.name);
                }
            }
            return results;
        }
    }
}
