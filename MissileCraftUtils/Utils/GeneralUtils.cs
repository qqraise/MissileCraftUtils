using UnityEngine;
using System.Collections;
using System;

public class GeneralUtils {

    public static bool IsCallbackValid(Delegate callback)
    {
        bool flag = true;
        if (callback == null)
        {
            return false;
        }
        if (!callback.Method.IsStatic)
        {
            flag = IsObjectAlive(callback.Target);
            if (!flag)
            {
                UnityEngine.Debug.LogError(string.Format("Target for callback {0} is null.", callback.Method.Name));
            }
        }
        return flag;
    }

    public static bool IsObjectAlive(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (!(obj is UnityEngine.Object))
        {
            return true;
        }
        UnityEngine.Object obj2 = (UnityEngine.Object)obj;
        return (bool)obj2;
    }

}

