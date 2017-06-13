using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidsManager : MonoBehaviour
{
    public int totalFlyBoids;
    public GameObject prefabFlyBoid;
    public GameObject parentFlyBoid;
    public List<GameObject> flyBoids;

    public int totalGroundBoids;
    public GameObject prefabGroundBoid;
    public GameObject parentGroundBoid;
    public List<GameObject> groundBoids;

    public int totalChaseeBoids;
    public GameObject prefabChaseeBoid;
    public GameObject parentChaseeBoid;
    public List<GameObject> chaseeBoids;

    public int totalPredBoids;
    public GameObject prefabPredBoid;
    public GameObject parentPredBoid;
    public List<GameObject> predBoids;

    public bool init;
    public float spawnRadius = 100f;
    public Vector3 bounds;
  
    public Vector3 centerOfMassFly;
    public Vector3 centerOfMassGround;

    public Material matGroundBoid;
    public Material matChaseeBoid;
    public Material matPredBoid;

    void Start()
    {
        init = false;

        centerOfMassFly = Vector3.zero;
        centerOfMassGround = Vector3.zero;


        Init("Fly", prefabFlyBoid, ref flyBoids, totalFlyBoids);
        Init("Ground", prefabGroundBoid, ref groundBoids, totalGroundBoids);
        Init("Chasee", prefabChaseeBoid, ref chaseeBoids, totalChaseeBoids);
        Init("Predators", prefabPredBoid, ref predBoids, totalPredBoids);

        PlaceBoids("Fly", prefabFlyBoid, parentFlyBoid, totalFlyBoids);
        PlaceBoids("Ground", prefabGroundBoid, parentGroundBoid, totalGroundBoids);
        PlaceBoids("Chasee", prefabChaseeBoid, parentChaseeBoid, totalChaseeBoids);
        PlaceBoids("Predators", prefabPredBoid, parentPredBoid, totalPredBoids);

        init = true;
    }

    void Init(string type, GameObject prefab, ref List<GameObject> boids, int totalBoid)
    {
        if (prefab == null)
        {
            Debug.Log("Please assign a " + type + " boid prefab");
            return;
        }
        boids = new List<GameObject>();
    }

    void PlaceBoids(string type, GameObject prefab, GameObject parent, int totalBoid)
    {
        for (int i = 0; i < totalBoid; i++)
        {
            GameObject boid = Instantiate(prefab);
            boid.tag = type;

           
            Vector2 pos = new Vector2(parent.transform.position.x, parent.transform.position.z) + Random.insideUnitCircle * spawnRadius;
            if(type == "Fly")
                 boid.transform.position = new Vector3(pos.x, Random.Range(parent.transform.position.y - 2, parent.transform.position.y + 2) , pos.y);
            else
                boid.transform.position = new Vector3(pos.x, parent.transform.position.y, pos.y);
            boid.transform.parent = parent.transform;

            BoidBehaviour boidBehaviour = boid.GetComponent<BoidBehaviour>();
            boidBehaviour.boidManager = this;
            boidBehaviour.boidsCenter = parent.transform.position;
            if (type == "Fly")
            {
                boidBehaviour.boids = this.flyBoids;
                flyBoids.Add(boid);
            }
              
            else if (type == "Ground")
            {
                boidBehaviour.boids = this.groundBoids;
                groundBoids.Add(boid);
                boid.GetComponentInChildren<Renderer>().material = matGroundBoid;
            }

            else if (type == "Chasee")
            {
                boidBehaviour.boids = this.chaseeBoids;
                chaseeBoids.Add(boid);
                boidBehaviour.predBoids = this.predBoids;
                boid.GetComponentInChildren<Renderer>().material = matChaseeBoid;
            }

            else if (type == "Predators")
            {
                boidBehaviour.boids = this.predBoids;
                predBoids.Add(boid);
                boidBehaviour.chaseeBoids = this.chaseeBoids;
                boid.GetComponentInChildren<Renderer>().material = matPredBoid;
            }
        }
    }

    void Update()
    {
        UpdateCenterOfMass(totalFlyBoids, flyBoids, centerOfMassFly);
        UpdateCenterOfMass(totalGroundBoids, groundBoids, centerOfMassGround);
    }

    void UpdateCenterOfMass(int totalBoids, List<GameObject> boids, Vector3 centerOfMass)
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i< totalBoids; i++)
        {
            center += boids[i].transform.position;
        }
        center /= totalBoids;
        if(centerOfMass != center)
            centerOfMass = center;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(parentFlyBoid.transform.position, new Vector3(bounds.x, bounds.y, bounds.z));
        Gizmos.DrawWireSphere(parentFlyBoid.transform.position, spawnRadius);
    }
}

