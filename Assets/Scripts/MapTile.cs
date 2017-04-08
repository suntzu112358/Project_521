using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile 
{
	private Resource resource;
	private TileType type;

	public MapTile(Resource resource, TileType type){
		this.resource = resource;
		this.type = type;
	}

	public bool isResource(){
		return resource == Resource.Nothing;
	}
		
	public Resource getResource(){
		return resource;
	}

	public TileType getTileType(){
		return type;
	}

	public void addResource(Resource r){
		resource = r;
	}

	public void removeResource(){
		resource = Resource.Nothing;
	}
}
