using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cards = new List<Card>();
    private System.Random rng = new System.Random();

    public Deck()
    {
        Reset();
    }

    public void Reset()
    {
        cards.Clear();

        // ’Êí‚Ì52–‡‚ð’Ç‰Á
        foreach (SuitType suit in new[] { SuitType.Hearts, SuitType.Diamonds, SuitType.Clubs, SuitType.Spades })
        {
            foreach (RankType rank in System.Enum.GetValues(typeof(RankType)))
            {
                if (rank == RankType.Joker) continue; // Joker‚Í•Ê‚Å’Ç‰Á
                cards.Add(new Card(suit, rank));
            }
        }

        // Joker‚ð2–‡’Ç‰Á
        cards.Add(new Card(SuitType.Joker, RankType.Joker));
        cards.Add(new Card(SuitType.Joker, RankType.Joker));

        Shuffle();
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
}