using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Player.Scripts;

// Surface types definition / Định nghĩa các loại bề mặt

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    #region Variables: Inspector Settings
    [Header("Movement Settings / Cài đặt di chuyển")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;

    [Header("Look / Camera Settings / Cài đặt góc nhìn Camera")]
    [SerializeField] private Transform _cameraPlayer;
    [SerializeField] private float _sentiviteMouse = 0.2f;

    [Header("Ground Check Settings / Cài đặt kiểm tra mặt đất")]
    [SerializeField] private float _rayLength = 1.1f;
    [SerializeField] private LayerMask _walkableLayers; // Walkable surface filter / Bộ lọc layer có thể đi lên

    [Header("Debug UI / Giao diện gỡ lỗi")]
    [SerializeField] private TMP_Text _textLog;
    #endregion

    #region Variables: Internal References & States
    // Component References / Tham chiếu Component nội bộ
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private StaminaComponet _stamina;
    private AbsMove _movementGround;
    private CheckLayer _checkLayer;

    // Input Action handles / Bộ lắng nghe sự kiện nút bấm
    private InputAction _move;
    private InputAction _sprint;
    private InputAction _jump;

    // Runtime state tracking / Biến lưu trữ trạng thái chạy ngầm
    private float _currentSpeed;
    private float _xRotation = 0f;
    private AbsMove _currentMovement;

    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        // Physics & Component Setup / Khởi tạo vật lý & nạp component
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _playerInput = GetComponent<PlayerInput>();
        _stamina = GetComponent<StaminaComponet>();

        // Bind Input Actions / Liên kết các phím bấm
        _move = _playerInput.actions.FindAction("Player/Move");
        _sprint = _playerInput.actions.FindAction("Player/Sprint");
        _jump = _playerInput.actions.FindAction("Player/Jump");

        // Lock & hide cursor / Khóa và ẩn con trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Class ngoài
        _movementGround = new MoveInGround(_rb, transform, _move, _sprint, _stamina, _moveSpeed);
        _checkLayer = new CheckLayer(_cameraPlayer, _rayLength, _walkableLayers);

    }

    private void OnEnable()
    {
        // Enable input events / Kích hoạt nhận tín hiệu phím
        _move?.Enable();
        _sprint?.Enable();
        _jump?.Enable();
    }

    private void OnDisable()
    {
        // Disable input events / Hủy nhận tín hiệu phím
        _move?.Disable();
        _sprint?.Disable();
        _jump?.Disable();
    }

    private void Update()
    {
        // Per-frame camera, ground check & jump input / Xử lý xoay cam, dò đất & nhận phím nhảy mỗi khung hình
        ControllerView();
        _checkLayer.CheckSurface(); //Cập nhật và kiểm tra loại surface

        if (_checkLayer.IsGrounded && _jump.WasPressedThisFrame()) PlayerJump();

        DebugLog();
    }

    private void FixedUpdate()
    {
        // Physics movement execution / Thực thi di chuyển vật lý
        SelectMovementState();
    }
    #endregion

    #region Movement & Action Logic
    private void SelectMovementState()
    {
        switch (_checkLayer.surface)
        {
            case CheckLayer.SurfaceType.Ground: 
                _currentMovement = _movementGround;
                break;
            case CheckLayer.SurfaceType.Water:
                // 
                break;
            default:
                _currentMovement = _movementGround;
                break;
        }
    }

    private void PlayerJump()
    {
        // Vertical impulse application / Reset vận tốc Y và đẩy lực nhảy tức thì
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void ControllerView()
    {
        if (Mouse.current == null || _cameraPlayer == null) return;

        // Mouse Delta Look calculation / Tính toán độ xoay góc nhìn chuột
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * _sentiviteMouse;

        // Vertical pitch clamp / Giới hạn góc nhìn lên xuống (-90 đến 90 độ)
        _xRotation -= mouseDelta.y;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _cameraPlayer.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x); // Horizontal yaw / Xoay thân nhân vật trái phải
    }
    #endregion

    #region Debugging
    private void DebugLog()
    {
        // UI Debug Text update / Cập nhật văn bản hiển thị thông số gỡ lỗi
        if (_textLog != null && _stamina != null)
        {
            _textLog.text = $"Tốc độ: {_currentSpeed:F1}\n" +
                            $"Thể lực: {_stamina.currentStamina:F0}/{_stamina.maxStamina:F0}\n" +
                            $"Bề mặt: {CurrentSurface}\n" +
                            $"Chạm đất: {IsGrounded}";
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    // Editor Raycast visualization / Hiển thị tia dò đất trực quan trong cửa sổ Scene
    //    Gizmos.color = IsGrounded ? Color.green : Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _rayLength);
    //}
    #endregion
}