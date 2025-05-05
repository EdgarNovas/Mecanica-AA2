using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    public float mass = 1f;
    public Vector3 velocity;
    public float angularVelocity = 0f;
    public float friction = 0.1f;
    public float airResistance = 0.01f;
    public float gravity = -9.81f;
    
    public float elasticity = 0.5f;

    private SphereColliderCustom sphereCollider;
    WallCollider[] walls;

    //Ballistic Collision
    private Vector3 groundNormalVector;
    private float collisionFactor;

    void Start()
    {
        sphereCollider = GetComponent<SphereColliderCustom>();
        walls = FindObjectsOfType<WallCollider>();
    }

    void FixedUpdate()
    {
        // Detectar todas las paredes en la escena
       

        foreach (var wallObj in walls)
        {
            Wall wall = wallObj.ToWall();
            if (sphereCollider.CollidesWith(wall, out Vector3 normal, out Vector3 contactPoint, out float penetration))
            {
                float vDotN = Vector3.Dot(velocity, normal);
                if (vDotN < 0)
                {
                    velocity -= (1 + elasticity) * vDotN * normal;
                }

                transform.position += normal * penetration;
            }
        }

        // Movimiento
        transform.position += velocity * Time.fixedDeltaTime;
    }

    //Ballistic Collision
    (Vector3, Vector3) CheckGroundCollision(Vector3 newPos, Vector3 oldPos, Vector3 newVel, Vector3 oldVel)
    {

        float oldDot = Vector3.Dot(oldPos, groundNormalVector);
        float newDot = Vector3.Dot(newPos, groundNormalVector);

        if (oldDot * newDot < 0)
        {

            float velocityDot = Vector3.Dot(newVel, groundNormalVector);
            Vector3 reflectedVelocity = newVel - (1 + collisionFactor) * velocityDot * groundNormalVector;

            Vector3 correctedPosition = newPos - (1 + collisionFactor) * newDot * groundNormalVector;
            correctedPosition += 0.01f * groundNormalVector;

            return (correctedPosition, reflectedVelocity);


        }
        else
        {
            return (newPos, newVel);
        }



    }

}