using System.Collections.Generic;
using UnityEngine;

public enum RowType { Front, Middle, Back }

[System.Serializable]
public class Hand
{
    public List<Card> front = new List<Card>();  // 3–‡
    public List<Card> middle = new List<Card>(); // 5–‡
    public List<Card> back = new List<Card>();   // 5–‡

    public void AddCard(Card card, RowType row)
    {
        switch (row)
        {
            case RowType.Front:
                if (front.Count < 3) front.Add(card);
                break;
            case RowType.Middle:
                if (middle.Count < 5) middle.Add(card);
                break;
            case RowType.Back:
                if (back.Count < 5) back.Add(card);
                break;
        }
    }

    public bool IsValid()
    {
        // ƒVƒ“ƒvƒ‹‚È—á: ‚·‚×‚Ä–„‚Ü‚Á‚Ä‚¢‚é‚©‚Ç‚¤‚©
        return front.Count == 3 && middle.Count == 5 && back.Count == 5;
    }

    public void Reset()
    {
        front.Clear();
        middle.Clear();
        back.Clear();
    }

    public override string ToString()
    {
        return $"Front[{front.Count}] Middle[{middle.Count}] Back[{back.Count}]";
    }
}
