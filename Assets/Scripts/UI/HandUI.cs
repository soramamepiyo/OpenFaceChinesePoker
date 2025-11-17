using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    [Header("カードエリア")]
    public Transform unplacedArea;
    public Transform topArea;
    public Transform middleArea;
    public Transform bottomArea;
    public Transform trashArea;

    [Header("Prefab")]
    public GameObject cardPrefab;

    [Header("決定ボタン")]
    public Button confirmButton;

    private List<CardUI> allCardUIs = new List<CardUI>();
    private float spacing = 1.2f; // 横並び間隔

    public void RenderInitialHand(List<Card> dealtCards)
    {
        ClearHand();

        foreach (var card in dealtCards)
        {
            GameObject go = Instantiate(cardPrefab, unplacedArea);
            CardUI c = go.GetComponent<CardUI>();
            c.Initialize(card, this);
            allCardUIs.Add(c);
        }

        ArrangeCards(unplacedArea);
        UpdateConfirmButtonState();
    }

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

    public Sprite GetCardSprite(Card card)
    {
        int id = GetDeck05ID(card);
        string path = $"Deck/Deck05/Deck05_{id}";
        Sprite s = Resources.Load<Sprite>(path);
        if (s == null)
            Debug.LogWarning($"Sprite not found: {path}");
        return s;
    }

    private int GetDeck05ID(Card card)
    {
        if (card.IsJoker()) return 56;

        int index = (card.rank == RankType.Ace) ? 0 : ((int)card.rank - 1);

        switch (card.suit)
        {
            case SuitType.Heart: return 0 + index;
            case SuitType.Club: return 14 + index;
            case SuitType.Diamond: return 28 + index;
            case SuitType.Spade: return 42 + index;
            default: return -1;
        }
    }

    private void ArrangeCards(Transform parent)
    {
        List<Transform> cards = new List<Transform>();

        // Tag == "Card" の子だけを抽出
        foreach (Transform child in parent)
        {
            if (child.CompareTag("Card"))
            {
                cards.Add(child);
            }
        }

        int n = cards.Count;
        float startX = -spacing * (n - 1) / 2f;

        for (int i = 0; i < n; i++)
        {
            cards[i].localPosition = new Vector3(startX + spacing * i, 0, 0);
        }
    }

    public void ClearHand()
    {
        foreach (var c in allCardUIs)
        {
            if (c != null) Destroy(c.gameObject);
        }
        allCardUIs.Clear();
    }
}
