using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Player.Scripts
{
    internal class CheckLayer
    {
        public enum SurfaceType { None, Ground, Water, ex1, ex2, ex3 }
        public SurfaceType surface { set; get; } = SurfaceType.None; /* => */ public bool IsGrounded => surface != SurfaceType.None;
        private RaycastHit _groundHit;

        //Get value in PlayerController: MonoBehaviour / Lấy dữ liệu từ PlayerController: MonoBehaviour
        private readonly Transform _cameraPlayer;
        private readonly float _rayLength;
        private readonly LayerMask _walkableLayers;

        public CheckLayer(Transform _cameraPlayer, float _rayLength, LayerMask _walkableLayers) { }

        public void CheckSurface()
        {
            // Downward raycast detection / Bắn tia dò mặt đất hướng xuống
            if (Physics.Raycast(_cameraPlayer.position, Vector3.down, out _groundHit, _rayLength, _walkableLayers))
            {
                // Layer-to-Name resolution / Lấy tên layer va chạm
                string layerName = LayerMask.LayerToName(_groundHit.collider.gameObject.layer);

                // Surface type mapping / Ánh xạ loại bề mặt tương ứng
                switch (layerName)
                {
                    case "Water":
                        surface = SurfaceType.Water;
                        break;

                    case "ex1":
                        surface = SurfaceType.ex1;
                        break;

                    case "Ground":
                    default:
                        surface = SurfaceType.Ground;
                        break;
                }
            }
            else
            {
                // Airborne fallback / Mặc định trạng thái trên không
                surface = SurfaceType.None;
            }
        }
    }
}
