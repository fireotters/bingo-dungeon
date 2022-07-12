using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Returns where the gameObject is in tha LayerMask or not.
    /// </summary>
    public static bool IsInLayerMask(this GameObject obj, LayerMask layerMask) => (layerMask & (1 << obj.layer)) > 0;

    /// <summary>
    /// Destroys all the children of the transform.
    /// </summary>
    public static void DestroyAllChildren(this Transform transform)
    {
        int num = transform.childCount;
        for (int i = 0; i < num; i++)
            Object.Destroy(transform.GetChild(0).gameObject);
    }

    public static void DestroyAllChildrenEditor(this Transform transform)
    {
        int num = transform.childCount;
        for (int i = 0; i < num; i++)
            Object.DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
