using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class MyMouseInput : MonoBehaviour
{
    int index = 0;
    int clickableLayerMask = 1 << 3;
 
    // Start is called before the first frame update
    void Start()
    {
 
    }
 
    // Update is called once per frame
    void Update()
    {
 
        if (Input.GetMouseButtonUp(0))  // check if left button is pressed
        {
            // take mouse position, convert from screen space to world space, do a raycast, store output of raycast into 
            // hitInfo object ...
 
            #region Screen To World
            RaycastHit hitInfo = new RaycastHit();
            // Add layer mask to prevent clicking of explosion particles
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, clickableLayerMask);
            if (hit)
            {
                
                /*
                #region HIDE
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.tag = "MyCube";
                cube.layer = LayerMask.NameToLayer("Clickable");
                cube.GetComponent<BoxCollider>().isTrigger = true;
                //cube.GetComponent<Renderer>().material = blockMaterial;
                cube.AddComponent<TriangleExplosion>();     // Add explosion script to cube
                #endregion
                */
                GameObject newObject = GameManager.Instance.GetObject();
 
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
                    if (hitInfo.normal == new Vector3(1, 0, 0)) // x+
                    {
                        newObject.transform.position = new Vector3(hitInfo.point.x + (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, 1, 0)) // y+
                    {
                        newObject.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y + (0.5f), hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, 0, -1)) // z-
                    {
                        newObject.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z - (0.5f));
                    }
                    if (hitInfo.normal == new Vector3(-1, 0, 0)) // x-
                    {
                        newObject.transform.position = new Vector3(hitInfo.point.x - (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, -1, 0)) // y-
                    {
                        newObject.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y - (0.5f), hitInfo.transform.position.z);
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

        // Right click
        if(Input.GetMouseButtonDown(1)) {
            RaycastHit hitInfo = new RaycastHit();
            // Add layer mask to prevent clicking of explosion particles
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, clickableLayerMask);
            if(hit) {
                if(hitInfo.transform.CompareTag("MyCube") || hitInfo.transform.CompareTag("MySphere") || hitInfo.transform.CompareTag("MyCapsule")) {
                    hitInfo.transform.gameObject.GetComponent<TriangleExplosion>().StartCoroutine("SplitMesh", true);
                }
            }
        }
    }
}