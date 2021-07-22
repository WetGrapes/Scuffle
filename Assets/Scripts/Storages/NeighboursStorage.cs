using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighboursStorage 
{
   
    public enum Position
    {
        UL = 1,
        UC = 2,
        UR = 3,
        C = 4,
        BL = 5,
        BC = 6,
        BR = 7
    };

    public static Dictionary<Position, List<Position>> NeighboringAreas =
        new Dictionary<Position, List<Position>>
        {
            {
                Position.UL, new List<Position>
                {
                    Position.UR,
                }
            },
            {
                Position.UC, new List<Position>
                {
                    Position.UL, Position.UR, Position.C,
                }
            },
            {
                Position.UR, new List<Position>
                {
                    Position.UC, 
                }
            },
            {
                Position.C, new List<Position>
                {
                    Position.UC, Position.BC
                }
            },
            {
                Position.BL, new List<Position>
                {
                    Position.BC, 
                }
            },
            {
                Position.BC, new List<Position>
                {
                    Position.BL, Position.BR
                }
            },
            {
                Position.BR, new List<Position>
                {
                    Position.BC, 
                }
            }
        };
    

}
