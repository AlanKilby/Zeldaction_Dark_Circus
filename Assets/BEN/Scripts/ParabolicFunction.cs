using UnityEngine;

namespace BEN.Math
{
    public class ParabolicFunction : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] private float speedMultiplier = 5f; 
        [Tooltip("0 = straight line. 2 = height will be double of distance"), Range(0f, 100f)] public float curvature = 20f;
        private float distance; // from start to end point  
        private sbyte orientation = -1;
        private float time = 0f;

        private float frameDelta;
        private const float frameDeltaInitialValue = 0.02f;
        private float _curvature;

        private byte
            _propsLayer = 6,
            _playerLayer = 10;
        private Vector3 _target; // until I fix parabolic formula
        public Vector3 _CasterPosition { get; set; }

        private bool invert = false;
        private Vector3 initialPosition;

        private void Start()
        {
            Destroy(gameObject, 5f);
            _CasterPosition = transform.position; // TODO : get caster's position when player projectile hits enemy's projectile
            // frameDelta = frameDeltaInitialValue;
            _target = PlayerMovement_Alan.sPlayerPos; 
        } 

        private void FixedUpdate()
        {
            /* _curvature = distance * 0.0005f * curvature;
            time += frameDelta * speed;
            time = Mathf.Repeat(time, distance + Mathf.Epsilon);
            transform.position = new Vector2(time, DoParabolicFunction() * distance * _curvature); */
            if (!invert)
                transform.Translate(Vector3.forward * Time.fixedDeltaTime * speedMultiplier, Space.Self);
            else 
            { 
                transform.Translate((_target - transform.position).normalized * Time.fixedDeltaTime * speedMultiplier * 4f, Space.Self); 
            } 

            Debug.DrawLine(_target, transform.position, Color.cyan); 
        }

        private void OnTriggerEnter(Collider other)
        {
            // temporary 

            if (other.gameObject.layer == _playerLayer) 
            {
                Destroy(gameObject);
                // apply damage
            }
            else if (other.gameObject.layer == _propsLayer)
            {
                Destroy(gameObject); 
            }
            else if (other.CompareTag("Enemy") && invert) 
            {
                Destroy(other.gameObject);
                Destroy(gameObject); 
            }

        } 

        float DoParabolicFunction() => (orientation * ((time * time))) + (distance * time);  

        public void InvertDirection()
        {
            invert = true;
            _target = _CasterPosition;
        }
    }
}
