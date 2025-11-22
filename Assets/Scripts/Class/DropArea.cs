using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.GPUSort;

/// <summary>
/// カードを受け取るエリア（Top / Middle / Bottom / Unplaced）
/// ドラッグ＆ドロップの判定用
/// </summary>
public class DropArea : MonoBehaviour, IDropHandler
{
    public AreaType areaType;

    public void OnDrop(PointerEventData eventData)
    {
        // 実際の処理は CardUI 内で行うので空でもOK
        // このスクリプトを置くことで PointerEnter から判定可能にする
    }

    public int GetMaxAlignCards()
    {
        return (areaType == AreaType.Top) ? 3 : 5;
    }

    public bool IsEnablePlace()
    {
        int cardsNum = 0;
        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("Card")) cardsNum++;
                
        }
        return cardsNum < GetMaxAlignCards();
    }

    public void LockCards()
    {
        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("Card"))
            {
                CardUI cui = child.GetComponent<CardUI>();
                cui.LockCardPlace();
            }
        }
    }
}
