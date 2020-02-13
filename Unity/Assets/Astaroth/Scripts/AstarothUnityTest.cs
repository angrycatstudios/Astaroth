using UnityEngine;
using System.Collections.Generic;
using Astaroth;

namespace AstarothUnity {

public class AstarothUnityTest : MonoBehaviour {

	[Header("Visualization")]
	public int gridPixelSize = 20;
	public int gridGap = 2;

	[Header("Map properties")]
	public int mapWidth = 80;
	public int mapHeight = 40;
	public int passabilityRate = 5;

	private Texture2D[] tex;
	private bool[,] map = null;
	private List<PathNode2D> path;

	private int startX, startY;
	private int endX, endY;
	private float t_start, t_end;

	private void Start()
	{
		tex = new Texture2D[3];
		for (int i = 0; i < tex.Length; i++) tex[i] = new Texture2D(1, 1);
		tex[0].SetPixel(1, 1, Color.gray);
		tex[1].SetPixel(1, 1, Color.white);
		tex[2].SetPixel(1, 1, Color.green);
		for (int i = 0; i < tex.Length; i++) tex[i].Apply();
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(5, 5, 256, 32), "New map")) {
			GenerateMap(mapWidth, mapHeight, passabilityRate);
			path = null;
			startX = startY = 0;
			endX = mapWidth - 1;
			endY = mapHeight - 1;
			FindPath();
		}

		if (map != null) {
			for (int y = 0; y < mapHeight; y++) {
				for (int x = 0; x < mapWidth; x++) {
					GUI.DrawTexture(new Rect(100 + x * (gridPixelSize + gridGap), 100 + y * (gridPixelSize + gridGap), gridPixelSize, gridPixelSize), tex[map[x, y] ? 0 : 1]);
				}
			}
			if (path != null) {
				foreach (PathNode2D node in path) {
					GUI.DrawTexture(new Rect(100 + node.X * (gridPixelSize + gridGap), 100 + node.Y * (gridPixelSize + gridGap), gridPixelSize, gridPixelSize), tex[2]);
				}
				GUI.Label(new Rect(5, 60, 512, 32), "time elapsed: " + ((t_end - t_start) * 1000).ToString() + " ms");
			}
			if (Input.GetMouseButtonDown(0) && IsMouseInsideMapBounds()) {
				float mouseY = Screen.height - Input.mousePosition.y;
				startX = (int) (Input.mousePosition.x - 100) / (gridPixelSize + gridGap);
				startY = (int) (mouseY - 100) / (gridPixelSize + gridGap);
				FindPath();
			}
			if (Input.GetMouseButtonDown(1) && IsMouseInsideMapBounds()) {
				float mouseY = Screen.height - Input.mousePosition.y;
				endX = (int) (Input.mousePosition.x - 100) / (gridPixelSize + gridGap);
				endY = (int) (mouseY - 100) / (gridPixelSize + gridGap);
				FindPath();
			}
		}

		GUI.Label(new Rect(5, 40, 512, 32), "startX: " + startX + ", startY: " + startY + ", endX: " + endX + ", endY: " + endY);
	}

	private void GenerateMap(int mx, int my, int passabilityRate)
	{
		map = new bool[mx, my];

		for (int y = 0; y < my; y++) {
			for (int x = 0; x < mx; x++) {
				map[x, y] = Random.Range(0, passabilityRate) > 0;
			}
		}

		int x1 = Random.Range(0, mx);
		int x2 = Random.Range(0, mx);
		int y1 = Random.Range(my / 4, my / 2);
		int y2 = Random.Range(y1 + 1, my / 4 * 3 + 1);
		for (int i = 0; i < mx; i++) { map[i, y1] = x1 == i; map[i, y2] = x2 == i; }
	}

	private void FindPath()
	{
		// Basic
		// PathFinderRectGrid2D pathFinder = new PathFinderRectGrid2D(map);

		// Generic
		PathFinderRectGrid2D pathFinder = new PFRG2D_Generic(map);

		t_start = Time.realtimeSinceStartup;
		path = pathFinder.FindPath(startX, startY, endX, endY);
		t_end = Time.realtimeSinceStartup;
	}

	private bool IsMouseInsideMapBounds()
	{
		float mouseY = Screen.height - Input.mousePosition.y;
		return Input.mousePosition.x >= 100 && Input.mousePosition.x < 100 + mapWidth * (gridPixelSize + gridGap) && mouseY >= 100 && mouseY < mapHeight * (gridPixelSize + gridGap);
	}

}

}