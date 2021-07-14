using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class MoveUtility 
{
    public static async UniTask<bool> Move(Transform whom, Vector3 from, Vector3 to, CancellationToken token = default,
        float speed = 5, bool simple = true)
    {
        await UniTask.WaitWhile(()=>token.IsCancellationRequested);
        if (simple) return await SimpleMove(whom, from, to, speed, token);
        return await NotSimpleMove(whom, from, to, speed, token);
    }

    private static async UniTask<bool> SimpleMove(Transform whom, Vector3 @from, Vector3 to,
        float speed, CancellationToken token = default)
    {
        for (float i = 0; i <= 1; i += Time.deltaTime*speed)
        {
            if (token.IsCancellationRequested) return false;
            await UniTask.Yield();
            whom.position = Vector3.Lerp(from, to, i);
        }
        whom.position = to;
        return true;
    }
    
    private static async UniTask<bool> NotSimpleMove(Transform whom, Vector3 from, Vector3 to,
        float speed, CancellationToken token = default)
    {
        var v = new Vector3();
        var tmp = Time.deltaTime * speed;
        while ( Vector3.Distance(from, to) > 1f)
        {
            if (token.IsCancellationRequested) return false;
            await UniTask.Yield();
            from = Vector3.SmoothDamp(from, to, ref v, tmp);
            whom.position = from;
            tmp = Time.deltaTime * speed;
        }
        whom.position = to;
        return true;
    }
}
