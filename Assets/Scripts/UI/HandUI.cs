// HandUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HandUI : MonoBehaviour
{
    public bool IsPlayerHandUI;

    [Header("カードエリア")]
    public Transform unplacedArea;
    public Transform topArea;
    public Transform middleArea;
    public Transform bottomArea;

    [Header("Prefab")]
    public GameObject cardPrefab;

    [Header("決定ボタン")]
    public Button confirmButton;

    [SerializeField] private HandManager handManager;
    private List<CardUI> allCardUIs = new List<CardUI>();
    private float spacing = 1.5f;

    private float MOVE_ANIM_SEC = 0.3f;


    public void RenderHand(List<Card> dealtCards)
    {
        foreach (var card in dealtCards)
        {
            GameObject go = Instantiate(cardPrefab, unplacedArea);
            go.tag = "Card"; // 整列時に判定
            CardUI c = go.GetComponent<CardUI>();
            c.Initialize(card, this);
            allCardUIs.Add(c);
        }

        ArrangeAlignCenter(unplacedArea);
        UpdateConfirmButtonState();
    }

    public void UpdateConfirmButtonState()
    {
        // 適切に配置されていたらOKボタンが押せる
        confirmButton.interactable 
            = CheckIsProperPlace(handManager.GetCurrentPhase());
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

    public void ArrangeAlignUnplacedArea()
    {
        ArrangeAlignCenter(unplacedArea);
    }

    public void ArrangeAlignPlacedArea()
    {
        ArrangeAlignLeft(topArea, 3);
        ArrangeAlignLeft(middleArea, 5);
        ArrangeAlignLeft(bottomArea, 5);
    }

    // 中央揃え
    private void ArrangeAlignCenter(Transform area)
    {
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in area)
        {
            if (child.CompareTag("Card"))
                cards.Add(child);
        }

        int n = cards.Count;
        float startX = -spacing * (n - 1) / 2f;
        for (int i = 0; i < n; i++)
        {
            cards[i].DOLocalMove(new Vector3(startX + spacing * i, 0, 0), MOVE_ANIM_SEC);
        }
    }

    // 左揃え
    private void ArrangeAlignLeft(Transform area, int max_cards)
    {
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in area)
        {
            if (child.CompareTag("Card"))
                cards.Add(child);
        }

        int n = cards.Count;
        float startX = -spacing * (max_cards - 1) / 2f;
        for (int i = 0; i < n; i++)
        {
            cards[i].DOLocalMove(new Vector3(startX + spacing * i, 0, 0), MOVE_ANIM_SEC);
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

    /// <summary>
    /// 確定できる配置かどうか
    /// </summary>
    public bool CheckIsProperPlace(PlacePhase phase)
    {
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in unplacedArea)
        {
            if (child.CompareTag("Card"))
                cards.Add(child);
        }
        int num = cards.Count;
        return num == ((phase == PlacePhase.First) ? 0: 1);
    }
}
