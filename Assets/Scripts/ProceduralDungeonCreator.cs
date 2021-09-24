using System;
using System.CodeDom.Compiler;
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
	[SerializeField] private Collider dungeonBounds;
	private int iteration;
	[SerializeField] private int wallCount;
	[SerializeField] private bool randomPositionsForWallStart;
	[SerializeField][Range(1, 40)] private int resolution;
	[SerializeField] private float randomAngle = 90f;
	
	
	
	private Vector3 lastWalllastPos;
	private Vector3 lastDirection;
	private Vector3 lastScale;

	

	private List<Bounds> boundses = new List<Bounds>();


	private void Start()
    {
	    Random.InitState(_seed);
	    GenerateDungeonInCollider();
    }

	private void GenerateBorderWalls(Bounds bounds)
	{
		var rightScale = new Vector3(_wallScale.z * 2f, _wallScale.y * 5f, bounds.size.x);
		var forwardScale = new Vector3(_wallScale.z * 2f, _wallScale.y * 5f, bounds.size.z);

		var wall1Pos = new Vector3(bounds.size.x / 2, transform.position.y, bounds.center.z);
		var wall2Pos = new Vector3(-bounds.size.x / 2, transform.position.y, bounds.center.z);
		var wall3Pos = new Vector3(bounds.center.x, transform.position.y, bounds.size.z / 2);
		var wall4Pos = new Vector3(bounds.center.x, transform.position.y, -bounds.size.z / 2);

		WallMeshGenerator.SpawnWalls(_material, _parent, transform.position + wall1Pos, transform.forward, forwardScale, _wallTopWidth);
		WallMeshGenerator.SpawnWalls(_material, _parent, transform.position + wall2Pos, transform.forward, forwardScale, _wallTopWidth);
		WallMeshGenerator.SpawnWalls(_material, _parent, transform.position + wall3Pos, transform.right, rightScale, _wallTopWidth);
		WallMeshGenerator.SpawnWalls(_material, _parent, transform.position + wall4Pos, transform.right, rightScale, _wallTopWidth);

	}
	private void GenerateDungeonInCollider()
	{
		var bounds = dungeonBounds.bounds;
		GenerateBorderWalls(bounds);
		float gapX = (bounds.size.x) / (resolution + 1) ;
		float gapZ = (bounds.size.z) / (resolution + 1);
		iteration = resolution * resolution;
		
		for (int i = 0; i < iteration; i++)
		{
			float x = 0;
			float y = transform.position.y;
			float z = 0;
			if (randomPositionsForWallStart)
			{
				x = Random.Range(0, bounds.size.x);
				z = Random.Range(0, bounds.size.z);
			}
			else
			{
				x = (Math.DivRem(i, resolution, out int rem) + 1) * gapX;
				z = (rem + 1) * gapZ;
			}

			Vector3 cornerPos = transform.position - new Vector3(bounds.size.x / 2, 0, bounds.size.z / 2);
			Vector3 startPointInDungeon = new Vector3(x, y, z) + cornerPos;
			CreateDungeon(wallCount, startPointInDungeon);
		}
	}

	private bool IntersectWithLibrary(Vector3 pos, Vector3 scale)
	{
		var boundToBeChecked = new Bounds(pos, scale);
		for (int i = 0; i < boundses.Count; i++)
		{
			if (boundses[i].Intersects(boundToBeChecked))
			{
				return true;
			}
		}

		return false;
	}

	private bool IntersectWithDungeonBounds(Vector3 pos, Vector3 scale)
	{
		var boundToBeChecked = new Bounds(pos, scale);
		
		if (dungeonBounds.bounds.Intersects(boundToBeChecked))
		{
			return true;
		}
		return false;
	}

	private void AddToLibrary(Vector3 pos, Vector3 scale)
	{
		var positionedBound = new Bounds(pos, scale);
		boundses.Add(positionedBound);
	}
	
	


	private void CreateDungeon(int wallCount, Vector3 startingPos)
	{
		var firstAngle = (int)Random.Range(0, 4) * randomAngle;
		randomWallValues = new float[wallCount];
		lastDirection = Quaternion.Euler(0, firstAngle, 0) * transform.forward;
		lastWalllastPos = transform.position + startingPos;
		for (int i = 0; i < randomWallValues.Length; i++)
		{
			var angle = (int)Random.Range(0, 4) * randomAngle;
			Vector3 scale = _wallScale;
			Vector3 direction = Quaternion.Euler(0, angle, 0) * lastDirection.normalized;
			Vector3 pos = lastWalllastPos + lastDirection * lastScale.z / 2 + direction * scale.z / 2;
			if (IntersectWithLibrary(pos, scale) || !IntersectWithDungeonBounds(pos, scale))
			{
				for (int j = 0; j < Mathf.FloorToInt(360 / randomAngle) + 1; j++)
				{
					angle = randomAngle * j;
					direction = Quaternion.Euler(0, angle, 0) * lastDirection.normalized;
					pos = lastWalllastPos + lastDirection * lastScale.z / 2 + direction * scale.z / 2;
					if (!IntersectWithLibrary(pos, scale) && IntersectWithDungeonBounds(pos, scale))
					{
						goto Generate;
					}
				}
				for (int j = 0; j < 4; j++)
				{
					scale = Vector3.Scale(_wallScale, new Vector3(1f, 1f, 1f / (j + 1f)));
					direction = Quaternion.Euler(0, angle, 0) * lastDirection.normalized;
					pos = lastWalllastPos + lastDirection * lastScale.z / 2 + direction * scale.z / 2;
					if (!IntersectWithLibrary(pos, scale) && IntersectWithDungeonBounds(pos, scale))
					{
						goto Generate;
					}
				}
				
				return;
			}
			
			Generate:
			WallMeshGenerator.SpawnWalls(_material, _parent, pos, direction, scale, _wallTopWidth);
			lastScale = scale;
			lastWalllastPos = pos;
			lastDirection = direction;
			AddToLibrary(pos, scale);
		}
	}
}
