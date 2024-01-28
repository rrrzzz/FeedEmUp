using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Code
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class PlayerCollisionsController : MonoBehaviour
    {
        public static int WinScore;
        public static EventHandler ExplodeEvent;
        public static EventHandler FoodEatenEvent;
        public static EventHandler WinEvent;
        public static EventHandler PowerUpEatenEvent;
        [HideInInspector] public bool isGrowing;
        [HideInInspector] public float targScale;
        [HideInInspector] public bool growGradually = true;
        [HideInInspector] public bool isExploding;

        public bool isFirstPlayer;
        public float respawnTimeout = .5f;
        public float scaleChangeOnEat = 15f;
        public float scaleDelta = 0.005f;
        public float growingTime = 0.5f;
        public float suckingTime = 0.1f;
        public float shapingTime = 0.5f;
        public AudioSource explodeAudio;
        public AudioSource EvilLaugh;
        public ParticleSystem goreParticles;
        public float goreEmitterYOffset = 0.3f;
        public float explosionScaleMultiplier = 10f;
        public float explosionScalingTime = 1f;
        public int explodeFoodCount = 5;
        
        public Transform spawnPointsParent;
        
        private List<Transform> _spawnPoints;
        private int _currentFoodCount;
        private SkinnedMeshRenderer _meshRenderer;
        private float _currentShapeVal;
        private Vector3 _initialScale;
        private SphereCollider _col;
        private Rigidbody _rb;
        private int _currentScore;
        

        // public void SetFoodCount()
        // {
        //     _currentFoodCount = Mathf.RoundToInt(maxLocalScale / scaleChangeOnEat);
        // }

        private void Start()
        {
            _spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>().Skip(1).ToList();
            explodeAudio.GetComponent<AudioSource>();
            _meshRenderer = GetComponent<SkinnedMeshRenderer>();
            _initialScale = transform.localScale;
            _col = GetComponent<SphereCollider>();
            _rb = GetComponent<Rigidbody>();
            // SetFoodCount();
        }

        private void Update()
        {
            var trCenter = transform.TransformPoint(_col.center);
            var scaleChange = _initialScale.y / transform.localScale.y;
            goreParticles.transform.position = trCenter + Vector3.up * (goreEmitterYOffset * scaleChange);
            if (isExploding) 
                return;

            _meshRenderer.SetBlendShapeWeight(0, _currentShapeVal * 100);

            if (_currentFoodCount == explodeFoodCount)
            {
                _currentScore++;
                Explode();
            }
        }

        private void ResetPlayer()
        {
            transform.localScale = _initialScale;
            _meshRenderer.SetBlendShapeWeight(0, 0);
            _currentFoodCount = 0;
            _meshRenderer.enabled = true;
            _rb.isKinematic = false;
            isExploding = false;
            _currentShapeVal = 0;
        }

        private void Explode()
        {
            if (isExploding)
            {
                return;
            }

            _rb.isKinematic = true;
            isExploding = true;
            var finalScale = transform.localScale * explosionScaleMultiplier;

            var seq = DOTween.Sequence();
            seq.Append(DOTween.To(() => transform.localScale, value => transform.localScale = value, finalScale, 
                explosionScalingTime));
            seq.AppendCallback(() =>
            {
                _meshRenderer.enabled = false;
                explodeAudio.Play();
                EvilLaugh.Play();
                goreParticles.Play();
                ExplodeEvent?.Invoke(this, EventArgs.Empty);
                if (_currentScore == WinScore)
                {
                    WinEvent?.Invoke(this, EventArgs.Empty);
                    seq.Kill();
                }
                
            });
            seq.AppendInterval(respawnTimeout);
            seq.AppendCallback(() =>
            { 
                SpawnAtRandomPoint();
                ResetPlayer();
            });
        }

        private void OnCollisionEnter(Collision other)
        {
            var tr = other.transform;
        
            if (!tr.CompareTag(Constants.EatableTag))
                return;
        
            ProcessCollision(tr);
        }
        //
        // private void OnCollisionTriggerEnter(Collision other)
        // {
        //     var tr = other.transform;
        //
        //     if (!tr.CompareTag(Constants.EatableTag))
        //         return;
        //
        //     ProcessCollision(tr);a
        // }

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
            var newShapeVal = (float)_currentFoodCount / (explodeFoodCount - 1);
            DOTween.To(()=> _currentShapeVal, x => _currentShapeVal = x, newShapeVal, shapingTime);
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
        
        private void SpawnAtRandomPoint()
        {
            var range = _spawnPoints.Count;
            var idx = Random.Range(0, range);
            var spawnPoint = _spawnPoints[idx];
            transform.position = spawnPoint.position;
            var rotation = Quaternion.Euler(-90, spawnPoint.rotation.eulerAngles.y, 0);
            transform.rotation = rotation;
        }
    
        private Vector3 GetFinalScaleVector3(float s) => new Vector3(s, s, s);
    }
}
