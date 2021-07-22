using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class AreaBehaviour : MonoBehaviour
{
    #region  Enums and structs
    public enum Importance
    {
        small = 1,
        medium = 2,
        large = 3
    };
    [Serializable]
    public struct AreaInfo
    {
        [ReadOnly] public int PowerRed, PowerBlue, Power;
        [ReadOnly] public CardsBehaviour.Side Owner;
        [Header("Важность района"), HideLabel] public Importance Importance;
        [Header("Позиция района"), HideLabel] public NeighboursStorage.Position Position;
    }
    #endregion


    [SerializeField] private AreaInfo area;
    public AreaInfo GetArea() => area;

    private SpriteRenderer _childRenderer;
    private SpriteRenderer ChildRenderer
    {
        get
        {
            if (_childRenderer == null) _childRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            return _childRenderer;
        }
    }

    private SpriteRenderer _renderer;
    private SpriteRenderer Renderer
    {
        get
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            return _renderer;
        }
    }

    private Color CachedColor;
    private Vector3 _vector3;
    void Awake()
    {
        _vector3 = transform.localScale;
    }

    public bool PositionsEqual(NeighboursStorage.Position val) => area.Position == val;
    public void ChangePower(int power, CardsBehaviour.Side side)
    {
        if (side == CardsBehaviour.Side.Red) area.PowerRed += power;
        else area.PowerBlue += power;
        area.Power = area.PowerBlue - area.PowerRed;
        Color color;
        switch (Sign(area.Power))
        {
            case 1:
                ColorUtility.TryParseHtmlString("#06C5FA", out color);
                area.Owner = CardsBehaviour.Side.Blue;
                break;
            case -1:
                ColorUtility.TryParseHtmlString("#F65254", out color);
                area.Owner = CardsBehaviour.Side.Red;
                break;
            default:
                color = Color.white;
                area.Owner = CardsBehaviour.Side.Nobody;
                break;
        }
        SetColor(color);
    }

    private static int Sign(int val)
    {
        if (val == 0) return 0;
        if (val > 0) return 1;
        return -1;
    }
   
    public void OnMouseDown()
    {
        AreaStaticStorage.ReferringToNeighbours(area.Position);
    }
    public void OnMouseUp()
    {
        transform.localScale = _vector3;
    }

    public void SetColor(Color newColor)
    {
        Renderer.color = newColor;
    }
    public void SetOutlineColor(Color newColor)
    {
        ChildRenderer.color = newColor;
    }
    
    public void OnMouseEnter()
    {
        if (!Input.GetMouseButton(0)) return;
        transform.localScale = _vector3 * 1.1f;
    }
    public void OnMouseExit()
    {
        transform.localScale = _vector3;
    }
    
    
}