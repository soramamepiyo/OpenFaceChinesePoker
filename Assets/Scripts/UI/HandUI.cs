using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    [Header("カードエリア")]
    [SerializeField] private Transform topArea;
    [SerializeField] private Transform middleArea;
    [SerializeField] private Transform bottomArea;
    [SerializeField] private Transform trashArea;
    [SerializeField] private Transform unplacedArea; // 配られた未配置カード置き場

    [Header("カードPrefab設定")]
    [SerializeField] private GameObject cardPrefab; // 操作用CardPrefab
    [SerializeField] private string prefabFolder = "Asset_PlayingCards/Prefabs/Deck05";
    // 例: Assets/Resources/Asset_PlayingCards/Prefabs/Deck05/Deck05_Club_A.prefab

    [Header("決定ボタン")]
    [SerializeField] private Button confirmButton;

    private List<CardUI> allCardUIs = new List<CardUI>();

    private void Start()
    {
        confirmButton.interactable = false;
    }

    /// <summary>
    /// 配られたカードを未配置エリアに生成
    /// </summary>
    public void RenderInitialHand(List<Card> dealtCards)
    {
        foreach (var card in dealtCards)
        {
            GameObject go = Instantiate(cardPrefab, unplacedArea);
            CardUI cardUI = go.GetComponent<CardUI>();
            cardUI.Initialize(card, this);
            allCardUIs.Add(cardUI);
        }

        UpdateConfirmButtonState();
    }

    /// <summary>
    /// 配置済みかどうか確認してConfirmButtonを更新
    /// </summary>
    public void UpdateConfirmButtonState()
    {
        bool allPlaced = true;
        foreach (var cardUI in allCardUIs)
        {
            if (cardUI.CurrentArea == AreaType.Unplaced || cardUI.CurrentArea == AreaType.Trash)
            {
                allPlaced = false;
                break;
            }
        }

        confirmButton.interactable = allPlaced;
    }

    /// <summary>
    /// Card に対応する見た目Prefabを返す
    /// </summary>
    public GameObject GetCardPrefab(Card card)
    {
        string prefabName = "";

        if (card.suit == SuitType.Joker)
        {
            prefabName = "Deck05_Joker";
        }
        else
        {
            string suitStr = card.suit.ToString(); // Clubs, Hearts, etc.
            string rankStr = "";

            switch (card.rank)
            {
                case RankType.Ace: rankStr = "A"; break;
                case RankType.Jack: rankStr = "J"; break;
                case RankType.Queen: rankStr = "Q"; break;
                case RankType.King: rankStr = "K"; break;
                default: rankStr = ((int)card.rank).ToString(); break;
            }

            prefabName = $"Deck05_{suitStr}_{rankStr}";
        }

        string path = $"{prefabFolder.TrimEnd('/')}/{prefabName}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogWarning($"Card Prefab not found: {path}");
        }

        return prefab;
    }

    /// <summary>
    /// すべてのカードを削除（再描画用）
    /// </summary>
    public void ClearHand()
    {
        foreach (var cardUI in allCardUIs)
        {
            if (cardUI != null) Destroy(cardUI.gameObject);
        }
        allCardUIs.Clear();
    }
}