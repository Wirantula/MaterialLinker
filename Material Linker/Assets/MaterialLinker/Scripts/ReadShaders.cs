using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReadShaders
{
    Shader shaderToRead;
    int amountOfProperties;
    public List<string> textureNames = new List<string>();
    public List<string> textureDescriptions = new List<string>();

    public List<string> GetShaderInfo(Shader shader)
    {
        amountOfProperties = 0;
        textureNames.Clear();
        shaderToRead = shader;
        amountOfProperties = shaderToRead.GetPropertyCount();
        for (int i = 0; i < amountOfProperties; i++)
        {
            Debug.Log(shaderToRead.GetPropertyName(i) + " // " + shaderToRead.GetPropertyType(i));
            if(shaderToRead.GetPropertyType(i).ToString() == "Texture")
            {
                textureDescriptions.Add(shaderToRead.GetPropertyDescription(i));
                textureNames.Add(shaderToRead.GetPropertyName(i));
            }

        }
        return textureNames;
    }

    public List<string> GetShaderTextureDescriptions()
    {
        return textureDescriptions;
    }

}