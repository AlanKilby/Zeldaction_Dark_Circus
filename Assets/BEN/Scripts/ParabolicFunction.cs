using UnityEngine;

namespace BEN.Scripts
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


        private void Start()
        {
            Destroy(gameObject, 5f);
            frameDelta = frameDeltaInitialValue;
            _target = PlayerMovement_Alan.sPlayerPos; 
            distance = Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos);
        } 

        private void FixedUpdate()
        {
            /* _curvature = distance * 0.0005f * curvature;
            time += frameDelta * speed;
            time = Mathf.Repeat(time, distance + Mathf.Epsilon);
            transform.position = new Vector2(time, DoParabolicFunction() * distance * _curvature); */
            transform.Translate((_target - transform.position).normalized * Time.fixedDeltaTime * speedMultiplier, Space.Self); 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _playerLayer)
            {
                Destroy(gameObject);
                // apply damage
            }
            else if (other.gameObject.layer == _propsLayer)
            {
                Destroy(gameObject); 
            }
        } 

        float DoParabolicFunction() => (orientation * ((time * time))) + (distance * time);  
    }
}
