using UnityEngine;

public class PreviewObject : MonoBehaviour {
    private Renderer objRenderer;
    private MeshFilter meshFilter;
    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;
    private CapsuleCollider capsuleCollider;
    private bool inCollision;

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

    void Update() {
        CheckMousePosition();
    }

    private void CheckMousePosition() {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, (1 << 3));
        if(hit) {
            if(hitInfo.transform.tag.Equals("Base")) {
                transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + (0.5f), hitInfo.point.z);
                objRenderer.material = ((inCollision) ? transparentRed : transparentYellow);
            } else {
                if(hitInfo.normal == new Vector3(0, 0, 1)) {    // z+
                    transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z + (0.5f));
                    objRenderer.material = ((inCollision) ? transparentRed : transparentGreen);
                } else if(hitInfo.normal == new Vector3(1, 0, 0)) {     // x+
                    transform.position = new Vector3(hitInfo.point.x + (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    objRenderer.material = ((inCollision) ? transparentRed : transparentGreen);
                }
                else if(hitInfo.normal == new Vector3(0, 1, 0)) {   // y+
                    transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y + (0.5f), hitInfo.transform.position.z);
                    objRenderer.material = ((inCollision) ? transparentRed : transparentGreen);
                }
                else if(hitInfo.normal == new Vector3(0, 0, -1)) {  // z-
                    transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z - (0.5f));
                    objRenderer.material = ((inCollision) ? transparentRed : transparentGreen);
                }
                else if(hitInfo.normal == new Vector3(-1, 0, 0)) {  // x-
                    transform.position = new Vector3(hitInfo.point.x - (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    objRenderer.material = ((inCollision) ? transparentRed : transparentGreen);
                }
                else if(hitInfo.normal == new Vector3(0, -1, 0)) {  // y-
                    transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y - (0.5f), hitInfo.transform.position.z);
                    objRenderer.material = ((inCollision) ? transparentRed : transparentGreen);
                } else {
                    objRenderer.material = transparentRed;  // Cannot be placed on object
                }
            }
        } else {
            objRenderer.material = transparentRed;
        }
    }

    public void ChangeObject(ObjectType obj) {
        switch(obj) {
            case ObjectType.CUBE:
                meshFilter.mesh = cubeMesh;
                boxCollider.enabled = true;
                sphereCollider.enabled = false;
                capsuleCollider.enabled = false;
                break;
            case ObjectType.SPHERE:
                meshFilter.mesh = sphereMesh;
                boxCollider.enabled = false;
                sphereCollider.enabled = true;
                capsuleCollider.enabled = false;
                break;
            default:        // ObjectType.CAPSULE
                meshFilter.mesh = capsuleMesh;
                boxCollider.enabled = false;
                sphereCollider.enabled = false;
                capsuleCollider.enabled = true;
                break;
        }
    }

    /*
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("MyCube") || other.gameObject.CompareTag("MySphere") || other.gameObject.CompareTag("MyCapsule")) {
            if(snapTo == null) {
                inCollision = true;
            } else if(GameObject.ReferenceEquals(snapTo, other.gameObject)) {
                inCollision = false;
            }
        }
        Debug.Log(inCollision);
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.CompareTag("MyCube") || other.gameObject.CompareTag("MySphere") || other.gameObject.CompareTag("MyCapsule")) {
            inCollision = false;
        }
        Debug.Log(inCollision);
    }
    */

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

    public bool InCollision { get { return inCollision; } }
    
    // public GameObject SnapTo { set { snapTo = value; } }
}