using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float lerpSpeed = 20.0f;

        public Vector3 offset;
        public Vector3 targetPos;
        public bool isLoading = false;

        private void Start()
        {
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
            else
            {
                offset = new Vector3(0, 0, -10);
            }

        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }
            else if(!isLoading)
            {
                targetPos = target.position + offset;
                transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
                LoadingController.Instance.IsCamera = true;
                isLoading = true;
            }

            targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        }

    }
}
