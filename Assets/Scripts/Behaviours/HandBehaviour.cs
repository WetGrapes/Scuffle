using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class HandBehaviour : MonoBehaviour
{
    public static event Action Notify;
    [ShowInInspector] private List<CardsBehaviour> Cards = new List<CardsBehaviour>();
    [SerializeField] private int capacity;
    [ShowInInspector] private bool moveHasTaken;
    public bool RemoveFromHand(CardsBehaviour card, Transform newParent)
    {
        if (card == null || newParent == null) return false;
        card.transform.parent = newParent;
        Cards.Remove(card);
        moveHasTaken = true;
       
        Notify?.Invoke();
        return true;
    }

    public bool MoveHasTaken => moveHasTaken;
    public async UniTask<bool> AddCardToHand(CardsBehaviour card, float speed)
    {
        if(!HasPlace) return false;
        await MoveUtility.Move(card.transform, card.transform.position,
            transform.position - new Vector3(0, 0, -5.5f), CancellationToken.None, speed);
        card.transform.SetParent(transform);
        Cards.Add(card);
        moveHasTaken = false;
        Notify?.Invoke();
        return true;
    }

    public bool HasPlace => Cards.Count < capacity;
}
