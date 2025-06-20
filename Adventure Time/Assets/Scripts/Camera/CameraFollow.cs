using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow Instance { get; private set; }

        public Transform target;
        public float lerpSpeed = 20.0f;

        public Vector3 offset;
        public Vector3 targetPos;
        public bool isLoading = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
 
        }

        private void Start()
        {
            offset = new Vector3(0, 0, -10);
            isLoading = false;

            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    target = playerObj.transform;
                    LoadingController.Instance.IsCamera = true;
                }
            }

            transform.position = target.position;
        }

        private void Update()
        {
            if (target == null) return;

            if (!isLoading)
            {
                targetPos = target.position + offset;
                transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
                LoadingController.Instance.IsCamera = true;
                isLoading = true;
            }

            targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        }

        public void SetCameraOnPlayer()
        {
            if (target != null)
                transform.position = target.position;
        }
    }
}
