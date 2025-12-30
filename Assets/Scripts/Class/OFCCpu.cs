using System.Collections;
using UnityEngine;

public class OFCCpu : MonoBehaviour
{
    public IEnumerator ActionCpu(HandManager handMg)
    {
        yield return new WaitForSeconds(5);
        handMg.EndPhase();
    }
}
