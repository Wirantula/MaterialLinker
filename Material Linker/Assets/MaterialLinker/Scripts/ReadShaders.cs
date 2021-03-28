using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReadShaders
{
    Shader shaderToRead;
    public List<string> textureNames = new List<string>();
    public List<string> textureDescriptions = new List<string>();
    public List<string> colorNames = new List<string>();
    public List<string> colorDescriptions = new List<string>();
    public List<string> floatNames = new List<string>();
    public List<string> floatDescriptions = new List<string>();

    public List<string> GetTextureNames(Shader shader)
    {
        int amountOfProperties = 0;
        textureNames.Clear();
        textureDescriptions.Clear();
        shaderToRead = shader;
        amountOfProperties = shaderToRead.GetPropertyCount();
        for (int i = 0; i < amountOfProperties; i++)
        {
            if(shaderToRead.GetPropertyType(i).ToString() == "Texture")
            {
                textureDescriptions.Add(shaderToRead.GetPropertyDescription(i));
                textureNames.Add(shaderToRead.GetPropertyName(i));
            }

        }
        return textureNames;
    }

    public List<string> GetTextureDescriptions()
    {
        return textureDescriptions;
    }

    public List<string> GetColorNames(Shader shader)
    {
        int amountOfProperties = 0;
        colorNames.Clear();
        colorDescriptions.Clear();
        shaderToRead = shader;
        amountOfProperties = shaderToRead.GetPropertyCount();
        for (int i = 0; i < amountOfProperties; i++)
        {
            if (shaderToRead.GetPropertyType(i).ToString() == "Color")
            {
                colorDescriptions.Add(shaderToRead.GetPropertyDescription(i));
                colorNames.Add(shaderToRead.GetPropertyName(i));
            }

        }
        return colorNames;
    }

    public List<string> GetColorDescriptions()
    {
        return colorDescriptions;
    }

    public List<string> GetFloatNames(Shader shader)
    {
        int amountOfProperties = 0;
        floatNames.Clear();
        floatDescriptions.Clear();
        shaderToRead = shader;
        amountOfProperties = shaderToRead.GetPropertyCount();
        for (int i = 0; i < amountOfProperties; i++)
        {
            if (shaderToRead.GetPropertyType(i).ToString() == "Float" || shaderToRead.GetPropertyType(i).ToString() == "Range")
            {
                floatDescriptions.Add(shaderToRead.GetPropertyDescription(i));
                floatNames.Add(shaderToRead.GetPropertyName(i));
            }

        }
        return floatNames;
    }

    public List<string> GetFloatDescriptions()
    {
        return floatDescriptions;
    }
}