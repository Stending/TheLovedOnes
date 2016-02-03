using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {

	public int actualAct = 0;

	public List<AudioSource> musicParts;
	public Act[] actsArray = new Act[12];
	// Use this for initialization
	void Awake(){
		instantPlay (0);
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	 /*if(Input.GetKeyDown("m")){
			if(actualAct<14){
				actualAct++;
				StopCoroutine("fadingTo");
				StartCoroutine(fadingTo(actualAct, 2));
			}
		}*/
	}


	public void instantPlay(int i){
		for(int j=0;j<musicParts.Count;j++){
			if(actsArray[i].parts[j]){
				musicParts[j].volume = 1.0f;
			}else{
				musicParts[j].volume = 0.0f;
			}
		}
	}

	public void SwitchToNextAct(){
		SwitchToAct (actualAct + 1);;
	}

	public void SwitchToAct(int act){
		if (act < actsArray.Length) {
			actualAct = act;
			StartCoroutine (fadingTo (actualAct, 2));
		}
	}

	public IEnumerator fadingTo(int act, int speed){
		while (!isEveryActiveTo(act, 1.0f) || !isEveryInactiveTo(act, 0.0f)) {
			for(int j=0;j<musicParts.Count;j++){
				if(actsArray[act].parts[j] && musicParts[j].volume < 1.0f)
					musicParts[j].volume += speed*0.005f;
				if(!actsArray[act].parts[j] && musicParts[j].volume > 0.0f)
					musicParts[j].volume -= speed*0.005f;
			}

			yield return null;
		}

		setEveryActiveTo (act, 1.0f);
		setEveryInactiveTo (act, 0.0f);
	}

	public IEnumerator fadingEnd(int speed){
		while (!isSomethingNotAt(0)) {
			for(int j=0;j<musicParts.Count;j++){

				if(musicParts[j].volume > 0.0f)
					musicParts[j].volume -= speed*0.005f;
			}
			
			yield return null;
		}

	}

	public bool isAct(int act, float vol){
		return(isEveryActiveTo (act, vol) && isEveryInactiveTo (act, 0.0f));
	}

	public void setEveryActiveTo(int act, float vol){
		for(int j=0;j<musicParts.Count;j++){
			if(actsArray[act].parts[j]){
				musicParts[j].volume = vol;
			}
		}
	}

	public void setEveryInactiveTo(int act, float vol){
		for(int j=0;j<musicParts.Count;j++){
			if(!actsArray[act].parts[j]){
				musicParts[j].volume = vol;
			}
		}
	}
	
	public bool isEveryActiveTo(int act, float vol){
		for(int j=0;j<musicParts.Count;j++){
			if((actsArray[act].parts[j] && musicParts[j].volume != vol)){
				return false;
			}
		}
		return true;
	}
	
	public bool isEveryInactiveTo(int act, float vol){
		for(int j=0;j<musicParts.Count;j++){
			if((!actsArray[act].parts[j] && musicParts[j].volume != vol)){
				return false;
			}
		}
		return true;
	}

	public bool isSomethingNotAt(float vol){
		for(int j=0;j<musicParts.Count;j++){
			if((musicParts[j].volume != vol)){
				return false;
			}
		}
		return true;
	}
}