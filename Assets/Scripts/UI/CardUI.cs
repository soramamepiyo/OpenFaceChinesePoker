using UnityEngine;
using UnityEngine.EventSystems;

public enum AreaType
{
    Unplaced,
    Top,
    Middle,
    Bottom,
    Trash
}

public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("カードデータ")]
    public Card cardData;

    [HideInInspector] public AreaType CurrentArea = AreaType.Unplaced;

    private HandUI handUI;

    // ドラッグ用
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(Card card, HandUI handUI)
    {
        this.cardData = card;
        this.handUI = handUI;

        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalPosition = transform.position;

        // 見た目を差し替え
        SetCardAppearance(card);
    }

    /// <summary>
    /// カードの見た目を差し替える
    /// </summary>
    private void SetCardAppearance(Card card)
    {
        // 既存の子オブジェクトを削除
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // カードに対応するPrefabを取得
        GameObject prefab = handUI.GetCardPrefab(card);
        if (prefab != null)
        {
            GameObject go = Instantiate(prefab, transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("カードPrefabが見つかりません: " + card.rank + " " + card.suit);
        }
    }

    #region ドラッグ処理

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // ドロップ先があるか確認
        if (eventData.pointerEnter != null)
        {
            DropArea dropArea = eventData.pointerEnter.GetComponentInParent<DropArea>();
            if (dropArea != null)
            {
                SetArea(dropArea.areaType, dropArea.transform);
                handUI.UpdateConfirmButtonState();
                return;
            }
        }

        // ドロップ先が無ければ元の位置に戻す
        transform.position = originalPosition;
    }

    #endregion

    /// <summary>
    /// カードを特定のエリアに配置
    /// </summary>
    public void SetArea(AreaType newArea, Transform newParent)
    {
        CurrentArea = newArea;
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
    }
}
