using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f;

    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    Transform center;

    [SerializeField]
    float boundaryRadius = 12.3f;

    CharacterController controller;

    Vector2 moveInput;
    Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Gravity();
        ClampInsideSphere();
    }

    void Move()
    {
        // カメラ方向
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // 上下方向を消す
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // カメラ基準移動
        Vector3 move =
            forward * moveInput.y +
            right * moveInput.x;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        controller.Move(
            move * moveSpeed * Time.deltaTime
        );
    }

    void Gravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }

    void ClampInsideSphere()
    {
        if (center == null) return;

        Vector3 dir = transform.position - center.position;
        float dist = dir.magnitude;

        if (dist > boundaryRadius)
        {
            // はみ出した分だけをMoveで戻す(直接position操作しない)
            Vector3 targetPos = center.position + dir.normalized * boundaryRadius;
            Vector3 correction = targetPos - transform.position;
            controller.Move(correction);
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}