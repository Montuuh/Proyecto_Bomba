using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundChange : MonoBehaviour
{
    public Image background;
    private Color32 imageColorToBeUsed;

    // How long it takes to return to the same color
    public float periodTime = 10f;

    private float intervalTime = 0f;
    private int interval = 1;
    private float counter = 0f;

    // Initial color
    public int maxColor = 132;
    public int minColor = 83;
    private float difference;

    // Initial values for the color change
    void Start()
    {
        imageColorToBeUsed = new Color(maxColor, minColor, minColor);
        difference = maxColor - minColor;

        intervalTime = periodTime / 6;
        background.color = imageColorToBeUsed;
    }

    // Changes color smothly
    void Update()
    {
        if(counter < intervalTime)
        {
            int currentColorNum = 0;

            // 6 intervals of the RGB wheel to apply gradient
            switch (interval)
            {
                case 1:
                    currentColorNum = (int)(minColor + difference * counter / intervalTime);
                    imageColorToBeUsed.g = (byte)currentColorNum;
                    imageColorToBeUsed.r = (byte)maxColor;
                    imageColorToBeUsed.b = (byte)minColor;
                    break;
                case 2:
                    currentColorNum = (int)(maxColor - difference * counter / intervalTime);
                    imageColorToBeUsed.r = (byte)currentColorNum;
                    imageColorToBeUsed.g = (byte)maxColor;
                    imageColorToBeUsed.b = (byte)minColor;
                    break;
                case 3:
                    currentColorNum = (int)(minColor + difference * counter / intervalTime);
                    imageColorToBeUsed.b = (byte)currentColorNum;
                    imageColorToBeUsed.g = (byte)maxColor;
                    imageColorToBeUsed.r = (byte)minColor;
                    break;
                case 4:
                    currentColorNum = (int)(maxColor - difference * counter / intervalTime);
                    imageColorToBeUsed.g = (byte)currentColorNum;
                    imageColorToBeUsed.b = (byte)maxColor;
                    imageColorToBeUsed.r = (byte)minColor;
                    break;
                case 5:
                    currentColorNum = (int)(minColor + difference* counter / intervalTime);
                    imageColorToBeUsed.r = (byte)currentColorNum;
                    imageColorToBeUsed.b = (byte)maxColor;
                    imageColorToBeUsed.g = (byte)minColor;
                    break;
                case 6:
                    currentColorNum = (int)(maxColor - difference * counter / intervalTime);
                    imageColorToBeUsed.b = (byte)currentColorNum;
                    imageColorToBeUsed.r = (byte)maxColor;
                    imageColorToBeUsed.g = (byte)minColor;
                    break;
                default:
                    break;
            }

            background.color = imageColorToBeUsed;

            counter += Time.deltaTime;
        } else
        {
            counter = 0f;
            interval++;
            
            if (interval > 6) interval = 1;
        }
    }
}
