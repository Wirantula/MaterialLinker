using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MLEditorWindow : EditorWindow
{
    public string[] matOptions = new string[] {"Create New Material", "Use Existing Material"};
    public int matOptionIndex = 0;
    public bool editMode = false;
    public bool sceneObjects = false;
    public bool assetsFolder = false;
    string materialName;
    string objectTag;
    List<Texture> texturesToEdit = new List<Texture>();
    Shader shaderToUse;
    List<string> TextureNames = new List<string>();
    List<string> TextureDescription = new List<string>();
    Material matToEdit;
    CreateMaterial _cm = new CreateMaterial();
    ReadShaders _rs = new ReadShaders();
    Vector2 scrollPos;

    public List<GameObject> allGameObjects = new List<GameObject>();

    [MenuItem("Window/Material Linker")]
    public static void ShowWindow()
    {
        GetWindow<MLEditorWindow>("Material Linker");
    }



    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        #region Shader
        GUILayout.BeginVertical(GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
        shaderToUse = (Shader)EditorGUILayout.ObjectField("Shader", shaderToUse, typeof(Shader));
        if (GUILayout.Button(new GUIContent("Load Shader Textures", "This will load the textures of the shader type. If an existing material is used this will also load in existing textures")))
        {
            if (shaderToUse != null)
            {
                TextureNames = _rs.GetShaderInfo(shaderToUse);
                TextureDescription = _rs.GetShaderTextureDescriptions();
                for (int i = 0; i < TextureNames.Count; i++)
                {
                    texturesToEdit.Add(default(Texture));
                }
            }
        }
        editMode = GUILayout.Toggle(editMode,new GUIContent("Edit Mode", "With this toggled on you can edit the material"), GUILayout.Width(100f));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
        GUILayout.Box("Textures", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
        for (int j = 0; j < (texturesToEdit.Count ); j++)
        {
            if (matToEdit != null && !editMode)
            {
                texturesToEdit[j] = matToEdit.GetTexture(TextureNames[j]);
            }
            texturesToEdit[j] = (Texture)EditorGUILayout.ObjectField(TextureDescription[j], texturesToEdit[j], typeof(Texture), false);
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
        #endregion
        #region Material
        GUILayout.BeginVertical(GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));

        matOptionIndex = EditorGUILayout.Popup("Material to use",matOptionIndex, matOptions);
        switch (matOptionIndex)
        {
            case 0:
                materialName = EditorGUILayout.TextField("Material Name", materialName);
                if (GUILayout.Button("Create Material"))
                {
                    matToEdit = _cm.CM(materialName, shaderToUse, TextureNames);
                    _cm._matToEdit = matToEdit;
                    _cm.EditMaterial(texturesToEdit, TextureNames);
                }
                break;
            case 1:
                matToEdit = (Material)EditorGUILayout.ObjectField("Material", matToEdit, typeof(Material));
                if(matToEdit != null)
                {
                    shaderToUse = matToEdit.shader;
                }
                if (GUILayout.Button(new GUIContent("Edit Material", "This will edit the current material. If the material is already assigned, the object with the material will change")))
                {
                    _cm._matToEdit = matToEdit;
                    _cm.EditMaterial(texturesToEdit, TextureNames);
                }
                break;
        }

        GUILayout.EndVertical();
        #endregion
        #region Linker
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MinWidth(200f), GUILayout.MaxWidth(300f));
        assetsFolder = GUILayout.Toggle(assetsFolder, "Apply to Assets in Asset folder", GUILayout.Width(200f));
        sceneObjects = GUILayout.Toggle(sceneObjects, "Apply to scene objects", GUILayout.Width(200f));
        objectTag = EditorGUILayout.TextField("Object Tag", objectTag, GUILayout.ExpandWidth(true));
        
        if (GUILayout.Button(new GUIContent("Apply Material on Object with Object Tag in Name", "This will add the material to objects that have the object tag in their name" )))
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
        #endregion
        GUILayout.EndHorizontal();
    }

    #region DataBase
    void RefreshDatabase()
    {
        if (assetsFolder)
        {
            allGameObjects = FindAllObjectsOfTypeInAssetDatabase<GameObject>();
        }
        if (sceneObjects)
        {
            foreach (GameObject o in FindObjectsOfType<GameObject>())
            {
                allGameObjects.Add(o);
            }
        }
    }

    List<T> FindAllObjectsOfTypeInAssetDatabase<T>() where T : Object
    {
        List<T> results = new List<T>();
        var guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (var s in guids)
        {
            T foundObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(T)) as T;
            if (foundObject != null)
            {
                results.Add(foundObject);
            }
        }
        return results;
    }
    #endregion
}
