using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    #region player_variables
    Rigidbody2D playerRB;
    private Camera cam;
    private SpriteRenderer sr;
    private Animator anim;
    private EventManager eventManager;
    #endregion

    #region movement_variables
    public float moveSpeed;
    private float xAxis;
    private float yAxis;
    private bool facingRight;
    Vector2 movementVector;
    #endregion

    #region attack_variables
    public float attackSpeed;
    private float attackTimer;
    public GameObject shot;
    public float bulletSpeed;

    private float mouseX;
    private float mouseY;
    Transform firepoint;
    Vector3 target;
    Vector3 direction;
    Vector2 direction2d;
    float shotAngle;
    Quaternion shotRotation;
    #endregion

    #region health_variables
    public float health;
    public Slider healthBar;
    private bool mercyInvuln;
    public float mercyInvulnLength;
    private float invulnTimer;
    private bool dying;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        xAxis = 0;
        yAxis = 0;
        movementVector = new Vector2(0, 0);
        facingRight = true;
        mercyInvuln = false;
        invulnTimer = 0;
        dying = false;

        attackTimer = attackSpeed;
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        eventManager = Camera.main.GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        movementVector = new Vector2(xAxis, yAxis);
        movementVector *= Time.deltaTime * moveSpeed;
        if (!dying)
        {
            playerRB.MovePosition(playerRB.position + movementVector);
        }

        target = Input.mousePosition;
        target = cam.ScreenToWorldPoint(new Vector3(target.x, target.y, cam.nearClipPlane));

        HandleFlip();

        invulnTimer -= Time.deltaTime;
        if (invulnTimer <= 0)
        {
            mercyInvuln = false;
        }

        attackTimer -= Time.deltaTime;
        if (Input.GetButton("Fire1") && attackTimer <= 0 && !dying)
        {
            Shoot();               
        }
    }

    private void HandleFlip()
    {
        float mouseX = target.x;
        if (mouseX > transform.position.x)
        {
            facingRight = true;
            sr.flipX = false;
        }
        else
        {
            facingRight = false;
            sr.flipX = true;
        }
    }

    private void Shoot()
    {
        attackTimer = attackSpeed;
        if (facingRight)
        {
            firepoint = transform.Find("FirePointRight");
        }
        else
        {
            firepoint = transform.Find("FirePointLeft");
        }
        direction = Vector3.Normalize(target - firepoint.position);
        direction2d = new Vector2(direction.x, direction.y);
        shotAngle = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x);
        if (direction.x < 0)
        {
            shotAngle += 180;
        }
        shotRotation = Quaternion.Euler(0, 0, shotAngle);
        GameObject go = Instantiate(shot, firepoint.position, shotRotation);
        go.GetComponent<Rigidbody2D>().AddForce(direction2d.normalized * bulletSpeed, ForceMode2D.Impulse);
    }

    private void ChangeHealth(int value)
    {
        health += value;
        healthBar.value += value;
        if (health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet") && !mercyInvuln)
        {
            mercyInvuln = true;
            invulnTimer = mercyInvulnLength;
            anim.SetTrigger("Hurt");
            ChangeHealth(-1);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy") && !mercyInvuln)
        {
            mercyInvuln = true;
            invulnTimer = mercyInvulnLength;
            anim.SetTrigger("Hurt");
            ChangeHealth(-1);
        }
        else if (collision.gameObject.CompareTag("EnemyHeal") && !(collision.gameObject.GetComponent<EyeEnemy>().Dying) && !dying)
        {
            ChangeHealth(1);
            collision.gameObject.GetComponent<EyeEnemy>().Die();
        }
    }

    private void Die()
    {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        dying = true;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.833f); //Length of EnemyDie animation
        eventManager.EndGame();
        Destroy(gameObject);
    }
}
