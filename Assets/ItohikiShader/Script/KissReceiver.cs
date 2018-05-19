using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARYKEI
{
    public class KissReceiver : MonoBehaviour
    {
        public Itohiki itohiki;
        public bool isKiss = false;

        private void Update()
        {
            if (isKiss)
            {
                itohiki.AddLiquid();
            }
        }

        private void OnTriggerEnter(Collider other) 
		{
            isKiss = true;
        }
		private void OnTriggerExit(Collider other) 
		{
            isKiss = false;
        }
    }
}