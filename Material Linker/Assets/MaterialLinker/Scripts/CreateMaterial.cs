using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateMaterial
{
    public string _name = "test";
    public Material _matToEdit;
    public List<string> _textureNames = new List<string>();
    public List<Texture> _textures = new List<Texture>();

    public Material CM(string matName, Shader shader, List<string> textNames)
    {
        _name = matName;
        _textureNames = textNames;
        Material mat = new Material(shader);
        AssetDatabase.CreateAsset(mat, "Assets/MaterialLinker/MLMaterials/" + _name + ".mat");
        return mat;
    }

    public void EditMaterial(List<Texture> texturesToUse, List<string> textNames)
    {
        _textures = texturesToUse;
        _textureNames = textNames;
        if(_matToEdit != null)
        {
            for (int i = 0; i < (texturesToUse.Count); i++)
            {
                _matToEdit.SetTexture(textNames[i], texturesToUse[i]);
            }
        }
    }
}
