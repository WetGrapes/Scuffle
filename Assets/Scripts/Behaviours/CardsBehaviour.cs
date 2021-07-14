
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CardsBehaviour : MonoBehaviour
{
    [SerializeField, Header("Сила"), HideLabel][Range(1,10)]
    private int _power;

    public enum Side
    {
        Nobody,Red, Blue 
    }
    [SerializeField, Header("Сторона"), HideLabel] private Side _side;
    public Side SideProperty { private set; get; }
    public int PowerProperty { private set; get; }

    private HandBehaviour handBehaviour;
    private SpriteRenderer _renderer;
    private SpriteRenderer Renderer
    {
        get
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            return _renderer;
        }
    }
    private TextMeshPro _text;
    private TextMeshPro text
    {
        get
        {
            if (_text == null) _text = GetComponentInChildren<TextMeshPro>();
            return _text;
        }
    }
    

    void Awake()
    {
        Init(_power,_side);
    }
    void OnValidate()
    {
        Init(_power,_side);
    }

    public void ReportLandingTo (Transform area)
    {
        handBehaviour.RemoveFromHand(this, area);
    }
    
    public void Init(int power, Side side, HandBehaviour handBehaviour = null)
    {
        if (side == Side.Nobody)
        {
            Debug.LogError("карты должны кому-то принадлежать");
            return;
        }
        SideProperty = side;
        PowerProperty = power;
        this.handBehaviour = handBehaviour;
        var col = new Color();
        if (side == Side.Blue) ColorUtility.TryParseHtmlString("#06C5FA", out col);
        else ColorUtility.TryParseHtmlString("#F65254", out col);
        Renderer.color = col;
        text.text = power.ToString();
    }
  
}

