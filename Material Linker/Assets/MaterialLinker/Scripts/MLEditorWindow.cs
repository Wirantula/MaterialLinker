using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MLEditorWindow : EditorWindow
{
    public string[] matOptions = new string[] {"Create New Material", "Use Existing Material"};
    public int matOptionIndex = 0;
    public int objectAmount = 0;
    public bool showToolHelp = false;
    public bool editMode = false;
    public bool sceneObjects = false;
    public bool assetsFolder = false;
    public bool editTextures = false;
    public bool editColors = false;
    public bool editFloats = false;
    string materialName;
    string objectTag;
    List<Texture> texturesToEdit = new List<Texture>();
    List<Color> colorsToEdit = new List<Color>();
    List<float> floatsToEdit = new List<float>();
    List<GameObject> objectsToLinkTo = new List<GameObject>();
    Shader shaderToUse;
    List<string> TextureNames = new List<string>();
    List<string> TextureDescription = new List<string>();
    List<string> ColorNames = new List<string>();
    List<string> ColorDescriptions = new List<string>();
    List<string> FloatNames = new List<string>();
    List<string> FloatDescriptions = new List<string>();
    Material matToEdit;
    CreateMaterial _cm = new CreateMaterial();
    ReadShaders _rs = new ReadShaders();
    Vector2 shaderScrolPos;
    Vector2 objectScrolPos;

    public List<GameObject> allGameObjects = new List<GameObject>();

    [MenuItem("Window/Material Linker")]
    public static void ShowWindow()
    {
        GetWindow<MLEditorWindow>("Material Linker");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        #region Material
        GUILayout.BeginVertical(GUILayout.MinWidth(300f), GUILayout.MaxWidth(300f));
        showToolHelp = GUILayout.Toggle(showToolHelp, new GUIContent("Show Tool Guide", "Toggle this to show the tool guide"), GUILayout.Width(200f));
        if (showToolHelp)
        {
            EditorGUILayout.HelpBox("This tool is made to make it easier to quickly make materials and assign them on objects without having to actually touch the objects and accidentally changing things you shouldn't change.\n" +
                "To use the tool, start by either selecting an existing material you want to use or create a new material.\n" +
                "To create a new material, type in the name you want to use, select a shader type and press the create material button.\n" +
                "When using an existing material the shader type will automatically get selected.\n" +
                "If you press load shader, the texture, color and float fields of the shader will be loaded and you can show them by toggling the types.\n" +
                "You will see that you cant edit them straight away, toggle the edit mode to be able to edit the values.\n" +
                "After editing the values you want to, press the edit material button and the material will get updated.\n" +
                "To now apply the material on objects you can choose to either add object slots to only assign to the selected objects, or you can type in an object tag, select whether to apply to scene objects, objects in the asset folder or both.\n" +
                "The object tag will make it so the material gets applied to everything that has that tag in their name, so if you type in rock, all objects that have the word rock in their name will get the material applied.", MessageType.None, true);
        }
        GUILayout.Box("Material", GUILayout.MinWidth(300f), GUILayout.MaxWidth(300f));
        matOptionIndex = EditorGUILayout.Popup("Material to use", matOptionIndex, matOptions);
        switch (matOptionIndex)
        {
            case 0:
                materialName = EditorGUILayout.TextField("Material Name", materialName);
                if (GUILayout.Button("Create Material"))
                {
                    if (materialName != null && shaderToUse != null)
                    {
                        matToEdit = _cm.CM(materialName, shaderToUse, TextureNames);
                        _cm._matToEdit = matToEdit;
                        _cm.EditMaterial(texturesToEdit, TextureNames, colorsToEdit, ColorNames, floatsToEdit, FloatNames);
                    }
                    else { Debug.Log("You either haven't written a name or you haven't selected a shader type"); }
                }
                if(GUILayout.Button(new GUIContent("Edit Material", "This will edit the current material. If the material is already assigned, the object with the material will change")))
                {
                    _cm._matToEdit = matToEdit;
                    _cm.EditMaterial(texturesToEdit, TextureNames, colorsToEdit, ColorNames, floatsToEdit, FloatNames);
                }
                break;
            case 1:
                matToEdit = (Material)EditorGUILayout.ObjectField("Material", matToEdit, typeof(Material));
                if (matToEdit != null)
                {
                    shaderToUse = matToEdit.shader;
                    UpdateShaderAfterMaterial();
                }
                if (GUILayout.Button(new GUIContent("Edit Material", "This will edit the current material. If the material is already assigned, the object with the material will change")))
                {
                    _cm._matToEdit = matToEdit;
                    _cm.EditMaterial(texturesToEdit, TextureNames, colorsToEdit, ColorNames, floatsToEdit, FloatNames);
                }
                break;
        }
        GUILayout.EndVertical();
        #endregion
        #region Shader
        GUILayout.BeginVertical(GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
        GUILayout.Box("Shader", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
        shaderToUse = (Shader)EditorGUILayout.ObjectField("Shader", shaderToUse, typeof(Shader));
        if (GUILayout.Button(new GUIContent("Load Shader", "This will load the selected types of the shader. If an existing material is used this will also load in existing textures")))
        {
            if (shaderToUse != null)
            {
                TextureNames.Clear();
                TextureDescription.Clear();
                texturesToEdit.Clear();
                ColorNames.Clear();
                ColorDescriptions.Clear();
                colorsToEdit.Clear();
                FloatNames.Clear();
                FloatDescriptions.Clear();
                floatsToEdit.Clear();
                TextureNames = _rs.GetTextureNames(shaderToUse);
                TextureDescription = _rs.GetTextureDescriptions();
                if (texturesToEdit.Count < TextureNames.Count)
                {
                    for (int i = 0; i < TextureNames.Count; i++)
                    {
                        texturesToEdit.Add(default(Texture));
                    }

                    ColorNames = _rs.GetColorNames(shaderToUse);
                    ColorDescriptions = _rs.GetColorDescriptions();
                    for (int i = 0; i < ColorNames.Count; i++)
                    {
                        colorsToEdit.Add(default(Color));
                    }

                    FloatNames = _rs.GetFloatNames(shaderToUse);
                    FloatDescriptions = _rs.GetFloatDescriptions();
                    for (int i = 0; i < FloatNames.Count; i++)
                    {
                        floatsToEdit.Add(default(float));
                    }
                }
            }
        }
        GUILayout.Box("Edit Mode", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
        editMode = GUILayout.Toggle(editMode,new GUIContent("Edit Mode", "With this toggled on you can edit the material"), GUILayout.Width(100f));
        GUILayout.Box("Types to edit", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
        editTextures = GUILayout.Toggle(editTextures, new GUIContent("Show Textures", "This will load the textures of the shader type"), GUILayout.Width(100f));
        editColors = GUILayout.Toggle(editColors, new GUIContent("Show Colors", "This will load the colors of the shader"), GUILayout.Width(100f));
        editFloats = GUILayout.Toggle(editFloats, new GUIContent("Show Floats", "This will load the floats of the shader"), GUILayout.Width(100f));
        shaderScrolPos = EditorGUILayout.BeginScrollView(shaderScrolPos, GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
        if (editTextures)
        {
            GUILayout.Box("Textures", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
            for (int i = 0; i < texturesToEdit.Count; i++)
            {
                if (matToEdit != null && !editMode)
                {
                    texturesToEdit[i] = matToEdit.GetTexture(TextureNames[i]);
                }
                texturesToEdit[i] = (Texture)EditorGUILayout.ObjectField(TextureDescription[i], texturesToEdit[i], typeof(Texture), false);
            }
        }
        if (editColors)
        {
            GUILayout.Box("Colors", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
            for (int i = 0; i < colorsToEdit.Count; i++)
            {
                if (matToEdit != null && !editMode)
                {
                    colorsToEdit[i] = matToEdit.GetColor(ColorNames[i]);
                }
                colorsToEdit[i] = EditorGUILayout.ColorField(new GUIContent(ColorNames[i]), colorsToEdit[i], true, true, true, GUILayout.Width(200f));
            }
        }
        if (editFloats)
        {
            GUILayout.Box("Ranges", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
            for (int i = 0; i < floatsToEdit.Count; i++)
            {
                if(matToEdit != null && !editMode)
                {
                    floatsToEdit[i] = matToEdit.GetFloat(FloatNames[i]);
                }
                floatsToEdit[i] = EditorGUILayout.Slider(new GUIContent(FloatDescriptions[i]), floatsToEdit[i], 0f, 1f, GUILayout.MinWidth(300f), GUILayout.ExpandWidth(true));
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
        #endregion
        #region Linker
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MinWidth(200f), GUILayout.MaxWidth(800f));
        GUILayout.Box("Auto Link", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
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
        GUILayout.Box("Objects to Link to", GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
        if(GUILayout.Button(new GUIContent("Apply Material only to selected GameObjects", "This will only apply to selected objects and not to objects with tag in their name")))
        {
            foreach(GameObject o in objectsToLinkTo)
            {
                o.GetComponent<Renderer>().material = matToEdit;
            }
        }
        if(GUILayout.Button(new GUIContent("Add new object slot", "This adds a new object slot to assign object to to apply material")))
        {
            objectAmount++;
            objectsToLinkTo.Add(default(GameObject));
        }
        if (GUILayout.Button(new GUIContent("Remove object slot", "This removes object slot to assign object to to apply material")) && objectAmount > 0)
        {
            objectAmount--;
            objectsToLinkTo.Remove(default(GameObject));
        }
        objectScrolPos = EditorGUILayout.BeginScrollView(objectScrolPos, GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(800f));
        for (int i = 0; i < objectsToLinkTo.Count; i++)
        {
            objectsToLinkTo[i] = (GameObject)EditorGUILayout.ObjectField("Object", objectsToLinkTo[i], typeof(GameObject), false);
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
        #endregion
        GUILayout.EndHorizontal();
    }

    public void UpdateShaderAfterMaterial()
    {
        TextureNames.Clear();
        TextureDescription.Clear();
        texturesToEdit.Clear();
        ColorNames.Clear();
        ColorDescriptions.Clear();
        colorsToEdit.Clear();
        FloatNames.Clear();
        FloatDescriptions.Clear();
        floatsToEdit.Clear();
        TextureNames = _rs.GetTextureNames(shaderToUse);
        TextureDescription = _rs.GetTextureDescriptions();
        if (texturesToEdit.Count < TextureNames.Count)
        {
            for (int i = 0; i < TextureNames.Count; i++)
            {
                texturesToEdit.Add(default(Texture));
            }

            ColorNames = _rs.GetColorNames(shaderToUse);
            ColorDescriptions = _rs.GetColorDescriptions();
            for (int i = 0; i < ColorNames.Count; i++)
            {
                colorsToEdit.Add(default(Color));
            }

            FloatNames = _rs.GetFloatNames(shaderToUse);
            FloatDescriptions = _rs.GetFloatDescriptions();
            for (int i = 0; i < FloatNames.Count; i++)
            {
                floatsToEdit.Add(default(float));
            }
        }
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
