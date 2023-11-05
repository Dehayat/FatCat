using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Events;

public class Cat : MonoBehaviour
{
    [System.Serializable]
    struct growSize
    {
        public float targetSize;
        public GameObject sprite;

    }

    public float speed;
    public GameObject eatTrigger;
    public float minFoodScale;
    public float maxFoodScale;
    public float size;
    public GameObject currentSprite;
    [SerializeField]
    private growSize[] growPoints;
    public Transform eatPoint;
    public int health;
    public Collider2D hurtBox;
    public float normalGrowDuration = 0.1f;
    public float eatDuration = 0.3f;
    public GameObject eatUpTrigger;
    public GameObject eatDownTrigger;
    public UnityEvent<float> onSizeChange;

    private Rigidbody2D rb;
    private int currentSizeStage = 0;
    private bool canMove = true;
    private bool canGetHit = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            return;
        }
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move.Normalize();
        rb.velocity = move * speed;
        if (!eatTrigger.activeSelf && move.sqrMagnitude > 0.1f)
        {
            eatTrigger.SetActive(true);
        }
        else if (eatTrigger.activeSelf && move.sqrMagnitude < 0.1f)
        {
            eatTrigger.SetActive(false);
            eatUpTrigger.SetActive(false);
            eatDownTrigger.SetActive(false);
        }
        var scale = transform.localScale;
        if (move.x < -0.1)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else if (move.x > 0.1)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        if (move.y < -0.1)
        {
            eatDownTrigger.SetActive(true);
        }
        else if (move.y > 0.1)
        {
            eatUpTrigger.SetActive(true);
        }
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var food = collision.GetComponent<Food>();
        if (food != null)
        {
            if (CanEat(food))
            {
                Eat(food);
            }
        }
    }

    private void Eat(Food food)
    {
        StartCoroutine(EatSequence(food));
    }

    IEnumerator EatSequence(Food food)
    {
        canMove = false;
        canGetHit = false;
        //AlignEatPoints(food.eatPoint.position);
        var offset = food.eatPoint.position - eatPoint.position;
        transform.DOBlendableLocalMoveBy(offset, eatDuration);
        yield return new WaitForSeconds(eatDuration);
        Grow(food.size);
        Destroy(food.gameObject);
        yield return null;
        canMove = true;
        canGetHit = true;
    }

    private void AlignEatPoints(Vector3 foodPoint)
    {
        var offset = foodPoint - eatPoint.position;
        transform.position += offset;
    }


    private Tweener scaleTweener;
    private void Grow(float increase)
    {
        scaleTweener?.Kill();

        size += increase;
        float sign = Mathf.Sign(transform.localScale.x);
        Vector3 scale = Vector3.one * size;
        scale.x *= sign;
        //EditorApplication.isPaused = true;
        if (currentSizeStage < growPoints.Length && size > growPoints[currentSizeStage].targetSize)
        {
            currentSprite.SetActive(false);
            currentSprite = growPoints[currentSizeStage].sprite;
            currentSprite.SetActive(true);
            transform.localScale = scale;
            currentSizeStage++;
            onSizeChange?.Invoke(size);
        }
        else
        {
            scaleTweener = transform.DOScale(scale, normalGrowDuration);
            scaleTweener.OnComplete(() => { scaleTweener = null; onSizeChange?.Invoke(size); });
        }
    }

    private bool CanEat(Food food)
    {
        if (!canMove)
        {
            return false;
        }
        float minSize = size * minFoodScale;
        float maxSize = size * maxFoodScale;
        if (food.size > minSize && food.size < maxSize)
        {
            return true;
        }
        return false;
    }

    public void Attack(int damage)
    {
        if (!canGetHit)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Lose");
        }
    }
}
