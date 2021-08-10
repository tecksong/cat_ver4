using System.Collections;
using System.Collections.Generic;
using Completed;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
	public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
	public int barelDamage = 1;                  //How much damage a player attack the barel.
	public Text HPText;                       //UI Text to display current player food total.
	public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
	public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
	public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
	public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
	//public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
	//public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
	public AudioClip gameOverSound;             //Audio clip to play when player dies.

	private Animator animator;                  //Used to store a reference to the Player's animator component.
	private int food;                           //Used to store player food points total during level.
	private bool facingLeft;

	public Image img;
	public float time = 5;
	public Color flashColor;
	private Color defaultColor;

	private bool damaged = false;

	//Start overrides the Start function of MovingObject
	protected override void Start()
	{
		facingLeft = true;
		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();
		defaultColor = img.color;

		//Get the current food point total stored in GameManager.instance between levels.
		food = MainMenu.HP;

		//Set the foodText to reflect the current player food total.
		HPText.text = "HP: " + food;

		//Call the Start function of the MovingObject base class.
		base.Start();


	}

	IEnumerator HitSpike()
	{

		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger("isDie");

		int food1 = food;
		//Subtract lost food points from the players total.
		food -= food;

		//Update the food display with the new total.
		HPText.text = "-" + food1 + " HP: " + food;

		yield return new WaitForSeconds(2);

		//Check to see if game has ended.
		CheckIfGameOver();
	}


	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable()
	{
		//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
		MainMenu.HP = food;
	}


	private void Update()
	{
		//If it's not the player's turn, exit the function.
		if (!GameManager.instance.playersTurn) return;

		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.

		PlayHurtEffect();

		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int)(Input.GetAxisRaw("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int)(Input.GetAxisRaw("Vertical"));

		//Check if moving horizontally, if so set vertical to zero.
		if (horizontal != 0)
		{
			vertical = 0;
		}

		//Check if we have a non-zero value for horizontal or vertical
		if (horizontal != 0 || vertical != 0)
		{
			if(horizontal>0 && facingLeft || horizontal<0 && !facingLeft)
            {
				facingLeft = !facingLeft;
				Vector3 theScale = transform.localScale;
				theScale.x *= -1;
				transform.localScale = theScale;
            }
			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			AttemptMove<Barel>(horizontal, vertical);
		}
	}

	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Barel, it also takes integers for x and y direction to move in.
	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		animator.SetTrigger("isWalk");
		//Every time player moves, subtract from food points total.
		food = food - 2;

		//Update food text display to reflect current score.
		HPText.text = "HP: " + food;

		//Call the AttemptMove method of the base class, passing in the component T (in this case Barel) and x and y direction to move.
		base.AttemptMove<T>(xDir, yDir);

		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D hit;

		//If Move returns true, meaning Player was able to move into an empty space.
		if (Move(xDir, yDir, out hit))
		{
			//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
			//SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		//Since the player has moved and lost food points, check if the game has ended.
		CheckIfGameOver();

		//Set the playersTurn boolean of GameManager to false now that players turn is over.
		GameManager.instance.playersTurn = false;
	}


	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Barel which the player can attack and destroy.
	protected override void OnCantMove<T>(T component)
	{
		//Set hitBarel to equal the component passed in as a parameter.
		Barel hitBarel = component as Barel;

		//Call the DamageBarel function of the Barel we are hitting.
		hitBarel.DamageBarel(barelDamage);

		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger("isAttack");
	}


	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D(Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		if (other.tag == "Exit")
		{
			//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
			Invoke("Restart", restartLevelDelay);

			//Disable the player object since level is over.
			enabled = false;
		}

		//Check if the tag of the trigger collided with is Food.
		else if (other.tag == "Food")
		{
			//Add pointsPerFood to the players current food total.
			food += pointsPerFood;

			//Update foodText to represent current total and notify player that they gained points
			HPText.text = "+" + pointsPerFood + "HP: " + food;

			//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

			//Disable the food object the player collided with.
			other.gameObject.SetActive(false);
		}

		//Check if the tag of the trigger collided with is Spike.
		else if (other.tag == "Spike")
		{
			StartCoroutine(HitSpike());

		}

	}


	//Restart reloads the scene when called.
	private void Restart()
	{
		//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
		//and not load all the scene object in the current scene.
		MainMenu.mapLevel += 1;
		MainMenu.HP = food;
		SceneManager.LoadScene(1);
	}


	//LoseFood is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
	public void LoseFood(int loss)
	{
		//Subtract lost food points from the players total.
		food -= loss;

		//Update the food display with the new total.
		HPText.text = "-" + loss + " HP: " + food;

		//Check to see if game has ended.
		CheckIfGameOver();
	}

	public void Hurt()
	{
		animator.SetTrigger("isHurt");
		damaged = true;
	}

	void PlayHurtEffect()
	{
		if (damaged)
		{
			img.color = flashColor;
		}
		else
		{
			img.color = Color.Lerp(img.color, Color.clear, time * Time.deltaTime);
		}

		damaged = false;
	}

	//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
	private void CheckIfGameOver()
	{
		//Check if food point total is less than or equal to zero.
		if (food <= 0)
		{
			//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
			SoundManager.instance.PlaySingle(gameOverSound);

			//Stop the background music.
			SoundManager.instance.musicSource.Stop();

			//Call the GameOver function of GameManager.
			GameManager.instance.GameOver();
		}
	}
}