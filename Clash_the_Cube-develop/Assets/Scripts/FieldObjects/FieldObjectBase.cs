using UnityEngine;
using Framework.Variables;
using Framework.SystemInfo;

namespace ClashTheCube
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class FieldObjectBase : MonoBehaviour
    {
        [SerializeField] protected Vector2Variable swipeDelta;
        [SerializeField] protected FloatReference swipeDeltaMultiplier;
        [SerializeField] protected FloatReference swipeDeltaMultiplierDesktop;

        [SerializeField] protected FloatReference xConstraint;
        [SerializeField] protected FloatReference force;
        [SerializeField] protected FloatReference velocity;
        
        [SerializeField] protected GameObject landingSpherePrefab;
        [SerializeField] private LayerMask _player;
        
        public FieldObjectState State { get; protected set; }
        public Rigidbody Body { get; private set; }

        protected Vector3 destPosition;
        protected bool sleeping;
        protected IFieldObjectHolder objectHolder;

        private Vector3 _touchPos;
        private Vector3 _newForce;
        private LineRenderer _lineRenderer;
        private GameObject _landingSphere;
        public bool _isActive = true;
        
        
        public void SetObjectHolder(IFieldObjectHolder holder)
        {
            objectHolder = holder;
            objectHolder.AddObject(this);
        }
        
        protected void Awake()
        {
            Body = GetComponent<Rigidbody>();
            _lineRenderer = GetComponent<LineRenderer>();
            sleeping = true;
        }

        protected void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                _lineRenderer.enabled = true;

                if (touch.phase != null)
                {
                    CalculateForce();
                    ShowTrajectory(transform.position, _newForce);
                }
            }
            else
            {
                _lineRenderer.enabled = false;
                Destroy(_landingSphere);
            }
            
            if (State != FieldObjectState.Initial)
            {
                return;
            }

            transform.position = Vector3.Lerp(transform.position, destPosition, velocity * Time.deltaTime);
        }

        protected void FixedUpdate()
        {
            sleeping = Body.velocity.magnitude < 0.1f;
        }

        public void GetTouchInput()
        {
            _touchPos = Input.GetTouch(0).position;
        }

        private void ShowTrajectory(Vector3 origin, Vector3 direction)
        {
            _lineRenderer.positionCount = 0;
            if (_isActive)
            {
                
                int resolution = 300;
            
                _lineRenderer.positionCount = resolution;
            
                float maxTime = (2 * direction.y) / Physics.gravity.y * 2;

                Vector3 landingPosition = Vector3.zero;
            
                for (int i = 0; i < resolution; i++)
                {
                    float t = i / (float)resolution * maxTime;
                    Vector3 position = origin + -direction * t + Physics.gravity * t * t / 2f;
                    _lineRenderer.SetPosition(i, position);

                    if (_landingSphere == null)
                    {
                        _landingSphere = Instantiate(landingSpherePrefab, landingPosition, Quaternion.identity);
                    }

                    if (Physics.CheckSphere(position, 1f, ~_player))
                    {
                        landingPosition = position;
                        _lineRenderer.positionCount = i + 1;
                        _landingSphere.transform.position = landingPosition;
                        break;
                    }
                }
            }
        }
        
        public void CalculateForce()
        {
            if (_isActive)
            {
                var Force = new Vector3(_touchPos.x - Input.GetTouch(0).position.x, (_touchPos.y - Input.GetTouch(0).position.y) / 1.3f, _touchPos.y - Input.GetTouch(0).position.y);
                _newForce = force * Force / 100;
            }
        }
        
        public void Accelerate()
        {
            if (_isActive)
            {
                _isActive = false;
                if (State != FieldObjectState.Initial)
                {
                    return;
                }
                Body.isKinematic = false;
                Body.AddForce(_newForce, ForceMode.VelocityChange);
                State = FieldObjectState.Transition;
            }
        }

        private float GetDeltaMultiplier()
        {
            return Platform.IsMobilePlatform() ? swipeDeltaMultiplier : swipeDeltaMultiplierDesktop;
        }
    }
}
