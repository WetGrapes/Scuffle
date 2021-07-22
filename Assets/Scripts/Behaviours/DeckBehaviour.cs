using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DeckBehaviour : MonoBehaviour
{

    [Range(1,15)]public float DeckSpeed = 7;
    public GameObject Prefab, Trash;
    public HandBehaviour FirstHand, SecondHand;
    private bool inProgress;
    async UniTaskVoid Start()
    {
        await UniTask.DelayFrame(5);
        await Turn();
        await Turn();
        await Turn();
    }

    public void TurnButton() => Turn();

    private async UniTask Turn()
    {
        if (inProgress) return;
        inProgress = true;
        
        var go = Instantiate(Prefab, transform);
        var card = go.GetComponent<CardsBehaviour>();
        card.Init(Random.Range(1, 11), CardsBehaviour.Side.Blue, FirstHand);
        var result = await FirstHand.AddCardToHand(card, new Vector3(-8,-7,0), DeckSpeed);
        if (!result)
        {
            await MoveUtility.Move(card.transform, card.transform.position,
                card.transform.position+new Vector3(-8,0,0), CancellationToken.None,10);
            await MoveUtility.Move(card.transform, card.transform.position,
                card.transform.position+new Vector3(0, -7,0), CancellationToken.None,20);
            MoveUtility.Rotate(go.transform, transform.rotation, Trash.transform.rotation, 
                CancellationToken.None, DeckSpeed);
            await MoveUtility.Move(go.transform, go.transform.position, 
                Trash.transform.position , CancellationToken.None, DeckSpeed);
            go.SetActive(false);
        }
        
        go = Instantiate(Prefab, transform);
        card = go.GetComponent<CardsBehaviour>();
        card.Init(Random.Range(1, 11), CardsBehaviour.Side.Red, SecondHand);
        result = await SecondHand.AddCardToHand(card,new Vector3(-8,7,0),DeckSpeed);
        if (!result)
        {
            await MoveUtility.Move(card.transform, card.transform.position,
                card.transform.position+new Vector3(-8,0,0), CancellationToken.None,10);
            await MoveUtility.Move(card.transform, card.transform.position,
                card.transform.position+new Vector3(0, 7,0), CancellationToken.None,20);
            MoveUtility.Rotate(go.transform, transform.rotation, Trash.transform.rotation, 
                CancellationToken.None, DeckSpeed);
            await MoveUtility.Move(go.transform, go.transform.position, 
                Trash.transform.position , CancellationToken.None, DeckSpeed);
            go.SetActive(false);
        }
        
        
        inProgress = false;
    }
}
