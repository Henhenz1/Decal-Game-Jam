using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleyeEnemy : MonoBehaviour
{
    #region enemy_variables
    public float health;
    public int value;
    public float moveSpeed;
    private Animator anim;
    private EventManager eventManager;
    #endregion

    #region spawn_variables
    private const float xMin = -7.5f;
    private const float xMax = 7.5f;
    private const float yMin = -3.5f;
    private const float yMax = 3.5f;
    public Vector3 Destination { get; private set; }
    private bool spawning;
    #endregion

    #region attack_variables
    public GameObject shot;
    public float bulletSpeed;
    public float fireRate;
    private float fireTimer;
    private bool dying;

    private Transform firepoint;
    private string[] fpStrings = { "FirePoint", "FirePoint (1)", "FirePoint (2)" };
    private float shotAngle;
    private Vector2 shotDirection;
    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        spawning = true;
        dying = false;
        fireTimer = fireRate;
        anim = GetComponent<Animator>();
        Destination = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
        eventManager = Camera.main.GetComponent<EventManager>();
        if (eventManager == null)
        {
            Debug.Log("can't find event manager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -90 * Time.deltaTime);
        fireTimer -= Time.deltaTime;

        if (spawning)
        {
            MoveToPosition();
        }

        if (fireTimer <= 0 && !spawning && !dying)
        {
            Fire();
            fireTimer = fireRate;
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

    #region spawn_functions
    void MoveToPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, Destination, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, Destination) < 0.001f)
        {
            spawning = false;
        }
    }
    #endregion

    void Fire()
    {
        for (int i = 0; i < 3; i++)
        {
            firepoint = transform.Find(fpStrings[i]);
            shotAngle = (90 - 120 * i) * Mathf.Deg2Rad;
            shotDirection = new Vector2(Mathf.Cos(shotAngle), Mathf.Sin(shotAngle));
            GameObject go = Instantiate(shot, firepoint.position, Quaternion.identity);
            go.GetComponent<Rigidbody2D>().AddForce(transform.rotation.normalized * shotDirection * bulletSpeed, ForceMode2D.Impulse);
        }
    }

    void Die()
    {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        dying = true;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.833f); //Length of EnemyDie animation
        eventManager.AddScore(value);
        Destroy(gameObject);
    }
}
