using UnityEngine;

namespace PlasmaDragon
{
    /// <summary>
    /// Smooth camera follow script for testing flight mechanics
    /// Follows target from behind and above with smooth interpolation
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private bool autoFindTarget = true;
        
        [Header("Position Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 3, -8); // Behind and above
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float rotationSpeed = 3f;
        
        [Header("Look Settings")]
        [SerializeField] private bool lookAtTarget = true;
        [SerializeField] private Vector3 lookOffset = Vector3.up; // Look slightly above target
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Camera cam;
        
        void Start()
        {
            cam = GetComponent<Camera>();
            
            // Auto-find target if not set
            if (autoFindTarget && target == null)
            {
                FindTarget();
            }
            
            // Set initial position if target exists
            if (target != null)
            {
                SetInitialPosition();
            }
        }
        
        void LateUpdate()
        {
            if (target == null)
            {
                if (autoFindTarget)
                    FindTarget();
                return;
            }
            
            UpdateCameraPosition();
            UpdateCameraRotation();
        }
        
        private void FindTarget()
        {
            // Look for the dragon model first (Unka Toon)
            GameObject dragon = GameObject.Find("Unka Toon");
            if (dragon != null)
            {
                target = dragon.transform;
                Debug.Log("🐉 Camera found dragon target: " + target.name);
                return;
            }
            
            // Look for the test cube second
            GameObject testCube = GameObject.Find("🎯 MCP Test Cube");
            if (testCube != null)
            {
                target = testCube.transform;
                Debug.Log("🎯 Camera found target: " + target.name);
                return;
            }
            
            // Fallback: Look for any object with BasicFlightController
            BasicFlightController flightController = FindObjectOfType<BasicFlightController>();
            if (flightController != null)
            {
                target = flightController.transform;
                Debug.Log("🎯 Camera found flight controller target: " + target.name);
                return;
            }
            
            // Last resort: Look for any GameObject with "dragon" in the name (case insensitive)
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("dragon") || obj.name.ToLower().Contains("unka"))
                {
                    target = obj.transform;
                    Debug.Log("🔍 Camera found dragon-like target: " + target.name);
                    return;
                }
            }
            
            Debug.LogWarning("❌ No suitable target found! Please ensure dragon has BasicFlightController component.");
        }
        
        private void SetInitialPosition()
        {
            Vector3 desiredPosition = target.position + target.TransformDirection(offset);
            transform.position = desiredPosition;
            
            if (lookAtTarget)
            {
                Vector3 lookTarget = target.position + lookOffset;
                transform.LookAt(lookTarget);
            }
        }
        
        private void UpdateCameraPosition()
        {
            // Calculate desired position relative to target's rotation
            Vector3 desiredPosition = target.position + target.TransformDirection(offset);
            
            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 
                                            followSpeed * Time.deltaTime);
        }
        
        private void UpdateCameraRotation()
        {
            if (!lookAtTarget) return;
            
            Vector3 lookTarget = target.position + lookOffset;
            Vector3 direction = (lookTarget - transform.position).normalized;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                                                    rotationSpeed * Time.deltaTime);
            }
        }
        
        void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 100));
            GUILayout.Label("📷 CAMERA FOLLOW DEBUG", GUI.skin.box);
            
            if (target != null)
            {
                GUILayout.Label($"Target: {target.name}");
                GUILayout.Label($"Distance: {Vector3.Distance(transform.position, target.position):F1}m");
                GUILayout.Label($"Offset: {offset}");
            }
            else
            {
                GUILayout.Label("❌ No target found!");
            }
            
            GUILayout.EndArea();
        }
        
        // Public methods for runtime adjustment
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
                SetInitialPosition();
        }
        
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
    }
}