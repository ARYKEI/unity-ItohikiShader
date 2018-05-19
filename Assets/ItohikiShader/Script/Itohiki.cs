using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// 液体が糸を引くやつ
///
namespace ARYKEI
{
    public class Itohiki : MonoBehaviour
    {
        public float ClearSpeed = 0.1f;
        public float AddSpeed = 1f;
        public int DelayCount = 5;
        public float DistanceClearSpeed = 5;

        private List<Transform> targetList;
        private Transform CurrentTarget;

        private float CurrentAmount = 0;
        private float DistanceToTarget = 0;

        private MeshRenderer smr;
        private Vector3 StartSize;
        private int LastAddedFrame;
        private Matrix4x4[] DelayedTransformMatrix;
        private float parentLossyScale = 1;

        // Property ID
        private static int PID_DelayMat = Shader.PropertyToID("_DelayedMat");
        private static int PID_DelayEnable = Shader.PropertyToID("_DelayEnable");
        private static int PID_Distance = Shader.PropertyToID("_Distance");
        private static int PID_Amount = Shader.PropertyToID("_Amount");

        private void Start()
        {
            Init();
            InitDelayedTransform();
            InitTarget();

			if(transform.parent != null)
			{
                parentLossyScale = transform.parent.lossyScale.x;
            }
        }

        private void Init()
        {
            StartSize = transform.localScale;
            smr = GetComponent<MeshRenderer>();
        }
        private void InitDelayedTransform()
        {
            DelayedTransformMatrix = new Matrix4x4[DelayCount];
        }
        private void UpdateDelayedTransform()
        {
            DelayedTransformMatrix[Time.frameCount % DelayCount] = transform.localToWorldMatrix;
        }
        private void InitTarget()
        {
            var etarget = FindObjectsOfType<ItohikiTarget>();
            targetList = new List<Transform>();
            for (int i = 0; i < etarget.Length; i++)
            {
                targetList.Add(etarget[i].transform);
            }
        }
        private void UpdateTarget()
        {
            if (CurrentTarget == null)
            {
                CurrentTarget = SearchNearestTarget();
            }
            DistanceToTarget = (transform.position - CurrentTarget.position).magnitude;
        }

        private Transform SearchNearestTarget()
        {
            float dist = float.MaxValue;
            int nearestIdx = 0;
            for (int i = 0; i < targetList.Count; i++)
            {
                var d = (transform.position - targetList[i].position).sqrMagnitude;
                if (dist > d)
                {
                    dist = d;
                    nearestIdx = i;
                }
            }
            return targetList[nearestIdx];
        }

        private void UdpateItohiki()
        {
            if (Time.frameCount - LastAddedFrame > 1)
            {
                UpdateTarget();

                // Rotate toward target
                transform.LookAt(CurrentTarget.position, Vector3.up);
                // Change the length
                transform.localScale = new Vector3(StartSize.x, StartSize.y, DistanceToTarget/parentLossyScale);

                // Reduce the amount of liquid
                CurrentAmount -= Time.deltaTime * (ClearSpeed + DistanceToTarget * DistanceClearSpeed);
                CurrentAmount = Mathf.Clamp01(CurrentAmount);
            }
        }
        private void UpdateVisibility()
        {
            if (CurrentAmount > 0)
            {
                smr.enabled = true;
                SetParameters();
            }
            else
            {
                smr.enabled = false;
            }
        }
        private void SetParameters()
        {
            // Delayed Transform
            smr.material.SetMatrix(PID_DelayMat, DelayedTransformMatrix[(Time.frameCount + 1) % DelayCount]);
            smr.material.SetFloat(PID_DelayEnable, 1);	// for only playmode
            smr.material.SetFloat(PID_Distance, DistanceToTarget);
            smr.material.SetFloat(PID_Amount, CurrentAmount);
        }

        void LateUpdate()
        {
            UpdateDelayedTransform();
            UdpateItohiki();
            UpdateVisibility();
        }

        public void AddLiquid()
        {
            CurrentAmount += Time.deltaTime * AddSpeed;
            LastAddedFrame = Time.frameCount;
            CurrentTarget = null;
            DistanceToTarget = 0;
            transform.localScale = Vector3.zero;
        }
    }
}
