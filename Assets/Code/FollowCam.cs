using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float lerp = 0.3f;
    public AnimationCurve camSizeBasedOnTargetSize;
    public float sizeChangeDuration = 0.3f;

    private float z;
    private Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        z = transform.position.z;
    }
    private void OnEnable()
    {
        if (target != null)
        {
            var cat = target.GetComponent<Cat>();
            if (cat != null)
            {
                cat.onSizeChange.AddListener(SizeChanged);
            }
        }
    }

    private Tweener sizeTweener = null;
    private void SizeChanged(float newSize)
    {
        sizeTweener?.Kill();
        sizeTweener = cam.DOOrthoSize(camSizeBasedOnTargetSize.Evaluate(newSize), sizeChangeDuration);
        sizeTweener.OnComplete(() => sizeTweener = null);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, target.position, lerp);
        var pos = transform.position;
        pos.z = z;
        transform.position = pos;
    }
}
