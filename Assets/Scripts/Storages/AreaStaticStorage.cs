using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaStaticStorage : IStaticStorage
{
    public static event Action<AreaBehaviour> NotifySetArea;
    public static event Action<NeighboursStorage.PositionInSpace> NotifyFindNeighbours;
    public static event Action NotifySetNullArea;
    
    private static bool active;

    public void SetActive(bool val)
    {
        active = val;
        if(val) return;
        NotifyFindNeighbours = null;
        NotifySetArea = null;
        NotifySetNullArea = null;
    }

    public static void SetArea(AreaBehaviour val = null)
    {
        if(!active) return;
        if(val == null) NotifySetNullArea?.Invoke();
        else NotifySetArea?.Invoke(val);
    }

    public static void ReferringToNeighbours(NeighboursStorage.PositionInSpace position)
    {
        if(!active) return;
        NotifyFindNeighbours?.Invoke(position);
    }

   
}
