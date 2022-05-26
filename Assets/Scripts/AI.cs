using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public enum Action { Attack, Call, Mate }
    public SortedList<float, Action> actionOrder = new SortedList<float, Action>() { { 1, Action.Attack }, { 2, Action.Mate } };

    public float aiDelay = 0.5f;
    public LayerMask obstacleMask;
    public LayerMask targetMask;

    [HideInInspector]
    public SortedList<float, Transform> visibleTargets = new SortedList<float, Transform>();

    [HideInInspector]
    public Character character;

    Transform attacker;
    float desiredDistance;
    float fleeTime = 0;
    float followCallTimer = 0;
    float sawMate;

    float seenTarget = 0;
    Vector3 callPosition;
    public Vector3 lastTargetPosition;
    public Vector3 targetPosition;
    

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

    public void Defend(Transform attacker, float damage)
    {
        character.StopRest();
        this.attacker = attacker;
        fleeTime = character.fleeDuration;
    }

    void CalculateAction()
    {
        bool canMate = character.CanMate();
        SortedList<float, Action> newActionOrder = new SortedList<float, Action>();
        newActionOrder.Add(GetAttackPriority(), Action.Attack);
        newActionOrder.Add(GetMatingPriority(), Action.Mate);
        newActionOrder.Add(character.matingCallFrequency / (Time.time - sawMate), Action.Call);

        actionOrder = newActionOrder;
        if (attacker)
        {
            //Vector3 targetDirection = (ai.targetPosition - body.position).normalized;
            targetPosition = GetFleePosition();
        } else 
        {
            targetPosition = CalculateTargetPosition();
        }

        if (followCallTimer > 0)
        {
            targetPosition = callPosition;
        }
        //character.ShowMessage("position" + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
    }

    Vector3 CalculateTargetPosition()
    {
        float attackPriority = GetAttackPriority();
        float matingPriority = GetMatingPriority();
        
        float attackWeight = 0;
        float matingWeight = 0;

        Transform mate = null;
        Transform prey = null;
        foreach (KeyValuePair<float, Transform> target in visibleTargets)
        {
            AI ai = target.Value.gameObject.GetComponent<AI>();
            if (ai)
            {
                if (ai.character.type == character.type)
                {
                    float targetWeight = matingPriority / target.Key;
                    if (targetWeight > matingWeight)
                    {
                        mate = target.Value;
                        matingWeight = targetWeight;
                    }
                }
                else
                {
                    if (character.type == Character.FoodType.Herbivore)
                    {
                        Defend(target.Value, 0);
                        return GetFleePosition();
                    }
                    else
                    {
                        float targetWeight = attackPriority / target.Key;
                        if (targetWeight > attackWeight)
                        {
                            prey = target.Value;
                            attackWeight = targetWeight;
                        }
                    }
                }
            }
            else
            {
                Fruit fruit = target.Value.gameObject.GetComponent<Fruit>();
                if (fruit && character.type == Character.FoodType.Herbivore)
                {
                    float targetWeight = attackPriority / target.Key;
                    if (targetWeight > attackWeight)
                    {
                        prey = target.Value;
                        attackWeight = targetWeight;
                    }
                }
            }
        }

        if (!mate && !prey)
        {
            if (seenTarget > 0)
            {
                return lastTargetPosition;
            }

            Vector3 randomPosition = transform.position + DirectionFromAngle(Vector3.Angle(transform.forward, new Vector3(0, 0, 1)) + Random.Range(-character.changeRange, character.changeRange), true).normalized;
            if (Random.Range(0, 10) < character.changeChance)
            {
                randomPosition *= 10;
            }
            randomPosition.y = 0.5f;

            return randomPosition;
        }

        seenTarget = character.persistence;
        if (Random.Range(0, attackWeight + matingWeight) >= attackWeight && mate)
        {
            sawMate = Time.time;
            lastTargetPosition = mate.position;
            return mate.position;
        }

        lastTargetPosition = prey.position;
        return prey.position;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // From video https://www.youtube.com/watch?v=rQG9aUWarwE
    void FindVisibleTargets()
    {
        SortedList<float, Transform> newTargets = new SortedList<float, Transform>();
        // visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, character.viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            if (target.transform.root == transform || (target.parent && target.parent.transform.root == transform))
            {
                continue;
            }
            if (IsVisible(target))
            {
                newTargets.Add(Vector3.Distance(target.position, transform.position), target);
            }
        }

        visibleTargets = newTargets;
    }

    float GetAttackPriority()
    {
        if (!character.CanMate())
        {
            return 0;
        }

        return (character.health.current / character.health.max) / character.aggression;
    }

    public Transform GetAttackTarget()
    {
        Collider[] targetsInAttackRadius = Physics.OverlapSphere(transform.position, character.attackRadius, targetMask);

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
                if (character.type == Character.FoodType.Herbivore)
                {
                    Fruit ts = target.gameObject.GetComponent<Fruit>();
                    if (ts)
                    {
                        return target;
                    }
                }
                else
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

    Vector3 GetFleePosition()
    {
        Vector3 targetDirection = (transform.position - attacker.position).normalized;
        Vector3 targetPosition = transform.position + (targetDirection * 1000);
        targetPosition.y = 0.5f;

        return targetPosition;
    }

    float GetMatingPriority()
    {
        if (!character.CanMate())
        {
            return float.MaxValue;
        }

        if (followCallTimer > 0)
        {
            return 0;
        }

        return ((character.health.max - character.health.current) / character.health.max) * character.aggression;
    }

    public Character GetMatingTarget()
    {
        if (!character.CanMate())
        {
            return null;
        }

        Collider[] targetsInMatingRadius = Physics.OverlapSphere(transform.position, character.matingRadius, targetMask);
        foreach (Collider targetCollider in targetsInMatingRadius)
        {
            Transform target = targetCollider.transform;
            // No masturbating
            if (target.transform.root == transform || (target.parent && target.parent.transform.root == transform))
            {
                continue;
            }

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
            {
                AI ts = target.gameObject.GetComponent<AI>();
                if (ts && ts.WantsToMate(this))
                {
                    return ts.character;
                }
            }
        }

        return null;
    }

    public void HearCall(Vector3 position)
    {
        if (character.CanMate() && followCallTimer <= 0)
        {
            character.ShowMessage("*WOOSH*");
            followCallTimer = character.persistence * 2;
            callPosition = position;
        }
        
    }

    bool IsVisible(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) < character.viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
            {
                return true;
            }
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        sawMate = Time.time;

        lastTargetPosition = new Vector3(transform.position.x, 0, transform.position.z);
        character = gameObject.GetComponent<Character>();

        StartCoroutine("FindTargetsWithDelayRoutine", 0.2f);
        StartCoroutine("CalculateActionRoutine");
    }

    // Update is called once per frame
    void Update()
    {
        if (fleeTime > 0)
        {
            fleeTime -= Time.deltaTime;
        } else
        {
            fleeTime = 0;
            attacker = null;
        }

        if (followCallTimer > 0)
        {
            followCallTimer -= Time.deltaTime;
        }

        if (seenTarget > 0)
        {
            seenTarget -= Time.deltaTime;
        }
    }

    public bool WantsToMate(AI target)
    {
        return character.type == target.character.type && character.CanMate();
    }
}
