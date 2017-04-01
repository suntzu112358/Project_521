using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour {

	private bool isInitialized = false;
	private int posX;
	private int posY;

	private Knowledge knowledge;
	private Inventory bag;

	public void InitializeMinion (int posX, int posY, Knowledge knowledge)
	{
		this.posX = posX;
		this.posY = posY;
		this.bag = new Inventory ();
		this.knowledge = knowledge;
		this.isInitialized = true;
	}











	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
