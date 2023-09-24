using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraAdjustment : MonoBehaviour
{
    public float sceneWidth = 10;
    public float sceneHeight = 10;
    Camera _camera;
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        _camera.orthographicSize = desiredHalfHeight;*/
        float unitsPerPixel = sceneHeight / Screen.height;
        float desiredHalfWidth = 0.5f * unitsPerPixel * Screen.width;

        _camera.orthographicSize = desiredHalfWidth;
    }
}
