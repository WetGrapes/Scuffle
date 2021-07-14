using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseHandler
{
    public abstract void Interaction(Transform caller, Transform callee);
}
