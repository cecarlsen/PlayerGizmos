/*
	Copyright © Carl Emil Carlsen 2021
	http://cec.dk

	API is designed to mimic the Gizmos class, with a few additional shapes
	Not fully featured yet.

	TODO
		- Add remaining shapes.
		- Use new Mesh API (combining positions and colors into one array).
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; 

[ExecuteInEditMode]
public class PlayerGizmos : MonoBehaviour
{
	static PlayerGizmos _self;

	Material _lineMaterial;
	Mesh _lineMesh;

	List<Camera> _activeCameras = new List<Camera>();
	CommandBuffer _commandBuffer;

	public static Color color = Color.white;
	public static Matrix4x4 matrix = Matrix4x4.identity;

	List<Vector3> _lineVertices = new List<Vector3>();
	List<Color32> _lineColors = new List<Color32>();
	List<ushort> _lineIndices = new List<ushort>();

	Vector2[] _unitCircleLookup;

	const CameraEvent renderEvent = CameraEvent.AfterForwardAlpha;


	void Awake()
	{
		_self = this;
	}


	void OnDestroy()
	{
		if( Application.isPlaying ) {
			Destroy( _lineMaterial );
		} else {
			DestroyImmediate( _lineMaterial );
		}
	}


	void OnEnable()
	{
		Camera.onPreRender += OnPreRenderCamera;
	}


	void OnDisable()
	{
		Camera.onPreRender -= OnPreRenderCamera;
		foreach( Camera cam in _activeCameras ) if( cam && _commandBuffer != null ) cam.RemoveCommandBuffer( renderEvent, _commandBuffer );
	}


	void LateUpdate()
	{
		EnsureResources();

		if( _lineVertices.Count == 0 )
		{
			if( _lineMesh.vertexCount > 0 ) _lineMesh.Clear();
			return;
		}

		_lineMesh.SetVertices( _lineVertices );
		_lineMesh.SetIndices( _lineIndices, MeshTopology.Lines, 0 );
		_lineMesh.SetColors( _lineColors );

		_lineVertices.Clear();
		_lineIndices.Clear();
		_lineColors.Clear();
	}


	void OnPreRenderCamera( Camera cam )
	{
		if( cam.cameraType == CameraType.Preview || _activeCameras.Contains( cam ) ) return;
		_activeCameras.Add( cam );

		EnsureResources();

		cam.AddCommandBuffer( renderEvent, _commandBuffer );
	}


	static void Init()
	{
		_self = FindObjectOfType<PlayerGizmos>();
		if( !_self ) {
			_self = new GameObject( nameof( PlayerGizmos ) ).AddComponent<PlayerGizmos>();
			_self.gameObject.hideFlags = HideFlags.HideAndDontSave;
		}
	}


	void EnsureResources()
	{

		if( !_lineMaterial ) {
			_lineMaterial = new Material( Shader.Find( "Hidden/" + nameof( PlayerGizmos ) ) );
			_lineMaterial.name = nameof( PlayerGizmos );
		}
		if( !_lineMesh ) {
			_lineMesh = new Mesh();
			_lineMesh.name = nameof( PlayerGizmos );

		}
		if( _commandBuffer == null ) {
			_commandBuffer = new CommandBuffer();
			_commandBuffer.name = nameof( PlayerGizmos );
			_commandBuffer.DrawMesh( _lineMesh, Matrix4x4.identity, _lineMaterial, 0 );
		}
	}


	public static void DrawLine( Vector3 posA, Vector3 posB )
	{
		if( !_self ) Init();
		ushort i = (ushort) _self._lineVertices.Count;
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( posA ) );
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( posB ) );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineIndices.Add( i );
		_self._lineIndices.Add( (ushort) (i+1) );
	}


	public static void DrawWireCube( Vector3 center, Vector3 size )
	{
		if( !_self ) Init();

		Vector3 extents = size * 0.5f;
		Vector3 min = matrix.MultiplyPoint3x4( center - extents );
		Vector3 max = matrix.MultiplyPoint3x4( center + extents );
		ushort i = (ushort) _self._lineVertices.Count;
		_self._lineVertices.Add( min );
		_self._lineVertices.Add( new Vector3( max.x, min.y, min.z ) );
		_self._lineVertices.Add( new Vector3( max.x, min.y, max.z ) );
		_self._lineVertices.Add( new Vector3( min.x, min.y, max.z ) );
		_self._lineVertices.Add( new Vector3( min.x, max.y, min.z ) );
		_self._lineVertices.Add( new Vector3( max.x, max.y, min.z ) );
		_self._lineVertices.Add( max );
		_self._lineVertices.Add( new Vector3( min.x, max.y, max.z ) );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineIndices.Add( (ushort) (i+0) ); _self._lineIndices.Add( (ushort) (i+1) );
		_self._lineIndices.Add( (ushort) (i+1) ); _self._lineIndices.Add( (ushort) (i+2) );
		_self._lineIndices.Add( (ushort) (i+2) ); _self._lineIndices.Add( (ushort) (i+3) );
		_self._lineIndices.Add( (ushort) (i+3) ); _self._lineIndices.Add( (ushort) (i+0) );
		_self._lineIndices.Add( (ushort) (i+4) ); _self._lineIndices.Add( (ushort) (i+5) );
		_self._lineIndices.Add( (ushort) (i+5) ); _self._lineIndices.Add( (ushort) (i+6) );
		_self._lineIndices.Add( (ushort) (i+6) ); _self._lineIndices.Add( (ushort) (i+7) );
		_self._lineIndices.Add( (ushort) (i+7) ); _self._lineIndices.Add( (ushort) (i+4) );
		_self._lineIndices.Add( (ushort) (i+0) ); _self._lineIndices.Add( (ushort) (i+4) );
		_self._lineIndices.Add( (ushort) (i+1) ); _self._lineIndices.Add( (ushort) (i+5) );
		_self._lineIndices.Add( (ushort) (i+2) ); _self._lineIndices.Add( (ushort) (i+6) );
		_self._lineIndices.Add( (ushort) (i+3) ); _self._lineIndices.Add( (ushort) (i+7) );
	}


	public static void DrawWireSphere( Vector3 center, float radius )
	{
		if( !_self ) Init();
		if( _self._unitCircleLookup == null ) _self.CreateUnitCircleLookup();

		ushort i = (ushort) _self._lineVertices.Count;
		ushort i0 = i;
		ushort pLast = (ushort) ( _self._unitCircleLookup.Length-1 );
		for( ushort p = 0; p < _self._unitCircleLookup.Length; p++ ) {
			Vector3 point = _self._unitCircleLookup[ p ];
			float da = point.x * radius;
			float db = point.y * radius;
			_self._lineVertices.Add( new Vector3( center.x, center.y + da, center.z + db ) );
			_self._lineColors.Add( color );
			_self._lineIndices.Add( i++ ); _self._lineIndices.Add( p == pLast ? i0 : (ushort) (i+2) );
			_self._lineVertices.Add( new Vector3( center.x + da, center.y, center.z + db ) );
			_self._lineColors.Add( color );
			_self._lineIndices.Add( i++ ); _self._lineIndices.Add( p == pLast ? (ushort) (i0+1) : (ushort) (i+2) );
			_self._lineVertices.Add( new Vector3( center.x + da, center.y + db, center.z ) );
			_self._lineColors.Add( color );
			_self._lineIndices.Add( i++ ); _self._lineIndices.Add( p == pLast ? (ushort) (i0+1) : (ushort) (i+2) );
		}
	}



	public static void DrawWireCircle( Vector3 center, float radius )
	{
		if( !_self ) Init();
		if( _self._unitCircleLookup == null ) _self.CreateUnitCircleLookup();

		ushort i = (ushort) _self._lineVertices.Count;
		ushort iFirst = i;
		ushort pLast = (ushort) (_self._unitCircleLookup.Length-1);
		for( ushort p = 0; p < _self._unitCircleLookup.Length; p++ ) {
			Vector3 point = _self._unitCircleLookup[p];
			float xOff = point.x * radius;
			float yOff = point.y * radius;
			_self._lineVertices.Add( new Vector3( center.x + xOff, center.y + yOff, center.y ) );
			_self._lineColors.Add( color );
			_self._lineIndices.Add( i );
			_self._lineIndices.Add( p == pLast ? iFirst : ++i );
		}
	}


	public static void DrawFrustum( Vector3 center, float fov, float maxRange, float minRange, float aspect )
	{
		if( !_self ) Init();

		float extents = Mathf.Tan( Mathf.Deg2Rad * fov * 0.5f );
		float extentsY = extents * minRange;
		float extentsX = extentsY * aspect;
		float z = center.z + minRange;
		ushort i = (ushort) _self._lineVertices.Count;
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x - extentsX, center.y - extentsY, z ) ) );
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x + extentsX, center.y - extentsY, z ) ) );
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x + extentsX, center.y + extentsY, z ) ) );
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x - extentsX, center.y + extentsY, z ) ) );
		extentsY = extents * maxRange;
		extentsX = extentsY * aspect;
		z = center.z + maxRange;
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x - extentsX, center.y - extentsY, z ) ) );
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x + extentsX, center.y - extentsY, z ) ) );
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x + extentsX, center.y + extentsY, z ) ) );
		_self._lineVertices.Add( matrix.MultiplyPoint3x4( new Vector3( center.x - extentsX, center.y + extentsY, z ) ) );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineColors.Add( color );
		_self._lineIndices.Add( (ushort) (i+0) ); _self._lineIndices.Add( (ushort) (i+1) );
		_self._lineIndices.Add( (ushort) (i+1) ); _self._lineIndices.Add( (ushort) (i+2) );
		_self._lineIndices.Add( (ushort) (i+2) ); _self._lineIndices.Add( (ushort) (i+3) );
		_self._lineIndices.Add( (ushort) (i+3) ); _self._lineIndices.Add( (ushort) (i+0) );
		_self._lineIndices.Add( (ushort) (i+0) ); _self._lineIndices.Add( (ushort) (i+1) );
		_self._lineIndices.Add( (ushort) (i+1) ); _self._lineIndices.Add( (ushort) (i+2) );
		_self._lineIndices.Add( (ushort) (i+2) ); _self._lineIndices.Add( (ushort) (i+3) );
		_self._lineIndices.Add( (ushort) (i+3) ); _self._lineIndices.Add( (ushort) (i+0) );
		_self._lineIndices.Add( (ushort) (i+4) ); _self._lineIndices.Add( (ushort) (i+5) );
		_self._lineIndices.Add( (ushort) (i+5) ); _self._lineIndices.Add( (ushort) (i+6) );
		_self._lineIndices.Add( (ushort) (i+6) ); _self._lineIndices.Add( (ushort) (i+7) );
		_self._lineIndices.Add( (ushort) (i+7) ); _self._lineIndices.Add( (ushort) (i+4) );
		_self._lineIndices.Add( (ushort) (i+0) ); _self._lineIndices.Add( (ushort) (i+4) );
		_self._lineIndices.Add( (ushort) (i+1) ); _self._lineIndices.Add( (ushort) (i+5) );
		_self._lineIndices.Add( (ushort) (i+2) ); _self._lineIndices.Add( (ushort) (i+6) );
		_self._lineIndices.Add( (ushort) (i+3) ); _self._lineIndices.Add( (ushort) (i+7) );
	}



	void CreateUnitCircleLookup()
	{
		if( _unitCircleLookup != null ) return;

		_unitCircleLookup = new Vector2[ 64 ];
		for( int i = 0; i < _unitCircleLookup.Length; i++ ) {
			float a = i * Mathf.PI * 2 / (_unitCircleLookup.Length-1f);
			_unitCircleLookup[i] = new Vector2( Mathf.Cos( a ), Mathf.Sin( a ) );
		}
	}

}