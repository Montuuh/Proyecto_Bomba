using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MovementType
{
    RANDOM,
    HORIZONTAL,
    VERTICAL
}
public class BackgroundBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private RectTransform rtransform;

    public float timerTime = 0f;

    private Vector2 initPos;
    private Vector2 currentPos;

    public float forceMin;
    public float forceMax;

    public float timeForceMin;
    public float timeForceMax;

    private float rotSpeed;
    public float rotSpeedMin;
    public float rotSpeedMax;

    public float mass;

    public float horizontalSpeed;
    public bool isHorizontalPositive;

    private bool triggerOnce = false;

    private CameraShaker shaker;

    public Sprite normalBomb;
    public Sprite explodedBomb;

    public GameObject backgroundBomb;

    private Image image;

    public MovementType movementType;

    private void Start()
    {
        rtransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (movementType == MovementType.RANDOM)
        {
            timerTime = 0f;
            rb = GetComponent<Rigidbody2D>();
            rb.mass = mass;
            shaker = GameObject.Find("Shaker").GetComponent<CameraShaker>();
        }
        else
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(this.GetComponent<Rigidbody2D>());
        }

        initPos = rtransform.position;
    }

    // Update is called once per frame
    void Update()
    {

        switch (movementType)
        {
            case MovementType.RANDOM:

                if (shaker.active)
                {
                    triggerOnce = true;
                    image.sprite = explodedBomb;
                }
                else
                {
                    if (triggerOnce)
                    {
                        rtransform.position = initPos;
                        rb.velocity = Vector2.zero;
                        rb.angularVelocity = 0f;
                        triggerOnce = false;
                    }

                    image.sprite = normalBomb;
                }


                if (timerTime > 0f)
                {
                    timerTime -= Time.deltaTime;

                }
                else
                {
                    Vector2 direction = new Vector2((float)Random.Range(transform.position.x - 180, transform.position.x + 180), (float)Random.Range(transform.position.y, transform.position.y + 180));
                    float force = (float)Random.Range(forceMin, forceMax);
                    rb.AddForce(direction * force);

                    rb.angularVelocity = 0f;

                    rotSpeed = (float)Random.Range(rotSpeedMin, rotSpeedMax);

                    rb.AddTorque(force * rotSpeed);


                    timerTime = Random.Range(timeForceMin, timeForceMax);
                }
                break;
            case MovementType.HORIZONTAL:
                if (timerTime > 0)
                {
                    timerTime -= Time.deltaTime;
                    if(isHorizontalPositive)
                        currentPos.x = rtransform.position.x + horizontalSpeed * Time.deltaTime;
                    else
                        currentPos.x = rtransform.position.x - horizontalSpeed * Time.deltaTime;

                    currentPos.y = rtransform.position.y;
                    rtransform.position = currentPos;
                } else {
                    Destroy(this.gameObject);
                }
                break;
            case MovementType.VERTICAL:
                initPos.y = rtransform.position.y + horizontalSpeed * Time.deltaTime;
                initPos.x = rtransform.position.x;

                rtransform.position = initPos;
                break;
            default:
                break;
        }

    }

    private void OnBecameInvisible()
    {
        switch (movementType)
        {
            case MovementType.RANDOM:
                rtransform.position = initPos;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                break;
            case MovementType.HORIZONTAL:
                Destroy(gameObject);
                break;
            case MovementType.VERTICAL:
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}