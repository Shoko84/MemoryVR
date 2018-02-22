using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridCellData {
	public GameObject cell;
	public GameObject mesh;
}

public class PlayerCursorEvent : MonoBehaviour {

	public int width;
	public int height;
	public GameObject gridRef;
	public GameObject parentRef;

    public Transform parent;
    public GameObject propInstantiate;

	private GameObject ground;

    private float timeDur;
    private float timeRef;
    private uint touchCount;
    private List<GameObject> propsCloned = new List<GameObject>();

    private GameObject propLooked = null;
    private Vector2 cellLooked = new Vector2(-1, -1);

    private List<List<GridCellData>> grid = new List<List<GridCellData>>();

    bool LookingAtACell()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            for (int i = 0; i < grid.Count; ++i) {
                for (int j = 0; j < grid[i].Count; ++j) {
                    if (grid[i][j].cell.transform.position.x == hit.transform.position.x && grid[i][j].cell.transform.position.z == hit.transform.position.z) {
                        cellLooked = new Vector2(j, i);
                        return (true);
                    }
                }
            }
        }
        return (false);
    }

    bool LookingAtAnObject()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            for (int i = 0; i < grid.Count; ++i) {
                for (int j = 0; j < grid[i].Count; ++j) {
                    if (grid[i][j].mesh != null && grid[i][j].mesh.transform.position.x == hit.transform.position.x && grid[i][j].mesh.transform.position.z == hit.transform.position.z) {
                        propLooked = grid[i][j].mesh;
                        return (true);
                    }
                }
            }
        }
        return (false);
    }

    void RotateProp(GameObject prop, float xAngle, float yAngle, float zAngle)
    {
        if (prop != null)
            prop.transform.Rotate(xAngle, yAngle, zAngle);
    }

    void DestroyProp()
    {
        Destroy(grid[(int)cellLooked.y][(int)cellLooked.x].mesh);
        GridCellData d = new GridCellData();

        d.cell = grid[(int)cellLooked.y][(int)cellLooked.x].cell;
        d.mesh = null;
        grid[(int)cellLooked.y][(int)cellLooked.x] = d;
    }

    void SpawnProp()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && LookingAtACell() && grid[(int)cellLooked.y][(int)cellLooked.x].mesh == null)
        {
            GameObject newProp = Instantiate(propInstantiate, new Vector3(cellLooked.x, 1.0f, cellLooked.y), Quaternion.Euler(0, 180, 0)) as GameObject;
			GridCellData d = new GridCellData();

            d.cell = grid[(int)cellLooked.y][(int)cellLooked.x].cell;
            d.mesh = newProp;
            grid[(int)cellLooked.y][(int)cellLooked.x] = d;
        }
    }

    void MoveToPosition(Vector3 position)
    {
        parent.transform.position = new Vector3(position.x, 0.5f, position.z);
    }

    void TeleportToPosition()
    {
        // Teleportation du player
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            this.MoveToPosition(hit.point);
    }

	void Awake() {
		ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
		ground.transform.position = new Vector3(width / 2 - 0.49f, -0.001f, height / 2 - 0.49f);
		ground.transform.rotation = Quaternion.Euler(90, 0, 0);
		ground.transform.localScale = new Vector3(width + 0.02f, height + 0.02f, 0.001f);
		ground.transform.parent = parentRef.transform;
	}

    // Use this for initialization
    void Start ()
    {
		for (int i = 0; i < height; ++i) {
            List<GridCellData> l = new List<GridCellData>();
			for (int j = 0; j < width; ++j) {
				GridCellData d = new GridCellData();

				if (i == 0 && j == 0)
					d.cell = gridRef;
				else
				{
					GameObject newCell = Instantiate(gridRef, new Vector3(j + 0.01f, 0.0f, i + 0.01f), Quaternion.Euler(90, 0, 0)) as GameObject;
					newCell.transform.parent = parentRef.transform;
					d.cell = newCell;
				}
                d.mesh = null;
				l.Add(d);
			}
            grid.Add(l);
		}

        timeDur = 0.0f;
        timeRef = 0.0f;
        touchCount = 0;
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
          touchCount += 1;
        else if (timeDur > 0.2f)
        {
            if (touchCount == 1 && LookingAtAnObject())
            {
                RotateProp(propLooked, 0, 90, 0);
                timeDur = 0.0f;
                touchCount = 0;
                timeRef = Time.time;
            } else if (touchCount == 1)
            {
                TeleportToPosition();
                timeDur = 0.0f;
                touchCount = 0;
                timeRef = Time.time;
            } else if (touchCount == 2 && LookingAtAnObject()) {
                DestroyProp();
                timeDur = 0.0f;
                touchCount = 0;
                timeRef = Time.time;
            } else if (touchCount == 2) {
                SpawnProp();
                timeDur = 0.0f;
                touchCount = 0;
                timeRef = Time.time;
            }
        }
        else if (timeDur == 0.0f)
            timeRef = Time.time;
        timeDur = Time.time - timeRef;
    }
}
