using UnityEngine;

public enum SuitType { Hearts, Diamonds, Clubs, Spades, Joker }
public enum RankType
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace,
    Joker = 99 // “ÁŽêˆµ‚¢
}

[System.Serializable]
public class Card
{
    public SuitType suit;
    public RankType rank;

    public Card(SuitType suit, RankType rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public int GetValue()
    {
        // Joker‚Í“Á•Êˆµ‚¢
        return rank == RankType.Joker ? 0 : (int)rank;
    }

    public bool IsJoker()
    {
        return rank == RankType.Joker || suit == SuitType.Joker;
    }

    public override string ToString()
    {
        return IsJoker() ? "Joker" : $"{rank} of {suit}";
    }
}
