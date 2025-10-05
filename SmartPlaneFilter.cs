using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class SmartPlaneFilter : MonoBehaviour
{
    private ARPlaneManager planeManager;

    [Header("Filtering Settings")]
    [Tooltip("Minimum plane area in square meters to be considered valid.")]
    public float minPlaneArea = 0.2f;

    [Tooltip("Seconds a plane must be tracked before showing.")]
    public float minLifetime = 1.5f;

    // Keep track of how long each plane has been tracked
    private Dictionary<TrackableId, float> planeTimers = new Dictionary<TrackableId, float>();

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

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            // Start timer for new planes
            if (!planeTimers.ContainsKey(plane.trackableId))
                planeTimers[plane.trackableId] = 0f;
        }
    }

    void Update()
    {
        foreach (var plane in planeManager.trackables)
        {
            // Update timer
            if (!planeTimers.ContainsKey(plane.trackableId))
                planeTimers[plane.trackableId] = 0f;

            planeTimers[plane.trackableId] += Time.deltaTime;

            bool isValid = true;

            // ✅ 1. Filter out small planes
            float area = plane.extents.x * plane.extents.y;
            if (area < minPlaneArea)
                isValid = false;

#if UNITY_IOS
            // ✅ 2. Filter out unclassified planes (if supported)
            if (plane.classification == PlaneClassification.None)
                isValid = false;
#endif

            // ✅ 3. Hide until stable
            if (planeTimers[plane.trackableId] < minLifetime)
                isValid = false;

            // Apply
            plane.gameObject.SetActive(isValid);
        }
    }
}
