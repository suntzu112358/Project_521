using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile {

	public Transform sprite;
	private Resource resource = null;
	private TileType type;

	public bool isResource(){
		return resource == null;
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
}
