using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARRaycastManager))]
public class StrokeDrawer : MonoBehaviour
{
    [Header("Drawing Settings")]
    public GameObject strokePointPrefab; // small sphere prefab for drawing
    public bool eraserMode = false;
    public iOSBridgeStub bridge;

    public Color brushColor = Color.green;

    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new();
    private List<Vector3> currentStrokePoints = new();
    private bool isDrawing = false;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        brushColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
    }

    void Update()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartStroke(touch.position);
                break;
            case TouchPhase.Moved:
                UpdateStroke(touch.position);
                break;
            case TouchPhase.Ended:
                EndStroke();
                break;
        }
    }

    private void StartStroke(Vector2 screenPos)
    {
        currentStrokePoints.Clear();

        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            AddStrokePoint(hitPose.position);
            isDrawing = true;
        }
    }

    private void UpdateStroke(Vector2 screenPos)
    {
        if (!isDrawing) return;

        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Vector3 last = currentStrokePoints[^1];

            if (Vector3.Distance(last, hitPose.position) > 0.01f)
                AddStrokePoint(hitPose.position);
        }
    }

    private void AddStrokePoint(Vector3 position)
    {
        currentStrokePoints.Add(position);

        if (!eraserMode)
        {
            GameObject point = Instantiate(strokePointPrefab, position, Quaternion.identity);
            point.tag = "StrokePoint";

            var renderer = point.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.color = brushColor;
            }
        }
        else
        {
            foreach (var obj in GameObject.FindGameObjectsWithTag("StrokePoint"))
            {
                if (Vector3.Distance(obj.transform.position, position) < 0.05f)
                    Destroy(obj);
            }
        }
    }

    private void EndStroke()
    {
        if (!isDrawing || eraserMode)
        {
            isDrawing = false;
            return;
        }

        var stroke = new StrokeData
        {
            color = ColorUtility.ToHtmlStringRGB(brushColor),
            points = currentStrokePoints.ToArray()
        };

        string json = JsonUtility.ToJson(stroke, true);
        Debug.Log($"🖊️ Stroke finished ({stroke.points.Length} points)");
        bridge?.SendToiOS(json);

        isDrawing = false;
    }

    // Called by Swift or debug UI to toggle between pen/eraser
    public void SetMode(string mode)
    {
        eraserMode = mode == "eraser";
        Debug.Log($"✏️ Mode changed: {(eraserMode ? "Eraser" : "Pen")}");
    }
}
