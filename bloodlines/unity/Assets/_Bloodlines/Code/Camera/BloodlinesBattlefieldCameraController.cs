using UnityEngine;
using UnityEngine.InputSystem;
using UnityCamera = UnityEngine.Camera;

namespace Bloodlines.Camera
{
    /// <summary>
    /// First battlefield camera shell for the Unity production lane.
    /// This is intentionally hybrid MonoBehaviour control rather than an ECS camera
    /// system so the first playable shell can be exercised quickly while simulation
    /// state remains ECS-native.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BloodlinesBattlefieldCameraController : MonoBehaviour
    {
        [SerializeField] private UnityCamera controlledCamera = null;
        [SerializeField] private Transform cameraPivot = null;
        [SerializeField] private float panSpeed = 28f;
        [SerializeField] private float fastPanMultiplier = 1.65f;
        [SerializeField] private float edgeScrollPixels = 14f;
        [SerializeField] private float dragPanSensitivity = 0.03f;
        [SerializeField] private float rotationSpeedDegrees = 92f;
        [SerializeField] private float zoomSpeed = 20f;
        [SerializeField] private float zoomSmoothing = 12f;
        [SerializeField] private float minZoomDistance = 18f;
        [SerializeField] private float maxZoomDistance = 52f;
        [SerializeField] private float minPitchDegrees = 38f;
        [SerializeField] private float maxPitchDegrees = 62f;
        [SerializeField] private Vector2 mapMin = Vector2.zero;
        [SerializeField] private Vector2 mapMax = new Vector2(76f, 76f);
        [SerializeField] private Vector3 focusPosition = new Vector3(38f, 0f, 38f);
        [SerializeField] private float yawDegrees = 0f;
        [SerializeField] private float zoomDistance = 28f;

        private float targetZoomDistance;
        private Vector2 previousMousePosition;
        private bool middleMouseDragging;
        private bool initialized;

        private void Reset()
        {
            EnsureHierarchy();
            InitializeStateIfNeeded();
            SnapRigToState();
        }

        private void Awake()
        {
            EnsureHierarchy();
            InitializeStateIfNeeded();
            SnapRigToState();
        }

        private void OnEnable()
        {
            EnsureHierarchy();
            InitializeStateIfNeeded();
            SnapRigToState();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            EnsureHierarchy(allowTagAssignment: false);
            InitializeStateIfNeeded();
            SnapRigToState();
        }
#endif

        private void Update()
        {
            EnsureHierarchy();
            InitializeStateIfNeeded();

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            float dt = Time.deltaTime;
            float zoomT = Mathf.InverseLerp(minZoomDistance, maxZoomDistance, zoomDistance);
            float currentPanSpeed = panSpeed * Mathf.Lerp(0.8f, 1.45f, zoomT);
            if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
            {
                currentPanSpeed *= fastPanMultiplier;
            }

            Vector2 planarInput = GetKeyboardPanInput(keyboard) + GetEdgePanInput(Mouse.current);
            if (planarInput.sqrMagnitude > 1f)
            {
                planarInput.Normalize();
            }

            Quaternion yawRotation = Quaternion.Euler(0f, yawDegrees, 0f);
            focusPosition += yawRotation * new Vector3(planarInput.x, 0f, planarInput.y) * currentPanSpeed * dt;

            HandleDragPan(Mouse.current, yawRotation, zoomT);
            HandleRotation(keyboard, dt);
            HandleZoom(Mouse.current, dt);

            focusPosition.x = Mathf.Clamp(focusPosition.x, Mathf.Min(mapMin.x, mapMax.x), Mathf.Max(mapMin.x, mapMax.x));
            focusPosition.z = Mathf.Clamp(focusPosition.z, Mathf.Min(mapMin.y, mapMax.y), Mathf.Max(mapMin.y, mapMax.y));

            SnapRigToState();
        }

        public void ApplyMapConfiguration(Vector2 worldMin, Vector2 worldMax, Vector2 initialFocus)
        {
            EnsureHierarchy();
            InitializeStateIfNeeded();
            mapMin = worldMin;
            mapMax = worldMax;
            focusPosition = ClampFocusPosition(new Vector3(initialFocus.x, 0f, initialFocus.y));
            targetZoomDistance = zoomDistance;
            SnapRigToState();
        }

        public void FocusWorldPosition(Vector3 worldPosition)
        {
            EnsureHierarchy();
            InitializeStateIfNeeded();
            focusPosition = ClampFocusPosition(worldPosition);
            targetZoomDistance = zoomDistance;
            SnapRigToState();
        }

        private void EnsureHierarchy(bool allowTagAssignment = true)
        {
            if (cameraPivot == null)
            {
                var existingPivot = transform.Find("CameraPivot");
                if (existingPivot == null)
                {
                    var pivotObject = new GameObject("CameraPivot");
                    pivotObject.transform.SetParent(transform, false);
                    existingPivot = pivotObject.transform;
                }

                cameraPivot = existingPivot;
            }

            if (controlledCamera == null)
            {
                controlledCamera = cameraPivot.GetComponentInChildren<UnityCamera>(true);
                if (controlledCamera == null)
                {
                    var cameraObject = new GameObject("Main Camera");
                    cameraObject.transform.SetParent(cameraPivot, false);
                    controlledCamera = cameraObject.AddComponent<UnityCamera>();
                    cameraObject.AddComponent<AudioListener>();
                }
            }

            ConfigureCameraComponent(allowTagAssignment);
        }

        private void ConfigureCameraComponent(bool allowTagAssignment = true)
        {
            if (controlledCamera == null)
            {
                return;
            }

            if (allowTagAssignment)
            {
                controlledCamera.tag = "MainCamera";
            }

            controlledCamera.clearFlags = CameraClearFlags.SolidColor;
            controlledCamera.backgroundColor = new Color(0.57f, 0.62f, 0.68f, 1f);
            controlledCamera.orthographic = false;
            controlledCamera.fieldOfView = 40f;
            controlledCamera.nearClipPlane = 0.1f;
            controlledCamera.farClipPlane = 500f;
        }

        private void InitializeStateIfNeeded()
        {
            if (initialized)
            {
                return;
            }

            focusPosition = transform.position;
            yawDegrees = transform.eulerAngles.y;

            if (controlledCamera != null)
            {
                zoomDistance = Mathf.Clamp(Mathf.Abs(controlledCamera.transform.localPosition.z), minZoomDistance, maxZoomDistance);
            }

            if (zoomDistance <= 0f)
            {
                zoomDistance = 28f;
            }

            targetZoomDistance = zoomDistance;
            initialized = true;
        }

        private void HandleDragPan(Mouse mouse, Quaternion yawRotation, float zoomT)
        {
            if (mouse == null)
            {
                middleMouseDragging = false;
                return;
            }

            if (mouse.middleButton.wasPressedThisFrame)
            {
                previousMousePosition = mouse.position.ReadValue();
                middleMouseDragging = true;
            }
            else if (mouse.middleButton.wasReleasedThisFrame)
            {
                middleMouseDragging = false;
            }

            if (!middleMouseDragging || !mouse.middleButton.isPressed)
            {
                return;
            }

            Vector2 currentMousePosition = mouse.position.ReadValue();
            Vector2 delta = currentMousePosition - previousMousePosition;
            previousMousePosition = currentMousePosition;

            float dragScale = Mathf.Lerp(0.65f, 1.75f, zoomT);
            Vector3 dragOffset = yawRotation * new Vector3(-delta.x, 0f, -delta.y) * (dragPanSensitivity * dragScale);
            focusPosition += dragOffset;
        }

        private void HandleRotation(Keyboard keyboard, float dt)
        {
            if (keyboard.qKey.isPressed)
            {
                yawDegrees -= rotationSpeedDegrees * dt;
            }

            if (keyboard.eKey.isPressed)
            {
                yawDegrees += rotationSpeedDegrees * dt;
            }
        }

        private void HandleZoom(Mouse mouse, float dt)
        {
            if (mouse != null)
            {
                float scrollDelta = mouse.scroll.ReadValue().y;
                if (Mathf.Abs(scrollDelta) > Mathf.Epsilon)
                {
                    targetZoomDistance -= scrollDelta * 0.02f * zoomSpeed;
                }
            }

            targetZoomDistance = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
            zoomDistance = Mathf.Lerp(zoomDistance, targetZoomDistance, 1f - Mathf.Exp(-zoomSmoothing * dt));
        }

        private static Vector2 GetKeyboardPanInput(Keyboard keyboard)
        {
            Vector2 input = Vector2.zero;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                input.y += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }

            return input;
        }

        private Vector2 GetEdgePanInput(Mouse mouse)
        {
            if (mouse == null || !Application.isFocused)
            {
                return Vector2.zero;
            }

            Vector2 edgeInput = Vector2.zero;
            Vector2 mousePosition = mouse.position.ReadValue();

            if (mousePosition.x <= edgeScrollPixels)
            {
                edgeInput.x -= 1f;
            }
            else if (mousePosition.x >= Screen.width - edgeScrollPixels)
            {
                edgeInput.x += 1f;
            }

            if (mousePosition.y <= edgeScrollPixels)
            {
                edgeInput.y -= 1f;
            }
            else if (mousePosition.y >= Screen.height - edgeScrollPixels)
            {
                edgeInput.y += 1f;
            }

            return edgeInput;
        }

        private void SnapRigToState()
        {
            if (cameraPivot == null || controlledCamera == null)
            {
                return;
            }

            transform.position = focusPosition;
            transform.rotation = Quaternion.Euler(0f, yawDegrees, 0f);

            float pitchT = Mathf.InverseLerp(minZoomDistance, maxZoomDistance, zoomDistance);
            float pitch = Mathf.Lerp(maxPitchDegrees, minPitchDegrees, pitchT);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            controlledCamera.transform.localPosition = new Vector3(0f, 0f, -zoomDistance);
            controlledCamera.transform.localRotation = Quaternion.identity;
        }

        private Vector3 ClampFocusPosition(Vector3 worldPosition)
        {
            return new Vector3(
                Mathf.Clamp(worldPosition.x, Mathf.Min(mapMin.x, mapMax.x), Mathf.Max(mapMin.x, mapMax.x)),
                0f,
                Mathf.Clamp(worldPosition.z, Mathf.Min(mapMin.y, mapMax.y), Mathf.Max(mapMin.y, mapMax.y)));
        }
    }
}
