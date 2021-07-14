using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class DragAndDropComponent : SerializedMonoBehaviour
{
    public BaseHandler handler;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float error; 
    [Range(1,10)][SerializeField] private float autopilotSpeed;
    [SerializeField] private bool autopilot;
    [Space] [SerializeField] private float scaleOnStart, scaleOnDrag;
    
    private SpriteRenderer renderer;
    private BoxCollider2D collider;
    private Camera cam;
    private Vector3 startSize, startPos, mOffset;
    private float mZCoord;
    
    void Awake()
    {
        startSize = transform.localScale;
        transform.localScale = startSize * scaleOnStart;
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        cam = Camera.main;
        if (collider != null) return;
        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = renderer.size;
    }
    
      void OnMouseDown()
    {
        startPos = transform.position;
        transform.localScale = startSize * scaleOnDrag;
        collider.enabled = false;
        mZCoord = cam.WorldToScreenPoint(
            gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }
    
    private Vector3 GetMouseAsWorldPoint() {
        var mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return cam.ScreenToWorldPoint(mousePoint);
    }


    
    void OnMouseDrag() {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    async void OnMouseUp()
    {
        transform.localScale = startSize * scaleOnStart;
        var j = 0;
        var minRange = float.MaxValue;
        for (var i = 0; i < targets.Count; i++)
        {
            var x = targets[i].position.x - transform.position.x;
            var y = targets[i].position.y - transform.position.y;
            var distance = (x < 0 ? -x : x) + (y < 0 ? -y : y);
            if (!(distance < minRange)) continue;
            j = i;
            minRange = distance;
        }
        
        if (minRange < error)
        {
            await MoveUtility.Move(transform, transform.position, 
                targets[j].position, CancellationToken.None, autopilotSpeed);
            handler.Interaction(transform, targets[j]);
        }
        else
        {
            await MoveUtility.Move(transform, transform.position, 
                startPos, CancellationToken.None, autopilotSpeed);
            collider.enabled = true;
        }
    }
}
