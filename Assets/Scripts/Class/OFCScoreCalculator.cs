using System;
using System.Collections.Generic;
using UnityEngine;
using static HandEvaluator;

public class OFCScoreCalculator : MonoBehaviour
{
    // 上段スコア表
    private static readonly Dictionary<int, int> TopPoints = new()
    {
        // OnePair
        { 106, 1},
        { 107, 2},
        { 108, 3},
        { 109, 4},
        { 110, 5},
        { 111, 6},
        { 112, 7},
        { 113, 8},
        { 114, 9},
        // Trips
        { 302, 10 },
        { 303, 11 },
        { 304, 12 },
        { 305, 13 },
        { 306, 14 },
        { 307, 15 },
        { 308, 16 },
        { 309, 17 },
        { 310, 18 },
        { 311, 19 },
        { 312, 20 },
        { 313, 21 },
        { 314, 22 },
    };

    // 中段スコア表
    private static readonly Dictionary<HandRank, int> MiddlePoints = new()
    {
        { HandRank.ThreeOfKind,      2 },
        { HandRank.Straight,         4 },
        { HandRank.Flush,            8 },
        { HandRank.FullHouse,       12 },
        { HandRank.FourOfKind,      20 },
        { HandRank.StraightFlush,   30 },
        { HandRank.RoyalFlush,      50 },
    };

    // 下段スコア表
    private static readonly Dictionary<HandRank, int> BottomPoints = new()
    {
        { HandRank.Straight,         2 },
        { HandRank.Flush,            4 },
        { HandRank.FullHouse,        6 },
        { HandRank.FourOfKind,      10 },
        { HandRank.StraightFlush,   15 },
        { HandRank.RoyalFlush,      25 },
    };

    public int GetAreaScore(EvaluationResult result, AreaType area)
    {
        if (area == AreaType.Top)           return getTopAreaScore(result);
        else if (area == AreaType.Middle)   return getMiddleAreaScore(result);
        else if (area == AreaType.Bottom)   return getBottomAreaScore(result);

        return 0;
    }

    private int getTopAreaScore(EvaluationResult result)
    {
        if (TopPoints.TryGetValue(result.RankValue, out int p)) return p;
        return 0;
    }

    private int getMiddleAreaScore(EvaluationResult result)
    {
        if (MiddlePoints.TryGetValue(result.Rank, out int p)) return p;
        return 0;
    }

    private int getBottomAreaScore(EvaluationResult result)
    {
        if (BottomPoints.TryGetValue(result.Rank, out int p)) return p;
        return 0;
    }

}
