using System.Collections;
using System.Collections.Generic;
using Completed;
using UnityEngine;

public class Barel : MonoBehaviour
{
	public AudioClip chopSound1;                //1 of 2 audio clips that play when the wall is attacked by the player.
	public AudioClip chopSound2;                //2 of 2 audio clips that play when the wall is attacked by the player.
	public Sprite dmgSprite;                    //Alternate sprite to display after barel has been attacked by player.
	public int hp = 2;                          //hit points for the barel.


	private SpriteRenderer spriteRenderer;      //Store a component reference to the attached SpriteRenderer.


	void Awake()
	{
		//Get a component reference to the SpriteRenderer.
		spriteRenderer = GetComponent<SpriteRenderer>();
	}


	//DamageWall is called when the player attacks a Barel.
	public void DamageBarel(int loss)
	{
		////Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
		SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

		//Set spriteRenderer to the damaged wall sprite.
		spriteRenderer.sprite = dmgSprite;

		//Subtract loss from hit point total.
		hp -= loss;

		//If hit points are less than or equal to zero:
		if (hp <= 0)
			//Disable the gameObject.
			gameObject.SetActive(false);
	}
}
