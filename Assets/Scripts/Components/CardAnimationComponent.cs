using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardAnimationComponent : MonoBehaviour
{
    [SerializeField] private Vector3 shift = new Vector3(0, 1);
    [SerializeField] private float speed = 5;
    private bool animate = true, inMove, startMoment = true;
    private Vector3 StartPos = Vector3.zero;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    void OnEnable()
    {
        HandBehaviour.Notify += InitHandler;
        HandBehaviour.Notify += UpdatePosition;
    }
    void OnDisable()
    {
        HandBehaviour.Notify -= InitHandler;
        HandBehaviour.Notify -= UpdatePosition;
    }
    void InitHandler() => Init();
    public async UniTaskVoid Init()
    {
        await UniTask.DelayFrame(3);
        startMoment = false;
    }
    async void UpdatePosition()
    {
        await UniTask.DelayFrame(5);
        StartPos = transform.position;
    }
    public void OnMouseDown()
    {
        if(startMoment) return;
        animate = false;
        tokenSource.Cancel();
    }
    public void OnMouseUp()
    {
        if(startMoment) return;
        animate = true;
        tokenSource = new CancellationTokenSource();
    }
    public async void OnMouseEnter()
    {
        if(startMoment || !animate || inMove) return;
        inMove = true;
        if(StartPos == Vector3.zero) StartPos = transform.position;
        tokenSource.Cancel();
        await UniTask.Yield();
        tokenSource = new CancellationTokenSource();
        MoveUtility.Move(transform, StartPos, StartPos + shift,  tokenSource.Token, speed);
    }
    public async void OnMouseExit()
    {
        if(startMoment || !animate || !inMove) return;
        inMove = false;
        tokenSource.Cancel();
        await UniTask.Yield();
        tokenSource = new CancellationTokenSource();
        MoveUtility.Move(transform, StartPos + shift, StartPos,  tokenSource.Token, speed);
    }
    
}
