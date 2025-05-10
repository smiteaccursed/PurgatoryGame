using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : MonoBehaviour
{

    public GameObject visualCue;

    private bool playerInRange;

    void Start()
    {
        visualCue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {
            visualCue.SetActive(true);
            if(InputManager.GetInstance().GetInteractPressed())
            {
                PlayerData pd = SaveSystem.Instance.GetAllData();
                SaveSystem.Instance.SaveGame(pd);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
