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

    private void ChangePosition(Vector3 position, bool onBase) {
        transform.position = position;

        // Collision = red. Placing on base = yellow. Snap on other object = green
        objRenderer.material = ((inCollision) ? transparentRed : (onBase ? transparentYellow : transparentGreen));
    }

    // Modified version of MyMouseInput early left click
    public void Move(Vector3 mouse) {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(mouse), out hitInfo, Mathf.Infinity, (1 << 3));
        if(hit) {
            if(hitInfo.transform.tag.Equals("Base")) {
                ChangePosition(new Vector3(hitInfo.point.x, hitInfo.point.y + (0.5f), hitInfo.point.z), true);
            } else {
                if(hitInfo.normal == new Vector3(0, 0, 1)) {    // z+
                    ChangePosition(new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z + (0.5f)), false);
                } else if(hitInfo.normal == new Vector3(1, 0, 0)) {     // x+
                    ChangePosition(new Vector3(hitInfo.point.x + (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z), false);
                }
                else if(hitInfo.normal == new Vector3(0, 1, 0)) {   // y+
                    float yDiff = 0.5f;
                    if(GameController.Instance.ObjectChoosen == ObjectType.CAPSULE) { yDiff += 0.5f; }
                    ChangePosition(new Vector3(hitInfo.transform.position.x, hitInfo.point.y + (yDiff), hitInfo.transform.position.z), false);
                }
                else if(hitInfo.normal == new Vector3(0, 0, -1)) {  // z-
                    ChangePosition(new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z - (0.5f)), false);
                }
                else if(hitInfo.normal == new Vector3(-1, 0, 0)) {  // x-
                    ChangePosition(new Vector3(hitInfo.point.x - (0.5f), hitInfo.transform.position.y, hitInfo.transform.position.z), false);
                }
                else if(hitInfo.normal == new Vector3(0, -1, 0)) {  // y-
                    float yDiff = 0.5f;
                    if(GameController.Instance.ObjectChoosen == ObjectType.CAPSULE) { yDiff += 0.5f; }
                    ChangePosition(new Vector3(hitInfo.transform.position.x, hitInfo.point.y - (yDiff), hitInfo.transform.position.z), false);
                } else {
                    objRenderer.material = transparentRed;  // Cannot be placed on object
                }
            }
        } else {
            objRenderer.material = transparentRed;
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

    public bool InCollision { get { return inCollision; } }
}