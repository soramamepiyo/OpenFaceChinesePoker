using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HandEvaluator;

public class ResultPanelUI : MonoBehaviour
{
    [Header("カード")]
    [SerializeField] private List<Image> p_topCards;
    [SerializeField] private List<Image> p_middleCards;
    [SerializeField] private List<Image> p_bottomCards;
    [SerializeField] private List<Image> e_topCards;
    [SerializeField] private List<Image> e_middleCards;
    [SerializeField] private List<Image> e_bottomCards;
    [Header("役名")]
    [SerializeField] private TextMeshProUGUI p_topRank;
    [SerializeField] private TextMeshProUGUI p_middleRank;
    [SerializeField] private TextMeshProUGUI p_bottomRank;
    [SerializeField] private TextMeshProUGUI p_topPoint;
    [SerializeField] private TextMeshProUGUI p_middlePoint;
    [SerializeField] private TextMeshProUGUI p_bottomPoint;
    [SerializeField] private TextMeshProUGUI e_topRank;
    [SerializeField] private TextMeshProUGUI e_middleRank;
    [SerializeField] private TextMeshProUGUI e_bottomRank;
    [SerializeField] private TextMeshProUGUI e_topPoint;
    [SerializeField] private TextMeshProUGUI e_middlePoint;
    [SerializeField] private TextMeshProUGUI e_bottomPoint;
    [Header("バースト")]
    [SerializeField] private GameObject p_burstText;
    [SerializeField] private GameObject e_burstText;

    public void SetCards(
        List<Card> player_top, List<Card> player_middle, List<Card> player_bottom, 
        List<Card> enemy_top, List<Card> enemy_middle, List<Card> enemy_bottom)
    {
        for (int i = 0; i < player_top.Count; i++)
        {
            p_topCards[i].sprite = GetDeck05Sprite(player_top[i]);
        }
        for (int i = 0; i < player_middle.Count; i++)
        {
            p_middleCards[i].sprite = GetDeck05Sprite(player_middle[i]);
        }
        for (int i = 0; i < player_bottom.Count; i++)
        {
            p_bottomCards[i].sprite = GetDeck05Sprite(player_bottom[i]);
        }

        for (int i = 0; i < enemy_top.Count; i++)
        {
            e_topCards[i].sprite = GetDeck05Sprite(enemy_top[i]);
        }
        for (int i = 0; i < enemy_middle.Count; i++)
        {
            e_middleCards[i].sprite = GetDeck05Sprite(enemy_middle[i]);
        }
        for (int i = 0; i < enemy_bottom.Count; i++)
        {
            e_bottomCards[i].sprite = GetDeck05Sprite(enemy_bottom[i]);
        }
    }

    public void SetText(
        EvaluationResult p_er_top, EvaluationResult p_er_mid, EvaluationResult p_er_bot,
        int p_pt_top, int p_pt_mid, int p_pt_bot,
        EvaluationResult e_er_top, EvaluationResult e_er_mid, EvaluationResult e_er_bot,
        int e_pt_top, int e_pt_mid, int e_pt_bot)
    {
        p_topRank.text    = p_er_top.Rank.ToString();
        p_middleRank.text = p_er_mid.Rank.ToString();
        p_bottomRank.text = p_er_bot.Rank.ToString();
        p_topPoint.text   = p_pt_top.ToString() + "pt";
        p_middlePoint.text= p_pt_mid.ToString() + "pt";
        p_bottomPoint.text= p_pt_bot.ToString() + "pt";
        e_topRank.text    = e_er_top.Rank.ToString();
        e_middleRank.text = e_er_mid.Rank.ToString();
        e_bottomRank.text = e_er_bot.Rank.ToString();
        e_topPoint.text   = e_pt_top.ToString() + "pt";
        e_middlePoint.text= e_pt_mid.ToString() + "pt";
        e_bottomPoint.text= e_pt_bot.ToString() + "pt";
    }

    public void SetBurstText(bool p_is_burst, bool e_is_burst)
    {
        p_burstText.SetActive(p_is_burst);
        e_burstText.SetActive(e_is_burst);
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
