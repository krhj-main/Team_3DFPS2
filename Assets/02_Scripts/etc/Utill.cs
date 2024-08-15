using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utill
{
    public static void DestroyOnLoad(GameObject obj)
    {
        GameObject _go = GameObject.FindGameObjectWithTag("Enviorment");
        obj.transform.SetParent(_go.transform);
        obj.transform.SetParent(null);
    }
}
