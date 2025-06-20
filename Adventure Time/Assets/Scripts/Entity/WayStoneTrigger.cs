using UnityEngine;
using Cainos.PixelArtTopDown_Basic;
public class WayStoneTrigger : MonoBehaviour
{
    public GameObject visualCue;

    private bool playerInRange;

    void Start()
    {
        visualCue.SetActive(false);
    }

    void Update()
    {
        if (playerInRange)
        {
            visualCue.SetActive(true);
            if (InputManager.GetInstance().GetInteractPressed())
            {
                LoadingController.Instance.canvas.gameObject.SetActive(true);
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                Vector3 pos = playerObj.transform.position;
                pos.x = 0;
                pos.y = 0;
                playerObj.transform.position = pos;
                CameraFollow.Instance.SetCameraOnPlayer();
                LoadingController.Instance.canvas.gameObject.SetActive(false);
            }
        }
        else
        {
            visualCue.SetActive(false);
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
