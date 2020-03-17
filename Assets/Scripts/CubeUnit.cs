using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CubeUnit : MonoBehaviour
{
    public GameObject BulletPrefab;
    GameObject BulletSpawner;
    GameObject EnemyManager;
    List<GameObject> EnemyTargets;
    GameObject PlayerManager;
    GameObject Target;

    public GameObject HealthBar;

    const float BULLET_SPEED = 200;
    const float BULLET_COOLDOWN = 1;
    const float ROTATION_SPEED = 0.08f;
    const float TARGET_COOLDOWN = 1;
    const float MAX_HEALTH = 100;
    const float MAX_RANGE = 200;
    const int MY_LAYER = 1 << 8;

    bool moving = false;
    bool newTarget = false;
    bool shooting = false;
    
    float health = MAX_HEALTH;
    float bulletTimer = 0;
    float rotationStep = 0;
    float targetTimer = 0;
    int layerMask = ~MY_LAYER;

    Quaternion toRotation;
    RaycastHit targetHit;
    
    NavMeshAgent navMeshAgent;
    NavMeshPath navMeshPath;

    // Start is called before the first frame update
    void Start()
    {
        BulletSpawner = gameObject.transform.GetChild(0).gameObject;
        EnemyManager = GameObject.FindGameObjectWithTag("EnemyManager");
        PlayerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        PlayerManager.GetComponent<PlayerManager>().AddUnit(this.gameObject);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();

        if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health<=0)
        {
            health = 0;
            Death();
        }

        if (moving)
        {
            if (!navMeshAgent.pathPending)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0)
                    {
                        moving = false;
                    }
                }
            }
        }

        if (!moving)
        {
            if (Target == null)
            {
                shooting = false;
                RefreshEnemies();
                if (EnemyTargets.Count > 0)
                    FindTarget();
            }

            if (shooting)
            {
                Shoot();
            }
            else
            {
                targetTimer += Time.deltaTime;
                if (targetTimer > TARGET_COOLDOWN)
                {
                    Target = null;
                    targetTimer = 0;
                }
            }
        }
        

        if (newTarget)
        {
            rotationStep += ROTATION_SPEED * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationStep);
            if (Target!=null && FacingTarget(Target.transform.position))
            {
                shooting = true;
            }
            if (transform.rotation == toRotation)
            {
                newTarget = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Bullet":
                health -= 10;
                Destroy(collision.gameObject);
                HealthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health);
                break;
        }
    }
    void Death()
    {
        PlayerManager.GetComponent<PlayerManager>().RemoveUnit(this.gameObject);
        Destroy(this.gameObject);
    }

    void ResetState()
    {
        Target = null;
        newTarget = false;
        moving = false;
        shooting = false;
        bulletTimer = 0;
    }

    public void Move(Vector3 targetDestination)
    {
        if (CalculateNewPath(targetDestination))
        {
            ResetState();
            navMeshAgent.destination = targetDestination;
            moving = true;
        }
    }

    bool CalculateNewPath(Vector3 targetPoint)
    {
        navMeshAgent.CalculatePath(targetPoint, navMeshPath);

        if(navMeshPath.status != NavMeshPathStatus.PathComplete)
            return false;
        else
            return true;
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
        if (Physics.Raycast(transform.position, direction, out targetHit, MAX_RANGE, layerMask))
        {
            if (targetHit.transform.gameObject.tag == "EnemySquad")
            {
                return true;
            }
        }
        return false;
    }

    bool FacingTarget(Vector3 targetLocation)
    {
        if (Physics.Raycast(BulletSpawner.transform.position, transform.TransformDirection(Vector3.forward), out targetHit, MAX_RANGE, layerMask))
        {
            if (targetHit.transform.gameObject.tag == "EnemySquad")
            {
                return true;
            }
        }
        return false;
    }

    void RefreshEnemies()
    {
        EnemyTargets = EnemyManager.GetComponent<EnemyManager>().GetEnemies();
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
                newTarget = true;
                rotationStep = 0;
                toRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            }
        }
    }
}
