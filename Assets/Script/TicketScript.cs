using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TicketScript : MonoBehaviour {


	public GameScript game;

	public delegate void ActivateTicketAction(TicketScript ts);
	public event ActivateTicketAction TicketActivation = delegate {};

	public delegate void ActivateAction();
	public event ActivateAction NormalActivation = delegate {};
	public event ActivateAction NormalValidation= delegate {};

	public delegate void SendGainAction(string gain);
	public event SendGainAction TicketFinished  = delegate {};
	public event SendGainAction TicketVisualized = delegate {};
	

	public SpriteRenderer backgroundSpriteRend;

	public Sprite winSprite;
	public Sprite loseSprite;
	public Image winTextImg;
	public Image loseTextImg;
	public Text resultText;

	public Animator anim;
	public string state = "click";
	public ScratchZone scratchZone;
	public string gain = "won:3";

	void Start(){
		game = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameScript>();
	}

	void Update(){

		/*if (Input.GetMouseDown ()) {
			if (state == "click") {
				anim.SetBool("Corner", false);
				anim.SetBool("Table", true);
				state = "scratch";
				scratchZone.Scratched += valideTicket;
				Activation(this.GetComponent<TicketScript>());
			}
		}*/


	}

	void OnMouseDown(){
		if (state == "click") {
			GoToTable();
			state = "scratch";
			scratchZone.Scratched += valideTicket;
			scratchZone.Visualized += visualizeTicket;
			TicketActivation(this.GetComponent<TicketScript>());
			NormalActivation();
		}
	}

	public void GoToTable(){
		if (state == "click") {
			anim.SetBool("Corner", false);
			anim.SetBool("Table", true);
			Invoke ("StartMoveCenter", 0.9f);
		}
	}

	public void GoToCorner(){
		anim.SetBool("Corner", true);
		anim.SetBool("Table", false);
		state = "click";
	}

	public void Leave(){
		anim.SetBool("Corner", false);
		anim.SetBool("Table", false);
		Destroy (this.transform.parent.gameObject, 2.0f);
	}

	public void valideTicket(){
		TicketFinished (gain);
	}

	public void visualizeTicket(){
		TicketVisualized (gain);
		NormalValidation ();
	}

	public void UpdateGain(){
		if (gain.Split (':') [0] == "win") {
			winTextImg.gameObject.SetActive(true);
			loseTextImg.gameObject.SetActive(false);
			resultText.text = gain.Split(':')[1] + "x";
			resultText.gameObject.SetActive(true);
		}else if (gain.Split (':') [0] == "lose") {
			winTextImg.gameObject.SetActive(false);
			loseTextImg.gameObject.SetActive(true);
			resultText.gameObject.SetActive(false);
		}
	}


	public void StartMoveCenter(){
		StartCoroutine(MoveCenter());
	}

	public IEnumerator MoveCenter(){
		Transform centerTransform = this.transform.parent.transform;

		while (centerTransform.position.x < 0) {
			centerTransform.position = new Vector3 (centerTransform.position.x + Mathf.Min(Mathf.Abs(centerTransform.position.x/40.0f) + 0.01f, 0.3f), centerTransform.position.y, centerTransform.position.z);
			yield return null;
		}

		centerTransform.position = new Vector3 (0, centerTransform.position.y, centerTransform.position.z);

	}

	public void AutoScratchIn(float sec){
		Invoke ("AutoScratch", sec);
	}
	public void AutoScratch(){
		StartCoroutine (scratchZone.CleanScratch ());
	}
}
