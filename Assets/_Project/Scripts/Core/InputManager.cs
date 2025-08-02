using UnityEngine;

namespace PlasmaDragon.Core
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InputManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("InputManager");
                        _instance = go.AddComponent<InputManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Input Settings")]
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private bool _invertY = false;

        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool FirePrimaryPressed { get; private set; }
        public bool FireSecondaryPressed { get; private set; }
        public bool WeaponSwitchPressed { get; private set; }
        public bool PausePressed { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            UpdateMovementInput();
            UpdateLookInput();
            UpdateActionInputs();
        }

        private void UpdateMovementInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            MovementInput = new Vector2(horizontal, vertical);
        }

        private void UpdateLookInput()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            
            if (_invertY)
                mouseY = -mouseY;
                
            LookInput = new Vector2(mouseX, mouseY);
        }

        private void UpdateActionInputs()
        {
            JumpPressed = Input.GetButtonDown("Jump");
            FirePrimaryPressed = Input.GetButton("Fire1");
            FireSecondaryPressed = Input.GetButton("Fire2");
            WeaponSwitchPressed = Input.GetKeyDown(KeyCode.Q);
            PausePressed = Input.GetKeyDown(KeyCode.Escape);
        }

        public void SetMouseSensitivity(float sensitivity)
        {
            _mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 10f);
        }

        public void SetInvertY(bool invert)
        {
            _invertY = invert;
        }
    }
}