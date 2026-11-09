using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SP
{
    public class PartsController : MonoBehaviour
    {
        public Transform[] parts;
        public enum PartOrder { Sequential, Random, SequentialThenRandom };
        public PartOrder order;
        public Vector3 nextPartPosition;
        public float partDrawDistance=30;
        public float partDeleteDistance=30;
        private GameObject player;
        private int nextPartIndex = 0;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadParts();
        }

        // Update is called once per frame
        void Update()
        {
            RemoveParts();
            LoadParts();
        }


        void LoadParts()
        {
            while ((nextPartPosition - player.transform.position).x < partDrawDistance)
            {
                Transform part;
                if (order==PartOrder.Random || (order == PartOrder.SequentialThenRandom && nextPartIndex >= parts.Length))
                {
                    part = parts[Random.Range(0, parts.Length)];
                }
                else if(nextPartIndex >= parts.Length)
                {
                    return;
                }
                else
                {
                    part = parts[nextPartIndex++];
                }
                Transform newPart = Instantiate(part, nextPartPosition - part.Find("Start Point").localPosition, part.rotation, transform);

                nextPartPosition = newPart.Find("End Point").position;
            }
        }

        void RemoveParts()
        {
            if (transform.childCount > 0)
            {
                Transform part = transform.GetChild(0);
                Vector3 diff = player.transform.position - part.Find("End Point").position;

                if (diff.x > partDeleteDistance)
                {
                    Destroy(part.gameObject);
                }
            }

        }
    }
}