using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInfoBehaviour : MonoBehaviour
{
    public Text Power, Owner, Size, Position;
    private GameObject child;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        AreaStaticStorage.NotifySetArea += SetArea;
        AreaStaticStorage.NotifySetNullArea += UnsetArea;
        child = transform.GetChild(0).gameObject;
    }

    private void SetArea(AreaBehaviour areaBehaviour)
    {
        child.SetActive(true);
        var info = areaBehaviour.GetArea();
        Owner.text = info.Owner.ToString();
        Power.text = info.Power.ToString();
        Size.text = info.Importance.ToString();
        Position.text = info.PositionInSpace.ToString();
    }

    private void UnsetArea()
    {
        child.SetActive(false);
    }
}
