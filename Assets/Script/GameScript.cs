using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//id : 75721496 mdp : 92600

public class GameScript : MonoBehaviour {

	public bool started = false;
	public int tickets = 1;
	public int ticketsScratched = 0;
	public int failProgression = 0;
	
	public MusicManager musicManager;
	public int[] musicChangePoint;
	public int musicChangeCounter = 0;

	public int[] ticketsStock;
	public int[] stockValues;
	public Text ticketsNumberText;

	public int comboWin = 0;
	public int comboLose = 0;
	public TicketScript actualTicket;

	public List<Object> ticketPrefabs;
	public Object creditTicket1;
	public Object creditTicket2;
	public Object creditTicket3;
	public Object creditTicket4;

	public AudioClip laugh1;
	public AudioClip laugh2;
	public AudioClip sigh1;
	public AudioClip sigh2;
	public AudioClip paper;
	public AudioClip tableHit1;
	public AudioClip ohoh;
	public AudioClip ding;
	public AudioClip darkDing;

	public AudioSource audioSource;

	public Animator cameraAnim;
	public Animator pictureAnim;
	public Animator ticketNbAnim;
	public Animator clickToStartAnim;
	public Animator titleTagAnim;

	public PictureScript picture;

	public TutorialHandScript tutoHand;


	void Awake(){
		Screen.SetResolution (900, 675, false);
	}
	void Start(){
		//TicketScript ts = CreateRandomTicket ();
		//ts.GoToCorner ();
		UpdateTicketText ();
		Destroy ((Instantiate (ticketPrefabs [0]) as GameObject), 2.0f);
	}

	void Update(){
		if (Input.GetMouseButton (0)) {
			if(!started){
				PrepareGame();
			}
		}
		/*if (Input.GetKeyDown ("e")) {
			PrepareEnding();
		}*/
	}

	public void PrepareGame(){
		cameraAnim.SetBool ("ZoomActive", false);
		pictureAnim.SetBool ("Menu", false);
		clickToStartAnim.SetBool ("Active", false);
		titleTagAnim.SetBool("Menu", false);
		picture.ChangeToState (1);
		started = true;
		Invoke ("StartGame", 1.5f);
	}

	public void StartGame(){
		musicManager.SwitchToAct (1);
		TicketScript ts = CreateNextTicket ();
		ts.GoToCorner ();
		audioSource.PlayOneShot (ohoh);
		tutoHand.PointTo (new Vector2 (ts.transform.position.x, 5), -10);
		ts.NormalActivation += tutoHand.Stop;
		ts.NormalActivation += ScratchTuto1;
		ts.NormalValidation += tutoHand.Stop;
	}

	public void ScratchTuto1(){
		Invoke ("ScratchTuto2", 2.1f);
	}

	public void ScratchTuto2(){
		tutoHand.ScratchTo (new Vector2 (-1.5f, -5), -45);
	}

	public TicketScript CreateNextTicket(){
		TicketScript ts;
		switch (ticketsScratched) {
		case 0:
			ts = CreateTicket("win:3", 0);
			break;
		case 1:
			ts = CreateTicket("lose:0", 0);
			break;
		default:
			ts = CreateRandomTicket();
			break;
		}

		return ts;
	}

	public TicketScript CreateTicket(string gain, int type){

		GameObject ticketGO =Instantiate(ticketPrefabs[type]) as GameObject;
		ticketGO.transform.position = new Vector3 (Random.Range (-9, 0), ticketGO.transform.position.y, ticketGO.transform.position.z);
		//ticketGO.transform.eulerAngles = new Vector3 (0, 0, Random.Range (-30, 30));
		TicketScript ts = ticketGO.GetComponentInChildren<TicketScript> ();
		ts.TicketActivation += SelectTicket;


		ts.gain = gain;
		ts.UpdateGain ();

		string[] splittedGain = gain.Split (':');
		if (splittedGain [0] == "win") {
			comboLose = 0;
			comboWin += int.Parse(splittedGain[1]);
		} else {
			comboLose++;
			comboWin = 0;
		}

		return ts;

	}
	public TicketScript CreateRandomTicket(){

		int ticketsUnlocked = (int)Mathf.Min ((Mathf.Floor (ticketsScratched / 5.0f))+1, ticketPrefabs.Count);
		GameObject ticketGO =Instantiate(ticketPrefabs[Random.Range(0,ticketsUnlocked)]) as GameObject;
		ticketGO.transform.position = new Vector3 (Random.Range (-9, 0), ticketGO.transform.position.y, ticketGO.transform.position.z);
		//ticketGO.transform.eulerAngles = new Vector3 (0, 0, Random.Range (-30, 30));
		TicketScript ts = ticketGO.GetComponentInChildren<TicketScript> ();
		ts.TicketActivation += SelectTicket;
		int gain;
		if (TotalLoseTickets/2 > TotalWinTickets) {
			gain = getRandomGain (0, 1);
		} else if (comboWin >= 7) {
			gain = getRandomGain (0, 2);
		} else if (comboLose >= 3) {
			gain = getRandomGain (1);
		}else if (tickets <= 3) {
			gain = getRandomGain (1);
		} else if (tickets >= 20+(ticketsScratched/2)) {
			gain = getRandomGain (0, 2);
		}else{
			gain = getRandomGain();
		}


		if (gain == 0) {
			ts.gain = "lose:0";
			comboLose++;
			comboWin = 0;
		} else {
			ts.gain = "win:" + gain.ToString ();
			comboLose = 0;
			comboWin +=gain;
		}
		ts.UpdateGain ();
		return ts;
	}

	public int getRandomGain(){
		return getRandomGain (0);
	}

	public int getRandomGain(int min){
		return getRandomGain (min, ticketsStock.Length);
	}

	public int getRandomGain(int min, int max){
		int total = 0;
		int i;
		for (i=min; i<max; i++) {
			total += ticketsStock[i];
		}
		if (total > 0) {
			int randomGain = Random.Range (0, total);
			int counter = 0;
			
			for (i =min; i<max; i++) {
				counter += ticketsStock [i];
				if (randomGain < counter)
					break;
			}
			ticketsStock [i]--;
			return stockValues [i];
		} else {
			return getRandomGain();
		}
	}

	public void SelectTicket(TicketScript ts){
		audioSource.PlayOneShot (paper);
		if(actualTicket != null)
			actualTicket.Leave ();
		actualTicket = ts;
		ts.TicketFinished += receiveGain;
		ts.TicketVisualized += viewGain;
		tickets--;
		UpdateTicketText ();

	}


	public void viewGain(string gain){

		string[] splittedGain;
		splittedGain = gain.Split (':');
		
		if (splittedGain [0] == "win") {
			if(int.Parse(splittedGain[1]) >= 3){
				audioSource.PlayOneShot(laugh1);
				cameraAnim.Play("Shake2");
			}else{
				audioSource.PlayOneShot(sigh1);
				cameraAnim.Play("Shake1");
			}
		} else {
			audioSource.PlayOneShot(tableHit1);
			cameraAnim.Play("Shake3");
		}

	}


	public void receiveGain(string gain){

		ticketsScratched++;
		if (ticketsScratched == 1) {
			ticketNbAnim.SetBool ("Active", true);
		}

		string[] splittedGain;
		splittedGain = gain.Split (':');

		if (splittedGain [0] == "win") {
			audioSource.PlayOneShot(ding);
			tickets += int.Parse (splittedGain [1]);
		} else {
			picture.Downgrade();
			failProgression++;
			if(musicChangeCounter < musicChangePoint.Length-1 && musicChangePoint[musicChangeCounter] <= failProgression){
				musicManager.SwitchToNextAct();
				musicChangeCounter++;
			}

			if(picture.IsFinished){
				PrepareEnding();
			}
		}
		UpdateTicketText ();
		if (picture.IsFinished) {
			actualTicket.Leave();
		} else {
			CreateNextTicket ().GoToCorner ();
		}
	}

	public void PrepareEnding(){
		pictureAnim.SetBool ("End", true);
		ticketNbAnim.SetBool ("End", true);
		titleTagAnim.SetBool ("End", true);
		Invoke ("StartEndStep1", 2.0f);
	}

	public void StartEndStep1(){
		StartCoroutine (EndCinematic ());
	}

	public void StartEndStep2(){
		pictureAnim.SetBool ("End2", true);
		ticketNbAnim.SetBool ("End2", true);
		tutoHand.ScratchTo(new Vector2(0,0), -30);
		picture.BecomeMemory ();
		ScratchZone sz = picture.HideWithScratchZone ();
		sz.Scratched += StartEndStep3;
		sz.Visualized += tutoHand.Stop;
	}

	public void StartEndStep3(){
		StartCredit ();
	}
	public void UpdateTicketText(){
		ticketsNumberText.text = tickets + "x";
	}

	public TicketScript CreateEndTicket(int side, string gain, int type, int z){
		
		GameObject ticketGO =Instantiate(ticketPrefabs[type]) as GameObject;

		//ticketGO.transform.position = new Vector3 (0, Random.Range (-7, -2), z);
		//ticketGO.transform.eulerAngles = new Vector3 (0, 0, Random.Range (-30, 30));
		TicketScript ts = ticketGO.GetComponentInChildren<TicketScript> ();
		if (side == 1)
			ticketGO.transform.localScale = new Vector3 (-1, 1, 1);

		ts.anim.Play ("VerticalTranslation");
		//ts.AutoScratchIn (1.0f);
		ts.gain = gain;
		ts.UpdateGain ();
		return ts;
		
	}


	public void StartCredit(){
		(Instantiate (creditTicket1) as GameObject).GetComponentInChildren<Animator> ().SetBool ("Center", true);
		Invoke ("CreditStep2", 2.0f);
	}

	public void CreditStep2(){
		(Instantiate (creditTicket2) as GameObject).GetComponentInChildren<Animator> ().SetBool ("Center", true);
		Invoke ("CreditStep3", 2.0f);
	}
	public void CreditStep3(){
		(Instantiate (creditTicket3) as GameObject).GetComponentInChildren<Animator> ().SetBool ("Center", true);
		Invoke ("CreditStep4", 2.0f);
	}

	public void CreditStep4(){
		(Instantiate (creditTicket4) as GameObject).GetComponentInChildren<Animator> ().SetBool ("Center", true);
		musicManager.SwitchToAct (15);
	}

	public IEnumerator EndCinematic(){

		int noteTotale = 50;
		int counter = 0;
		int totalEndTickets = tickets;
		float scale = Mathf.Pow (2f,1.0f/12.0f);
		while (tickets > 0) {
			int coef = 1 + (tickets*5)/totalEndTickets;
			TicketScript et = CreateEndTicket(counter%2, "lose:0", Random.Range(0, ticketPrefabs.Count), -2);
			tickets--;
			UpdateTicketText ();
			counter++;
			audioSource.pitch = Mathf.Pow(scale,(noteTotale/totalEndTickets)*(1-counter));
			audioSource.PlayOneShot(darkDing);
			Destroy(et.gameObject.transform.parent.gameObject, 4.0f);
			yield return new WaitForSeconds(0.3f*coef*0.7f);

		}

		Invoke ("StartEndStep2", 2.0f);
	}

	public int TotalTickets{
		get{ return getSumInCases(ticketsStock, 0, ticketsStock.Length);}
		set{ ;}
	}

	public int TotalLoseTickets{
		get{ return ticketsStock[0];}
		set{ ;}
	}

	public int TotalWinTickets{
		get{ return getSumInCases(ticketsStock, 1, ticketsStock.Length);}
		set{ ;}
	}

	private int getSumInCases(int[] arr, int min, int max){
		int res = 0;
		for(int i=min;i<max;i++){
			res+=arr[i];
		}
		return res;

	}
}
