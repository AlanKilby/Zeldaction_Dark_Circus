using UnityEngine;

namespace BEN.Scripts
{
    public class ParabolicFunction : MonoBehaviour
    {
        [Range(1f, 10f)] public float speed = 5f;
        [Range(1f, 10f)] public float amplitude = 2f;

        private Vector3 m_Target;
        private bool _targetIsSet;
        private Vector3 _direction;

        private bool _hasBeenHit;
        private Vector3 m_Caster;

        private void Start()
        {
            Destroy(gameObject, 5f); 
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _hasBeenHit = true;
        }

        private void FixedUpdate()
        {
            if (!_targetIsSet) return;
            _direction = _hasBeenHit ? (m_Caster - transform.position).normalized * 3f : (m_Target - transform.position).normalized; 
            transform.Translate(_direction * Time.fixedDeltaTime * speed, Space.World);
        } 

        public void SetTargetPosition(Vector3 target, Vector3 caster)
        {
            _targetIsSet = true; 
            m_Target = target;
            m_Caster = caster; 
        } 

        private void DoParabolicTranslation()
        {
            //
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.CompareTag("Enemy") && _hasBeenHit)
            {
                Destroy(other.transform.root.gameObject);
                Destroy(gameObject);
            } 
            else if (other.CompareTag("Player"))
            {
                Destroy(gameObject);  
            }
        }
    }
}
