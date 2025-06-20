using UnityEngine;
using System.Collections;
public class BulletRain : MonoBehaviour
{
    public float wait = 0.5f;
    public float startSpeed = 3f;
    public float endSpeed = 20f;
    public float duration = 0.5f;
    public float damage = 10f;
    public Vector3 dir;
    private Animator animator;
    private float timer = 0f;
    private bool isFrozen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.CrossFade("BulletRain", 0.1f);

        TimeManger.OnTimeStop += Freeze;
        TimeManger.OnTimeResume += Unfreeze;
    }

    public void BulletStart(Vector3 vector3)
    {
        dir = vector3;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(wait);
        while (true)
        {
            if (!isFrozen)
            {
                float t = Mathf.Clamp01(timer / duration);
                float currentSpeed = Mathf.Lerp(startSpeed, endSpeed, t);
                transform.position += dir * currentSpeed * Time.deltaTime;
                timer += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.GetDamage(damage);
            Destroy(gameObject);
        }
        else if(other.CompareTag("Tile"))
        {
            Destroy(gameObject);
        }
    }

    public void Freeze()
    {
        isFrozen = true;
        if (animator != null)
            animator.speed = 0f;
    }

    public void Unfreeze()
    {
        isFrozen = false;
        if (animator != null)
            animator.speed = 1f;
    }

    private void OnDestroy()
    {
        TimeManger.OnTimeStop -= Freeze;
        TimeManger.OnTimeResume -= Unfreeze;
    }
}
