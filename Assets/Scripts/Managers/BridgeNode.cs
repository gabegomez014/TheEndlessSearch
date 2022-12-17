using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeNode : MonoBehaviour
{
    public int SectionConnectingTo;
    public Transform ConnectionEntryPoint;
    public Transform SectionParent;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            if (ConnectionEntryPoint != null) {
                SectionManager.Instance.SectionChange(SectionConnectingTo, ConnectionEntryPoint);
            } else {
                Debug.LogWarning("Please add a connection entry point");
            }
        }
    }
}
