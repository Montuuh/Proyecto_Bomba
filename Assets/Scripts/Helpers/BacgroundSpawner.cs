using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacgroundSpawner : MonoBehaviour
{

    // + Right, - Left -> direction of movement
    public bool isHorizontalPositive;

    public float spawnTime = 1f;
    private float spawnCounter;

    public GameObject objectToSpawn;

    private void Start()
    {
        spawnCounter = spawnTime;
    }

    void Update()
    {
        // Keep instantiating bombs periodically
        if(spawnCounter > 0)
        {
            spawnCounter -= Time.deltaTime;
        } else
        {
            GameObject temp = Instantiate(objectToSpawn);
            temp.transform.SetParent(gameObject.transform);

            temp.GetComponent<RectTransform>().position = gameObject.GetComponent<RectTransform>().position;
            temp.GetComponent<BackgroundBehaviour>().movementType = MovementType.HORIZONTAL;

            if (isHorizontalPositive)
                temp.GetComponent<BackgroundBehaviour>().isHorizontalPositive = true;
            else
                temp.GetComponent<BackgroundBehaviour>().isHorizontalPositive = false;

            spawnCounter = spawnTime;
        }
    }
}
