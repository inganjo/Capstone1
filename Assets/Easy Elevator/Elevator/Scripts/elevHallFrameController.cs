using UnityEngine;
using System.Collections;

public class elevHallFrameController : MonoBehaviour {

	public int floor;
	public Transform callButtonLight;		
	public Transform HallLedPanel;
	[Tooltip("The tag of the elevator that will use this hall frame")]
	public string elevTag = "elev01";
	[Tooltip("Use the Animation component instead of of the Animator ")]
	public bool legacyAnimation = true; 

	private	elevControl elevator;
	private	AnimationClip openAnim, closeAnim;
	[HideInInspector]
	public Animator animator;

	void Start(){
		//GRAB ELEVATOR REF
		elevator = GameObject.FindGameObjectWithTag( elevTag ).transform.GetComponent<elevControl>();

		//SET ANIMATION CLIPS (LEGACY)
		if (legacyAnimation) {
			openAnim = transform.GetComponent<Animation> ().GetClip ("OpenDoorsV2");
			closeAnim = transform.GetComponent<Animation> ().GetClip ("CloseDoorsV2");
		} else {
			animator = GetComponent<Animator> ();
		}
	}

	/// <summary>
	/// Opens the Hall Frame Doors.
	/// </summary>
	public void OpenDoor( bool legacy ){
		if (legacy) {
			transform.GetComponent<Animation> ().clip = openAnim;
			transform.GetComponent<Animation> ().Play ();
		} else {
			animator.SetInteger ("doorState", 1 );
		}
			
		
	}

	/// <summary>
	/// Closes the Hall Frame Door.
	/// </summary>
	public void CloseDoor( bool legacy){
		if (legacy) {
		transform.GetComponent<Animation>().clip = closeAnim;
		transform.GetComponent<Animation>().Play();
		} else {
			animator.SetInteger ("doorState", 0 );
		}
	}

	/// <summary>
	/// Turn Call Button light ON/OFF .
	public void CallButtonLight( bool turnOn ){
		//CHANGE BUTTON OBJECT MATERIAL
		if( turnOn )	
			callButtonLight.GetComponent<Renderer>().material = elevator.buttonOnMat;
		else
			callButtonLight.GetComponent<Renderer>().material  = elevator.buttonOffMat;
	}

	public void CallElevator(){
		CallButtonLight( true );
		elevator.MoveElevator( floor, true );
	}
}
