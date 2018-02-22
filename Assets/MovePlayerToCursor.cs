using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerToCursor : MonoBehaviour {

    public UnityEngine.UI.Text consoleText;
    public Transform parent;
    public GameObject propInstantiate;

    private uint counter;

    private float timeDur;
    private float timeRef;
    private uint touchCount;
    private List<GameObject> propsCloned = new List<GameObject>();
    private GameObject propLooked = null;

    bool LookingAtAnObject()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            for (var i = 0; i < propsCloned.Count; i++)
            {
                if (propsCloned[i].transform.position == hit.transform.position)
                {
                    propLooked = propsCloned[i];
                    return (true);
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

    void SpawnProp()
    {
        consoleText.text = "J'ai spawn un prop";
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject newProp = Instantiate(propInstantiate, new Vector3(hit.point.x, 1.0f, hit.point.z), Quaternion.Euler(0, 180, 0)) as GameObject;
            propsCloned.Add(newProp);
        }
    }

    void MoveToPosition(Vector3 position)
    {
        parent.transform.position = new Vector3(position.x, 1, position.z);
    }

    void TeleportToPosition()
    {
        // Teleportation du player
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            counter += 1;
            consoleText.text = "Vous vous êtes téléporté " + counter + " fois !";
            this.MoveToPosition(hit.point);
        }
    }

    // Use this for initialization
    void Start ()
    {
        counter = 0;
        timeDur = 0.0f;
        timeRef = 0.0f;
        touchCount = 0;
        consoleText.text = "Vous vous êtes téléporté " + counter + " fois !";
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
            }
            else if (touchCount == 1)
            {
                TeleportToPosition();
                timeDur = 0.0f;
                touchCount = 0;
                timeRef = Time.time;
            }
            else if (touchCount > 1)
            {
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
