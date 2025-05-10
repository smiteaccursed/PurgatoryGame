using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] public GameObject visualCue;

    public PassiveAbility passiveAbility;
    public ActiveAbility activeAbility;
    public GameObject Icon;
    private bool playerInRange;


    private void Start()
    {
        SpriteRenderer sr = Icon.GetComponent<SpriteRenderer>();
        PlayerAbilities pa = GetComponent<PlayerAbilities>();
        if (passiveAbility != null)
        {
            sr.sprite = passiveAbility.icon;
            Debug.Log("абилка стоит");
        }
        else if (activeAbility != null)
        {
            sr.sprite = activeAbility.icon;
            Debug.Log("абилка стоит");
        }
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if(playerInRange)
        {
            visualCue.SetActive(true);
            if (InputManager.GetInstance().GetInteractPressed())
            {
                PlayerAbilities pa = FindObjectOfType<PlayerAbilities>();
                DataManager.Instance.abilityOnFloor.RemoveAll(abilityData => abilityData.name == transform.name);
                if (passiveAbility != null)
                {
                    pa.AddPassive(passiveAbility);
                    Destroy(gameObject);
                    return;
                }
                else if (activeAbility != null)
                {
                    pa.SetActiveAbility(activeAbility);
                    Destroy(gameObject);
                    return;
                }
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
