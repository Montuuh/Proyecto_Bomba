using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacgroundSpawner : MonoBehaviour
{

    public float spawnTime = 1f;
    public bool isHorizontalPositive;

    private float spawnCounter;

    public GameObject objectToSpawn;

    private void Start()
    {
        spawnCounter = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
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
