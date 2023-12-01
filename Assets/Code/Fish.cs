using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fish : MonoBehaviour
{
    public UnityEvent onWin;

    private void OnDestroy()
    {
        onWin?.Invoke();
    }
}
