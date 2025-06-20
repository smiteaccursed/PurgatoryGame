using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingController:MonoBehaviour
{
    private static LoadingController instance;
    public static LoadingController Instance { get { return instance; } }
    public Canvas canvas;
    private void Awake()
    {
        canvas.gameObject.SetActive(true);
        Debug.Log("Loading...");
        if (instance != null && instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] private bool _isWorld;
    [SerializeField] private bool _isPlayer;
    [SerializeField] private bool _isAbility;
    [SerializeField] private bool _isCamera;

    public bool IsWorld
    {
        get => _isWorld;
        set
        {
            _isWorld = value;
            CheckAllFlags();
        }
    }

    public bool IsPlayer
    {
        get => _isPlayer;
        set
        {
            _isPlayer = value;
            CheckAllFlags();
        }
    }

    public bool IsAbility
    {
        get => _isAbility;
        set
        {
            _isAbility = value;
            CheckAllFlags();
        }
    }

    public bool IsCamera
    {
        get => _isCamera;
        set
        {
            _isCamera = value;
            CheckAllFlags();
        }
    }

    private void CheckAllFlags()
    {
        if(_isWorld&& _isPlayer && _isAbility && _isCamera)
        {
            StartCoroutine(Waiting());
             
        }
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(0f);
        TimeManger.GetInstance().TriggerTimeStop();
        TimeManger.GetInstance().TriggerTimeResume();
        canvas.gameObject.SetActive(false);
        Debug.Log("Готово");
    }
}
