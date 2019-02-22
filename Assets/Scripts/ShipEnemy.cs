using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEnemy : MonoBehaviour
{
    #region enemy_variables
    private GameObject player;
    private Animator anim;
    private EventManager eventManager;
    public float moveSpeed;
    public float health;
    public int value;
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
    public float lockOnPeriod;
    public float rotationSpeed;
    public float firingDelay;
    public float burstSpeed;
    public int burstCount;
    public float bulletSpeed;
    public GameObject shot;

    private float lockOnTimer;
    private float delayTimer;
    private float burstTimer;
    private int burstTracker;

    private bool lockingOn;
    private bool aboutToFire;
    private bool firing;
    private bool dying;

    private Transform firepoint;
    private float lookAngle;
    private Quaternion lookRotation;
    private Vector3 direction;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        eventManager = Camera.main.GetComponent<EventManager>();
        if (eventManager == null)
        {
            Debug.Log("can't find event manager");
        }
        spawning = true;
        lockingOn = false;
        aboutToFire = false;
        firing = false;
        dying = false;

        Destination = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);

        lockOnTimer = lockOnPeriod;
        delayTimer = firingDelay;
        burstTimer = burstSpeed;
        burstTracker = burstCount;

        firepoint = transform.Find("FirePoint");
    }

    // Update is called once per frame
    void Update()
    {
        if(spawning)
        {
            MoveToPosition();
        }

        if(lockingOn)
        {
            LockOn();
        }

        if(aboutToFire)
        {
            PauseForEffect();
        }

        if(firing)
        {
            Burst();
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
            lockingOn = true;
        }
    }
    #endregion

    #region attack_functions
    void LockOn()
    {
        lockOnTimer -= Time.deltaTime;
        if (player != null)
        {
            direction = (player.transform.position - firepoint.position).normalized;
        }
        lookAngle = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x) + 90;
        if (direction.x < 0)
        {
            lookAngle += 180;
        }
        lookRotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        if (lockOnTimer <= 0)
        {
            lockingOn = false;
            lockOnTimer = lockOnPeriod;
            aboutToFire = true;
        }
    }

    void PauseForEffect()
    {
        delayTimer -= Time.deltaTime;
        if (delayTimer <= 0)
        {
            aboutToFire = false;
            delayTimer = firingDelay;
            firing = true;
        }
    }

    void Burst()
    {
        burstTimer -= Time.deltaTime;
        if (burstTimer <= 0 && !dying)
        {
            Fire();
            burstTracker -= 1;
            burstTimer = burstSpeed;
        }
        if (burstTracker == 0)
        {
            firing = false;
            burstTracker = burstCount;
            lockingOn = true;
        }
    }

    void Fire()
    {
        GameObject go = Instantiate(shot, firepoint.position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().AddForce(transform.rotation.normalized * Vector2.down * bulletSpeed, ForceMode2D.Impulse);
    }

    void Die()
    {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        spawning = false;
        lockingOn = false;
        aboutToFire = false;
        firing = false;
        dying = true;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(0.833f); //Length of EnemyDie animation
        eventManager.AddScore(value);
        Destroy(gameObject);
    }

    #endregion
}
