/*
	Copyright © Carl Emil Carlsen 2021
	http://cec.dk
*/

using UnityEngine;

[ExecuteInEditMode]
public class PlayerGizmosExample : MonoBehaviour
{
	public bool usePlayerGizmos = true;
	public bool useAnimatedMatrix = true;
	public UnityExtensions.Layer _playerGizmoLayer = new UnityExtensions.Layer( 1 );
	


	void Update()
	{
		if( !usePlayerGizmos ) return;

		PlayerGizmos.layer = _playerGizmoLayer.index;

		PlayerGizmos.matrix = GetAnimatedMatrix();

		PlayerGizmos.color = Color.yellow;
		PlayerGizmos.DrawLine( Vector3.zero, Vector3.up );

		PlayerGizmos.color = Color.green;
		PlayerGizmos.DrawWireCube( Vector3.right, Vector3.one * 0.5f );

		PlayerGizmos.color = Color.magenta;
		PlayerGizmos.DrawWireSphere( Vector3.right*2, 0.5f );

		PlayerGizmos.color = Color.red;
		PlayerGizmos.DrawFrustum( Vector3.left*2, 60, 0.5f, 4, 1.77f );

		PlayerGizmos.color = Color.cyan;
		PlayerGizmos.DrawRect( new Rect( 3, -0.25f, 0.5f, 0.5f ) );

		PlayerGizmos.matrix = Matrix4x4.identity;

		PlayerGizmos.layer = 0;
	}


	void OnDrawGizmos()
	{
		if( usePlayerGizmos ) return;
		
		Gizmos.matrix = GetAnimatedMatrix();

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine( Vector3.zero, Vector3.up );

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube( Vector3.right, Vector3.one * 0.5f );

		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere( Vector3.right * 2, 0.5f );

		Gizmos.color = Color.red;
		Gizmos.DrawFrustum( Vector3.left*2, 60, 0.5f, 4, 1.77f );

		Gizmos.matrix = Matrix4x4.identity;
	}


	Matrix4x4 GetAnimatedMatrix()
	{
		float a = useAnimatedMatrix ? Time.time * 0.3f : Mathf.Deg2Rad * 45;
		return Matrix4x4.TRS(
			Vector3.right * Mathf.Sin( a ), 
			Quaternion.Euler( 0, a * Mathf.Rad2Deg, 0 ), 
			Vector3.one//Vector3.one * Mathf.Lerp( 0.5f, 1, Mathf.Sin( t * 0.8461f + 2.8936f ) * 0.5f + 0.5f )
		);
	}
}