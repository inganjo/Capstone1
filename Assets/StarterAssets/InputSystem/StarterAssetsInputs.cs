using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool crouch;
		public bool item1;
		public bool item2;
		public bool item3;
		public bool item4;
		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnCrouch(InputValue value)
		{
			CrouchInput(value.isPressed);
		}

/*		public void OnItem1(InputValue value)
		{
			Item1Input(value.isPressed);
		}

		public void OnItem2(InputValue value)
		{
			Item2Input(value.isPressed);
		}

		public void OnItem3(InputValue value)
		{
			Item3Input(value.isPressed);
		}

		public void OnItem4(InputValue value)
		{
			Item4Input(value.isPressed);
		}*/

#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void CrouchInput(bool newCrouchState)
		{
			crouch = newCrouchState;
		}

		public void Item1Input(bool newItem1State)
		{
			item1 = newItem1State;
		}
		public void Item2Input(bool newItem2State)
		{
			item2 = newItem2State;
		}
		public void Item3Input(bool newItem3State)
		{
			item3 = newItem3State;
		}
		public void Item4Input(bool newItem4State)
		{
			item4 = newItem4State;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}