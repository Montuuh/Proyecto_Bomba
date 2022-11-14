using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    Server server;

    private void Start()
    {
        server = gameObject.GetComponent<Server>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            SceneManager.LoadScene("Jovani");
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SceneManager.LoadScene("Client");
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    server.SendStringToAll("Test");
        //}
    }
}
