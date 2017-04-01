using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile {

	private Transform spriteInstance;
	private Resource resource;
	private TileType type;

	public MapTile(Transform sprite, Resource resource, TileType type){
		this.spriteInstance = sprite;
		this.resource = resource;
		this.type = type;
	}

	public bool isResource(){
		return resource == Resource.Nothing;
	}

	/**
	 * @pre must be a resource tile or will return null
	 */
	public Resource getResource(){
		return resource;
	}

	public TileType getTileType(){
		return type;
	}

	public void setPosition(Vector3 newPos){
		spriteInstance.position = newPos;
	}

	public void Destroy(){
		GameObject.Destroy (spriteInstance.gameObject);
	}
}
