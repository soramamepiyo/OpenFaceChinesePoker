using Mono.Cecil;
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
    public Card cardData;
    public AreaType CurrentArea = AreaType.Unplaced;

    [SerializeField] private Transform frontTransform;
    [SerializeField] private Transform backTransform;
    private SpriteRenderer frontRenderer;
    private SpriteRenderer backRenderer;

    private HandUI handUI;
    private Vector3 originalPosition;

    private void Awake()
    {
        frontRenderer = frontTransform.GetComponent<SpriteRenderer>();
        backRenderer = backTransform.GetComponent<SpriteRenderer>();
    }

    public void Initialize(Card card, HandUI handUI)
    {
        this.cardData = card;
        this.handUI = handUI;
        originalPosition = transform.position;

        SetCardAppearance(card);
    }

    public void SetCardAppearance(Card card)
    {
        if (frontRenderer != null)
        {
            Sprite sprite = handUI.GetCardSprite(card);
            if (sprite != null)
            {
                frontRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogWarning("Sprite not found for card: " + card);
            }
        }
    }

    #region Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPoint.z = 0;
        transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // DropêÊîªíË
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

        // ÉhÉçÉbÉvêÊÇ™Ç»ÇØÇÍÇŒå≥Ç…ñﬂÇ∑
        transform.position = originalPosition;
    }
    #endregion

    public void SetArea(AreaType newArea, Transform newParent)
    {
        CurrentArea = newArea;
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
    }
}
