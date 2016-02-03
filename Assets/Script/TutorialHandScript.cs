using UnityEngine;
using System.Collections;

public class TutorialHandScript : MonoBehaviour {

	public GameObject mainCenterhand;
	public Animator CenterAnim;
	public Animator handAnim;

	public void PointTo(Vector2 pos, float angle){
		mainCenterhand.transform.position = new Vector3 (pos.x, pos.y, mainCenterhand.transform.position.z);
		mainCenterhand.transform.eulerAngles = new Vector3 (0, 0, angle);
		CenterAnim.SetBool ("Active", true);
		handAnim.SetBool ("Scratch", false);
		handAnim.SetBool ("Point", true);
	}

	public void ScratchTo(Vector2 pos, float angle){
		mainCenterhand.transform.position = new Vector3 (pos.x, pos.y, mainCenterhand.transform.position.z);
		mainCenterhand.transform.eulerAngles = new Vector3 (0, 0, angle);
		CenterAnim.SetBool ("Active", true);
		handAnim.SetBool ("Scratch", false);
		handAnim.SetBool ("Point", true);
	}

	public void Stop(){
		CenterAnim.SetBool ("Active", false);
		handAnim.SetBool ("Scratch", false);
		handAnim.SetBool ("Point", false);
	}

}
