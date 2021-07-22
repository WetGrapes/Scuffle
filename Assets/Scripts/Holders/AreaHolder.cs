using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class AreaHolder : MonoBehaviour
{
    //public static AreaHolder Instance;
    [ShowInInspector] private List<GameObject> Childes;
    [ShowInInspector] private List<AreaBehaviour> ChildesAreaBehaviours;
    [ShowInInspector] private AreaBehaviour SelectedArea;

    private NeighboursStorage.Position? previousSpace;
    private readonly Color empty = new Color(0, 0, 0, 0);

    void Awake()
    {
        Init();
        AreaStaticStorage.NotifySetArea += value => SelectedArea = value;
        AreaStaticStorage.NotifySetNullArea += () => SelectedArea = null;
        AreaStaticStorage.NotifyFindNeighbours += FindNeighbors;
    }


    private void FindNeighbors(NeighboursStorage.Position MyPosition)
    {
        if (MyPosition != previousSpace)
        {
            previousSpace = MyPosition;
            var positions = NeighboursStorage.NeighboringAreas[MyPosition];
            for (var i = 0; i < Childes.Count; i++)
            {
                foreach (var _ in positions.Where(position => ChildesAreaBehaviours[i].PositionsEqual(position)))
                {
                    ChildesAreaBehaviours[i].SetOutlineColor(Color.red);
                    goto Found;
                }

                if (ChildesAreaBehaviours[i].PositionsEqual(MyPosition))
                {
                    ChildesAreaBehaviours[i].SetOutlineColor(Color.green);
                    AreaStaticStorage.SetArea(ChildesAreaBehaviours[i]);
                }
                else
                    ChildesAreaBehaviours[i].SetOutlineColor(empty);
                Found: ;
            }
        }
        else
        {
            previousSpace = null;
            for (var i = 0; i < Childes.Count; i++)
            {
                ChildesAreaBehaviours[i].SetOutlineColor(empty);
                AreaStaticStorage.SetArea();
            }
        }
    }

    void OnValidate()
    {
        Init();
    }

    void Init()
    {
        Childes = new List<GameObject>();
        ChildesAreaBehaviours = new List<AreaBehaviour>();
        foreach (Transform child in transform)
        {
            Childes.Add(child.gameObject);
            ChildesAreaBehaviours.Add(child.GetComponent<AreaBehaviour>());
        }
    }
}
