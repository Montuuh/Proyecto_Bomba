using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Health : MonoBehaviour
{

    private HealthManager healthManager;
    public List<GameObject> heartsList;

    private GameObject go;
    private RectTransform rTransform;
    void Start()
    {
    }

    // Displays current scores of players
    void Update()
    {
        healthManager = GameObject.Find("ClientManager").GetComponent<HealthManager>();

        List<ClientData> healthUI = healthManager.currentPlayingPlayers;

        foreach (Transform child in gameObject.transform) {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < healthUI.Count; i++)
        {
            for (int j = 0; j < healthUI[i].health; j++)
            {
                switch (healthUI[i].colorPlayer)
                {
                    case ColorPlayer.NONE:
                        break;
                    case ColorPlayer.RED:
                        go = GameObject.Instantiate(heartsList[0]);
                        go.transform.SetParent(this.gameObject.transform, false);
                        rTransform = go.GetComponent<RectTransform>();
                        rTransform.localPosition = new Vector3(840 - 125 * j, 400 - 125 * i, 0);

                        break;
                    case ColorPlayer.GREEN:
                        go = GameObject.Instantiate(heartsList[1]);
                        go.transform.SetParent(this.gameObject.transform, false);
                        rTransform = go.GetComponent<RectTransform>();
                        rTransform.localPosition = new Vector3(840 - 125 * j, 400 - 125 * i, 0);
                        break;
                    case ColorPlayer.BLUE:
                        go = GameObject.Instantiate(heartsList[2]);
                        go.transform.SetParent(this.gameObject.transform, false);

                        rTransform = go.GetComponent<RectTransform>();
                        rTransform.localPosition = new Vector3(840 - 125 * j, 400 - 125 * i, 0);
                        break;
                    case ColorPlayer.YELLOW:
                        go = GameObject.Instantiate(heartsList[3]);
                        go.transform.SetParent(this.gameObject.transform, false);
                        rTransform = go.GetComponent<RectTransform>();
                        rTransform.localPosition = new Vector3(840 - 125 * j, 400 - 125 * i, 0);
                        break;
                    default:
                        break;
                }
            }
        }
        

    }

    private int SortByScore(ClientData c1, ClientData c2)
    {
        return c2.score.CompareTo(c1.score);
    }
}
