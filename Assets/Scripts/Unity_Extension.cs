using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Unity_Extension
{
    public static T[] GetComponentsOnlyInChildren<T>(this GameObject _gameObject, bool _includeInactive= true) where T : Component
        {
            T[] tmp = _gameObject.GetComponentsInChildren<T>(_includeInactive);
            if(tmp[0].gameObject.GetInstanceID() == _gameObject.GetInstanceID())
            {
                System.Collections.Generic.List<T> list= new System.Collections.Generic.List<T>(tmp);
                list.RemoveAt(0);
                return list.ToArray();
            }
            return tmp;
        }
}
