using UnityEngine;

public class iOSBridgeStub : MonoBehaviour
{
    public void SendToiOS(string json)
    {
        Debug.Log($"[BridgeStub] → Swift: {json}");
        // Swift wrapper will handle Firebase API call here
    }

    // Optional: simulate Swift calling Unity to change modes
    public StrokeDrawer strokeDrawer;
    public void SimulateModeChange(string mode)
    {
        strokeDrawer.SetMode(mode);
    }
}
