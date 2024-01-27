using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class PlayerCollisionsController : MonoBehaviour
    {
        public static EventHandler ExplodeEvent;
        public static EventHandler FoodEatenEvent;
        public static EventHandler PowerUpEatenEvent;
        [HideInInspector] public bool isGrowing;
        [HideInInspector] public float targScale;
        [HideInInspector] public bool growGradually = true;
        [HideInInspector] public bool isExploding;
        // [HideInInspector] public float scaleChangeOnEat = 0.1f;
        // [HideInInspector] public float maxLocalScale = 0.5f;
    
        public float scaleChangeOnEat = 0.1f;
        public float maxLocalScale = 0.5f;
        [SerializeField] private float scaleDelta = 0.005f;
        [SerializeField] private float growingTime = 0.5f;
        [SerializeField] private float suckingTime = 0.1f;

        public int _explodeFoodCount;
        public TMP_Text _text;
        private int _currentFoodCount;
        private SkinnedMeshRenderer _meshRenderer;

        // public void SetFoodCount()
        // {
        //     _currentFoodCount = Mathf.RoundToInt(maxLocalScale / scaleChangeOnEat);
        // }

        private void Start()
        {
            _meshRenderer = GetComponent<SkinnedMeshRenderer>();
            // SetFoodCount();
        }

        private void Update()
        {
            if (isExploding) 
                return;

            //transform.localScale.x > maxLocalScale || 
            if (_currentFoodCount == _explodeFoodCount)
            {
                _text.enabled = true;
                // Debug.Log("Explode!");
                Explode();
            }
        }
    
        private void Explode()
        {
            if (isExploding)
            {
                return;
            }
            ExplodeEvent?.Invoke(this, EventArgs.Empty);
        
            isExploding = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            var tr = other.transform;
        
            if (!tr.CompareTag(Constants.EatableTag))
                return;
        
            ProcessCollision(tr);
        }
        
        private void OnCollisionTriggerEnter(Collision other)
        {
            var tr = other.transform;
        
            if (!tr.CompareTag(Constants.EatableTag))
                return;
        
            ProcessCollision(tr);
        }

        public void ProcessCollision(Transform tr)
        {
            if (isExploding)
                return;

            tr.tag = Constants.UneatableTag;
            tr.GetComponent<BoxCollider>().enabled = false;
            FoodEatenEvent?.Invoke(this, EventArgs.Empty);

            _currentFoodCount++;
             PerformSpherization();
            
            // TODO: Optionally take into account food volume
            // var mr = tr.GetComponentInChildren<MeshRenderer>();
            // var meshGlobalVolume = mr.bounds.size;
            // var volume = meshGlobalVolume.x * meshGlobalVolume.y * meshGlobalVolume.z;
            // StartCoroutine(ScaleUp(Mathf.Pow(volume, 1f / 3f)));
        
            StartCoroutine(ScaleDown(tr));
            StartCoroutine(ScaleUp(scaleChangeOnEat));
        }

        private void PerformSpherization()
        {
            _meshRenderer.SetBlendShapeWeight(0, (float)_currentFoodCount / _explodeFoodCount * 10);
        }

        private IEnumerator ScaleUp(float volume)
        {
            var scaleVelocity = Vector3.zero;
            var targetScale = transform.localScale.x + volume;
            targScale = Mathf.Max(targetScale, targScale);
            //TODO: change scale along all axes
            var finalScale = GetFinalScaleVector3(targetScale);
            while (targetScale - transform.localScale.x > scaleDelta)
            {
                isGrowing = true;
                transform.localScale =
                    Vector3.SmoothDamp(transform.localScale, finalScale, ref scaleVelocity, growingTime, 1000, Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            isGrowing = false;
        }
    
        private IEnumerator ScaleDown(Transform tr)
        {
            if (!tr)
            {
                yield break;
            }
        
            var moveVelocity = Vector3.zero;
            var scaleVelocity = Vector3.zero;
        
            var finalScale = tr.localScale.x * 0.1f;
            var targetScale = GetFinalScaleVector3(finalScale);
        
            //TODO: may have other colliders here
            var meshCols = tr.GetComponentsInChildren<MeshCollider>();
        
            foreach (var mc in meshCols)
            {
                mc.enabled = false;
            }
        
            if (!tr)
            {
                yield break;
            }
        
            while (tr.localScale.x - finalScale > finalScale * 0.1f)
            {
                if (!tr)
                {
                    yield break;
                }
                tr.position =
                    Vector3.SmoothDamp(tr.position, transform.position, ref moveVelocity, suckingTime, 1000, Time.deltaTime);
                tr.localScale =
                    Vector3.SmoothDamp(tr.localScale, targetScale, ref scaleVelocity, suckingTime, 1000, Time.deltaTime);
            
                if (!tr)
                {
                    yield break;
                }
                yield return new WaitForEndOfFrame();
            }
        
            Destroy(tr.gameObject);
        }
    
        private Vector3 GetFinalScaleVector3(float s) => new Vector3(s, s, s);
    }
}
