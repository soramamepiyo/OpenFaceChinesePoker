using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [Header("カードを並べる位置")]
    [SerializeField] private Transform frontArea;
    [SerializeField] private Transform middleArea;
    [SerializeField] private Transform backArea;

    [Header("カード配置設定")]
    [SerializeField] private float spacing = 0.5f;

    [Header("カードPrefabが入っているフォルダ名")]
    [SerializeField] private string prefabFolder = "Asset_PlayingCards/Prefabs/Deck05";
    // 例: Assets/Resources/Asset_PlayingCards/Prefabs/Deck05/Deck05_Club_A.prefab

    private List<GameObject> activeCards = new List<GameObject>();

    public void RenderHand(Hand hand)
    {
        ClearHand();
        RenderRow(hand.front, frontArea);
        RenderRow(hand.middle, middleArea);
        RenderRow(hand.back, backArea);
    }

    private void RenderRow(List<Card> cards, Transform parent)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            string prefabName = GetPrefabName(card);

            GameObject prefab = Resources.Load<GameObject>($"{prefabFolder}/{prefabName}");
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab not found: {prefabName}");
                continue;
            }

            Vector3 pos = parent.position + new Vector3(i * spacing, 0, 0);
            GameObject go = Instantiate(prefab, pos, Quaternion.identity, parent);
            activeCards.Add(go);
        }
    }

    private string GetPrefabName(Card card)
    {
        string suitStr = "";
        string rankStr = "";

        switch (card.suit)
        {
            case SuitType.Clubs: suitStr = "Club"; break;
            case SuitType.Diamonds: suitStr = "Diamond"; break;
            case SuitType.Hearts: suitStr = "Heart"; break;
            case SuitType.Spades: suitStr = "Spade"; break;
            case SuitType.Joker: return "Deck05_Joker"; // Joker特例
        }

        // Rank変換（1→A, 11→J, 12→Q, 13→K）
        switch (card.rank)
        {
            case RankType.Ace: rankStr = "A"; break;
            case RankType.Jack: rankStr = "J"; break;
            case RankType.Queen: rankStr = "Q"; break;
            case RankType.King: rankStr = "K"; break;
            default: rankStr = ((int)card.rank).ToString(); break;
        }

        return $"Deck05_{suitStr}_{rankStr}";
    }

    public void ClearHand()
    {
        foreach (var go in activeCards)
        {
            if (go != null) Destroy(go);
        }
        activeCards.Clear();
    }
}
