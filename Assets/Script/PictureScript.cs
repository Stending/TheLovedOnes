using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PictureScript : MonoBehaviour {

	public SpriteRenderer pictureSpriteRend;
	public Object ScratchPicture;

	public int state = 0;
	public int lastState = 30;
	public List<Sprite> states;
	public Animator flashAnim;


	public AudioClip photoSound;
	public AudioSource audioSource;


	public void Downgrade(){
		if(state+1<states.Count)
			ChangeToState (state + 1);
	}

	public void ChangeToState(int s){
		state = s;
		Flash ();
		pictureSpriteRend.sprite = states [state];
	}

	public void Flash(){
		flashAnim.Play ("Flash");
		audioSource.PlayOneShot (photoSound);
	}

	public void BecomeMemory(){
		ChangeToState (lastState + 1);
	}

	public ScratchZone HideWithScratchZone(){
		GameObject scratchGO = Instantiate (ScratchPicture) as GameObject;
		scratchGO.transform.parent = this.transform;
		scratchGO.transform.localPosition = new Vector3 (0, 0, -1);
		scratchGO.transform.localEulerAngles = new Vector3 (0, 0, 0);
		scratchGO.transform.localScale = new Vector3 (1, 1, 1);
		return scratchGO.GetComponent<ScratchZone> ();
	}

	public bool IsFinished{
		get{ return state == lastState;}
		set{ ;}
	}

}
