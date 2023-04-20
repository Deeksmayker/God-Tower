using UnityEngine;

namespace Code.Global.Feeling
{
    public class TransformSway : MonoBehaviour
    {
        [Header("Common")]
        [SerializeField] private Vector2 force = Vector2.one;
        [SerializeField, Min(0f)] private float multiplier = 5f;
        [SerializeField] private bool inverseX;
        [SerializeField] private bool inverseY;

        [Header("Clamp")] 
        [SerializeField] private Vector2 minMaxX;
        [SerializeField] private Vector2 minMaxY;
        
        private const string MouseX = "Mouse X";
        private const string MouseY = "Mouse Y";
        
        protected float AdditionalX;
        protected float AdditionalY;

        private float _mouseX, _mouseY;
        private float _velocityY;

        private void LateUpdate()
        {
            PerformTransformSway();
        }

        private void PerformTransformSway()
        {
            var deltaTime = Time.deltaTime;
            var inverseSwayX = inverseX ? -1f : 1f;
            var inverseSwayY = inverseY ? -1f : 1f;

            _mouseX = Input.GetAxis(MouseX) * inverseSwayX;
            _mouseY = Input.GetAxis(MouseY) * inverseSwayY;
            
            OnSwayPerforming(deltaTime);

            var currentX = _mouseY * force.y;
            var currentY = _mouseX * force.x;

            var endEulerAngleX = Mathf.Clamp(currentX + AdditionalX, minMaxX.x, minMaxX.y);
            var endEulerAngleY = Mathf.Clamp(currentY + AdditionalY, minMaxY.x, minMaxY.y);

            var moment = deltaTime * multiplier;
            var localEulerAngles = transform.localEulerAngles;
            
            localEulerAngles.x = Mathf.LerpAngle(localEulerAngles.x, endEulerAngleX, moment);
            localEulerAngles.y = Mathf.LerpAngle(localEulerAngles.y, endEulerAngleY, moment);
            localEulerAngles.z = 0f;

            transform.localEulerAngles = localEulerAngles;
        }
        
        protected virtual void OnSwayPerforming(in float deltaTime) { }
    }
}