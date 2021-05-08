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

        private bool _invert = false;
        public Transform CasterTransform { get; set; }
        private Vector3 direction; 

        private void Start()
        {
            Destroy(gameObject, 5f);
            // frameDelta = frameDeltaInitialValue;
            direction = (PlayerMovement_Alan.sPlayerPos - transform.position).normalized; 
        } 

        private void FixedUpdate()
        {
            /* _curvature = distance * 0.0005f * curvature;
            time += frameDelta * speed;
            time = Mathf.Repeat(time, distance + Mathf.Epsilon);
            transform.position = new Vector2(time, DoParabolicFunction() * distance * _curvature); */
            if (!_invert)
            {
                transform.Translate(direction * Time.fixedDeltaTime * speedMultiplier, Space.World);
            }
            else 
            { 
                transform.Translate(direction * Time.fixedDeltaTime * speedMultiplier * 4f, Space.World); // :(
            } 
        } 

        private void OnTriggerEnter(Collider other)
        {
            // temporary 

            if (other.CompareTag("Player")) 
            {
                Destroy(other.transform.parent.gameObject); // placeholder 
                Destroy(gameObject); 
                // apply damage
            }
            else if (other.gameObject.layer == _propsLayer)
            {
                Destroy(gameObject); 
            }
            else if (other.CompareTag("Enemy") && _invert) 
            {
                Destroy(other.gameObject);
                Destroy(gameObject); 
            }

        } 

        float DoParabolicFunction() => (orientation * ((time * time))) + (distance * time);  

        public void InvertDirection()
        {
            _invert = true;
            direction = (CasterTransform.position - transform.position).normalized;
        }
    }
}
