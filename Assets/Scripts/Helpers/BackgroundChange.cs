using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundChange : MonoBehaviour
{
    public Image background;
    private Color32 imageColorToBeUsed = new Color(255f, 177f, 177f);

    public float periodTime = 10f;
    private float intervalTime = 0f;
    private int interval = 1;
    private float counter = 0f;

    public int maxColor = 255;
    public int minColor = 177;

    // Start is called before the first frame update
    void Start()
    {
        intervalTime = periodTime / 6;
        background.color = imageColorToBeUsed;
    }

    // Update is called once per frame
    void Update()
    {
        if(counter < intervalTime)
        {
            int currentColorNum = 0;

            switch (interval)
            {
                case 1:
                    currentColorNum = (int)(minColor + 78 * counter / intervalTime);
                    imageColorToBeUsed.g = (byte)currentColorNum;
                    imageColorToBeUsed.r = (byte)maxColor;
                    imageColorToBeUsed.b = (byte)minColor;
                    break;
                case 2:
                    currentColorNum = (int)(maxColor - 78f * counter / intervalTime);
                    imageColorToBeUsed.r = (byte)currentColorNum;
                    imageColorToBeUsed.g = (byte)maxColor;
                    imageColorToBeUsed.b = (byte)minColor;
                    break;
                case 3:
                    currentColorNum = (int)(minColor + 78f * counter / intervalTime);
                    imageColorToBeUsed.b = (byte)currentColorNum;
                    imageColorToBeUsed.g = (byte)maxColor;
                    imageColorToBeUsed.r = (byte)minColor;
                    break;
                case 4:
                    currentColorNum = (int)(maxColor - 78f * counter / intervalTime);
                    imageColorToBeUsed.g = (byte)currentColorNum;
                    imageColorToBeUsed.b = (byte)maxColor;
                    imageColorToBeUsed.r = (byte)minColor;
                    break;
                case 5:
                    currentColorNum = (int)(minColor + 78f * counter / intervalTime);
                    imageColorToBeUsed.r = (byte)currentColorNum;
                    imageColorToBeUsed.b = (byte)maxColor;
                    imageColorToBeUsed.g = (byte)minColor;
                    break;
                case 6:
                    currentColorNum = (int)(maxColor - 78f * counter / intervalTime);
                    imageColorToBeUsed.b = (byte)currentColorNum;
                    imageColorToBeUsed.r = (byte)maxColor;
                    imageColorToBeUsed.g = (byte)minColor;
                    break;
                default:
                    break;
            }

            //Debug.Log("Red: " + imageColorToBeUsed.r + "    Green: " + imageColorToBeUsed.g + "    Blue: " + imageColorToBeUsed.b);

            background.color = imageColorToBeUsed;

            counter += Time.deltaTime;
        } else
        {
            counter = 0f;
            interval++;
            
            //Debug.Log("Change");

            if (interval > 6) interval = 1;
        }
    }
}
