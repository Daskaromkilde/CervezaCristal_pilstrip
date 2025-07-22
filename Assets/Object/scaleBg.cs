using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class scaleBg : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        float cameraHeight = cam.orthographicSize * 2f;
        float cameraWidth = cameraHeight * cam.aspect;

        float targetWidth = cameraWidth * 0.66f; // leaves 2/3th on each side
        float targetHeight = cameraHeight;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr.sprite == null) return;

        Vector2 spriteSize = sr.sprite.bounds.size;
        Vector3 scale = transform.localScale;

        scale.x = targetWidth / spriteSize.x;
        scale.y = targetHeight / spriteSize.y;

        transform.localScale = scale;
        //Debug.Log("scale.x " + scale.x + " and scale.y " + scale.y);
    }
}
