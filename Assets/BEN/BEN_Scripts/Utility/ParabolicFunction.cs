using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace BEN.Math
{
    [DefaultExecutionOrder(7)] 
    public class ParabolicFunction : MonoBehaviour
    {
        [FormerlySerializedAs("speedMultiplier")] [SerializeField, Range(1, 10)] private float speed = 5f; 
        [Tooltip("0 = straight line. 2 = height will be double of distance"), Range(0f, 100f)] public float curvature = 20f;
        private float distance; // from start to end point  
        private sbyte orientation = -1;
        private float time = 0f;
        [SerializeField] private bool useAsParabolic; 
        [ConditionalShow("useAsParabolic", true)] public AnimationCurve _curve;

        [SerializeField] private bool _destroyRoot; 
        private float frameDelta;
        private const float frameDeltaInitialValue = 0.02f;
        private float _curvature;
        private float timer;
        private float _speedModifMultiplier = 1f;
        [FormerlySerializedAs("isCimeterre")] public bool destroyOnWallCollision;
        [SerializeField] private bool _freezeYPosition;
        [SerializeField] private Vector3 _casterBias;
        [SerializeField, Range(1, 10)] private sbyte _dmgToFakirOnReturn = 10;


        // ALAN Variables
        //public UnityEvent playerHitEvent;

        public LayerMask
            _wallLayer,
            _playerLayer,
            _enemyLayer;

        private bool _invert = false;
        public Transform CasterTransform { get; set; }
        public Vector3 Direction { get; private set; }
        private float _duration;
        private int YDirection;
        private Vector3 _initialPosition; 
        
        private void Start() 
        {
            Destroy(_destroyRoot ? transform.root.gameObject : gameObject, 10f); 
            // frameDelta = frameDeltaInitialValue;
            Direction = _freezeYPosition ?  (new Vector3(PlayerMovement_Alan.sPlayerPos.x, 0f, PlayerMovement_Alan.sPlayerPos.z)
                                             - new Vector3(transform.position.x, 0f, transform.position.z)).normalized: 
                                            (PlayerMovement_Alan.sPlayerPos - transform.position).normalized;
            distance = Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos);
            _duration = distance / speed;
            _initialPosition = transform.position; // because somehow nulled from FakirAttack call.. 
            //playerHitEvent.AddListener(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement_Alan>().HitAnim());
        } 

        private void FixedUpdate() 
        {
            _speedModifMultiplier = _invert ? 2f : 1f;
            transform.Translate(Direction * Time.fixedDeltaTime * speed * _speedModifMultiplier, Space.World);

            if (_invert || !useAsParabolic) return;  
            timer += 0.02f / _duration;
            YDirection = timer <= 0.5f ? 1 : -1;
            var flooredValue = _curve.Evaluate(Mathf.Clamp(timer, 0f, 1f)) * 0.2f;  
            transform.position = new Vector3(transform.position.x, 
                                           transform.position.y + (flooredValue * YDirection),  
                                             transform.position.z); 
        }

        private void OnTriggerEnter(Collider other)
        {
            // TEMPORARY 
            if (Mathf.Pow(2f, other.gameObject.layer) == _playerLayer) 
            {
                if(!AK_PlayerHit.isInvincible)
                other.GetComponent<Health>().DecreaseHp(1); // super temporary

                Destroy(gameObject);
            }
            else if (Mathf.Pow(2f, other.gameObject.layer) == _wallLayer)
            {
                if (!destroyOnWallCollision) return;   
                Destroy(gameObject, 0.2f); 
            }
            else if (Mathf.Pow(2f, other.gameObject.layer) == _enemyLayer && _invert)
            {
                other.GetComponent<Health>().DecreaseHp(_dmgToFakirOnReturn);  
                Destroy(gameObject); 
            }

        } 

        float DoParabolicFunction() => (orientation * ((time * time))) + (distance * time);  

        public void InvertDirection() 
        {
            _invert = true;
            Direction = (_initialPosition - transform.position).normalized;
        }
    }
}
