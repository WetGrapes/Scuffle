using UnityEngine;

public class AreaHandler : BaseHandler
{
    public override void Interaction(Transform caller, Transform callee)
    {
        var areaBehaviour = callee.GetComponent<AreaBehaviour>();
        var card = caller.GetComponent<CardsBehaviour>();
        areaBehaviour.ChangePower(card.PowerProperty, card.SideProperty);
        card.ReportLandingTo(callee);
        AreaStaticStorage.SetArea(areaBehaviour);
        caller.gameObject.SetActive(false);
    }
}
