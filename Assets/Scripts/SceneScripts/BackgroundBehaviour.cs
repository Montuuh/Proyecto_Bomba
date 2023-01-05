using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private RectTransform rtransform;

    private float addForceTime = 0.3f;

    private Vector2 initPos;

    public float forceMin;
    public float forceMax;

    public float timeForceMin;
    public float timeForceMax;

    private float rotSpeed;
    public float rotSpeedMin;
    public float rotSpeedMax;

    public float mass;

    private bool explosionOnce = false;

    private CameraShaker shaker;

    public Sprite normalBomb;
    public Sprite explodedBomb;

    private Image image;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rtransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        shaker = GameObject.Find("Shaker").GetComponent<CameraShaker>();


        initPos = rtransform.localPosition;

        rb.mass = mass;

        Vector2 direction = new Vector2((float)Random.Range(transform.position.x - 180, transform.position.x + 180), (float)Random.Range(transform.position.y - 180, transform.position.y + 180));
        float force = (float)Random.Range(forceMin, forceMax);
        rb.AddForce(direction * force);


        rotSpeed = (float)Random.Range(rotSpeedMin, rotSpeedMax);

        rb.AddTorque(force * rotSpeed);
    }

    // Update is called once per frame
    void Update()
    {

        if (shaker.active)
        {
            explosionOnce = true;
            image.sprite = explodedBomb;
        }
        else
        {
            if(explosionOnce)
            {
                rtransform.localPosition = initPos;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                explosionOnce = false;
            }

            image.sprite = normalBomb;
        }

        if(addForceTime > 0f)
        {
            addForceTime -= Time.deltaTime;

        } else
        {
            Vector2 direction = new Vector2((float)Random.Range(transform.position.x - 180, transform.position.x + 180), (float)Random.Range(transform.position.y, transform.position.y + 180));
            float force = (float)Random.Range(forceMin, forceMax);
            rb.AddForce(direction * force);

            rb.angularVelocity = 0f;

            rotSpeed = (float)Random.Range(rotSpeedMin, rotSpeedMax);

            rb.AddTorque(force * rotSpeed);


            addForceTime = Random.Range(timeForceMin, timeForceMax);
        }
    }

    private void OnBecameInvisible()
    {
        rtransform.localPosition = initPos;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
