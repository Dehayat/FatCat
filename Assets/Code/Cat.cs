using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Rigidbody2D rb;
    private int currentSizeStage = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var food = collision.GetComponent<Food>();
        if (food != null)
        {
            if (CanEat(food))
            {
                AlignEatPoints(food.eatPoint.position);
                Grow(food.size);
                Destroy(food.gameObject);
            }
        }
    }

    private void AlignEatPoints(Vector3 foodPoint)
    {
        var offset = foodPoint - eatPoint.position;
        transform.position += offset;
    }


    private void Grow(float increase)
    {
        size += increase;
        float sign = Mathf.Sign(transform.localScale.x);
        Vector3 scale = Vector3.one * size;
        scale.x *= sign;
        transform.localScale = scale;
        if (currentSizeStage < growPoints.Length)
        {
            if (size > growPoints[currentSizeStage].targetSize)
            {
                currentSprite.SetActive(false);
                currentSprite = growPoints[currentSizeStage].sprite;
                currentSprite.SetActive(true);
                currentSizeStage++;
            }
        }
    }

    private bool CanEat(Food food)
    {
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
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Lose");
        }
    }
}
