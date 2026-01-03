using UnityEngine;

[System.Serializable]
public class Player
{
    public string playerName;
    public int totalScore = 0;
    public bool isAI = false;

    public Player(string name, bool isAI = false)
    {
        this.playerName = name;
        this.isAI = isAI;
    }

    public override string ToString()
    {
        return $"{playerName} - Score: {totalScore}";
    }
}
