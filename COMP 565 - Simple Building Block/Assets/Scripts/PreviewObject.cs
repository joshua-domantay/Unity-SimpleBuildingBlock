using UnityEngine;

public class PreviewObject : MonoBehaviour {
    private Renderer objRenderer;
    private MeshFilter meshFilter;
    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;
    private CapsuleCollider capsuleCollider;
    private bool inCollision;
    private bool canPlace;

    [Header("Mesh")]
    [SerializeField] private Mesh cubeMesh;
    [SerializeField] private Mesh sphereMesh;
    [SerializeField] private Mesh capsuleMesh;
    [Header("Transparent Materials")]
    [SerializeField] private Material transparentRed;
    [SerializeField] private Material transparentYellow;
    [SerializeField] private Material transparentGreen;

    void Awake() {
        objRenderer = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Change mesh and enable/disable appropriate colliders
    public void ChangeObject(ObjectType obj) {
        switch(obj) {
            case ObjectType.CUBE:
                meshFilter.mesh = cubeMesh;
                // boxCollider.enabled = true;
                // sphereCollider.enabled = false;
                // capsuleCollider.enabled = false;
                break;
            case ObjectType.SPHERE:
                meshFilter.mesh = sphereMesh;
                // boxCollider.enabled = false;
                // sphereCollider.enabled = true;
                // capsuleCollider.enabled = false;
                break;
            default:        // ObjectType.CAPSULE
                meshFilter.mesh = capsuleMesh;
                // boxCollider.enabled = false;
                // sphereCollider.enabled = false;
                // capsuleCollider.enabled = true;
                break;
        }
    }

    private void ChangePos(Vector3 position, bool onBase) {
        transform.position = position;

        // Collision = red. Placing on base = yellow. Snap on other object = green
        objRenderer.material = ((inCollision) ? transparentRed : (onBase ? transparentYellow : transparentGreen));
    }

    private void ChangePosForCapsule(RaycastHit hitInfo, Vector3 position) {
        if(hitInfo.point.y < hitInfo.transform.position.y) {    // Lower half of capsule
            position.y -= 0.5f;
        } else {    // Upper half of capsule
            position.y += 0.5f;
        }
        ChangePos(position, false);
    }

    // Used for when placing objects that adds onto y position
    public float GetYDiff(bool onYAxis, RaycastHit hitInfo) {
        /*
        float yDiff = (onYAxis ? 0.5f : 0f);
        if(GameController.Instance.ObjectChoosen == ObjectType.CAPSULE) {
            if(hitInfo.transform.CompareTag("MyCapsule") && onYAxis) {      // Do not add 0.5 when attaching to another capsule on Y axis
                return yDiff;
            }
            yDiff += 0.5f;
        }
        return yDiff;
        */
        float yDiff = 0f;
        if(GameController.Instance.ObjectChoosen == ObjectType.CAPSULE) {
            if(onYAxis) { yDiff = 0.5f; }
            if(hitInfo.transform.CompareTag("MyCapsule") && onYAxis) {      // Do not add 0.5 when attaching to another capsule on Y axis
                return yDiff;
            }
            yDiff += 0.5f;
        } else {
            if(!hitInfo.transform.CompareTag("MyCapsule") && onYAxis) { yDiff = 0.5f; }     // Do not add 0.5 when attaching to capsule on y axis
        }
        return yDiff;
    }

    public bool IsPlaceable() { return (canPlace && !inCollision); }

    // Modified version of MyMouseInput early left click
    public void Move(Vector3 mouse) {
        /*
        canPlace = true;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(mouse), out hitInfo, Mathf.Infinity, (1 << 3));
        if(hit) {
            if(hitInfo.transform.tag.Equals("Base")) {
                ChangePos(new Vector3(hitInfo.point.x, (hitInfo.point.y + GetYDiff(true, hitInfo)), hitInfo.point.z), true);
            } else {
                if(hitInfo.normal == new Vector3(0, 0, 1)) {    // z+
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.point.z + (0.5f));
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                } else if(hitInfo.normal == new Vector3(1, 0, 0)) {     // x+
                    Vector3 newPos = new Vector3(hitInfo.point.x + (0.5f), (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(0, 1, 0)) {   // y+
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.point.y + GetYDiff(true, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(0, 0, -1)) {  // z-
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.point.z - (0.5f));
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(-1, 0, 0)) {  // x-
                    Vector3 newPos = new Vector3(hitInfo.point.x - (0.5f), (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(0, -1, 0)) {  // y-
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.point.y - GetYDiff(true, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                } else {
                    objRenderer.material = transparentRed;  // Cannot be placed on object
                    canPlace = false;
                }
            }
        } else {
            objRenderer.material = transparentRed;
            canPlace = false;
        }
        */
        canPlace = true;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(mouse), out hitInfo, Mathf.Infinity, (1 << 3));
        if(hit) {
            if(hitInfo.transform.tag.Equals("Base")) {
                ChangePos(new Vector3(hitInfo.point.x, (hitInfo.point.y + GetYDiff(true, hitInfo)), hitInfo.point.z), true);
            } else {
                if(hitInfo.normal == new Vector3(0, 0, 1)) {    // z+
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.point.z + (0.5f));
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                } else if(hitInfo.normal == new Vector3(1, 0, 0)) {     // x+
                    Vector3 newPos = new Vector3(hitInfo.point.x + (0.5f), (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(0, 1, 0)) {   // y+
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.point.y + GetYDiff(true, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(0, 0, -1)) {  // z-
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.point.z - (0.5f));
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(-1, 0, 0)) {  // x-
                    Vector3 newPos = new Vector3(hitInfo.point.x - (0.5f), (hitInfo.transform.position.y + GetYDiff(false, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                }
                else if(hitInfo.normal == new Vector3(0, -1, 0)) {  // y-
                    Vector3 newPos = new Vector3(hitInfo.transform.position.x, (hitInfo.point.y - GetYDiff(true, hitInfo)), hitInfo.transform.position.z);
                    if(!hitInfo.transform.CompareTag("MyCapsule")) {
                        ChangePos(newPos, false);
                    } else {
                        ChangePosForCapsule(hitInfo, newPos);
                    }
                } else {
                    objRenderer.material = transparentRed;  // Cannot be placed on object
                    canPlace = false;
                }
            }
        } else {
            objRenderer.material = transparentRed;
            canPlace = false;
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("MyCube") || other.CompareTag("MySphere") || other.CompareTag("MyCapsule")) {
            inCollision = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("MyCube") || other.CompareTag("MySphere") || other.CompareTag("MyCapsule")) {
            inCollision = false;
        }
    }
}