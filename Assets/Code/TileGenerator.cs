using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public Vector2 tileSize;

    public bool gen = false;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gen)
        {
            gen = false; foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if (child != transform)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(child.gameObject);
                    };

                }
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                Gen();
            };
        }
    }

    void Gen()
    {
        tileSize = tilePrefab.GetComponent<SpriteRenderer>().size;
        tileSize.x -= 0.02f;
        tileSize.y -= 0.02f;
        var pos = transform.position;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var go = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                go.transform.position = pos;
                go.transform.parent = transform;
                pos.y += tileSize.y;
            }
            pos.y = transform.position.y;
            pos.x += tileSize.x;
        }
    }
#endif
}
