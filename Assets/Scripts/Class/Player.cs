using UnityEngine;

[System.Serializable]
public class Player
{
    public string playerName;
    public Hand hand = new Hand();
    public int totalScore = 0;
    public bool isAI = false;

    public Player(string name, bool isAI = false)
    {
        this.playerName = name;
        this.isAI = isAI;
    }

    public void PlaceCard(Card card, RowType row)
    {
        hand.AddCard(card, row);
        Debug.Log($"{playerName} placed {card} to {row}");
    }

    public void ResetHand()
    {
        hand.Reset();
    }

    public override string ToString()
    {
        return $"{playerName} - Score: {totalScore}";
    }
}
