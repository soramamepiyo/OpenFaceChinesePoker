using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum AreaType
{
    Unplaced,
    Top,
    Middle,
    Bottom
}

public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card cardData;
    public AreaType CurrentArea = AreaType.Unplaced;
    private bool isLocked; // 配置が確定して、もう動かせないか?

    [SerializeField] private Transform frontTransform;
    [SerializeField] private Transform backTransform;
    private SpriteRenderer frontRenderer;
    private SpriteRenderer backRenderer;

    private HandUI handUI;
    private Vector3 originalPosition;

    private float MOVE_ANIM_SEC = 0.3f;

    private void Awake()
    {
        frontRenderer = frontTransform.GetComponent<SpriteRenderer>();
        backRenderer = backTransform.GetComponent<SpriteRenderer>();
    }

    public void Initialize(Card card, HandUI handUI)
    {
        this.cardData = card;
        this.handUI = handUI;
        this.isLocked = false;
        originalPosition = transform.position;

        SetCardAppearance(card);
    }

    public void SetCardAppearance(Card card)
    {
        if (frontRenderer != null)
        {
            Sprite sprite = handUI.GetCardSprite(card);
            if (sprite != null)
                frontRenderer.sprite = sprite;
            else
                Debug.LogWarning("Sprite not found for card: " + card);
        }
    }

    #region Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPoint.z = 0;
        transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // 画面座標をワールド座標に変換
        Vector3 worldPoint3D = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPoint3D.z = 0f; // カードと HitArea の Z を同じにする

        int dropAreaLayer = LayerMask.GetMask("DropArea");
        RaycastHit2D hit = Physics2D.Raycast(worldPoint3D, Vector2.zero, Mathf.Infinity, dropAreaLayer);

        if (hit.collider != null)
        {
            // 親に DropArea がついているかチェック
            DropArea dropArea = hit.collider.GetComponentInParent<DropArea>();
            if (dropArea != null)
            {
                if (dropArea.IsEnablePlace())
                {
                    // エリアにカードを移動
                    SetArea(dropArea.areaType, dropArea.transform);
                    // 戻すときのみUnplacedAreaも整列する
                    if (dropArea.areaType == AreaType.Unplaced) handUI.ArrangeAlignUnplacedArea();
                    handUI.ArrangeAlignPlacedArea();
                    handUI.UpdateConfirmButtonState();
                    return;
                }
            }
        }

        // ここまで来たら DropArea にヒットせず、元に戻す
        transform.DOMove(originalPosition, MOVE_ANIM_SEC);
    }
    #endregion

    public void SetArea(AreaType newArea, Transform newParent)
    {
        CurrentArea = newArea;
        transform.SetParent(newParent);

        // 配置したカードは右端（最後）に来るようにする
        transform.SetAsLastSibling();
    }

    public void LockCardPlace()
    {
        isLocked = true;
        frontRenderer.color = new Color(0.8f, 0.8f, 0.8f);
    }
}
