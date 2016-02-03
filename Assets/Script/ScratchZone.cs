using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScratchZone : MonoBehaviour {

	public delegate void BasicAction();
	public event BasicAction Scratched = delegate {};
	public event BasicAction Visualized = delegate {};

	public SpriteRenderer scratchZoneSpriteRend;
	public Texture2D scratchZoneTexture;
	public Vector3 mousePos;
	public int totalPixelsScratched = 0;

	public int zoneRadius = 30;

	private bool cleaning = false;
	private bool cleaned = false;
	private Texture2D actualScratchZoneTexture;

	public float scratching = 0.0f;

	public AudioSource audio;

	void Start () {
		ResetScratchZone ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			
			RaycastHit hit;
			if (!Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
				return;
			}
			if(hit.collider.transform == this.transform){
				Sprite sprite = hit.collider.GetComponent<SpriteRenderer> ().sprite;
				Vector3 localPos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - hit.collider.transform.position;
				localPos = getRelativePosition(hit.collider.transform, Camera.main.ScreenToWorldPoint (Input.mousePosition));
				Vector2 pixelPosition = new Vector2 (sprite.texture.width / 2 - localPos.x * (100/this.transform.lossyScale.x ), sprite.texture.height / 2 - localPos.y * (100/this.transform.lossyScale.y));
				eraseCircle(sprite.texture, pixelPosition, zoneRadius);

			}
		}

		/*if (Input.GetKeyDown ("space")) {
			StartCoroutine ("CleanScratch");
		}*/

		if (totalPixelsScratched > ZoneArea * 0.4f) {
			if (!cleaning && !cleaned){
				Visualized();
				StartCoroutine ("CleanScratch");
			}
		}

		scratching = Mathf.Max (0, scratching - Time.deltaTime);
		if (scratching == 0) {
			if (audio.isPlaying)
				audio.Pause ();
		} else {
			if (!audio.isPlaying)
				audio.Play ();
		}

	}


	public Vector3 getRelativePosition(Transform origin, Vector3 position) {
		Vector3 distance = origin.position - position;
		Vector3 relativePosition = Vector3.zero;
		relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
		relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
		relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);
		
		return relativePosition;
	}
		


	public Texture2D copyTexture2D(Texture2D texture){
		Texture2D croppedTexture = new Texture2D( (int)texture.width, (int)texture.height );
		Color[] pixels = texture.GetPixels(  0, 0, texture.width, texture.height);
		croppedTexture.SetPixels( pixels );
		croppedTexture.Apply();
		return croppedTexture;

	}

	public void eraseSquare(Texture2D texture, Vector2 pos, int size){
		for (int i=(int)pos.x - size; i<(int)pos.x + size; i++) {
			for (int j=(int)pos.y - size; j<(int)pos.y + size; j++) {
				if(i>0 && i < texture.width && j>0 && j<texture.height){
					Color col = texture.GetPixel(i, j);
					if(col.a != 0){
						col.a = 0;
						totalPixelsScratched ++;
						texture.SetPixel (i, j, col);
					}
				}
			}
		}
		texture.Apply ();
		
	}

	public int eraseCircle(Texture2D texture, Vector2 pos, int size) {
		int pixelsScratched = 0;
		for (int i=(int)pos.x - size; i<(int)pos.x + size; i++) {
			for (int j=(int)pos.y - size; j<(int)pos.y + size; j++) {
				if(i>0 && i < texture.width && j>0 && j<texture.height) {
					if(Vector2.Distance(new Vector2(i,j), pos) <= size) {
						Color col = texture.GetPixel(i, j);
						if(col.a != 0){
							col.a = 0;
							pixelsScratched++;
							totalPixelsScratched ++;
							texture.SetPixel (i, j, col);
						}
					}
				}
			}
		}
		if (pixelsScratched > 0)
			scratching = 0.1f;
		texture.Apply ();
		return pixelsScratched;
	}

	public IEnumerator CleanScratch(){

		cleaning = true;
		cleaned = false;
		Vector2 direction = new Vector2 (1, 1);
		int nextLineSpeed = 50, speed = 10;
		for(int i = scratchZoneTexture.height; i > 0; i -= nextLineSpeed*(zoneRadius/30)) {
			for(int j = 0, k = i; j < scratchZoneTexture.width && k < scratchZoneTexture.height; j += (int)direction.x  *speed*(zoneRadius/30), k += (int)direction.y * speed*(zoneRadius/30)) {
				int scratched = eraseCircle(scratchZoneSpriteRend.sprite.texture, new Vector2(j,k), zoneRadius);
				if(scratched > 100){
					/*if(!viewed && totalPixelsScratched > ZoneArea * 0.8f){
						Visualized();
						viewed = true;
					}*/
					yield return null;
				}
			}
		} 
		for(int i = 0; i < scratchZoneTexture.width; i += nextLineSpeed*(zoneRadius/30)) {
			for(int j = i, k = 0; j < scratchZoneTexture.width && k < scratchZoneTexture.height; j += (int)direction.x  *speed, k += (int)direction.y * speed) {
				int scratched = eraseCircle(scratchZoneSpriteRend.sprite.texture, new Vector2(j,k), zoneRadius);
				if(scratched > 100){
					/*if(!viewed && totalPixelsScratched > ZoneArea*0.8f){
						Visualized();
						viewed = true;
					}*/
					yield return null;
				}
			}
		}

		
		cleaned = true;
		Scratched();


	}


	public void ResetScratchZone(){
		cleaning = cleaned = false;
		actualScratchZoneTexture = copyTexture2D (scratchZoneTexture);
		scratchZoneSpriteRend.sprite = Sprite.Create(actualScratchZoneTexture, new Rect(0, 0, actualScratchZoneTexture.width, actualScratchZoneTexture.height),  new Vector2(0.5f, 0.5f));
		totalPixelsScratched = 0;
	}


	public int ZoneArea{
		get{ return scratchZoneTexture.width * scratchZoneTexture.height;}
		set{ ; }
	}


}
