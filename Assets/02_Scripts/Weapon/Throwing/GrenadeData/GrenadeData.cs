using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeData 
{
    public static Mesh mesh;
    public static Material material;
    public static Sprite sprite;
    public static Vector3 midle ;
    public static Vector3 scale ;
    public static GameObject effect;
    public GrenadeData()
    {
        if (mesh == null)
        {
            mesh = Resources.Load<Mesh>("Grenades, Bombs & explosives Pack/Models & Textures/Flashbang/Flashbang");
        }

        if (material == null)
        {
            material = Resources.Load<Material>("Grenades, Bombs & explosives Pack/Models & Textures/Flashbang/Materials/Flashbang_Base_Color");
        }
        if (effect == null)
        {
            effect = Resources.Load<GameObject>("FlashGranadeEffect");
        }
        if (sprite == null)
        {
            Sprite[] spriteAll = Resources.LoadAll<Sprite>("Light theme spritesheet 1");
            sprite = spriteAll[5];
        }

    }
}
