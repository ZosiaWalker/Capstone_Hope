using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SP
{
    public enum ScoreType
    {
        Points,
        Coins,
        Custom
    }
    public enum ScoreScope
    {
        Total,
        Current,
        Best
    }
    public enum AddScoreScope
    {
        Total,
        Current,
        CurrentAndTotal
    }

    [IncludeInSettings(true)]
    public class SPEnums
    {
    }
}
