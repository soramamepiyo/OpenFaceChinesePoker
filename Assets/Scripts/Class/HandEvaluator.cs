using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public enum HandRank
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfKind,
    Straight,
    Flush,
    FullHouse,
    FourOfKind,
    StraightFlush,
    RoyalFlush
}

public class HandEvaluator : MonoBehaviour
{
    // 役判定結果をまとめて返すための構造体
    public struct EvaluationResult
    {
        public HandRank Rank;   // 役
        public int RankValue;   // 強さ（比較用）
        public List<int> Kickers; // 同じ役同士での比較用
    }

    // ===========================
    // ▼ 評価メイン関数
    // ===========================
    public EvaluationResult Evaluate(List<Card> cards)
    {
        if (cards.Count == 3)
            return Evaluate3Card(cards);
        if (cards.Count == 5)
            return Evaluate5Card(cards);

        throw new ArgumentException("Hand must be 3 or 5 cards.");
    }

    // ===========================
    // ▼ 3枚の役判定（Top）
    // ===========================
    private EvaluationResult Evaluate3Card(List<Card> cards)
    {
        var values = cards.Select(c => (int)c.rank).OrderByDescending(v => v).ToList();
        
        int threeKind = FindNKindValue(cards, 3);
        List<int> pairs = FindPairs(cards);

        // Three of a Kind
        if (threeKind > 0)
        {
            List<int> kickers = new List<int>(values);
            kickers.RemoveAll(v => v == threeKind);
            return new EvaluationResult { Rank = HandRank.ThreeOfKind, RankValue = 300 + threeKind, Kickers = kickers };
        }

        // One Pair
        if (pairs.Count == 1)
        {
            int pairV = pairs[0];
            List<int> kickers = values.Where(v => v != pairV).ToList();

            return new EvaluationResult
            {
                Rank = HandRank.OnePair,
                RankValue = 100 + pairV,
                Kickers = kickers
            };
        }

        // High Card
        return new EvaluationResult
        {
            Rank = HandRank.HighCard,
            RankValue = 0,
            Kickers = values
        };
    }

    // ===========================
    // ▼ 5枚の役判定（Middle / Bottom）
    // ===========================
    private EvaluationResult Evaluate5Card(List<Card> cards)
    {
        bool isFlush = IsFlush(cards);
        bool isStraight = IsStraight(cards);
        int fourKind = FindNKindValue(cards, 4);
        int threeKind = FindNKindValue(cards, 3);
        List<int> pairs = FindPairs(cards);

        var values = cards.Select(c => (int)c.rank).OrderByDescending(v => v).ToList();

        // Royal Flush
        if (isFlush && IsRoyal(cards))
            return new EvaluationResult { Rank = HandRank.RoyalFlush, RankValue = 900, Kickers = values };

        // Straight Flush
        if (isFlush && isStraight)
            return new EvaluationResult { Rank = HandRank.StraightFlush, RankValue = 800, Kickers = values };

        // Four of a Kind
        if (fourKind > 0)
        {
            List<int> kickers = new List<int>(values);
            kickers.RemoveAll(v => v == fourKind);
            return new EvaluationResult { Rank = HandRank.FourOfKind, RankValue = 700 + fourKind, Kickers = kickers };
        }

        // Full House
        if (threeKind > 0 && pairs.Count >= 1)
        {
            return new EvaluationResult
            {
                Rank = HandRank.FullHouse,
                RankValue = 600 + threeKind,
                Kickers = new List<int> { pairs.Max() }
            };
        }

        // Flush
        if (isFlush)
            return new EvaluationResult { Rank = HandRank.Flush, RankValue = 500, Kickers = values };

        // Straight
        if (isStraight)
            return new EvaluationResult { Rank = HandRank.Straight, RankValue = 400, Kickers = values };

        // Three of a Kind
        if (threeKind > 0)
        {
            List<int> kickers = new List<int>(values);
            kickers.RemoveAll(v => v == threeKind);
            return new EvaluationResult { Rank = HandRank.ThreeOfKind, RankValue = 300 + threeKind, Kickers = kickers };
        }

        // Two Pair
        if (pairs.Count == 2)
        {
            int highPair = pairs.Max();
            int lowPair = pairs.Min();
            int kicker = values.Where(v => v != highPair && v != lowPair).Max();

            return new EvaluationResult
            {
                Rank = HandRank.TwoPair,
                RankValue = 200 + highPair,
                Kickers = new List<int> { lowPair, kicker }
            };
        }

        // One Pair
        if (pairs.Count == 1)
        {
            int pairV = pairs[0];
            List<int> kickers = values.Where(v => v != pairV).ToList();

            return new EvaluationResult
            {
                Rank = HandRank.OnePair,
                RankValue = 100 + pairV,
                Kickers = kickers
            };
        }

        // High Card
        return new EvaluationResult
        {
            Rank = HandRank.HighCard,
            RankValue = 0,
            Kickers = values
        };
    }

    // ===========================
    // ▼ 基本的な補助関数
    // ===========================
    private bool IsFlush(List<Card> cards)
        => cards.Select(c => c.suit).Distinct().Count() == 1;

    private bool IsStraight(List<Card> cards)
    {
        var sorted = cards.Select(c => (int)c.rank).Distinct().OrderBy(v => v).ToList();
        if (sorted.Count != 5) return false;

        // A2345
        if (sorted.SequenceEqual(new List<int> { 2, 3, 4, 5, 14 })) return true;

        return sorted.Last() - sorted.First() == 4;
    }

    private bool IsRoyal(List<Card> cards)
    {
        var set = cards.Select(c => (int)c.rank).OrderBy(v => v).ToList();
        return set.SequenceEqual(new List<int> { 10, 11, 12, 13, 14 });
    }

    private bool IsNOfAKind(List<Card> cards, int n)
        => cards.GroupBy(c => c.rank).Any(g => g.Count() == n);

    private int FindNKindValue(List<Card> cards, int n)
        => cards.GroupBy(c => c.rank)
               .Where(g => g.Count() == n)
               .Select(g => (int)g.Key)
               .DefaultIfEmpty(0)
               .Max();

    private int FindPairValue(List<Card> cards)
        => FindNKindValue(cards, 2);

    private List<int> FindPairs(List<Card> cards)
        => cards.GroupBy(c => c.rank)
                .Where(g => g.Count() == 2)
                .Select(g => (int)g.Key)
                .ToList();
}
