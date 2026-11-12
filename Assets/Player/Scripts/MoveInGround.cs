using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Player.Scripts
{
    class MoveInGround : AbsMove
    {
        public MoveInGround(Rigidbody rb,
            Transform transform,
            InputAction move,
            InputAction sprint,
            StaminaComponet stamina,
            float moveSpeed) : base(rb, transform, move, sprint, stamina, moveSpeed ) { }

        public override void Move()
        {
            base.Move();
            // Sprint & Stamina evaluation / Đánh giá điều kiện chạy nhanh & hao hụt thể lực
            bool isSprinting = _sprint.IsPressed() && _stamina != null && _stamina.currentStamina > 1f && moveInput.sqrMagnitude > 0;

            if (isSprinting)
            {
                _currentSpeed = _moveSpeed * 1.8f;
                _stamina.TakeStamina(15f * Time.fixedDeltaTime);
            }
            else
            {
                _currentSpeed = _moveSpeed;
                if (_stamina != null)
                    _stamina.RecoveryStamina(10f * Time.fixedDeltaTime);
            }

            // Velocity application / Áp dụng vận tốc di chuyển vào Rigidbody
            _rb.linearVelocity = new Vector3(moveDirection.x * _currentSpeed , _rb.linearVelocity.y, moveDirection.z * _currentSpeed);
        }
    }
}
