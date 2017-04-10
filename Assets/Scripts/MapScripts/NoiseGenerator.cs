using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
	private int octaves;
	private float offsetX;
	private float offsetY;

	public NoiseGenerator(int octaves)
	{
		this.octaves = octaves;
		offsetX = Random.Range (0.0f, 100.0f);
		offsetY = Random.Range (0.0f, 100.0f);
	}

	//@return noise value between 0 and 1
	public float GetNoise(float x, float y)
	{
		float noise = 0;
		float frequency = 4f;
		float amplitude = 1;
		float maxNoise = 0;
		for (int i = 0; i < octaves; i++) 
		{
			noise += Mathf.PerlinNoise(offsetX + x*frequency, offsetY + y*frequency) * amplitude;
			maxNoise += amplitude;
			frequency *= 2;
			amplitude /= 2;
		}

		return noise / maxNoise;
	}

}
