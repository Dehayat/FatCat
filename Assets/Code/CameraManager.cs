using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CameraManager : MonoBehaviour
{
    public Camera followCam;
    public Camera staticCam;
    public Camera betweenCam;

    public void SetFollowCam()
    {
        if (followCam != null)
        {
            followCam.enabled = true;
            staticCam.enabled = false;
            betweenCam.enabled = false;
        }
    }
    public void SetStaticCam()
    {
        if (staticCam != null)
        {
            staticCam.enabled = true;
            followCam.enabled = false;
            betweenCam.enabled = false;
        }
    }
    public void SetBetweenCam()
    {
        if (followCam.enabled)
        {
            betweenCam.orthographicSize = followCam.orthographicSize;
            betweenCam.transform.position = followCam.transform.position;
        }
        else if (staticCam.enabled)
        {
            betweenCam.orthographicSize = staticCam.orthographicSize;
            betweenCam.transform.position = staticCam.transform.position;
        }
        betweenCam.enabled = true;
        followCam.enabled = false;
        staticCam.enabled = false;
    }

    private void LateUpdate()
    {
        if (betweenCam.enabled)
        {

            betweenCam.orthographicSize = Mathf.Lerp(staticCam.orthographicSize, followCam.orthographicSize, lerpValue);
            betweenCam.transform.position = Vector3.Lerp(staticCam.transform.position, followCam.transform.position, lerpValue);
        }
    }
    private float lerpValue;
    public void SetBetweenCamValue(float value)
    {
        lerpValue = value;
    }
}
