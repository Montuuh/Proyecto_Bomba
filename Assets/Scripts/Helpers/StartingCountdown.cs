using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingCountdown : MonoBehaviour
{
    public List<GameObject> numbers;

    // First 3, 2, 1 countdown at start of game
    public void SendNumber(int index, float x, float y)
    {
        GameObject tmpObj = Instantiate(numbers[index]);

        tmpObj.transform.SetParent(GameObject.Find("Grid").transform, false);

        Vector3 position = new Vector3(x, y, 0);
        tmpObj.transform.position = position;
    }
}
