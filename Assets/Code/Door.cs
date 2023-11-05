using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float maxDist = 0.3f;
    public Transform midPoint;
    public SpriteRenderer overlay;
    public CameraManager cameraManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Cat>() == null)
        {
            return;
        }
        cameraManager.SetBetweenCam();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Cat>() == null)
        {
            return;
        }
        var pos = collision.transform.position.y;
        var progress = pos - midPoint.transform.position.y;
        progress = Mathf.Clamp(progress, -maxDist, maxDist);
        cameraManager.SetBetweenCamValue(1 - (progress / (2 * maxDist) + 0.5f));
        if (progress > 0)
        {
        }
        else
        {
            var alpha = -progress / maxDist;
            var color = overlay.color;
            color.a = alpha;
            overlay.color = color;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Cat>() == null)
        {
            return;
        }
        if (collision.transform.position.y > midPoint.position.y)
        {
            cameraManager.SetStaticCam();
            var color = overlay.color;
            color.a = 0;
            overlay.color = color;
        }
        else
        {
            cameraManager.SetFollowCam();
            var color = overlay.color;
            color.a = 1;
            overlay.color = color;
        }
    }
}
