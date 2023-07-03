using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraLook : MonoCache
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float m_XSensitivity = 2f;
      [SerializeField] private float m_YSensitivity = 2f;
      [SerializeField] private bool m_ClampVerticalRotation = true;
      [SerializeField] private float m_MinimumX = -90F;
      [SerializeField] private float m_MaximumX = 90F;
      [SerializeField] private bool m_Smooth = false;
      [SerializeField] private float m_SmoothTime = 5f;
      [SerializeField] private bool m_LockCursor = true;

      private Quaternion m_CharacterTargetRot;
      private Quaternion m_CameraTargetRot;
      private bool m_cursorIsLocked = true;

    private bool _responseToInput = true;
        
      private PlayerInput _playerInput;

      private void Awake()
      { 
          m_CharacterTargetRot = transform.localRotation;
          m_CameraTargetRot = cameraTransform.localRotation;

          _playerInput = Get<PlayerInput>();
          
          if (m_cursorIsLocked)
          {
              Cursor.visible = false;
              Cursor.lockState = CursorLockMode.Locked;
          }
          
          SetSense(SettingsController.Sensitivity);
      }

      protected override void Run()
      {
          LookRotation();
      }
      
      public void LookRotation()
      {
          if (TimeController.Instance.IsPaused || !_responseToInput || Time.timeScale == 0)
              return;
           
           var mouseDelta = _playerInput.actions["Look"].ReadValue<Vector2>();
           float yRot = mouseDelta.x * m_XSensitivity;
           float xRot = mouseDelta.y * m_YSensitivity;

           m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
           m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

           if (m_ClampVerticalRotation)
           {
               m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
           }

           if (m_Smooth)
           {
               transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CharacterTargetRot,
                    m_SmoothTime * Time.deltaTime);
               cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, m_CameraTargetRot,
                    m_SmoothTime * Time.deltaTime);
           }
           else
           {
               transform.localRotation = m_CharacterTargetRot;
               cameraTransform.localRotation = m_CameraTargetRot;
           }

           //UpdateCursorLock();
       }

        public void SetCursorLock(bool value)
        {
            m_LockCursor = value;
            if (!m_LockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void SetSense(float value)
        {
            m_XSensitivity = value;
            m_YSensitivity = value;
        }

        /*public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (m_LockCursor)
            {
                InternalLockUpdate();
            }
        }*/

        /*private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }*/

        private Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, m_MinimumX, m_MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }   

    public void DisableInputResponse()
    {
        _responseToInput = false;
    }

    public void EnableInputResponse()
    {
        _responseToInput = true;
    }
}
