using UnityEngine;
using UnityEditor;

public class PackageTool
{
    [MenuItem("Package/Update Package")]
    static void UpdatePackage()
    {
		AssetDatabase.ExportPackage( "Assets/PlayerGizmos", "PlayerGizmos.unitypackage", ExportPackageOptions.Recurse );
        Debug.Log( "Exported package!\n");
    }
}
