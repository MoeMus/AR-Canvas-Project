using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneFilter : MonoBehaviour
{
    private ARPlaneManager planeManager;
    public float minPlaneArea = 0.5f; // in square meters

    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            // Only keep classified planes
            if (plane.classification == UnityEngine.XR.ARSubsystems.PlaneClassification.None)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}