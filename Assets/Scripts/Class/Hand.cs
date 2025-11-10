using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hand
{
    public List<Card> top = new List<Card>();  // 3–‡
    public List<Card> middle = new List<Card>(); // 5–‡
    public List<Card> bottom = new List<Card>();   // 5–‡

    public void AddCard(Card card, RowType row)
    {
        switch (row)
        {
            case RowType.Top:
                if (top.Count < 3) top.Add(card);
                break;
            case RowType.Middle:
                if (middle.Count < 5) middle.Add(card);
                break;
            case RowType.Bottom:
                if (bottom.Count < 5) bottom.Add(card);
                break;
        }
    }

    public bool IsValid()
    {
        // ƒVƒ“ƒvƒ‹‚È—á: ‚·‚×‚Ä–„‚Ü‚Á‚Ä‚¢‚é‚©‚Ç‚¤‚©
        return top.Count == 3 && middle.Count == 5 && bottom.Count == 5;
    }

    public void Reset()
    {
        top.Clear();
        middle.Clear();
        bottom.Clear();
    }

    public override string ToString()
    {
        return $"Top[{top.Count}] Middle[{middle.Count}] Bottom[{bottom.Count}]";
    }
}
