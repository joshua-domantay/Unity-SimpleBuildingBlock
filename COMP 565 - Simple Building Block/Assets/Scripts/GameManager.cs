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

public class GameManager : MonoBehaviour {
    private static GameManager instance;

    private ObjectType objectChoosen = ObjectType.CUBE;
    private MaterialType materialChoosen = MaterialType.STONE;

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
        switch(objectChoosen) {
            case ObjectType.CUBE:
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.tag = "MyCube";
                obj.GetComponent<BoxCollider>().isTrigger = true;
                break;
            case ObjectType.SPHERE:
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.tag = "MySphere";
                obj.GetComponent<SphereCollider>().isTrigger = true;
                break;
            default:        // ObjectType.CAPSULE
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obj.tag = "MyCapsule";
                obj.GetComponent<CapsuleCollider>().isTrigger = true;
                break;
        }
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

    public void ChangeObject(ObjectType x) { objectChoosen = x; }

    public void ChangeMaterial(MaterialType x) { materialChoosen = x; }

    public static GameManager Instance { get { return instance; } }

    public ObjectType ObjectChoosen { get { return objectChoosen; } }
    public MaterialType MaterialChoosen { get { return materialChoosen; } }
}