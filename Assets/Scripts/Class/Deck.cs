using System.Collections.Generic;
using UnityEngine;

public class Deck
{

    // ============================================
    // DEBUG MENU
    // ============================================
    bool DBG_ENABLE = true;
    bool DBG_SHUFFLE = true;
    // ============================================

    private List<Card> cards = new List<Card>();
    private System.Random rng = new System.Random();

    public Deck()
    {
        Reset();
    }

    public void Reset()
    {
        cards.Clear();

        // 通常の52枚を追加
        foreach (SuitType suit in new[] { SuitType.Heart, SuitType.Diamond, SuitType.Club, SuitType.Spade })
        {
            foreach (RankType rank in System.Enum.GetValues(typeof(RankType)))
            {
                if (rank == RankType.Joker) continue; // Jokerは別で追加
                cards.Add(new Card(suit, rank));
            }
        }

        // Jokerを2枚追加
        cards.Add(new Card(SuitType.Joker, RankType.Joker));
        cards.Add(new Card(SuitType.Joker, RankType.Joker));

        Shuffle();

        if (DBG_ENABLE && DBG_SHUFFLE) debugShuffle();
    }

    public void Shuffle()
    {
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
    }

    public Card Draw()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        Card top = cards[0];
        cards.RemoveAt(0);
        return top;
    }

    public int Count => cards.Count;

    private void debugShuffle()
    {
        List<Card> cards = new List<Card>();
        cards.Add(new Card(SuitType.Spade, RankType.Five));
        cards.Add(new Card(SuitType.Spade, RankType.Ace));
        cards.Add(new Card(SuitType.Spade, RankType.King));
        cards.Add(new Card(SuitType.Spade, RankType.Nine));
        cards.Add(new Card(SuitType.Spade, RankType.Jack));

        cards.Add(new Card(SuitType.Joker, RankType.Joker));
        cards.Add(new Card(SuitType.Heart, RankType.Ten));
        cards.Add(new Card(SuitType.Club, RankType.Two));

        cards.Add(new Card(SuitType.Club, RankType.Ten));
        cards.Add(new Card(SuitType.Diamond, RankType.Ten));
        cards.Add(new Card(SuitType.Diamond, RankType.Two));

        cards.Add(new Card(SuitType.Diamond, RankType.Seven));
        cards.Add(new Card(SuitType.Club, RankType.Seven));
        cards.Add(new Card(SuitType.Heart, RankType.Two));

        injectCardsOnTop(cards);
    }

    private void injectCardsOnTop(List<Card> fixedCards)
    {
        // 既存デッキから同一カードを除外
        foreach (var fc in fixedCards)
        {
            cards.RemoveAll(c => c.suit == fc.suit && c.rank == fc.rank);
        }

        // 先頭に逆順で挿入
        for (int i = fixedCards.Count - 1; i >= 0; i--)
        {
            cards.Insert(0, fixedCards[i]);
        }
    }
}