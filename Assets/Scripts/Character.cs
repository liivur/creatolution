using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string identifier;
    public float aiDelay = 0.2f;
    public float changeChance = 0.25f;
    public float changeRange = 60;
    public float speed = 5f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public float viewRadius = 100;
    public float viewAngle = 90;
    public string type = "Herbivore";
    public float staminaRegen = 3;
    public float attackCooldown = 2;
    public float attackRadius = 2;
    public float attackDamage = 20;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public Vector3 lastTargetPosition;
    private float desiredDistance;
    bool seenTarget = false;
    public bool resting = false;
    float attackTimer = 0;
    float health = 100;
    public float restTimer = 0;
    public float stamina = 1000;
    Animator animator;

    Rigidbody body;
    IEnumerator CalculateActionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(aiDelay);
            CalculateAction();
        }
    }
    IEnumerator FindTargetsWithDelayRoutine(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void CalculateAction()
    {
        Transform mainTarget = null;
        float leastDistance = float.MaxValue;
        foreach (Transform target in visibleTargets) 
        {
            float targetDistance = Vector3.Distance(target.transform.position, transform.position);
            if (type == "Herbivore")
            {
                Fruit script = target.gameObject.GetComponent<Fruit>();
                if (script && leastDistance > targetDistance)
                {
                    mainTarget = target;
                }
            }
            else
            {
                Character script = target.gameObject.GetComponent<Character>();
                // Ignore creatures of same type for now
                if (script && type != script.type)
                {
                    if (leastDistance > targetDistance)
                    {
                        mainTarget = target;
                    }

                    // print("Seeing script " + ts.identifier);
                    // Vector3 moveTo = Vector3.MoveTowards(body.position, target.position, 1).normalized;

                }
            }
        }

        if (mainTarget)
        {
            seenTarget = true;
            lastTargetPosition = new Vector3(mainTarget.position.x, 0, mainTarget.position.z);
            Fruit script = mainTarget.gameObject.GetComponent<Fruit>();
            if (type == "Herbivore")
            {
                if (script)
                {
                    desiredDistance = 0;
                } else
                {
                    desiredDistance = float.MaxValue;
                }
            }
            else
            {
                if (!script)
                {
                    desiredDistance = 0;
                }
            }
        } else if (Random.Range(0, 10) < changeChance)
        {
            //Vector3 input = new Vector3(Random.Range(-speed, speed), 0, Random.Range(-speed, speed));
            //var rot = Quaternion.AngleAxis(Random.Range(-changeRange, changeRange), transform.up);
            //body.MoveRotation(rot);

            lastTargetPosition = transform.position + DirectionFromAngle(Vector3.Angle(transform.forward, new Vector3(0, 0, 1)) + Random.Range(-changeRange, changeRange), true).normalized * Random.Range(1, 5);
            lastTargetPosition.y = 0;
            desiredDistance = 0;

            //Vector3.Angle(transform.forward, directionToTarget)
        }
    }

    // From video https://www.youtube.com/watch?v=rQG9aUWarwE
    void FindVisibleTargets()
    {
        List<Transform> newTargets = new List<Transform>();
        // visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            if (target.transform.root == transform || (target.parent && target.parent.transform.root == transform))
            {
                continue;
            }
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    newTargets.Add(target);
                }
            }
        }

        visibleTargets = newTargets;
    }

    Transform GetAttackTarget()
    {
        Collider[] targetsInAttackRadius = Physics.OverlapSphere(transform.position, attackRadius, targetMask);

        for (int i = 0; i < targetsInAttackRadius.Length; i++)
        {
            Transform target = targetsInAttackRadius[i].transform;
            if (target.transform.root == transform || (target.parent && target.parent.transform.root == transform))
            {
                continue;
            }
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
            {
                if (type == "Herbivore")
                {
                    Fruit ts = target.gameObject.GetComponent<Fruit>();
                    if (ts)
                    {
                        return target;
                    }
                } else
                {
                    Character ts = target.gameObject.GetComponent<Character>();
                    if (ts)
                    {
                        return target;
                    }
                }
            }
        }

        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        StartCoroutine("FindTargetsWithDelayRoutine", 0.2f);
        StartCoroutine("CalculateActionRoutine");

        lastTargetPosition = new Vector3(transform.position.x, 0, transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (seenTarget)
        //{
            
        //}

        //body.AddForce(transform.forward * speed / 30, ForceMode.Impulse);

        //print(body.rotation);
        //if (body.rotation)
        

        // Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = (lastTargetPosition - body.position).normalized;
        float distance = Vector3.Distance(lastTargetPosition, transform.position);
        if (distance < desiredDistance)
        {
            targetDirection *= -1;
        }

        Vector3 turnDirection = Vector3.RotateTowards(transform.forward, targetDirection, Time.deltaTime, 0);

        body.MoveRotation(Quaternion.LookRotation(turnDirection));

        if (transform.up.y < 0.5)
        {
            Vector3 direction = Vector3.RotateTowards(transform.up, new Vector3(0, 1, 0), Time.deltaTime, 0);
            body.MoveRotation(Quaternion.LookRotation(direction));
            //body.MoveRotation(Quaternion.LookRotation(Vector3.up));
            //var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            //body.AddTorque(new Vector3(rot.x, rot.y, rot.z) * (-4 * (transform.up.y - 1.1f)));
        }

        float distanceToTarget = Vector3.Distance(transform.position, lastTargetPosition);
        float angle = Vector3.Angle(transform.forward, targetDirection);
        if (angle < 70 && distanceToTarget > attackRadius && !resting && stamina > 0)
        {
            body.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
            stamina -= speed * Time.deltaTime;
        }

        if (resting)
        {
            stamina += staminaRegen;
            restTimer -= Time.deltaTime;
            if (stamina > 800 || restTimer < 0)
            {
                resting = false;
                restTimer = 0;
            }
        }
        else if (stamina <= 0)
        {
            resting = true;
            restTimer = 5;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer < 0)
        {
            Transform target = GetAttackTarget();
            if (target)
            {
                if (type == "Carnivore")
                {
                    Character ts = target.gameObject.GetComponent<Character>();
                    if (ts)
                    {
                        health += attackDamage;
                        ts.health -= attackDamage;
                        attackTimer = attackCooldown;
                        // print(name + " attacked " + target.name + ". Health left " + ts.health);
                    }
                }
                else
                {
                    Fruit ts = target.gameObject.GetComponent<Fruit>();
                    if (ts)
                    {
                        health += attackDamage;
                        ts.health -= attackDamage;
                        attackTimer = attackCooldown;
                        // print(name + " attacked " + target.name + ". Health left " + ts.health);
                    }
                }

                animator.SetTrigger("Attack");
            }
        }
        

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
