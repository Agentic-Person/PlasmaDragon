using UnityEngine;

namespace PlasmaDragon
{
    /// <summary>
    /// Basic flight controller for testing cube movement and physics
    /// This validates our flight mechanics before integrating the full dragon system
    /// </summary>
    public class BasicFlightController : MonoBehaviour
    {
        [Header("Flight Settings")]
        [SerializeField] private float flightSpeed = 10f;
        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float rotationSpeed = 100f;
        
        [Header("Mouse Controls")]
        [SerializeField] private float mouseSensitivity = 3f;
        [SerializeField] private bool invertMouseY = false;
        [SerializeField] private float mouseSmoothing = 5f;
        
        [Header("Roll Controls")]
        [SerializeField] private float rollSpeed = 90f;
        [SerializeField] private float rollReturnSpeed = 45f;
        [SerializeField] private bool autoLevelRoll = true;
        
        [Header("Pitch Controls")]
        [SerializeField] private float pitchSpeed = 60f;
        [SerializeField] private float pitchReturnSpeed = 30f;
        [SerializeField] private float maxPitchAngle = 60f;
        [SerializeField] private bool autoLevelPitch = true;
        
        [Header("Flight Dynamics")]
        [SerializeField] private float forwardThrustMultiplier = 1.5f;
        [SerializeField] private bool useRelativeMovement = true;
        
        [Header("Physics")]
        [SerializeField] private float drag = 2f;
        [SerializeField] private float angularDrag = 5f;
        
        [Header("Controls")]
        [SerializeField] private KeyCode flyToggle = KeyCode.Space;
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        
        [Header("Testing GUI")]
        [SerializeField] private bool showTestingGUI = true;
        [SerializeField] private KeyCode toggleTestingGUI = KeyCode.F1;
        
        private Rigidbody rb;
        private bool isFlying = false;
        private bool isSprinting = false;
        
        // Mouse control variables
        private Vector2 mouseInput;
        private Vector2 smoothedMouseInput;
        
        // Roll control variables
        private float currentRoll = 0f;
        private float targetRoll = 0f;
        
        // Pitch control variables  
        private float currentPitch = 0f;
        private float targetPitch = 0f;
        
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            // Configure physics for smooth flight
            rb.linearDamping = drag;
            rb.angularDamping = angularDrag;
            rb.useGravity = !isFlying;
            
            Debug.Log($"🚁 BasicFlightController initialized on {gameObject.name}");
            Debug.Log("🎮 ENHANCED FLIGHT CONTROLS: SPACE = Toggle fly, MOUSE = Pitch & Yaw, W/S = Thrust, A/D = Roll, Q/E = Vertical, Shift = Sprint, F1 = Testing GUI");
            Debug.Log("✈️ REALISTIC FLIGHT DYNAMICS: Dragon tilts and flies in the direction it's pointing!");
        }
        
        void Update()
        {
            HandleInput();
            UpdatePhysics();
        }
        
        void HandleInput()
        {
            // Toggle flying mode
            if (Input.GetKeyDown(flyToggle))
            {
                ToggleFlyMode();
            }
            
            // Toggle testing GUI
            if (Input.GetKeyDown(toggleTestingGUI))
            {
                showTestingGUI = !showTestingGUI;
                Debug.Log($"🎛️ Testing GUI: {(showTestingGUI ? "ENABLED" : "DISABLED")} - Press F1 to toggle");
            }
            
            // Sprint toggle
            isSprinting = Input.GetKey(sprintKey);
            
            // Only process movement if flying
            if (isFlying)
            {
                HandleMovement();
            }
        }
        
        void ToggleFlyMode()
        {
            isFlying = !isFlying;
            rb.useGravity = !isFlying;
            
            if (isFlying)
            {
                // Stop falling when entering fly mode
                rb.linearVelocity = Vector3.zero;
                Debug.Log("🚁 Flight mode ENABLED");
            }
            else
            {
                Debug.Log("🚁 Flight mode DISABLED");
            }
        }
        
        void HandleMovement()
        {
            // === MOUSE CONTROLS ===
            // Get mouse input for pitch and yaw rotation
            mouseInput.x = Input.GetAxis("Mouse X"); // Yaw (turning left/right)
            mouseInput.y = Input.GetAxis("Mouse Y"); // Pitch (nose up/down)
            
            if (invertMouseY)
                mouseInput.y = -mouseInput.y;
            
            // Smooth mouse input for better control
            smoothedMouseInput = Vector2.Lerp(smoothedMouseInput, mouseInput, mouseSmoothing * Time.deltaTime);
            
            // === KEYBOARD CONTROLS ===
            // W/S for forward/backward thrust
            float forwardBackward = Input.GetAxis("Vertical"); // W/S
            
            // A/D for rolling (Z-axis rotation)
            float rollInput = 0f;
            if (Input.GetKey(KeyCode.A)) rollInput = 1f;  // Roll left
            if (Input.GetKey(KeyCode.D)) rollInput = -1f; // Roll right
            
            // Q/E for additional up/down thrust (overrides mouse pitch)
            float extraUpDown = 0f;
            if (Input.GetKey(KeyCode.Q)) extraUpDown = -1f; // Down
            if (Input.GetKey(KeyCode.E)) extraUpDown = 1f;  // Up
            
            // === FLIGHT DYNAMICS ===
            Vector3 movement = Vector3.zero;
            
            if (useRelativeMovement)
            {
                // REALISTIC FLIGHT: Movement relative to dragon's orientation
                
                // Primary forward/backward thrust
                movement += transform.forward * forwardBackward * forwardThrustMultiplier;
                
                // Side-to-side strafe (less powerful than forward thrust)
                movement += transform.right * smoothedMouseInput.x * mouseSensitivity * 0.3f;
                
                // Vertical thrust (Q/E keys override mouse when pressed)
                if (Mathf.Abs(extraUpDown) > 0.1f)
                {
                    movement += Vector3.up * extraUpDown;
                }
                else
                {
                    // Subtle vertical component from pitch (nose up = slight climb)
                    float pitchInfluence = -currentPitch / maxPitchAngle; // Negative because positive pitch = nose up
                    movement += Vector3.up * pitchInfluence * 0.2f;
                }
            }
            else
            {
                // ARCADE FLIGHT: Direct world space movement (old system)
                movement.z = forwardBackward;
                movement.x = smoothedMouseInput.x * mouseSensitivity;
                movement.y = (smoothedMouseInput.y * mouseSensitivity) + extraUpDown;
            }
            
            // Apply sprint multiplier
            float currentSpeed = flightSpeed;
            if (isSprinting)
            {
                currentSpeed *= sprintMultiplier;
            }
            
            // Apply movement forces
            if (movement.magnitude > 0.1f)
            {
                rb.AddForce(movement * currentSpeed, ForceMode.Force);
            }
            
            // === ROTATION CONTROLS ===
            HandlePitchControl(smoothedMouseInput.y);
            HandleRollControl(rollInput);
        }
        
        void HandlePitchControl(float pitchInput)
        {
            // Update target pitch based on mouse Y input
            if (Mathf.Abs(pitchInput) > 0.1f)
            {
                targetPitch += pitchInput * pitchSpeed * Time.deltaTime;
                targetPitch = Mathf.Clamp(targetPitch, -maxPitchAngle, maxPitchAngle); // Limit pitch angle
            }
            else if (autoLevelPitch)
            {
                // Auto-level pitch when no input
                targetPitch = Mathf.MoveTowards(targetPitch, 0f, pitchReturnSpeed * Time.deltaTime);
            }
            
            // Smoothly interpolate current pitch to target
            currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, rotationSpeed * Time.deltaTime);
        }
        
        void HandleRollControl(float rollInput)
        {
            // Update target roll based on A/D input
            if (Mathf.Abs(rollInput) > 0.1f)
            {
                targetRoll += rollInput * rollSpeed * Time.deltaTime;
                targetRoll = Mathf.Clamp(targetRoll, -45f, 45f); // Limit roll angle
            }
            else if (autoLevelRoll)
            {
                // Auto-level roll when no input
                targetRoll = Mathf.MoveTowards(targetRoll, 0f, rollReturnSpeed * Time.deltaTime);
            }
            
            // Smoothly interpolate current roll to target
            currentRoll = Mathf.LerpAngle(currentRoll, targetRoll, rotationSpeed * Time.deltaTime);
            
            // Apply both pitch and roll rotation together
            Vector3 currentEuler = transform.eulerAngles;
            currentEuler.x = currentPitch; // Pitch (nose up/down)
            currentEuler.z = currentRoll;  // Roll (banking left/right)
            transform.eulerAngles = currentEuler;
        }
        
        void UpdatePhysics()
        {
            // Limit max velocity to prevent runaway speed
            if (rb.linearVelocity.magnitude > flightSpeed * sprintMultiplier * 1.5f)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * flightSpeed * sprintMultiplier * 1.5f;
            }
        }
        
        void OnGUI()
        {
            // Show flight status debug info
            GUILayout.BeginArea(new Rect(10, 10, 320, 200));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label($"🚁 Flight Mode: {(isFlying ? "ENABLED" : "DISABLED")}");
            GUILayout.Label($"⚡ Sprint: {(isSprinting ? "ON" : "OFF")}");
            GUILayout.Label($"🎯 Velocity: {rb.linearVelocity.magnitude:F1}");
            GUILayout.Label($"🔄 Roll: {currentRoll:F1}° (Target: {targetRoll:F1}°)");
            GUILayout.Label($"📐 Pitch: {currentPitch:F1}° (Target: {targetPitch:F1}°)");
            GUILayout.Label($"✈️ Flight Style: {(useRelativeMovement ? "REALISTIC" : "ARCADE")}");
            
            GUILayout.Space(10);
            GUILayout.Label("🎮 ENHANCED FLIGHT CONTROLS:");
            GUILayout.Label("• SPACE = Toggle fly mode");
            GUILayout.Label("• MOUSE = Pitch & Yaw (look around)");
            GUILayout.Label("• W/S = Forward/backward thrust");
            GUILayout.Label("• A/D = ROLL left/right");
            GUILayout.Label("• Q/E = Direct up/down thrust");
            GUILayout.Label("• Shift = Sprint");
            GUILayout.Label("• F1 = Toggle testing GUI");
            
            GUILayout.Space(5);
            GUILayout.Label($"Mouse Input: ({smoothedMouseInput.x:F2}, {smoothedMouseInput.y:F2})");
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
            
            // Show testing GUI if enabled
            if (showTestingGUI)
            {
                DrawTestingGUI();
            }
            
            // Debug info in bottom-right to confirm OnGUI is working
            GUILayout.BeginArea(new Rect(Screen.width - 200, Screen.height - 50, 190, 40));
            GUILayout.BeginVertical("box");
            GUILayout.Label($"GUI Active: {showTestingGUI} | F1 to toggle");
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        void DrawTestingGUI()
        {
            // Ensure the GUI fits on screen
            float guiWidth = 330f;
            float guiHeight = 400f;
            float xPos = Mathf.Max(10, Screen.width - guiWidth - 10); // Ensure it's not off-screen
            float yPos = 10f;
            
            // Testing controls panel on the right side
            GUILayout.BeginArea(new Rect(xPos, yPos, guiWidth, guiHeight));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("🎛️ REAL-TIME TESTING CONTROLS", GUI.skin.box);
            GUILayout.Space(10);
            
            // Mouse Controls Section
            GUILayout.Label("🖱️ MOUSE CONTROLS:", GUI.skin.box);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Mouse Sensitivity: {mouseSensitivity:F1}", GUILayout.Width(150));
            mouseSensitivity = GUILayout.HorizontalSlider(mouseSensitivity, 0.5f, 10f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Mouse Smoothing: {mouseSmoothing:F1}", GUILayout.Width(150));
            mouseSmoothing = GUILayout.HorizontalSlider(mouseSmoothing, 1f, 15f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Invert Mouse Y:", GUILayout.Width(150));
            invertMouseY = GUILayout.Toggle(invertMouseY, "", GUILayout.Width(20));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Flight Controls Section
            GUILayout.Label("🚁 FLIGHT CONTROLS:", GUI.skin.box);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Flight Speed: {flightSpeed:F1}", GUILayout.Width(150));
            flightSpeed = GUILayout.HorizontalSlider(flightSpeed, 5f, 50f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Sprint Multiplier: {sprintMultiplier:F1}", GUILayout.Width(150));
            sprintMultiplier = GUILayout.HorizontalSlider(sprintMultiplier, 1.5f, 5f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Roll Controls Section
            GUILayout.Label("🔄 ROLL CONTROLS:", GUI.skin.box);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Roll Speed: {rollSpeed:F0}°/s", GUILayout.Width(150));
            rollSpeed = GUILayout.HorizontalSlider(rollSpeed, 30f, 180f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Roll Return: {rollReturnSpeed:F0}°/s", GUILayout.Width(150));
            rollReturnSpeed = GUILayout.HorizontalSlider(rollReturnSpeed, 15f, 90f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Auto Level Roll:", GUILayout.Width(150));
            autoLevelRoll = GUILayout.Toggle(autoLevelRoll, "", GUILayout.Width(20));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Pitch Controls Section
            GUILayout.Label("📐 PITCH CONTROLS:", GUI.skin.box);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Pitch Speed: {pitchSpeed:F0}°/s", GUILayout.Width(150));
            pitchSpeed = GUILayout.HorizontalSlider(pitchSpeed, 20f, 120f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Max Pitch: {maxPitchAngle:F0}°", GUILayout.Width(150));
            maxPitchAngle = GUILayout.HorizontalSlider(maxPitchAngle, 30f, 90f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Auto Level Pitch:", GUILayout.Width(150));
            autoLevelPitch = GUILayout.Toggle(autoLevelPitch, "", GUILayout.Width(20));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Flight Dynamics Section
            GUILayout.Label("✈️ FLIGHT DYNAMICS:", GUI.skin.box);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Realistic Flight:", GUILayout.Width(150));
            useRelativeMovement = GUILayout.Toggle(useRelativeMovement, "", GUILayout.Width(20));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Forward Thrust: {forwardThrustMultiplier:F1}x", GUILayout.Width(150));
            forwardThrustMultiplier = GUILayout.HorizontalSlider(forwardThrustMultiplier, 0.5f, 3f, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Quick Preset Buttons
            GUILayout.Label("🎯 QUICK PRESETS:", GUI.skin.box);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Responsive", GUILayout.Width(100)))
            {
                ApplyPreset("responsive");
            }
            if (GUILayout.Button("Smooth", GUILayout.Width(100)))
            {
                ApplyPreset("smooth");
            }
            if (GUILayout.Button("Aggressive", GUILayout.Width(100)))
            {
                ApplyPreset("aggressive");
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label("💡 Tip: Adjust mouse sensitivity first!", GUI.skin.box);
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        void ApplyPreset(string presetName)
        {
            switch (presetName.ToLower())
            {
                case "responsive":
                    mouseSensitivity = 5f;
                    mouseSmoothing = 8f;
                    flightSpeed = 15f;
                    rollSpeed = 120f;
                    rollReturnSpeed = 60f;
                    pitchSpeed = 80f;
                    pitchReturnSpeed = 40f;
                    maxPitchAngle = 45f;
                    forwardThrustMultiplier = 2f;
                    useRelativeMovement = true;
                    Debug.Log("🎯 Applied RESPONSIVE preset - Quick, agile flight");
                    break;
                    
                case "smooth":
                    mouseSensitivity = 3f;
                    mouseSmoothing = 5f;
                    flightSpeed = 10f;
                    rollSpeed = 90f;
                    rollReturnSpeed = 45f;
                    pitchSpeed = 60f;
                    pitchReturnSpeed = 30f;
                    maxPitchAngle = 60f;
                    forwardThrustMultiplier = 1.5f;
                    useRelativeMovement = true;
                    Debug.Log("🎯 Applied SMOOTH preset - Balanced realistic flight");
                    break;
                    
                case "aggressive":
                    mouseSensitivity = 8f;
                    mouseSmoothing = 12f;
                    flightSpeed = 25f;
                    rollSpeed = 150f;
                    rollReturnSpeed = 75f;
                    pitchSpeed = 100f;
                    pitchReturnSpeed = 50f;
                    maxPitchAngle = 75f;
                    forwardThrustMultiplier = 2.5f;
                    useRelativeMovement = true;
                    Debug.Log("🎯 Applied AGGRESSIVE preset - High-speed combat flight");
                    break;
            }
        }
    }
}