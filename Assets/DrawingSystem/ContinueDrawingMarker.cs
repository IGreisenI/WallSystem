using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawingSystem;

public class ContinueDrawingMarker : MonoBehaviour
{
    [SerializeField] private GameObject continueDrawingMarkerPrefab;
    [SerializeField] private Drawing drawing;

    private GameObject continueDrawingMarkerGameObject;

    private void Start()
    {
        continueDrawingMarkerGameObject = Instantiate(continueDrawingMarkerPrefab, Vector3.zero, Quaternion.identity);
        continueDrawingMarkerGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (drawing.IsDrawing()) return;

        RaycastHit hit = drawing.RaycastUsingDrawingRaycaster();
        if (drawing.RaycastUsingDrawingRaycaster().collider != null)
        {
            for (int i = 0; i < drawing.GetLines().Count; i++)
            {
                if (drawing.GetLines()[i].linePoints?.Count == 0) continue;

                if (Vector3.Distance(drawing.GetLines()[i].linePoints[^1], hit.point) < drawing.GetContinueDistanceThreshold())
                {
                    continueDrawingMarkerGameObject.transform.position = drawing.GetLines()[i].linePoints[^1];
                    continueDrawingMarkerGameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                    continueDrawingMarkerGameObject.SetActive(true);
                    return;
                }
            }
        }

        continueDrawingMarkerGameObject.SetActive(false);
    }
}
