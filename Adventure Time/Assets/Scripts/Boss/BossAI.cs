using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BossAI : MonoBehaviour
{
    public float maxHP = 10000f;
    public float HP = 10000f;
    public Vector3 bossPos;
    public List<int> attackPackege;
    public int choise;
    public float colDown = 1f;
    public int fromCoordX = -14;
    public int toCoordX = 14;
    public int fromCoordY = -7;
    public int toCoordY = 7;
    public int countAttack;
    public int minAttack;
    public List<float> positions = new List<float>{-8f, 0f, 8f};
    public GameObject bulletRain;
    public GameObject bulletShot;
    public GameObject bulletFollow;
    public GameObject fill;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer bossSR;
    private Color bossColor;

    private bool isAttack;
    private bool isFrozen;
    void Start()
    {
        choise = -1;
        TimeManger.OnTimeStop += Freeze;
        TimeManger.OnTimeResume += Unfreeze;
        bossPos = new Vector3(0, 6, 0);
        bossSR = GetComponent<SpriteRenderer>();
        bossColor = bossSR.color;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        StartCoroutine(ChangePosition());
    }

    void Update()
    {
        if (isAttack || isFrozen)
            return;
        switch(choise)
        {
            case 1:
                Debug.Log("Attack 1");
                StartCoroutine(BulletRainHor(Vector3.down, toCoordY));
                break;
            case 2:
                Debug.Log("Attack 2");
                StartCoroutine(BulletRainHor(Vector3.up, fromCoordY));
                break;
            case 3:
                Debug.Log("Attack 3");
                StartCoroutine(BulletRainVer(Vector3.left, toCoordX));
                break;
            case 4:
                Debug.Log("Attack 4");
                StartCoroutine(BulletRainVer(Vector3.right, fromCoordX));
                break;
            case 5:
                Debug.Log("Attack 5");
                StartCoroutine(BulletsShot());
                break;
            case 6:
                Debug.Log("Attack 6");
                StartCoroutine(BulletsFollow());
                break;
            case 7:
                Debug.Log("Teleport");
                StartCoroutine(ChangePosition());
                break;
            case -1:
                Debug.Log("Choising");
                attackPackege = MakePackege();
                MakeChoise();
                break;
        }
    }

    public void GetDamage(float dmg)
    {
        HP -= dmg;
        Vector3 buf = fill.transform.localScale;
        buf.x = HP / maxHP;
        fill.transform.localScale = buf;
        StartCoroutine(ChangeColor());
        if(HP<=0)
        {
            HP = 0;
            buf.x = HP / maxHP;
            fill.transform.localScale = buf;
            GameOverController.Instance.DeathScreen(1);
            Destroy(gameObject);
        }
    }

    IEnumerator ChangeColor()
    {
        bossSR.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        bossSR.color = bossColor;
    }
    IEnumerator BulletsShot()
    {
        isAttack = true;
        Vector3 pos1 = bossPos;
        pos1.x += 2;
        Vector3 pos2 = bossPos;
        pos2.x -= 2;
        Vector3 buf;
        for(int i =0; i<6; i++)
        {
            if (i % 2 == 0)
                buf = pos1;
            else
                buf = pos2;
            GameObject bull = Instantiate(bulletShot, buf, Quaternion.identity);
            BulletShot br = bull.GetComponent<BulletShot>();
            br.BulletStart();
            while(isFrozen)
            {
                br.Freeze();
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }

        MakeChoise();
        yield return new WaitForSeconds(colDown);
        isAttack = false;

    }

    IEnumerator BulletsFollow()
    {
        isAttack = true;
        Vector3 pos1 = bossPos;
        pos1.x += 2;
        Vector3 pos2 = bossPos;
        pos2.x -= 2;
        Vector3 buf;
        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == 0)
            {
                buf = pos1;
            }
            else
            {
                buf = pos2;
            }
            GameObject bull = Instantiate(bulletFollow, buf, Quaternion.identity);
            BulletFollow br = bull.GetComponent<BulletFollow>();
            br.BulletStart();
            while (isFrozen)
            {
                br.Freeze();
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }

        MakeChoise();
        yield return new WaitForSeconds(colDown);
        isAttack = false;
    }

    IEnumerator BulletRainVer(Vector3 dir, int to)
    {
        isAttack = true;
        Vector3 pos = new Vector3(to, fromCoordY);
        for (int i = fromCoordY; i < toCoordY; i++)
        {
            GameObject bull = Instantiate(bulletRain, pos, Quaternion.identity);
            BulletRain br = bull.GetComponent<BulletRain>();
            br.BulletStart(dir);
            pos.y++;
            while (isFrozen)
            {
                br.Freeze();
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
        }
        MakeChoise();
        yield return new WaitForSeconds(colDown);
        isAttack = false;
    }
    IEnumerator BulletRainHor(Vector3 dir, int to)
    {
        isAttack = true;
        Vector3 pos = new Vector3( fromCoordX, to);
        for(int i = fromCoordX; i<toCoordX; i++)
        {
            GameObject bull = Instantiate(bulletRain, pos, Quaternion.identity);
            BulletRain br = bull.GetComponent<BulletRain>();
            br.BulletStart(dir);
            pos.x++;
            while (isFrozen)
            {
                br.Freeze();
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
        }
        MakeChoise();
        yield return new WaitForSeconds(colDown);
        isAttack = false;
    }
    
    void MakeChoise()
    {
        //choise = Random.Range(minAttack, countAttack+1);
        choise = attackPackege[0];
        attackPackege.RemoveAt(0);
    }
    IEnumerator ChangePosition()
    {
        isAttack = true;

        yield return Fade(0f, 1f);
        rb.simulated = false;
        col.enabled = false;

        float newX = GetNewX(transform.position.x);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        bossPos.x = newX;
        yield return Fade(1f, 1f);
        rb.simulated = true;
        col.enabled = true;

        isAttack = false;
        MakeChoise();
    }

    public List<int> MakePackege()
    {
        List<int> pk = new List<int>();
        for(int i=minAttack; i<=countAttack; i++)
        {
            pk.Add(i);
        }

        for (int i = pk.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = pk[i];
            pk[i] = pk[j];
            pk[j] = temp;
        }
        pk.Add(-1);
        return pk;
    }

    float GetNewX(float currentX)
    {
        List<float> options = new List<float>(positions);
        options.RemoveAll(x => Mathf.Approximately(x, currentX));

        if (options.Count == 0)
            return currentX;

        return options[Random.Range(0, options.Count)];
    }
    IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = bossSR.color.a;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            bossSR.color = new Color(bossSR.color.r, bossSR.color.g, bossSR.color.b, a);
            yield return null;
        }
        bossSR.color = new Color(bossSR.color.r, bossSR.color.g, bossSR.color.b, targetAlpha);
    }

    public void Freeze()
    {
        isFrozen = true;
    }

    public void Unfreeze()
    {
        isFrozen = false;
    }
}
