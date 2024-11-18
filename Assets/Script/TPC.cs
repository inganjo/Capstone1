 using UnityEngine;
 using System.Collections;
 using Photon.Pun; 
 using Photon.Pun.Demo.PunBasics;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class TPC : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Crouch speed of the character in m/s")]
        public float CrouchSpeed = 1.0f;

        [Tooltip("Push speed of the character in m/s")]
        public float PushSpeed = 0.5f;

        [Tooltip("TwoHand Holding speed coefficient of the character in m/s")]
        public float TwoHandCoef = 0.5f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        public bool isHold = false;
        private int equipNum;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        //Item
        public GameObject item_L;
        public GameObject item_R;
        public ParticleSystem extEffect;
        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // inventory
        private Inventory inventory;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        //Aiming
        private Transform cameraTransform;
        float xAxis,zAxis;
        float temp=0;

        //BasicRigidbody Push
        public LayerMask pushLayers;
        public bool canPush;
        private bool isPushing = false;
        public float rayDistance = 0.5f;  // 레이캐스트 거리
        public LayerMask pushableLayer;   // Pushable 물체의 레이어
        [Range(0.1f, 5f)] public float strength = 1.1f;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDCrouch;
        private int _animIDTwoHanded;
        private int _animIDOneHanded;
        private int _animIDIsPush;
        private int _animIDStopPush;
        private int _animIDUseItem;
        private int _animIDMoveX;
        private int _animIDMoveZ;



#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private GameObject _cameraRoot;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // PhotonView photonView = GetComponent<PhotonView>();
            // if(photonView.IsMine)
            // {
            //     PlayerManager.LocalPlayerInstance = this.gameObject;
            // }
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            if (_cameraRoot == null)
            {
                _cameraRoot = transform.GetChild(0).gameObject;
            }

            if (inventory == null)
            {
                inventory=GetComponent<Inventory>();
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            cameraTransform=Camera.main.transform;
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            Debug.Log(_input);
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if(PhotonNetwork.IsConnected == true){
                PhotonView photonView = GetComponent<PhotonView>();
                if(photonView.IsMine)
                {
                    // Debug.Log("PhotonNetwork working");
                    PlayerAction();
                }
            }
            else{
                PlayerAction();
            }
        }
        private void PlayerAction(){
                _hasAnimator = TryGetComponent(out _animator);
                MoveDirection();
                JumpAndGravity();
                GroundedCheck();
                EquipItem();
                Crouch();
                Move();
                CheckCanPush();
                UseItem();
        }
        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDCrouch = Animator.StringToHash("Crouch");
            _animIDTwoHanded=Animator.StringToHash("TwoHanded");
            _animIDOneHanded=Animator.StringToHash("OneHanded");
            _animIDIsPush=Animator.StringToHash("IsPush");
            _animIDStopPush=Animator.StringToHash("StopPush");
            _animIDUseItem=Animator.StringToHash("UseItem");
            _animIDMoveX=Animator.StringToHash("MoveX");
            _animIDMoveZ=Animator.StringToHash("MoveZ");
        }

        private bool IsAnimationPlaying(string animationName)
        {
            // 현재 애니메이터 상태 정보 가져오기
            AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);

            // 애니메이션 이름과 상태가 일치하는지 확인
            return currentState.IsName(animationName) && currentState.normalizedTime < 1.0f;
        }
        private void MoveDirection()
        {
            xAxis = Input.GetAxis("Horizontal");
            zAxis = Input.GetAxis("Vertical");
            if (Mathf.Abs(xAxis) < 0.1f) xAxis = 0.0f;
            if (Mathf.Abs(zAxis) < 0.1f) zAxis = 0.0f;

            // 좌우 이동 값
            _animator.SetFloat(_animIDMoveX, xAxis);
            // 전후 이동 값
            _animator.SetFloat(_animIDMoveZ, zAxis);
        }
        private void CheckCanPush() {
                Vector3 forward = transform.TransformDirection(Vector3.forward);            
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                Debug.DrawRay(new Vector3(transform.position.x,transform.position.y+0.9f,transform.position.z),forward,new Color(0,1,0));
                if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y+0.9f,transform.position.z),forward, rayDistance,pushableLayer) 
                    && !isHold && Grounded && !(_animator.GetBool(_animIDCrouch))) {
                    canPush=true;
                }
                else {
                canPush=false;
                }
            }
            else {
                canPush=false;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (canPush) PushRigidBodies(hit);
            else StopPushAnimation();
        }

        private void PushRigidBodies(ControllerColliderHit hit)
        {
            // https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html\
            // make sure we hit a non kinematic rigidbody
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            // make sure we only push desired layer(s)
            var bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & pushLayers.value) == 0) return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f) return;

            // Calculate push direction from move direction, horizontal motion only
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
            // Apply the push and take strength into account
            body.AddForce(pushDir * strength, ForceMode.Impulse);
            if (!isPushing)  // 이미 밀기 중인 경우 새로 시작하지 않음
            {
                StartCoroutine(WaitForPush());
            }
        }

        private IEnumerator WaitForPush()
        {
            isPushing = true;
            _animator.SetBool(_animIDIsPush, true);  // 밀기 애니메이션 시작

            while (canPush && isPushing)  // 밀기가 지속되는 동안 루프
            {
                yield return null;  // 다음 프레임까지 대기
            }

            StopPushAnimation();  // 밀기가 멈추면 애니메이션 중단
        }

        private void StopPushAnimation()
        {
            if (isPushing)
            {
                _animator.SetBool(_animIDIsPush, false);  // 애니메이션 종료
                isPushing = false;
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            float targetSpeed;
            // set target speed based on move speed, sprint speed and if sprint is pressed
            if (_animator.GetBool(_animIDTwoHanded))
            {
                targetSpeed=(_input.crouch ? CrouchSpeed : MoveSpeed)*TwoHandCoef;
            }
            else if (_animator.GetBool(_animIDIsPush))
            {
                targetSpeed=PushSpeed;
            }
            else
            {
                targetSpeed = _input.crouch ? CrouchSpeed : _input.sprint ? SprintSpeed : MoveSpeed;
            }

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.fire) {
                _animator.SetBool(_animIDUseItem,true);
            }
            else _animator.SetBool(_animIDUseItem,false);


            if (_input.fire && _animator.GetBool(_animIDTwoHanded))
            {
                _targetRotation = _mainCamera.transform.eulerAngles.y;
            }
            else if (_input.move != Vector2.zero)  // 비조준 상태에서는 이동 방향 기준으로 회전
            {
                 _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            }

            // Smooth rotation 적용
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            Vector3 moveDirection = transform.right * xAxis + transform.forward * zAxis;
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            if (_input.fire && _animator.GetBool(_animIDTwoHanded))
            {
                _controller.Move(moveDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else if (_input.move != Vector2.zero)  // 움직임이 있을 때만 이동
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else
            {
                // 이동 입력이 없는 경우에도 중력은 적용
                _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private int CheckInput()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1)) // 숫자 1 키를 뗄 때
            {
                return 1; 
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2)) // 숫자 2 키를 뗄 때
            {
                return 2; 
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3)) // 숫자 3 키를 뗄 때
            {
                return 3; 
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4)) // 숫자 4 키를 뗄 때
            {
                return 4; 
            }         
            return -1;
        }

        private void EquipItem()
        {
            int inputSlot=CheckInput();
            if (Grounded) {
                if (inputSlot >= 0 && inputSlot <= inventory.instance.items.Count)
                {
                    if (!isHold)
                    {
                        // 아이템 들기 동작
                        ItemHold(inputSlot);
                        equipNum = inputSlot; // 현재 들고 있는 아이템 슬롯 번호 갱신
                        FindObjectOfType<InventoryUI>().SetCurrentSlot(inputSlot - 1);
                    }
                    else if (isHold && inputSlot == equipNum)
                    {
                        // 아이템 내려놓기
                        ItemPutDown();
                        equipNum=-2;
                        FindObjectOfType<InventoryUI>().SetCurrentSlot(-1); // 선택 해제
                    }
                    else
                    {
                        // 다른 아이템으로 교체
                        ItemPutDown();
                        ItemHold(inputSlot);
                        equipNum = inputSlot;
                        FindObjectOfType<InventoryUI>().SetCurrentSlot(inputSlot - 1); // InventoryUI에 슬롯 번호 전달
                    }
                }
                else
                {
                    
                }
            }
        }

        private void ItemHold(int slotNumber)
        {
           if (inventory.instance.items.Count >= slotNumber)
            {
                Item selectedItem = inventory.instance.items[slotNumber - 1]; // Inventory의 items 리스트 사용
                if (selectedItem != null)
                {
                    if (selectedItem.itemType == ItemType.OneHand)
                    {
                        _animator.SetBool(_animIDOneHanded,true);
                        item_L.SetActive(true);
                    }
                    else if (selectedItem.itemType == ItemType.TwoHand)
                    {
                        _animator.SetBool(_animIDTwoHanded,true);
                        item_R.SetActive(true);
                    }
                    isHold=true;
                }
                else
                {
                    Debug.Log("해당 슬롯에 아이템이 없습니다.");
                }
            }
            else
            {
                Debug.Log("해당 슬롯에 아이템이 없습니다.");
            }
        }
        private void ItemPutDown()
        {
            if (_animator.GetBool(_animIDOneHanded)) _animator.SetBool(_animIDOneHanded,false);
            if (_animator.GetBool(_animIDTwoHanded)) _animator.SetBool(_animIDTwoHanded,false);
            item_L.SetActive(false);
            item_R.SetActive(false);
            isHold=false;
        }

        private void Crouch()
        {
            if (Grounded)
            {
                if (_input.crouch)
                {
                    if(_animator.GetBool(_animIDTwoHanded)){
                        _controller.height=1.2f;
                        _controller.center=new Vector3(0,0.6f,0);
                        Vector3 tmpTrans=_cameraRoot.transform.position;
                        _cameraRoot.transform.position=new Vector3(tmpTrans.x,transform.position.y+1.1f,tmpTrans.z);
                    }
                    else if(_animator.GetBool(_animIDOneHanded)){
                        _controller.height=1.2f;
                        _controller.center=new Vector3(0,0.6f,0);
                        Vector3 tmpTrans=_cameraRoot.transform.position;
                        _cameraRoot.transform.position=new Vector3(tmpTrans.x,transform.position.y+1.2f,tmpTrans.z);
                    }
                    else{
                        _controller.height=0.8f;
                        _controller.center=new Vector3(0,0.44f,0);
                        Vector3 tmpTrans=_cameraRoot.transform.position;
                        _cameraRoot.transform.position=new Vector3(tmpTrans.x,transform.position.y+0.9f,tmpTrans.z);
                    }
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDCrouch, true);
                    }
                }
                else
                {
                    _controller.height=1.8f;
                    _controller.center=new Vector3(0,0.99f,0);
                    Vector3 tmpTrans=_cameraRoot.transform.position;
                    _cameraRoot.transform.position=new Vector3(tmpTrans.x,transform.position.y+1.5f,tmpTrans.z);
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDCrouch, false);
                    }
                }
            }
        }
        
        private void UseItem() {
            if (_animator.GetBool(_animIDTwoHanded))
            {
                if (_input.fire)
                {
                    ActivateParticle();
                }
                else
                {
                    DeactivateParticle();
                }
            }
            if (_animator.GetBool(_animIDOneHanded))
            {
                if(_input.fire)
                {
                    if (temp<=1){
                        temp+=Time.deltaTime*3;
                    }
                    _animator.SetLayerWeight(1,temp);
                }
                else
                {
                    if (temp>=0){
                        temp-=Time.deltaTime*3;
                    }
                    _animator.SetLayerWeight(1,temp);
                }
            }
        }

        void ActivateParticle()
        {
            if (!extEffect.isPlaying)
            {
                extEffect.Play();
            }
        }

        void DeactivateParticle()
        {
            if (extEffect.isPlaying)
            {
                extEffect.Stop();
            }
        }
        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && !(_animator.GetBool(_animIDTwoHanded)) && !(_animator.GetBool(_animIDCrouch)))
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}