using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HandEvaluator;

public class ResultPanelUI : MonoBehaviour
{
    [Header("ÉJÅ[Éh")]
    [SerializeField] private List<Image> topCards;
    [SerializeField] private List<Image> middleCards;
    [SerializeField] private List<Image> bottomCards;
    [Header("ññº")]
    [SerializeField] private TextMeshProUGUI topRank;
    [SerializeField] private TextMeshProUGUI middleRank;
    [SerializeField] private TextMeshProUGUI bottomRank;
    [SerializeField] private TextMeshProUGUI topPoint;
    [SerializeField] private TextMeshProUGUI middlePoint;
    [SerializeField] private TextMeshProUGUI bottomPoint;

    public void SetCards(List<Card> top, List<Card> middle, List<Card> bottom)
    {
        for (int i = 0; i < top.Count; i++)
        {
            topCards[i].sprite = GetDeck05Sprite(top[i]);
        }
        for (int i = 0; i < middle.Count; i++)
        {
            middleCards[i].sprite = GetDeck05Sprite(middle[i]);
        }
        for (int i = 0; i < bottom.Count; i++)
        {
            bottomCards[i].sprite = GetDeck05Sprite(bottom[i]);
        }
    }

    public void SetText(
        EvaluationResult er_top, EvaluationResult er_mid, EvaluationResult er_bot,
        int pt_top, int pt_mid, int pt_bot)
    {
        topRank.text    = er_top.Rank.ToString();
        middleRank.text = er_mid.Rank.ToString();
        bottomRank.text = er_bot.Rank.ToString();
        topPoint.text   = pt_top.ToString();
        topPoint.text   = pt_top.ToString();
        topPoint.text   = pt_top.ToString() + "pt";
        middlePoint.text= pt_mid.ToString() + "pt";
        bottomPoint.text= pt_bot.ToString() + "pt";
    }

    private Sprite GetDeck05Sprite(Card card)
    {
        // Card -> id
        int sprite_id = -1;
        if (card.IsJoker()) sprite_id = 56;
        int index = (card.rank == RankType.Ace) ? 0 : ((int)card.rank - 1);
        switch (card.suit)
        {
            case SuitType.Heart: 
                sprite_id = 0 + index;
                break;
            case SuitType.Club: 
                sprite_id = 14 + index;
                break;
            case SuitType.Diamond: 
                sprite_id = 28 + index;
                break;
            case SuitType.Spade: 
                sprite_id = 42 + index;
                break;
            default:
                break;
        }

        // id -> Sprite
        string path = $"Deck/Deck05/Deck05_{sprite_id}";
        Sprite s = Resources.Load<Sprite>(path);
        if (s == null)
            Debug.LogWarning($"Sprite not found: {path}");
        return s;
    }

}
