using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public GameObject BulletPrefab;
    public GameObject EnemyManager;
    public GameObject Target;
    GameObject PlayerManager;
    GameObject BulletSpawner;
    GameObject TargetPoint;

    const float MAX_HEALTH = 100;
    const float BULLET_SPEED = 200;
    const float BULLET_COOLDOWN = 1;
    const float MAX_RANGE = 200;
    const float TARGET_COOLDOWN = 1;
    const int MY_LAYER = 1 << 11;

    float health = MAX_HEALTH;
    float bulletTimer = 0;
    float targetTimer = 0;
    int layerMask = ~MY_LAYER;

    List<GameObject> EnemyTargets;
    RaycastHit targetHit;

    // Start is called before the first frame update
    void Start()
    {
        BulletSpawner = gameObject.transform.GetChild(0).gameObject;
        TargetPoint = gameObject.transform.GetChild(1).gameObject;
        EnemyManager = GameObject.FindGameObjectWithTag("EnemyManager");
        EnemyManager.GetComponent<EnemyManager>().AddEnemy(this.gameObject);
        PlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
    }

    // Update is called once per frame
    void Update()
    {
        //if (health <= 0)
        //    Death();

        if (Target != null)
        {
            transform.LookAt(Target.transform);
            Shoot();
        }
        else
        {
            targetTimer += Time.deltaTime;
            if (targetTimer > TARGET_COOLDOWN)
            {
                RefreshEnemies();
                FindTarget();
                targetTimer = 0;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "LightBullet":
                health -= 10;
                Destroy(collision.gameObject);
                break;
            case "MediumBullet":
                health -= 25;
                Destroy(collision.gameObject);
                break;
            case "HeavyBullet":
                health -= 70;
                Destroy(collision.gameObject);
                break;
        }
    }

    void Death()
    {
        EnemyManager.GetComponent<EnemyManager>().RemoveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }

    void Shoot()
    {
        if (Target != null)
        {
            bulletTimer += Time.deltaTime;
            if (bulletTimer > BULLET_COOLDOWN)
            {
                if (TargetVisible(Target.transform.position))
                {
                    var bullet = (GameObject)Instantiate(BulletPrefab, BulletSpawner.transform.position, BulletSpawner.transform.rotation);
                    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * BULLET_SPEED;
                }
                else
                {
                    Target = null;
                }

                bulletTimer = 0;
            }
        }

    }

    bool TargetVisible(Vector3 targetLocation)
    {
        Vector3 direction = (targetLocation - transform.position).normalized;
        if (Physics.Raycast(TargetPoint.transform.position, direction, out targetHit, MAX_RANGE, layerMask))
        {
            if (targetHit.transform.gameObject.tag == "PlayerSquad")
            {
                return true;
            }
        }
        return false;
    }

    void RefreshEnemies()
    {
        EnemyTargets = PlayerManager.GetComponent<PlayerManager>().GetPlayerSquad();
    }

    void FindTarget()
    {
        float previousDistance = 0;
        float currentDistance = 0;

        foreach (GameObject Enemy in EnemyTargets)
        {
            if ((Enemy != null) && (TargetVisible(Enemy.transform.position)))
            {
                if (Target != null)
                {
                    currentDistance = Vector3.Distance(Enemy.transform.position, transform.position);
                    if (currentDistance < previousDistance)
                    {
                        Target = Enemy;
                        previousDistance = currentDistance;
                    }
                }
                else
                {
                    Target = Enemy;
                    previousDistance = Vector3.Distance(Target.transform.position, transform.position);
                }
            }
        }
    }
}
