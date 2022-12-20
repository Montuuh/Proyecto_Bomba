using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClearBehaviour : MonoBehaviour
{

  
    private int[] directions = new int[] { -1, 1};
    private int rotDirection = 0;

    private float rotSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 direction = new Vector2((float)Random.Range(transform.position.x - 180, transform.position.x + 180), (float)Random.Range(transform.position.y, transform.position.y + 180));

        float force = (float)Random.Range(1, 7.5f);
        gameObject.GetComponent<Rigidbody2D>().AddForce(direction * force);

        rotDirection = directions[Random.Range(0, directions.Length)];
        rotSpeed = (float)Random.Range(10, 15);
    }

    void FixedUpdate()
    {
        gameObject.GetComponent<Rigidbody2D>().rotation += rotSpeed;
        gameObject.GetComponent<Rigidbody2D>().rotation *= rotDirection;

    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
