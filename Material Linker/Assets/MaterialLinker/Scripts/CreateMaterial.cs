using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateMaterial
{
    public string name = "test";
    public Material _matToEdit;

    public Material CM(string matName)
    {
        name = matName;
        Material mat = new Material(Shader.Find("Standard"));
        AssetDatabase.CreateAsset(mat, "Assets/MaterialLinker/MLMaterials/" + name + ".mat");
        return mat;
    }

    public void EditMaterial(Texture main, Texture detail, Texture normal, Texture detailMainTex, Texture detailNormal, Texture emission, Texture occlusion, Texture metallic, Texture parallax)
    {
        if(_matToEdit != null)
        {
            _matToEdit.SetTexture("_MainTex", main);
            _matToEdit.SetTexture("_BumpMap", normal);
            if(emission != null)
            {
                _matToEdit.EnableKeyword("_EMISSION");
                _matToEdit.SetTexture("_EmissionMap", emission);
            }
            _matToEdit.SetTexture("_OcclusionMap", occlusion);
            _matToEdit.SetTexture("_MetallicGlossMap", metallic);
            _matToEdit.SetTexture("_ParallaxMap", parallax);

            _matToEdit.SetTexture("_DetailNormalMap", detailNormal);
            _matToEdit.SetTexture("_DetailMask", detail);
            _matToEdit.SetTexture("_DetailAlbedoMap", detailMainTex);
        }
    }
}
