using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class MyMouseInput : MonoBehaviour {
    // Start is called before the first frame update
    void Start() { }
 
    // Update is called once per frame
    void Update() {
        /*
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())  // check if left button is pressed AND mouse is not over UI
        {
            // take mouse position, convert from screen space to world space, do a raycast, store output of raycast into 
            // hitInfo object ...
 
            #region Screen To World
            RaycastHit hitInfo = new RaycastHit();
            // Add layer mask to prevent clicking of explosion particles
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, GameController.ClickableLayerMask);
            if (hit && !GameController.Instance.PreviewObj.InCollision)
            {
                
                // #region HIDE
                // var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube.tag = "MyCube";
                // cube.layer = LayerMask.NameToLayer("Clickable");
                // cube.GetComponent<BoxCollider>().isTrigger = true;
                // //cube.GetComponent<Renderer>().material = blockMaterial;
                // cube.AddComponent<TriangleExplosion>();     // Add explosion script to cube
                // #endregion
                GameObject newObject = GameController.Instance.GetObject();
 
                //cube.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + 0.5f, hitInfo.point.z);
                #region HIDE
                if (hitInfo.transform.tag.Equals("Base"))
                {
                    newObject.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + (0.5f), hitInfo.point.z);
                }
                #region HIDE
                else
                {
                    if (hitInfo.normal == new Vector3(0, 0, 1)) // z+
                    {
                        newObject.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z + (0.5f));
                    }
                    #region HIDE
                    else if (hitInfo.normal == new Vector3(1, 0, 0)) // x+
                    {
                        newObject.transform.position = new Vector3(hitInfo.point.x + (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    else if (hitInfo.normal == new Vector3(0, 1, 0)) // y+
                    {
                        newObject.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y + (0.5f), hitInfo.transform.position.z);
                    }
                    else if (hitInfo.normal == new Vector3(0, 0, -1)) // z-
                    {
                        newObject.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z - (0.5f));
                    }
                    else if (hitInfo.normal == new Vector3(-1, 0, 0)) // x-
                    {
                        newObject.transform.position = new Vector3(hitInfo.point.x - (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    else if (hitInfo.normal == new Vector3(0, -1, 0)) // y-
                    {
                        newObject.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y - (0.5f), hitInfo.transform.position.z);
                    }
                    else {
                        Destroy(newObject);     // Cannot be placed on object
                    }
                    #endregion
                }
                #endregion
 
                //Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, 2, false);
                //Debug.Log(hitInfo.normal);
                #endregion
 
 
            }
            else
            {
                Debug.Log("No hit");
            }
            #endregion
        }
        */

        // Move PreviewObject
        GameController.Instance.PreviewObj.Move(Input.mousePosition);

        // IF left click AND mouse not over UI, THEN create object using PreviewObject's position
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            if(GameController.Instance.PreviewObj.IsPlaceable()) {
                GameObject newObject = GameController.Instance.GetObject();
                newObject.transform.position = GameController.Instance.PreviewObj.transform.position;
            }
        }

        // IF right click AND mouse is not over UI, THEN run explosion script
        if(Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) {
            RaycastHit hitInfo = new RaycastHit();
            // Add layer mask to prevent clicking of explosion particles
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, GameController.ClickableLayerMask);
            if(hit) {
                if(hitInfo.transform.CompareTag("MyCube") || hitInfo.transform.CompareTag("MySphere") || hitInfo.transform.CompareTag("MyCapsule")) {
                    hitInfo.transform.gameObject.GetComponent<TriangleExplosion>().StartCoroutine("SplitMesh", true);
                }
            }
        }
    }
}