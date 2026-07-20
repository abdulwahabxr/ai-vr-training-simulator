// WarehouseBuilder.cs
// Spawns a grey-box VR training room from Unity primitives.
// MUST live inside a folder named "Editor" (e.g. Assets/Editor/WarehouseBuilder.cs).
// Run from the top menu: Tools > Build Warehouse Room
//
// URP note: materials use the "Universal Render Pipeline/Lit" shader.
// If you're on the Built-in pipeline, change SHADER_NAME below to "Standard".

using UnityEngine;
using UnityEditor;

public static class WarehouseBuilder
{
    private const string SHADER_NAME = "Universal Render Pipeline/Lit";

    [MenuItem("Tools/Build Warehouse Room")]
    public static void BuildRoom()
    {
        // Root + category parents
        var environment = new GameObject("Environment");
        Undo.RegisterCreatedObjectUndo(environment, "Build Warehouse Room");

        var walls   = MakeParent("Walls",   environment.transform);
        var shelves = MakeParent("Shelves", environment.transform);
        var hazards = MakeParent("Hazards", environment.transform);

        // Materials (color-coded so hazards are easy to spot)
        var mFloor  = MakeMat("M_Floor",  new Color(0.75f, 0.75f, 0.75f));
        var mWall   = MakeMat("M_Wall",   new Color(0.55f, 0.55f, 0.55f));
        var mShelf  = MakeMat("M_Shelf",  new Color(0.35f, 0.35f, 0.35f));
        var mSpill  = MakeMat("M_Spill",  new Color(0.20f, 0.45f, 0.95f)); // blue
        var mWiring = MakeMat("M_Wiring", new Color(0.95f, 0.85f, 0.10f)); // yellow
        var mExit   = MakeMat("M_Exit",   new Color(0.20f, 0.80f, 0.30f)); // green
        var mCrate  = MakeMat("M_Crate",  new Color(0.70f, 0.45f, 0.20f)); // brown
        var mStart  = MakeMat("M_Start",  new Color(0.15f, 0.75f, 0.75f)); // teal

        // Floor (top surface sits at Y = 0)
        Cube("Floor", environment.transform, mFloor,
            new Vector3(0f, -0.1f, 0f), new Vector3(10f, 0.2f, 10f));

        // Walls (3m tall)
        Cube("Wall_North", walls, mWall, new Vector3(0f, 1.5f,  5f), new Vector3(10f, 3f, 0.2f));
        Cube("Wall_South", walls, mWall, new Vector3(0f, 1.5f, -5f), new Vector3(10f, 3f, 0.2f));
        Cube("Wall_East",  walls, mWall, new Vector3( 5f, 1.5f, 0f), new Vector3(0.2f, 3f, 10f));
        Cube("Wall_West",  walls, mWall, new Vector3(-5f, 1.5f, 0f), new Vector3(0.2f, 3f, 10f));

        // Shelves
        Cube("Shelf_1", shelves, mShelf, new Vector3( 3f,   1f,  3.5f), new Vector3(2f, 2f, 0.5f));
        Cube("Shelf_2", shelves, mShelf, new Vector3(-3.5f, 1f, -0.5f), new Vector3(0.5f, 2f, 3f));

        // Hazard 1: spill (flat puddle on floor)
        Cube("Hazard_Spill", hazards, mSpill,
            new Vector3(0f, 0.02f, 1.5f), new Vector3(1.5f, 0.04f, 1.5f));

        // Hazard 2: exposed wiring (junction box on east wall)
        Cube("Hazard_Wiring", hazards, mWiring,
            new Vector3(4.7f, 1.1f, -0.5f), new Vector3(0.4f, 0.5f, 0.15f));

        // Hazard 3: blocked fire exit (door marker + two crates)
        Cube("Exit_Door", hazards, mExit,
            new Vector3(0f, 1f, -4.88f), new Vector3(1.2f, 2f, 0.05f));
        Cube("Crate_1", hazards, mCrate,
            new Vector3(0f, 0.5f, -4f), new Vector3(1f, 1f, 1f));
        Cube("Crate_2", hazards, mCrate,
            new Vector3(0.6f, 0.4f, -3.7f), new Vector3(0.8f, 0.8f, 0.8f));

        // Start marker (where you'll place the XR Origin)
        Cylinder("StartMarker", environment.transform, mStart,
            new Vector3(-3.5f, 0.02f, 3.5f), new Vector3(0.6f, 0.02f, 0.6f));

        Selection.activeGameObject = environment;
        Debug.Log("Warehouse room built. Place your XR Origin at (-3.5, 0, 3.5).");
    }

    // --- helpers ---

    private static Transform MakeParent(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static GameObject Cube(string name, Transform parent, Material mat,
        Vector3 pos, Vector3 scale)
    {
        return Prim(PrimitiveType.Cube, name, parent, mat, pos, scale);
    }

    private static GameObject Cylinder(string name, Transform parent, Material mat,
        Vector3 pos, Vector3 scale)
    {
        return Prim(PrimitiveType.Cylinder, name, parent, mat, pos, scale);
    }

    private static GameObject Prim(PrimitiveType type, string name, Transform parent,
        Material mat, Vector3 pos, Vector3 scale)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    private static Material MakeMat(string name, Color color)
    {
        var shader = Shader.Find(SHADER_NAME);
        if (shader == null)
        {
            Debug.LogWarning($"Shader '{SHADER_NAME}' not found. Falling back to Standard.");
            shader = Shader.Find("Standard");
        }

        var mat = new Material(shader) { name = name };

        // URP Lit uses _BaseColor; Standard uses _Color. Set whichever exists.
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color"))     mat.SetColor("_Color", color);

        // Save as an asset so it persists and can be reused.
        const string dir = "Assets/GreyboxMaterials";
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder("Assets", "GreyboxMaterials");

        string path = $"{dir}/{name}.mat";
        AssetDatabase.CreateAsset(mat, AssetDatabase.GenerateUniqueAssetPath(path));
        return mat;
    }
}
