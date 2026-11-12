using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Player.Scripts
{
    public abstract class AbsMove
    {
        protected readonly Rigidbody _rb;
        protected readonly Transform _transform;
        protected readonly InputAction _move;
        protected readonly InputAction _sprint;
        protected readonly StaminaComponet _stamina;
        protected readonly float _moveSpeed;
        
        protected float _currentSpeed;
        protected Vector2 moveInput;
        protected Vector3 moveDirection;

        protected AbsMove(
            Rigidbody rb,
            Transform transform,
            InputAction move,
            InputAction sprint,
            StaminaComponet stamina,
            float moveSpeed)
        {
            this._rb = rb;
            this._transform = transform;
            this._move = move;
            this._sprint = sprint;
            this._stamina = stamina;
            this._moveSpeed = moveSpeed;
        }
        
        public virtual void Move()
        {
            moveInput = this._move.ReadValue<Vector2>();
            moveDirection = (moveInput.y * this._transform.forward + moveInput.x * this._transform.right).normalized;
        }
    }
}
