using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum FoodType { Carnivore, Herbivore}

    public string identifier;
    public float speed = 5f;
    public FoodType type = FoodType.Herbivore;
    public float staminaRegen = 3;
    public float attackCooldown = 2;
    public float attackRadius = 2;
    public float attackDamage = 20;
    public float changeChance = 0.25f;
    public float changeRange = 60;
    public float viewAngle = 90;
    public float viewRadius = 100;
    public float matingCallCooldown = 10;
    public float matingCallFrequency = 30;
    public float matingCallRadius = 500;
    public float restDuration = 10;
    public bool resting = false;
    public GameObject floatingTextPrefab;
    
    public float restTimer = 0;
    public float stamina = 1000;

    Animator animator;
    float attackTimer = 0;
    float callTimer = 0;
    public float offspringContribution = 40;
    Rigidbody body;
    float matingCooldown = 5;
    public float matingRadius = 3;
    float matingThreshold = 50;
    float matingTimer = 0;

    [HideInInspector]
    public AI ai;
    [HideInInspector]
    public float aggression = 1;
    [HideInInspector]
    public float fleeDuration = 3;
    [HideInInspector]
    public Health health;
    [HideInInspector]
    public float persistence = 2;

    Plant prevPlant = null;

    bool Attack(Transform target)
    {
        bool attacked = false;
        float targetDistance = Vector3.Distance(target.position, transform.position);
        if (targetDistance < attackRadius)
        {
            Health targetHealth = target.gameObject.GetComponent<Health>();
            health.current = System.Math.Min(health.max, health.current + attackDamage);
            targetHealth.Defend(transform, attackDamage);
            
            attackTimer = attackCooldown;
            // print(name + " attacked " + target.name + ". Health left " + ts.health);

            animator.SetTrigger("Attack");
            ShowMessage("*CHOMP*");

            Fruit fruit = target.gameObject.GetComponent<Fruit>();
            if (fruit && fruit.parent)
            {
                if (!prevPlant)
                {
                    prevPlant = fruit.parent;
                } else if (prevPlant == fruit.parent)
                {
                    prevPlant.Mate(fruit.parent, transform.position);
                    prevPlant = null;
                }
            }

            attacked = true;
        }

        return attacked;
    }

    bool CallMate()
    {
        if (callTimer > 0 || !CanMateAfterCall())
        {
            return false;
        }

        callTimer = matingCallCooldown;
        ShowMessage("*MOOOOOOO!*");

        Collider[] targetsInCallRadius = Physics.OverlapSphere(transform.position, matingCallRadius, ai.targetMask);
        for (int i = 0; i < targetsInCallRadius.Length; i++)
        {
            Transform target = targetsInCallRadius[i].transform;
            if (target.transform.root == transform || (target.parent && target.parent.transform.root == transform))
            {
                continue;
            }

            Character cs = target.gameObject.GetComponent<Character>();
            if (cs && cs.type == type && cs.CanMate())
            {
                cs.HearCall(transform.position);
            }
        }

        return true;
    }

    bool CanAttack()
    {
        return attackTimer <= 0;
    }

    public bool CanMate()
    {
        return health.current > matingThreshold && matingTimer <= 0;
    }

    public bool CanMateAfterCall()
    {
        return health.current > (matingThreshold + GetCallCost()) && matingTimer <= 0;
    }

    float GetCallCost()
    {
        return matingCallRadius / 25;
    }

    public void HearCall(Vector3 position)
    {
        ai.HearCall(position);
    }

    // Start is called before the first frame update
    void Start()
    {
        health = gameObject.GetComponent<Health>();
        ai = gameObject.GetComponent<AI>();
        body = GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ShowMessage("test");
        //if (seenTarget)
        //{

        //}

        //body.AddForce(transform.forward * speed / 30, ForceMode.Impulse);

        //print(body.rotation);
        //if (body.rotation)


        // Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 targetDirection = (ai.targetPosition - body.position).normalized;
        targetDirection.y = 0;
        float distance = Vector3.Distance(ai.targetPosition, transform.position);
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

        float moveAmount = 0;
        if (resting)
        {
            stamina += staminaRegen;
            restTimer -= Time.deltaTime;
            if (stamina > 800 || restTimer < 0)
            {
                StopRest();
            }
        }
        else if (distance > 1)
        {
            moveAmount = speed;

            body.MovePosition(transform.position + transform.forward * moveAmount * Time.deltaTime);
            stamina -= speed * Time.deltaTime;
        }
        health.current -= (1 + moveAmount) * Time.deltaTime * 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (stamina <= 0)
        {
            Rest();
        }

        bool acted = false;
        foreach (KeyValuePair<float, AI.Action> action in ai.actionOrder)
        {
            if (action.Value == AI.Action.Attack && CanAttack())
            {
                Transform target = ai.GetAttackTarget();
                if (target)
                {
                    
                    acted = Attack(target);
                }
            }

            if (action.Value == AI.Action.Mate && CanMate())
            {
                Character target = ai.GetMatingTarget();
                if (target)
                {
                    acted = Mate(target);
                }
            }

            if (action.Value == AI.Action.Call)
            {
                acted = CallMate();
            }

            if (acted)
            {
                break;
            }
        }

        if (callTimer > 0)
        {
            callTimer -= Time.deltaTime;
        }
        
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (health.current <= 0 || transform.position.y < -1)
        {
            ShowMessage("*DEATH*");
            Destroy(gameObject);
        }
    }

    public bool Mate(Character target)
    {
        bool mated = false;
        MatingSpawner spawner = GetComponent<MatingSpawner>();

        float targetDistance = Vector3.Distance(target.transform.position, transform.position);

        if (health.current > matingThreshold && spawner && targetDistance < matingRadius)
        {
            health.current -= offspringContribution;
            target.health.current = target.offspringContribution;

            Character baby = spawner.SpawnObject(this, target);
            baby.health.current = System.Math.Min(baby.health.max, offspringContribution + target.offspringContribution);
            matingTimer = matingCooldown;
            target.matingTimer = target.matingCooldown;

            ShowMessage("*SHAG*");
            mated = true;
        }

        return mated;
    }

    void Rest()
    {
        if (!resting)
        {
            ShowMessage("*ZZzzzZZzzZZ*");
            resting = true;
            restTimer = restDuration;
        }
    }

    public void StopRest()
    {
        if (resting)
        {
            ShowMessage("*GOTIME!*");
            resting = false;
            restTimer = 0;
        }
    }

    public void ShowMessage(string text)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Camera.main.transform.rotation);
            TextMesh textMesh = prefab.GetComponentInChildren<TextMesh>();
            if (textMesh)
            {
                textMesh.text = text;
            }
        }
    }
}
