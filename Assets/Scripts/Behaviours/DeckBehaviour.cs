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
        
        var go = Instantiate(Prefab, transform.position, Quaternion.identity);
        var card = go.GetComponent<CardsBehaviour>();
        card.Init(Random.Range(1, 11), CardsBehaviour.Side.Blue, FirstHand);
        var result = await FirstHand.AddCardToHand(card,DeckSpeed);
        if (!result)
        {
            await MoveUtility.Move(go.transform, go.transform.position, 
                Trash.transform.position , CancellationToken.None, DeckSpeed);
            go.SetActive(false);
        }
        
        go = Instantiate(Prefab, transform.position, Quaternion.identity);
        card = go.GetComponent<CardsBehaviour>();
        card.Init(Random.Range(1, 11), CardsBehaviour.Side.Red, SecondHand);
        result = await SecondHand.AddCardToHand(card,DeckSpeed);
        if (!result)
        {
            await MoveUtility.Move(go.transform, go.transform.position, 
                Trash.transform.position , CancellationToken.None, DeckSpeed);
            go.SetActive(false);
        }
        
        
        inProgress = false;
    }
}
