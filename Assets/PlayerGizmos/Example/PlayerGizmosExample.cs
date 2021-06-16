/*
	Copyright © Carl Emil Carlsen 2021
	http://cec.dk
*/

using UnityEngine;

[ExecuteInEditMode]
public class PlayerGizmosExample : MonoBehaviour
{
	public bool usePlayerGizmos = true;


	void Update()
	{
		if( !usePlayerGizmos ) return;

		PlayerGizmos.color = Color.yellow;
		PlayerGizmos.DrawLine( Vector3.zero, Vector3.up );

		PlayerGizmos.color = Color.green;
		PlayerGizmos.DrawWireCube( Vector3.right, Vector3.one * 0.5f );

		PlayerGizmos.color = Color.magenta;
		PlayerGizmos.DrawWireSphere( Vector3.left, 0.5f );

		Camera cam = Camera.main;
		if( cam ) {
			PlayerGizmos.color = Color.red;
			PlayerGizmos.matrix = Matrix4x4.TRS( cam.transform.position, cam.transform.rotation, Vector3.one );
			PlayerGizmos.DrawFrustum( Vector3.zero, cam.fieldOfView, cam.nearClipPlane, cam.farClipPlane, cam.aspect );
			PlayerGizmos.matrix = Matrix4x4.identity;
		}
	}


	void OnDrawGizmos()
	{
		if( usePlayerGizmos ) return;

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine( Vector3.zero, Vector3.up );

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube( Vector3.right, Vector3.one * 0.5f );

		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere( Vector3.left, 0.5f );

		Camera cam = Camera.main;
		if( cam ) {
			Gizmos.color = Color.red;
			Gizmos.matrix = Matrix4x4.TRS( cam.transform.position, cam.transform.rotation, Vector3.one );
			Gizmos.DrawFrustum( Vector3.zero, cam.fieldOfView, cam.nearClipPlane, cam.farClipPlane, cam.aspect );
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}