using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerCampfire : MonoBehaviour
{

    public GameObject player;

    public GameObject campfireTemple;
    public int maxCampfires = 2;
    public int currentCount;
    private List<GameObject> campfires = new List<GameObject>();
    private static PlayerCampfire instance;
    private Vector2 playerCoord;
    public static PlayerCampfire Instance { get => instance;}

    private void Awake()
    {
        instance = this;
        currentCount = maxCampfires;
        if(campfireTemple==null)
        {
            campfireTemple = Resources.Load<GameObject>("CampfireTemple");

        }
    }

    private void Update()
    {
        if (InputManager.GetInstance().GetPlacePressed())
        {
            PlaceCampfire();
        }
    }

    private void PlaceCampfire()
    {
        if (currentCount == 0 ) return; // Ограничение по количеству

        playerCoord = new Vector2(
           player.transform.position.x,player.transform.position.y
       );

        Vector2 placePosition = PlayerMovment.GetInctance().GetPlayerPosition();
        //Debug.Log(placePosition.x);

        placePosition.x += playerCoord.x;
        placePosition.y += playerCoord.y;

        GameObject newCampfire = Instantiate(campfireTemple, placePosition, Quaternion.identity);

        campfires.Add(newCampfire);
        Transform lightTransform = newCampfire.transform.Find("Lighting");
        lightTransform.gameObject.SetActive(false);
        if (TimeManger.GetInstance().isLights)
        {
            lightTransform.gameObject.SetActive(true);
        }
        currentCount -= 1;
        if (lightTransform != null)
        {
            TimeManger.GetInstance().RegisterNewLight(lightTransform.gameObject);
        }


        StartCoroutine(DestroyCampfireAfterTime(newCampfire, newCampfire.GetComponent<Campfire>().maxLifetime));
    }

    private IEnumerator DestroyCampfireAfterTime(GameObject campfire, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        if (campfire != null)
        {
            campfires.Remove(campfire); // Удаляем из списка перед удалением
            Destroy(campfire);
        }
    }

    public void RemoveCampfire(GameObject campfire)
    {
        if (campfires.Contains(campfire))
        {
            campfires.Remove(campfire);
            Destroy(campfire);
        }
    }

    public void ResetCampfireCount()
    {
        currentCount = maxCampfires;
    }
}
