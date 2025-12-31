using System.Collections;
using UnityEditor;
using UnityEngine;

public class OFCCpu : MonoBehaviour
{

    AreaType[] areaList = { AreaType.Top, AreaType.Middle, AreaType.Bottom };

    public IEnumerator ActionCpu(HandManager handMg)
    {
        // çlÇ¶ÇƒÇ¢ÇÈÉtÉä
        yield return new WaitForSeconds(2);

        PlacePhase phase = handMg.GetCurrentPhase();

        for (int i = 0; i < handMg.GetDrawCardsNum(handMg.GetCurrentPhase());i++)
        {
            CardUI card = handMg.enemyHandUI.unplacedArea.GetComponentInChildren<CardUI>();
            if (card == null) {
                Debug.LogWarning("No Card");
                break;
            }
            foreach (var a in areaList) {
                if (placeCard(handMg, card, a)) break;
            }
        }

        handMg.EndPhase();
    }

    private bool placeCard(HandManager handMg, CardUI card, AreaType area)
    {
        DropArea d_area = handMg.enemyHandUI.GetDropArea(area);

        if (!d_area.IsEnablePlace()) return false;

        card.SetArea(area, d_area.transform);
        handMg.enemyHandUI.ArrangeAlignPlacedArea();
        return true;
    }
}
