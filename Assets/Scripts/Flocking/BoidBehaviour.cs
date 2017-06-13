using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidBehaviour : MonoBehaviour
{
    public BoidsManager boidManager;

    public Vector3 boidsCenter;
    public List<GameObject> boids;
    public List<GameObject> predBoids;
    public List<GameObject> chaseeBoids;

    // the overall speed of the simulation
    public float speed;
    // max speed any particular drone can move at
    public float maxSpeed;
    // maximum steering power
    public float maxSteer = .05f;

    // weights: used to modify the drone's movement
    public float separationWeight = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;

    public float neighborRadius = 50f;
    public float avoidRadius = 20f;
    public float chasedRadius = 30f;
    public float desiredSeparation = 6f;
    public float eatRadius = 2.0f;

    // velocity influences
    private Vector3 _separation;
    private Vector3 _alignment;
    private Vector3 _cohesion;
    private Vector3 _bounds;
    private Vector3 _chase;
    private Vector3 _evade;

    private Vector3 _direction;
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        if (boidManager.init)
            Flock();
    }

    void FixedUpdate ()
    {
        Vector3 oldRot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0.0f, oldRot.y, 0.0f);
    }

    public virtual void Flock()
    {
        Vector3 newVelocity = Vector3.zero;

        CalculateVelocities();
        CalculateBounds();
        FaceTowardsHeading();

        newVelocity += _separation * separationWeight;
        newVelocity += _alignment * alignmentWeight;
        newVelocity += _cohesion * cohesionWeight;

        newVelocity += _bounds;
        newVelocity = newVelocity * speed;


        if (gameObject.tag == "Chasee")
            Evade();
        if (_evade != Vector3.zero)
        {
            newVelocity += _evade;
            newVelocity += _bounds;

            float speedChase = speed * 5;

            maxSpeed = Random.Range(speedChase, speedChase + 10);
        }
        if(gameObject.tag == "Predators")
            Chase();
        if (_chase != Vector3.zero)
        {
            newVelocity += _chase;
            newVelocity += _bounds;
            float speedPred = speed * 3;
            maxSpeed = Random.Range(speedPred, speedPred + 10);
        }

        if (gameObject.tag != "Fly") 
            newVelocity.y = 0;
       
        if(newVelocity == Vector3.zero)
        {
            _direction = MoveTowardRandBoid();
            newVelocity = _direction * 60;
        }

        newVelocity = _rigidbody.velocity + newVelocity;
        _rigidbody.AddForce(Limit(newVelocity, maxSpeed) - _rigidbody.velocity, ForceMode.VelocityChange);
    }

    Vector3 MoveTowardRandBoid()
    {
        Vector3 direction = Vector3.zero;
        foreach (var boid in boids)
        {
            if (boid == null) continue;

            if (boid.gameObject != gameObject)
            {
                direction = boid.transform.position - transform.position;
                direction.Normalize();
                break;
            }
        }
        return direction;
    }

    protected void CalculateVelocities()
    {
        Vector3 separationSum = Vector3.zero;
        Vector3 alignmentSum = Vector3.zero;
        Vector3 cohesionSum = Vector3.zero;
    
        int separationCount = 0;
        int alignmentCount = 0;
        int cohesionCount = 0;

        foreach (var boid in boids)
        {
            if (boid == null) continue;
            if (boid.gameObject != gameObject)
            {
                //separation
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance < desiredSeparation)
                {
                    // calculate vector headed away from myself
                    Vector3 direction = transform.position - boid.transform.position;
                    direction.Normalize();
                    direction = direction / distance; // weight by distance
                    separationSum += direction;
                    separationCount++;
                }

                // alignment & cohesion
                if (distance < neighborRadius)
                {
                    alignmentSum += boid.GetComponent<Rigidbody>().velocity;
                    alignmentCount++;

                    cohesionSum += boid.transform.position;
                    cohesionCount++;
                }
            }
        }

        // end
        _separation = separationCount > 0 ? separationSum / separationCount : separationSum;
        _alignment = alignmentCount > 0 ? Limit(alignmentSum / alignmentCount, maxSteer) : alignmentSum;
        _cohesion = cohesionCount > 0 ? Steer(cohesionSum / cohesionCount, false) : cohesionSum;
    }

    private void CalculateBounds()
    {
        Vector3 boundsSum = Vector3.zero;
        int boundsCount = 0;
        if(gameObject.tag == "Fly")
        {
            if (transform.position.x <= -240 || transform.position.x >= 240 || transform.position.y >= 300 || transform.position.y <= 150 || transform.position.z >= 240 || transform.position.z <= -240)
            {
                Vector3 diff = boidsCenter - transform.position;

                if (diff.magnitude > 0)
                {
                    diff.Normalize();
                    boundsSum = diff;
                    boundsCount = 1;
                }
            }
        }
        else
        {
            if (transform.position.x <= -240 || transform.position.x >= 240 || transform.position.z >= 240 || transform.position.z <= -240)
            {
                Vector3 diff = boidsCenter - transform.position;

                if (diff.magnitude > 0)
                {
                    diff.Normalize();
                    boundsSum = diff;
                    boundsCount = 1;
                }
            }
        }
        _bounds = boundsCount > 0 ? boundsSum : Vector3.zero;
    }

    protected Vector3 Steer(Vector3 target, bool slowDown)
    {
        // the steering vector
        Vector3 steer = Vector3.zero;
        Vector3 targetDirection = target - transform.position;
        float targetDistance = targetDirection.magnitude;

        transform.LookAt(target);

        if (targetDistance > 0)
        {
            // move towards the target
            targetDirection.Normalize();

            // we have two options for speed
            if (slowDown && targetDistance < 100f * speed)
            {
                targetDirection *= (maxSpeed * targetDistance / (100f * speed));
                targetDirection *= speed;
            }
            else
                targetDirection *= maxSpeed;

            // set steering vector
            steer = targetDirection - _rigidbody.velocity;
            steer = Limit(steer, maxSteer);
        }

        return steer;
    }

    protected Vector3 Limit(Vector3 v, float max)
    {
        if (v.magnitude > max)
        {
            return v.normalized * max;
        }
        else
        {
            return v;
        }
    }

    void FaceTowardsHeading()
    {
        transform.LookAt(transform.position + GetComponent<Rigidbody>().velocity.normalized, transform.up);
    }

    public void Evade()
    {
        float bestDistance = float.MaxValue;
        Vector3 direction = Vector3.zero;
        foreach (var predBoid in predBoids)
        {
            //distance between current chasee and enemy
            float distance = Vector3.Distance(transform.position, predBoid.transform.position);
            if (distance < bestDistance && distance < avoidRadius)
            {
                //calculate dir away from enemy
                bestDistance = distance;
                Vector3 predPos = predBoid.transform.position;
                direction = transform.position - predPos;
                direction.Normalize();
            }
        }

        _evade = direction;
    }

    public void Chase()
    {
        float bestDistance = float.MaxValue;
        Vector3 direction = Vector3.zero;

        foreach(var chasee in chaseeBoids)
        {
            //distance between current enemy and chasee
            float distance = Vector3.Distance(transform.position, chasee.transform.position);
            if(distance < bestDistance && distance < chasedRadius)
            {
                //calculate dir to chasee
                bestDistance = distance;
                Vector3 chaseePos = chasee.transform.position;
                direction = chaseePos - transform.position;
                direction.Normalize();
            }
            //eat chasee
            if(distance < eatRadius)
            {
                Destroy(chasee.gameObject);
                chaseeBoids.Remove(chasee);
                break;
            }
        }
        _chase = direction;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + _alignment.normalized * neighborRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _separation.normalized * neighborRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + _cohesion.normalized * neighborRadius);
    }
}
