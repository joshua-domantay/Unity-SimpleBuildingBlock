using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class ObjExporterScript {
    private static int StartIndex = 0;

    public static void Start() {
        StartIndex = 0;
    }
    public static void End() {
        StartIndex = 0;
    }


    public static string MeshToString(MeshFilter mf, Transform t) {
        Vector3 s = t.localScale;
        Vector3 p = t.localPosition;
        Quaternion r = t.localRotation;


        int numVertices = 0;
        Mesh m = mf.sharedMesh;
        if(!m) {
            return "####Error####";
        }
        Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

        StringBuilder sb = new StringBuilder();

        sb.Append(string.Format("mtllib {0}.mtl", t.name)).Append("\n");

        foreach(Vector3 vv in m.vertices) {
            Vector3 v = t.TransformPoint(vv);
            numVertices++;
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
        }
        sb.Append("\n");
        foreach(Vector3 nn in m.normals) {
            Vector3 v = r * nn;
            sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
        }
        sb.Append("\n");
        foreach(Vector3 v in m.uv) {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for(int material = 0; material < m.subMeshCount; material++) {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            // sb.Append("usemap ").Append(mats[material].name).Append("_").Append(mats[material].mainTexture.name).Append("\n");

            int[] triangles = m.GetTriangles(material);
            for(int i = 0; i < triangles.Length; i += 3) {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
            }
        }

        StartIndex += numVertices;
        return sb.ToString();
    }
}

public class ObjExporter : ScriptableObject {
    [MenuItem("File/Export/Wavefront OBJ")]
    static void DoExportWSubmeshes() {
        DoExport(true);
    }

    [MenuItem("File/Export/Wavefront OBJ (No Submeshes)")]
    static void DoExportWOSubmeshes() {
        DoExport(false);
    }

    static void DoExport(bool makeSubmeshes) {
        if(Selection.gameObjects.Length == 0) {
            Debug.Log("Didn't Export Any Meshes; Nothing was selected!");
            return;
        }

        string meshName = "Output";     // Output file to be called "Output.obj"
        string fileName = EditorUtility.SaveFilePanel("Export .obj file", "", meshName, "obj");

        ObjExporterScript.Start();

        StringBuilder meshString = new StringBuilder();

        meshString.Append("#" + meshName + ".obj"
            + "\n#" + System.DateTime.Now.ToLongDateString()
            + "\n#" + System.DateTime.Now.ToLongTimeString()
            + "\n#-------"
            + "\n\n");

        // Create parent GameObject to create one .obj with all selection
        GameObject allSelection = new GameObject(meshName);

        // Get center from all selected objects
        Vector3 center = Vector3.zero;
        foreach(GameObject selected in Selection.gameObjects) {
            center += selected.transform.position;
        }
        center /= Selection.gameObjects.Length;

        allSelection.transform.position = center;       // Set parent GameObject position to center

        // Set parent of all selection to new parent GameObject
        foreach(GameObject selected in Selection.gameObjects) {
            selected.transform.SetParent(allSelection.transform);
        }
        Transform t = allSelection.transform;       // Use parent GameObject transform instead of first selection

        Vector3 originalPosition = t.position;
        t.position = Vector3.zero;

        if(!makeSubmeshes) {
            meshString.Append("g ").Append(t.name).Append("\n");
        }
        meshString.Append(processTransform(t, makeSubmeshes));

        WriteToFile(meshString.ToString(), fileName);

        t.position = originalPosition;

        ObjExporterScript.End();

        // Remove selected objects parent
        foreach(GameObject selected in Selection.gameObjects) {
            selected.transform.parent = null;
        }
        Destroy(allSelection);      // Discard parent GameObject

        Debug.Log("Exported Mesh: " + fileName);
    }

    static string processTransform(Transform t, bool makeSubmeshes) {
        StringBuilder meshString = new StringBuilder();

        meshString.Append("#" + t.name
        + "\n#-------"
        + "\n");

        if(makeSubmeshes) {
            meshString.Append("g ").Append(t.name).Append("\n");
        }

        MeshFilter mf = t.GetComponent<MeshFilter>();
        if(mf) {
            meshString.Append(ObjExporterScript.MeshToString(mf, t));
        }

        for(int i = 0; i < t.childCount; i++) {
            meshString.Append(processTransform(t.GetChild(i), makeSubmeshes));
        }

        return meshString.ToString();
    }

    static void WriteToFile(string s, string filename) {
        using(StreamWriter sw = new StreamWriter(filename)) {
            sw.Write(s);
        }
    }



    //[MenuItem("GameObject/Export OBJ with Material")]
    static void ExportSelectedObject()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            Debug.LogError("Please select a GameObject first.");
            return;
        }

        string outputPath = EditorUtility.SaveFilePanel("Export OBJ", "", selectedObject.name, "obj");
        if (outputPath.Length == 0)
        {
            return;
        }

        List<string> materialNames = new List<string>();
        MeshRenderer[] renderers = selectedObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            foreach (Material material in materials)
            {
                if (material != null && !materialNames.Contains(material.name))
                {
                    materialNames.Add(material.name);
                }
            }
        }

        using (StreamWriter sw = new StreamWriter(outputPath))
        {
            sw.WriteLine("mtllib {0}.mtl", selectedObject.name);

            MeshFilter[] filters = selectedObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter filter in filters)
            {
                Mesh mesh = filter.sharedMesh;

                // Vertices
                foreach (Vector3 vertex in mesh.vertices)
                {
                    sw.WriteLine("v {0} {1} {2}", vertex.x, vertex.y, vertex.z);
                }

                // Normals
                foreach (Vector3 normal in mesh.normals)
                {
                    sw.WriteLine("vn {0} {1} {2}", normal.x, normal.y, normal.z);
                }

                // UVs
                foreach (Vector2 uv in mesh.uv)
                {
                    sw.WriteLine("vt {0} {1}", uv.x, uv.y);
                }

                // Material
                Material material = filter.GetComponent<MeshRenderer>().sharedMaterial;
                if (material != null)
                {
                    int materialIndex = materialNames.IndexOf(material.name);
                    if (materialIndex == -1)
                    {
                        materialIndex = materialNames.Count;
                        materialNames.Add(material.name);
                        ExportMaterial(outputPath, material);
                    }
                    sw.WriteLine("usemtl {0}", material.name);
                }

                // Faces
                int[] triangles = mesh.triangles;
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int i1 = triangles[i] + 1;
                    int i2 = triangles[i + 1] + 1;
                    int i3 = triangles[i + 2] + 1;
                    sw.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", i1, i2, i3);
                }
            }
        }

        AssetDatabase.Refresh();
    }

    static void ExportMaterial(string outputPath, Material material)
    {
        using (StreamWriter sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(outputPath), string.Format("{0}.mtl", material.name))))
        {
            Color color = material.color;
            sw.WriteLine("newmtl {0}", material.name);
            sw.WriteLine("Ka {0} {1} {2}", color.r, color.g, color.b);
            sw.WriteLine("Kd {0} {1} {2}", color.r, color.g, color.b);
            sw.WriteLine("Ks 0 0 0");
            sw.WriteLine("d {0}", color.a);
            if (material.mainTexture != null) {
                string texturePath = AssetDatabase.GetAssetPath(material.mainTexture);
                if (!string.IsNullOrEmpty(texturePath))
                {
                    string textureFileName = Path.GetFileName(texturePath);
                    string textureOutputPath = Path.Combine(Path.GetDirectoryName(outputPath), textureFileName);
                    FileUtil.CopyFileOrDirectory(texturePath, textureOutputPath);
                    sw.WriteLine("map_Kd {0}", textureFileName);
                }
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("File/Export/MTL")]
    public static void ExportSelectedObjectMTL()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogWarning("No object selected");
            return;
        }

        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();
        Material[] materials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].sharedMaterial;
        }

        if (materials.Length == 0)
        {
            Debug.LogWarning("No materials found");
            return;
        }

        string texturePath = AssetDatabase.GetAssetPath(materials[0].mainTexture);

        if (string.IsNullOrEmpty(texturePath))
        {
            Debug.LogWarning("No texture file found");
            return;
        }

        string folderPath = Path.GetDirectoryName(texturePath);
        string objectName = selectedObject.name;
        string mtlPath = Path.Combine(folderPath, objectName + ".mtl");

        using (StreamWriter writer = new StreamWriter(mtlPath))
        {
            // Write the necessary data to the .mtl file
            // Refer to the Wavefront .mtl file format specification for the required data
            writer.WriteLine("newmtl " + objectName);
            writer.WriteLine("Ka 0.000 0.000 0.000");
            writer.WriteLine("Kd 1.000 1.000 1.000");
            writer.WriteLine("Ks 0.000 0.000 0.000");
            writer.WriteLine("d 1.0");
            writer.WriteLine("illum 1");
            writer.WriteLine("map_Kd " + Path.GetFileName(texturePath));

            foreach (Material material in materials)
            {
                if (material == null)
                    continue;

                writer.WriteLine("newmtl " + material.name);
                writer.WriteLine("Ka 0.000 0.000 0.000");
                writer.WriteLine("Kd 1.000 1.000 1.000");
                writer.WriteLine("Ks 0.000 0.000 0.000");
                writer.WriteLine("d 1.0");
                writer.WriteLine("illum 1");
                writer.WriteLine("usemtl " + material.name);
                writer.WriteLine("map_Kd " + Path.GetFileName(AssetDatabase.GetAssetPath(material.mainTexture)));
            }
        }

        AssetDatabase.Refresh();

        Debug.Log("MTL file exported: " + mtlPath);
    }

    [MenuItem("File/Export/Diffuse Texture")]
    private static void GenerateDiffuse()
    {
        // Create parent GameObject to create one .obj with all selection
        GameObject allSelection = new GameObject(Selection.gameObjects[0].name);

        // Get center from all selected objects
        Vector3 center = Vector3.zero;
        foreach(GameObject selected in Selection.gameObjects) {
            center += selected.transform.position;
        }
        center /= Selection.gameObjects.Length;

        allSelection.transform.position = center;       // Set parent GameObject position to center

        // Set parent of all selection to new parent GameObject
        foreach(GameObject selected in Selection.gameObjects) {
            selected.transform.SetParent(allSelection.transform);
        }

        // Find all MeshRenderer components in the selected GameObject and its children
        MeshRenderer[] meshRenderers = allSelection.GetComponentsInChildren<MeshRenderer>();

        // Create a list of all the unique main textures used by the MeshRenderer components
        List<Texture2D> mainTextures = new List<Texture2D>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            Material material = meshRenderer.sharedMaterial;
            if (material == null)
            {
                Debug.LogWarning("One of the MeshRenderer components does not have a material assigned.");
                continue;
            }

            Texture2D mainTexture = material.mainTexture as Texture2D;
            if (mainTexture == null)
            {
                Debug.LogWarning("One of the materials does not have a main texture.");
                continue;
            }

            if (!mainTextures.Contains(mainTexture))
            {
                mainTextures.Add(mainTexture);
            }
        }

        // Create a new diffuse texture with the combined size of all the unique main textures
        int width = 0;
        int height = 0;
        foreach (Texture2D mainTexture in mainTextures)
        {
            width += mainTexture.width;
            height = Mathf.Max(height, mainTexture.height);
        }
        Texture2D diffuseTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Copy the RGB values from each main texture to the appropriate location in the diffuse texture
        int x = 0;
        foreach (Texture2D mainTexture in mainTextures)
        {
            Color[] pixels = mainTexture.GetPixels();
            for (int y = 0; y < mainTexture.height; y++)
            {
                for (int i = 0; i < mainTexture.width; i++)
                {
                    int index = y * mainTexture.width + i;
                    Color pixel = pixels[index];
                    diffuseTexture.SetPixel(x + i, y, new Color(pixel.r, pixel.g, pixel.b));
                }
            }
            x += mainTexture.width;
        }

        diffuseTexture.Apply();

        // Save the diffuse texture as a PNG file in the same directory as the scene file, using the name of the GameObject as part of the file name
        byte[] bytes = diffuseTexture.EncodeToPNG();
        string filePath = Application.dataPath + "/" + allSelection.name + "_Diffuse.png";
        System.IO.File.WriteAllBytes(filePath, bytes);

        Debug.Log("Diffuse texture saved to " + filePath);

        // Remove selected objects parent
        foreach(GameObject selected in Selection.gameObjects) {
            selected.transform.parent = null;
        }
        Destroy(allSelection);      // Discard parent GameObject
    }
}