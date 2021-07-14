using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGradient : BaseMeshEffect
{
    public Color mColor1 = Color.white;
    public Color mColor2 = Color.white;
    [Range(0, 1)] public float Ratio = 1;
    [Range(-180f, 180f)]
    public float Angle = -145f;
    public bool mIgnoreRatio = true;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!enabled) return;
        var rect = graphic.rectTransform.rect;
        var dir = UIGradientUtils.RotationDir(Angle);

        if (!mIgnoreRatio)
            dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

        var localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

        var vertex = default(UIVertex);
        for (var i = 0; i < vh.currentVertCount; i++) {
            vh.PopulateUIVertex (ref vertex, i);
            var localPosition = localPositionMatrix * vertex.position;
            vertex.color *= Color.Lerp(mColor2, mColor1, localPosition.y*Ratio);
            vh.SetUIVertex (vertex, i);
        }
    }
}
