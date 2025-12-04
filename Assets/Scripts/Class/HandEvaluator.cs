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
    // 役判定結果をまとめて返す構造体（元のまま）
    public struct EvaluationResult
    {
        public HandRank Rank;
        public int RankValue;
        public List<int> Kickers;
    }

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
        var values = cards
            .Where(c => c.suit != SuitType.Joker)
            .Select(c => (int)c.rank)
            .OrderByDescending(v => v)
            .ToList();

        int jokerCount = CountJokers(cards);

        // Joker だけのケース
        if (values.Count == 0)
        {
            return new EvaluationResult
            {
                Rank = HandRank.ThreeOfKind,
                RankValue = 300 + 14,
                Kickers = new List<int>()
            };
        }

        int threeKind = FindNKindValue(cards, 3);
        List<int> pairs = FindPairs(cards);

        // Three of a Kind
        if (threeKind > 0)
        {
            var kickers = new List<int>(values);
            kickers.RemoveAll(v => v == threeKind);

            return new EvaluationResult
            {
                Rank = HandRank.ThreeOfKind,
                RankValue = 300 + threeKind,
                Kickers = kickers
            };
        }

        // One Pair
        if (pairs.Count == 1)
        {
            int pairV = pairs[0];

            List<int> kickers = values
                .Where(v => v != pairV)
                .ToList();

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

        var values = GetNonJokerRanks(cards)
            .OrderByDescending(v => v)
            .ToList();

        int jokerCount = CountJokers(cards);

        // Joker だけの特殊ケース（5 Jokers）
        if (values.Count == 0)
        {
            return new EvaluationResult
            {
                Rank = HandRank.RoyalFlush,
                RankValue = 900,
                Kickers = new List<int> { 14, 13, 12, 11, 10 }
            };
        }

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

            return new EvaluationResult
            {
                Rank = HandRank.FourOfKind,
                RankValue = 700 + fourKind,
                Kickers = kickers
            };
        }

        // Full House
        var fh = FindFullHouse(cards);
        if (fh.threeRank > 0 && fh.pairRank > 0)
        {
            return new EvaluationResult
            {
                Rank = HandRank.FullHouse,
                RankValue = 600 + fh.threeRank,
                Kickers = new List<int> { fh.pairRank }
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

            return new EvaluationResult
            {
                Rank = HandRank.ThreeOfKind,
                RankValue = 300 + threeKind,
                Kickers = kickers
            };
        }

        // Two Pair
        if (pairs.Count == 2)
        {
            int highPair = pairs.Max();
            int lowPair = pairs.Min();

            int kicker = values
                .Where(v => v != highPair && v != lowPair)
                .DefaultIfEmpty(0)
                .Max();

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
    // ▼ Joker util
    // ===========================
    private int CountJokers(List<Card> cards)
        => cards.Count(c => c.suit == SuitType.Joker);

    private List<int> GetNonJokerRanks(List<Card> cards)
        => cards.Where(c => c.suit != SuitType.Joker)
                .Select(c => (int)c.rank)
                .ToList();

    // ===========================
    // ▼ フラッシュ（Joker 対応）
    // ===========================
    private bool IsFlush(List<Card> cards)
    {
        var suits = cards.Where(c => c.suit != SuitType.Joker)
                         .Select(c => c.suit)
                         .Distinct()
                         .ToList();

        if (suits.Count == 0) return true;
        if (suits.Count == 1) return true;

        return false;
    }

    // ===========================
    // ▼ ストレート（Joker 対応）
    // ===========================
    private bool IsStraight(List<Card> cards)
    {
        int joker = CountJokers(cards);

        var r = GetNonJokerRanks(cards)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        // A2345
        bool CheckLowAce()
        {
            var need = new List<int> { 2, 3, 4, 5, 14 };
            int miss = need.Count(v => !r.Contains(v));
            return miss <= joker;
        }
        if (CheckLowAce()) return true;

        for (int start = 1; start <= 10; start++)
        {
            var need = Enumerable.Range(start, 5).ToList();
            int miss = need.Count(v => !r.Contains(v));
            if (miss <= joker) return true;
        }
        return false;
    }

    // ===========================
    // ▼ フルハウス（Joker 対応）
    // ===========================
    // Joker を含めて FullHouse (3 + 2) が作れるか探索し、作れるなら (threeRank, pairRank) を返す。
    // 見つからなければ (0,0) を返す。
    private (int threeRank, int pairRank) FindFullHouse(List<Card> cards)
    {
        int jokerCount = CountJokers(cards);

        // グループ情報（非Jokerのみ）
        var groups = cards
            .Where(c => c.suit != SuitType.Joker)
            .GroupBy(c => c.rank)
            .Select(g => new { Rank = (int)g.Key, Count = g.Count() })
            .ToList();

        // すべての候補ランクを上位から試す（強い方を優先）
        // 2..14 を対象（Ace=14）
        for (int three = 14; three >= 2; three--)
        {
            int countThree = groups.FirstOrDefault(g => g.Rank == three)?.Count ?? 0;
            int needForThree = Math.Max(0, 3 - countThree);
            if (needForThree > jokerCount) continue;

            int remJoker = jokerCount - needForThree;

            // ペアの候補は three と同じでないランクを試す（強い方優先）
            for (int pair = 14; pair >= 2; pair--)
            {
                if (pair == three) continue;

                int countPair = groups.FirstOrDefault(g => g.Rank == pair)?.Count ?? 0;
                int needForPair = Math.Max(0, 2 - countPair);

                if (needForPair <= remJoker)
                {
                    // 成立
                    return (three, pair);
                }
            }

            // 特殊ケース：ペアを Joker だけで作る（ペアランクを最強の Ace(14) とする）
            if (remJoker >= 2)
            {
                return (three, 14);
            }
        }

        // 可能性として「3枚を Joker だけで作る」ケースも試す（groups.Count==0 handled below）
        // 例えば groups empty and jokerCount >=3 -> threeRank = 14, need pair from remaining jokers
        if (groups.Count == 0 && jokerCount >= 5)
        {
            // 5 Jokers -> treat as Royal-ish but FullHouse not meaningful; keep safe fallback
            return (14, 14);
        }

        // 最後に「非既存ランクで3枚・2枚を作る」ケース（ほぼ covered above by testing ranks 14..2）
        return (0, 0);
    }


    // ===========================
    // ▼ ロイヤル（Joker 対応）
    // ===========================
    private bool IsRoyal(List<Card> cards)
    {
        int joker = CountJokers(cards);
        var r = GetNonJokerRanks(cards);

        var need = new List<int> { 10, 11, 12, 13, 14 };
        int miss = need.Count(v => !r.Contains(v));

        return miss <= joker;
    }

    // ===========================
    // ▼ NKind（Joker 対応）
    // ===========================
    private int FindNKindValue(List<Card> cards, int n)
    {
        int jokerCount = CountJokers(cards);

        var groups = cards.Where(c => c.suit != SuitType.Joker)
            .GroupBy(c => c.rank)
            .Select(g => new { Rank = (int)g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ThenByDescending(g => g.Rank)
            .ToList();

        foreach (var g in groups)
        {
            if (g.Count + jokerCount >= n)
                return g.Rank;
        }

        if (groups.Count == 0 && jokerCount >= n)
            return 14;

        return 0;
    }

    private List<int> FindPairs(List<Card> cards)
    {
        int jokerCount = CountJokers(cards);

        var groups = cards.Where(c => c.suit != SuitType.Joker)
            .GroupBy(c => c.rank)
            .Select(g => new { Rank = (int)g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Rank)
            .ToList();

        List<int> pairs = new List<int>();

        foreach (var g in groups)
        {
            if (g.Count >= 2)
                pairs.Add(g.Rank);
            else if (g.Count == 1 && jokerCount > 0)
            {
                pairs.Add(g.Rank);
                jokerCount--;
            }
        }

        if (jokerCount >= 2)
            pairs.Add(14);

        return pairs;
    }
}
