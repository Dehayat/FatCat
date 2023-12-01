using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Cat : MonoBehaviour
{

    public Collider2D HurtBox
    {
        get
        {
            return stages[currentSizeStage].hurtBox;
        }
    }
    public GameObject EatTrigger
    {
        get
        {
            return stages[currentSizeStage].eatSideTrigger;
        }
    }
    public GameObject EatUpTrigger
    {
        get
        {
            return stages[currentSizeStage].eatUpTrigger;
        }
    }
    public GameObject EatDownTrigger
    {
        get
        {
            return stages[currentSizeStage].eatDownTrigger;
        }
    }
    public GameObject EatNearTrigger
    {
        get
        {
            return stages[currentSizeStage].eatNearTigger;
        }
    }
    public Transform EatPoint
    {
        get
        {
            return stages[currentSizeStage].eatPoint;
        }
    }

    public float speed;
    public float minFoodScale;
    public float maxFoodScale;
    public float size;
    public GameObject currentStageObject;
    [SerializeField]
    private CatStage[] stages;
    public int health;
    public float normalGrowDuration = 0.1f;
    public float eatDuration = 0.3f;
    public UnityEvent<float> onSizeChange;
    public UnityEvent onLose;
    public UnityEvent onWin;

    private Rigidbody2D rb;
    private int currentSizeStage = 0;
    private bool canMove = true;
    private bool canGetHit = true;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        canMove = false;
    }
    private void Start()
    {
        currentStageObject = stages[currentSizeStage].gameObject;
    }

    void Update()
    {
        if (!canMove)
        {
            anim.SetBool("Walk", false);
            return;
        }
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (move.magnitude > 0.1)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }
        move.Normalize();
        rb.velocity = move * speed;
        if (!EatTrigger.activeSelf && move.sqrMagnitude > 0.1f)
        {
            EatTrigger.SetActive(true);
        }
        else if (EatTrigger.activeSelf && move.sqrMagnitude < 0.1f)
        {
            EatTrigger.SetActive(false);
            EatUpTrigger.SetActive(false);
            EatDownTrigger.SetActive(false);
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
            EatDownTrigger.SetActive(true);
        }
        else if (move.y > 0.1)
        {
            EatUpTrigger.SetActive(true);
        }
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canMove)
        {
            return;
        }
        var food = collision.GetComponent<Food>();
        if (food != null)
        {
            if (CanEat(food))
            {
                Eat(food);
            }
        }
    }

    public void SetInvincible()
    {
        canGetHit = false;
    }

    private void Eat(Food food)
    {
        StartCoroutine(EatSequence(food));
    }

    IEnumerator EatSequence(Food food)
    {
        if (food.TryGetComponent<Car>(out var car))
        {
            car.Stop();
        }
        rb.simulated = false;
        canMove = false;
        canGetHit = false;
        anim.SetTrigger("Eat");
        var offset = food.eatPoint.position - EatPoint.position;
        food.SetDrawOrder(3);
        transform.DOBlendableLocalMoveBy(offset, eatDuration);
        chomp = false;
        yield return new WaitUntil(() => chomp);
        Grow(food.digestSize);
        Destroy(food.gameObject);
        float digestSize = EatAllNear();
        Grow(digestSize);
        finishEat = false;
        yield return new WaitUntil(() => finishEat);

        canGetHit = true;
        canMove = true;
        rb.simulated = true;
    }

    public Collider2D[] colCache = new Collider2D[30];
    private float EatAllNear()
    {
        float totalGrowSize = 0;
        var contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        contactFilter.layerMask = -1;
        int colCount = Physics2D.OverlapCollider(EatNearTrigger.GetComponent<Collider2D>(), new ContactFilter2D(), colCache);
        for (int i = 0; i < colCount; i++)
        {
            if (colCache[i].TryGetComponent<Food>(out var food) && CanEat(food))
            {
                totalGrowSize += food.digestSize;
                Destroy(colCache[i].gameObject);
            }
        }
        return totalGrowSize;
    }

    private bool chomp = false;
    public void EatFood()
    {
        chomp = true;
    }
    private bool finishEat = false;
    public void FinishEatFood()
    {
        finishEat = true;
    }

    private Tweener scaleTweener;

    private void Grow(float increase)
    {
        scaleTweener?.Kill();

        size += increase;
        float sign = Mathf.Sign(transform.localScale.x);
        Vector3 scale = Vector3.one * size;
        scale.x *= sign;
        if (currentSizeStage < stages.Length - 1 && size > stages[currentSizeStage].targetSize)
        {
            currentStageObject.SetActive(false);
            currentSizeStage++;
            currentStageObject = stages[currentSizeStage].gameObject;
            currentStageObject.SetActive(true);
            transform.localScale = scale;
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
        float minSize = size * minFoodScale;
        float maxSize = size * maxFoodScale;
        if (food.size >= minSize - Mathf.Epsilon && food.size <= maxSize + Mathf.Epsilon)
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
            health = 0;
            SetInvincible();
            StartCoroutine(DieStuff());
        }
    }

    IEnumerator DieStuff()
    {
        EnableControls(false);
        scaleTweener = transform.DOScale(Vector3.one, 0.7f);
        GetComponent<AudioSource>().Play();
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.3f);
        currentStageObject.SetActive(false);
        currentSizeStage = 0;
        currentStageObject = stages[currentSizeStage].gameObject;
        currentStageObject.SetActive(true);
        //onSizeChange?.Invoke(size);
        yield return new WaitForSeconds(0.7f);
        onLose?.Invoke();
    }

    public void EnableControls(bool enable)
    {
        canMove = enable;
    }

    public void Win()
    {
        anim.SetTrigger("Win");
    }
}
