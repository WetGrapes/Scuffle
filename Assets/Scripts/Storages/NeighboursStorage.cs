using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighboursStorage 
{
   
    public enum PositionInSpace
    {
        Centre = 1,
        CentreLeft = 2,
        CentreRight = 3,
        BottomLeft = 4,
        BottomRight = 5,
        TopLeft = 6,
        TopRight = 7
    };

    public static Dictionary<PositionInSpace, List<PositionInSpace>> NeighboringAreas =
        new Dictionary<PositionInSpace, List<PositionInSpace>>
        {
            {
                PositionInSpace.Centre, new List<PositionInSpace>
                {
                    PositionInSpace.CentreLeft, PositionInSpace.CentreRight,
                }
            },
            {
                PositionInSpace.CentreLeft, new List<PositionInSpace>
                {
                    PositionInSpace.Centre, PositionInSpace.BottomLeft,
                    PositionInSpace.CentreRight, PositionInSpace.TopLeft
                }
            },
            {
                PositionInSpace.CentreRight, new List<PositionInSpace>
                {
                    PositionInSpace.Centre, PositionInSpace.BottomRight,
                    PositionInSpace.CentreLeft, PositionInSpace.TopRight
                }
            },
            {
                PositionInSpace.TopLeft, new List<PositionInSpace>
                {
                    PositionInSpace.BottomLeft, PositionInSpace.CentreLeft
                }
            },
            {
                PositionInSpace.TopRight, new List<PositionInSpace>
                {
                    PositionInSpace.BottomRight, PositionInSpace.CentreRight
                }
            },
            {
                PositionInSpace.BottomRight, new List<PositionInSpace>
                {
                    PositionInSpace.TopRight, PositionInSpace.CentreRight
                }
            },
            {
                PositionInSpace.BottomLeft, new List<PositionInSpace>
                {
                    PositionInSpace.TopLeft, PositionInSpace.CentreLeft
                }
            }
        };
    

}
