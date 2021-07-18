using UnityEngine;

public class AreaHandler : BaseHandler
{
    public override bool Interaction(Transform caller, Transform callee)
    {
        var card = caller.GetComponent<CardsBehaviour>();
        if (card.HandBlocked) return false;
        var areaBehaviour = callee.GetComponent<AreaBehaviour>();
        areaBehaviour.ChangePower(card.PowerProperty, card.SideProperty);
        card.ReportLandingTo(callee);
        AreaStaticStorage.SetArea(areaBehaviour);
        caller.gameObject.SetActive(false);
        return true;
    }
}
