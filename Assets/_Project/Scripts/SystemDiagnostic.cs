using UnityEngine;

namespace PlasmaDragon
{
    /// <summary>
    /// Quick diagnostic script to verify all systems are connected properly
    /// </summary>
    public class SystemDiagnostic : MonoBehaviour
    {
        [Header("System Check")]
        [SerializeField] private bool runDiagnostic = true;
        
        void Start()
        {
            if (runDiagnostic)
            {
                RunSystemCheck();
            }
        }
        
        [ContextMenu("Run System Check")]
        void RunSystemCheck()
        {
            Debug.Log("🔍 === SYSTEM DIAGNOSTIC START ===");
            
            // Check for flight controller
            var flightController = FindObjectOfType<BasicFlightController>();
            if (flightController != null)
            {
                Debug.Log("✅ BasicFlightController found on: " + flightController.gameObject.name);
            }
            else
            {
                Debug.LogError("❌ BasicFlightController NOT found! Add it to the test cube.");
            }
            
            // Check for camera follow
            var cameraFollow = FindObjectOfType<CameraFollow>();
            if (cameraFollow != null)
            {
                Debug.Log("✅ CameraFollow found on: " + cameraFollow.gameObject.name);
            }
            else
            {
                Debug.LogError("❌ CameraFollow NOT found! Add it to Main Camera.");
            }
            
            // Check for test cube
            var testCube = GameObject.Find("🎯 MCP Test Cube");
            if (testCube != null)
            {
                Debug.Log("✅ Test cube found: " + testCube.name);
                
                // Check components on test cube
                var rb = testCube.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log("✅ Rigidbody found on test cube");
                }
                else
                {
                    Debug.LogWarning("⚠️ Rigidbody missing from test cube - add it!");
                }
            }
            else
            {
                Debug.LogError("❌ Test cube not found!");
            }
            
            Debug.Log("🔍 === SYSTEM DIAGNOSTIC COMPLETE ===");
            Debug.Log("💡 If any components are missing, follow the manual setup steps!");
        }
    }
}