using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralDungeonCreator : MonoBehaviour
{
	private float[] randomWallValues;
	[SerializeField] private Material _material;
	[SerializeField] private Transform _parent;
	[SerializeField] private Vector3 _wallScale;
	[SerializeField] private float _wallTopWidth;
	[SerializeField] private int _seed;
	private Vector3 lastWalllastPos;
	private Vector3 lastDirection;
	private Vector3 lastScale;

	

	private List<Bounds> boundses;


	private void Start()
    {
	    Random.InitState(_seed);
	    for (int i = 0; i < 15; i++)
	    {
		    CreateDungeon(15, Vector3.right * i * 10f);
		    
	    }


    }

	/*private bool Intersect(Vector3 pos)
	{
		
	}*/

	private void AddToLibrary(Vector3 pos, Vector3 direction, Vector3 scale)
	{
		var bounds = new Bounds {center = pos, size = scale};
		boundses.Add(bounds);

	}


	private void CreateDungeon(int wallCount, Vector3 startingPos)
	{
		randomWallValues = new float[wallCount];
		lastDirection = Vector3.forward;
		lastWalllastPos = startingPos;
		WallMeshGenerator.SpawnWalls(_material, _parent, lastWalllastPos, lastDirection, _wallScale + Vector3.up * 15f, _wallTopWidth);
		for (int i = 0; i < randomWallValues.Length; i++)
		{
			randomWallValues[i] = Random.value;
			int d100 = Random.Range(0, 100);
			var angle = Random.Range(-360/(float)wallCount, 360f/(float)wallCount);
			Vector3 scale = _wallScale + Vector3.forward * 5f;
			Vector3 direction = Quaternion.Euler(0, angle, 0) * lastDirection;
			Vector3 pos = lastWalllastPos + lastDirection * lastScale.z / 2 + direction * scale.z / 2;
			WallMeshGenerator.SpawnWalls(_material, _parent, pos, direction, scale, _wallTopWidth);
			lastScale = scale;
			lastWalllastPos = pos;
			lastDirection = direction;
		}
	}
}
