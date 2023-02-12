/*  Joshua Anthony Domantay
 *  Professor Vahe Karamian
 *  COMP 565 - 17046
 *  14 February 2023
 *  Unity - Mini Project
 */

using UnityEngine;

public enum ObjectType {
    CUBE,
    SPHERE,
    CAPSULE
}

public enum MaterialType {
    STONE,
    WALL,
    WOOD
}

public class GameController : MonoBehaviour {
    private static GameController instance;
    private static int clickableLayerMask = 1 << 3;

    private ObjectType objectChoosen = ObjectType.CUBE;
    private MaterialType materialChoosen = MaterialType.STONE;

    [SerializeField] private PreviewObject previewObj;
    [Header("UI Materials")]
    [SerializeField] private Material stoneMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material woodMaterial;

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }

    public GameObject GetObject() {
        GameObject obj;

        // Assign object
        switch(objectChoosen) {
            case ObjectType.CUBE:
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.tag = "MyCube";
                obj.GetComponent<BoxCollider>().isTrigger = true;
                break;
            case ObjectType.SPHERE:
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.tag = "MySphere";
                // obj.GetComponent<SphereCollider>().isTrigger = true;
                obj.GetComponent<SphereCollider>().enabled = false;     // Disable collider and treat it like a box
                obj.AddComponent<BoxCollider>().isTrigger = true;
                break;
            default:        // ObjectType.CAPSULE
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obj.tag = "MyCapsule";
                // obj.GetComponent<CapsuleCollider>().isTrigger = true;
                obj.GetComponent<CapsuleCollider>().enabled = false;        // Disable collider and treat it like a box
                obj.AddComponent<BoxCollider>().isTrigger = true;
                break;
        }

        // Assign material
        switch(materialChoosen) {
            case MaterialType.STONE:
                obj.GetComponent<Renderer>().material = stoneMaterial;
                break;
            case MaterialType.WALL:
                obj.GetComponent<Renderer>().material = wallMaterial;
                break;
            default:        // MaterialType.WOOD
                obj.GetComponent<Renderer>().material = woodMaterial;
                break;
        }
        obj.layer = LayerMask.NameToLayer("Clickable");
        obj.AddComponent<TriangleExplosion>();     // Add explosion script to cube

        return obj;
    }

    public void ChangeObject(int x) {
        objectChoosen = (ObjectType) x;

        // Change preview object mesh and enable/disable appropriate colliders
        previewObj.ChangeObject(objectChoosen);
    }

    public void ChangeMaterial(int x) { materialChoosen = (MaterialType) x; }

    public static GameController Instance { get { return instance; } }

    public static int ClickableLayerMask { get { return clickableLayerMask; } }

    public PreviewObject PreviewObj { get { return previewObj; } }

    public ObjectType ObjectChoosen { get { return objectChoosen; } }

    public MaterialType MaterialChoosen { get { return materialChoosen; } }
}