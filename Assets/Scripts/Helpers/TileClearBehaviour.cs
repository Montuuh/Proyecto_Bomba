using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClearBehaviour : MonoBehaviour
{

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float rotSpeed = 0;


    public bool makeItRainInBackground;
    public Sprite[] cellColors;
    public ColorPlayer color;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Vector2 direction = new Vector2((float)Random.Range(transform.position.x - 180, transform.position.x + 180), (float)Random.Range(transform.position.y, transform.position.y + 180));
        float force = (float)Random.Range(1, 7.5f);
        rb.AddForce(direction * force);


        rotSpeed = (float)Random.Range(-25, 25);

        rb.AddTorque(force * rotSpeed);

        if(makeItRainInBackground)
            spriteRenderer.sprite = cellColors[0];
    }

    void OnBecameInvisible()
    {
        if(makeItRainInBackground)
        {
            spriteRenderer.sortingOrder = 0;

            float spawnY =  Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height/2)).y;
            float spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width/3, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width*2/3, 0)).x);
            Vector2 spawnPosition = new Vector2(spawnX, spawnY);
            gameObject.transform.position = spawnPosition;

            rb.velocity = new Vector2(0f, 0f);

            Vector2 direction = new Vector2((float)Random.Range(transform.position.x - 180, transform.position.x + 180), (float)Random.Range(transform.position.y, transform.position.y + 180));
            float force = (float)Random.Range(2f, 7f);
            rb.AddForce(direction * force);

            rb.angularDrag = 0f;
            rb.AddTorque(force * rotSpeed);

            rb.gravityScale = Random.Range(0.5f, 1.5f);

            switch (color)
            {
                case ColorPlayer.NONE:
                    Destroy(gameObject);
                    break;
                case ColorPlayer.RED:
                    spriteRenderer.sprite = cellColors[1];
                    break;
                case ColorPlayer.BLUE:
                    spriteRenderer.sprite = cellColors[2];
                    break;
                case ColorPlayer.GREEN:
                    spriteRenderer.sprite = cellColors[3];
                    break;
                case ColorPlayer.YELLOW:
                    spriteRenderer.sprite = cellColors[4];
                    break;
                default:
                    break;
            }
        } else
        {
            Destroy(gameObject);
        }
    }
}
