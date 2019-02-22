using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeEnemy : MonoBehaviour
{
    #region enemy_variables
    public float health;
    public int value;
    public float moveSpeed;
    Animator anim;
    private EventManager eventManager;
    public bool Dying { get; private set; }
    #endregion

    #region attack_variables
    public GameObject shot;
    public float bulletSpeed;
    public float attackInterval;
    private float attackTimer;
    private GameObject player;
    Transform firepoint;
    string[] fpStrings;
    float shotAngle;
    Quaternion shotRotation;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        attackTimer = attackInterval;
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        fpStrings = new string[8] { "FirePoint", "FirePoint (1)", "FirePoint (2)", "FirePoint (3)", "FirePoint (4)", "FirePoint (5)", "FirePoint (6)", "FirePoint (7)" };
        Dying = false;
        eventManager = Camera.main.GetComponent<EventManager>();
        if (eventManager == null)
        {
            Debug.Log("can't find event manager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // todo orient pointing toward player but temporarily reset rotation during attack
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && !Dying)
        {
            Attack();
        }
        if (player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Bullet"))
        {
            Destroy(collider.gameObject);
            health -= 1;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        for (int i = 0; i < 8; i++)
        {
            shotAngle = 45 * i * -1;
            firepoint = transform.Find(fpStrings[i]);
            shotRotation = Quaternion.Euler(0, 0, shotAngle);
            GameObject go = Instantiate(shot, firepoint.position, shotRotation);
            go.GetComponent<Rigidbody2D>().AddForce(shotRotation * Vector3.right * bulletSpeed, ForceMode2D.Impulse);
        }
        attackTimer = attackInterval;
    }

    public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        Dying = true;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.833f); //Length of EnemyDie animation
        eventManager.AddScore(value);
        Destroy(gameObject);
    }
}
