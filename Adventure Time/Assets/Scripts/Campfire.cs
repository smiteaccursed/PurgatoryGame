using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Campfire : MonoBehaviour
{
    public float maxLifetime = 3f; 
    public float currentLifetime;
    private Light2D light2D;
    private SpriteRenderer fireSprite;

    void Start()
    {
        currentLifetime = maxLifetime;
        light2D = GetComponentInChildren<Light2D>();
        fireSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        currentLifetime -= Time.deltaTime;
        //if (light2D != null)
        //{
        //    light2D.intensity = Mathf.Lerp(0, 1, lifePercent); // яркость света
        //    light2D.pointLightOuterRadius = Mathf.Lerp(0, 3, lifePercent); // Радиус света
        //}

        //if (fireSprite != null)
        //{
        //    fireSprite.color = new Color(1f, 1f, 1f, lifePercent); // Костёр становится прозрачнее
        //}

        if (currentLifetime <= 0)
        {
            Destroy(gameObject); // Удаляем костёр
        }
    }
}
