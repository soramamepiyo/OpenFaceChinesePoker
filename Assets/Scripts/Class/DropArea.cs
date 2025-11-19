using UnityEngine;
using UnityEngine.EventSystems;

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
}
