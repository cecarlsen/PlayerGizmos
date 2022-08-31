/*
	Copyright © Carl Emil Carlsen 2021
	http://cec.dk

	API is designed to mimic the Gizmos class, with a few additional shapes
	Not fully featured yet.

	TODO
		- Move transformations to the GPU. Use custom instancing.
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

	Dictionary<int,LayerData> _layerLookup = new Dictionary<int,LayerData>();

	Color _color = Color.white;
	Matrix4x4 _matrix = Matrix4x4.identity;
	int _layer;

	

	Vector2[] _unitCircleLookup;

	const CameraEvent renderEvent = CameraEvent.AfterForwardAlpha;
	const string logPrepend = "<b>[" + nameof( PlayerGizmos ) + "]</b> ";


	public static Color color {
		get {
			if( !_self ) Init();
			return _self._color;
		}
		set {
			if( !_self ) Init();
			_self._color = value;
		}
	}

	public static Matrix4x4 matrix {
		get {
			if( !_self ) Init();
			return _self._matrix;
		}
		set {
			if( !_self ) Init();
			_self._matrix = value;
		}
	}

	public static int layer {
		get {
			if( !_self ) Init();
			return _self._layer;
		}
		set {
			if( value < 0 || value >= 32 ) {
				Debug.LogWarning( logPrepend+ "Layer index must be between 0-31\n" );
				return;
			}
			if( !_self ) Init();
			_self._layer = value;
		}
	}


	class LayerData
	{
		public Mesh lineMesh;
		public List<Vector3> lineVertices = new List<Vector3>();
		public List<Color32> lineColors = new List<Color32>();
		public List<ushort> lineIndices = new List<ushort>();

		public int vertexCount => lineVertices.Count;


		public LayerData()
		{
			lineMesh = new Mesh();
			lineMesh.name = nameof( PlayerGizmos );
			lineMesh.hideFlags = HideFlags.HideAndDontSave;
		}


		public void UploadToMesh()
		{
			lineMesh.Clear();
			lineMesh.SetVertices( lineVertices );
			lineMesh.SetIndices( lineIndices, MeshTopology.Lines, 0 );
			lineMesh.SetColors( lineColors );

			lineVertices.Clear();
			lineIndices.Clear();
			lineColors.Clear();
		}
	}


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


	void LateUpdate()
	{
		EnsureResources();

		foreach( KeyValuePair<int,LayerData> pair in _layerLookup )
		{
			int layer = pair.Key;
			LayerData data = pair.Value;
			if( data.vertexCount == 0 ) continue;

			pair.Value.UploadToMesh();
			Graphics.DrawMesh( data.lineMesh, Matrix4x4.identity, _lineMaterial, layer );
		}
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
	}


	public static void DrawLine( Vector3 posA, Vector3 posB )
	{
		if( !_self ) Init();
		LayerData data = _self.GetOrCreateLayerData();

		ushort i = (ushort) data.lineVertices.Count;
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( posA ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( posB ) );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineIndices.Add( i );
		data.lineIndices.Add( (ushort) (i+1) );
	}


	public static void DrawRect( Rect rect )
	{
		if( !_self ) Init();
		LayerData data = _self.GetOrCreateLayerData();

		ushort i = (ushort) data.lineVertices.Count;
		Vector2 min = rect.min;
		Vector2 max = rect.max;
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( min.x, min.y ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( min.x, max.y ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( max.x, max.y ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( max.x, min.y ) ) );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineIndices.Add( i ); data.lineIndices.Add( (ushort) ( i + 1 ) );
		data.lineIndices.Add( (ushort) ( i + 1 ) ); data.lineIndices.Add( (ushort) ( i + 2 ) );
		data.lineIndices.Add( (ushort) ( i + 2 ) ); data.lineIndices.Add( (ushort) ( i + 3 ) );
		data.lineIndices.Add( (ushort) ( i + 3 ) ); data.lineIndices.Add( i );
	}


	public static void DrawWireCube( Vector3 center, Vector3 size )
	{
		if( !_self ) Init();
		LayerData data = _self.GetOrCreateLayerData();

		Vector3 extents = size * 0.5f;
		Vector3 min = center - extents;
		Vector3 max = center + extents;
		ushort i = (ushort) data.lineVertices.Count;
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( min ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( max.x, min.y, min.z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( max.x, min.y, max.z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( min.x, min.y, max.z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( min.x, max.y, min.z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( max.x, max.y, min.z ) ));
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( max ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( min.x, max.y, max.z ) ) );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineIndices.Add( (ushort) (i+0) ); data.lineIndices.Add( (ushort) (i+1) );
		data.lineIndices.Add( (ushort) (i+1) ); data.lineIndices.Add( (ushort) (i+2) );
		data.lineIndices.Add( (ushort) (i+2) ); data.lineIndices.Add( (ushort) (i+3) );
		data.lineIndices.Add( (ushort) (i+3) ); data.lineIndices.Add( (ushort) (i+0) );
		data.lineIndices.Add( (ushort) (i+4) ); data.lineIndices.Add( (ushort) (i+5) );
		data.lineIndices.Add( (ushort) (i+5) ); data.lineIndices.Add( (ushort) (i+6) );
		data.lineIndices.Add( (ushort) (i+6) ); data.lineIndices.Add( (ushort) (i+7) );
		data.lineIndices.Add( (ushort) (i+7) ); data.lineIndices.Add( (ushort) (i+4) );
		data.lineIndices.Add( (ushort) (i+0) ); data.lineIndices.Add( (ushort) (i+4) );
		data.lineIndices.Add( (ushort) (i+1) ); data.lineIndices.Add( (ushort) (i+5) );
		data.lineIndices.Add( (ushort) (i+2) ); data.lineIndices.Add( (ushort) (i+6) );
		data.lineIndices.Add( (ushort) (i+3) ); data.lineIndices.Add( (ushort) (i+7) );
	}


	public static void DrawWireSphere( Vector3 center, float radius )
	{
		if( !_self ) Init();
		LayerData data = _self.GetOrCreateLayerData();
		if( _self._unitCircleLookup == null ) _self.CreateUnitCircleLookup();

		ushort i = (ushort) data.lineVertices.Count;
		ushort i0 = i;
		ushort pLast = (ushort) ( _self._unitCircleLookup.Length-1 );
		for( ushort p = 0; p < _self._unitCircleLookup.Length; p++ ) {
			Vector2 point = _self._unitCircleLookup[ p ];
			float da = point.x * radius;
			float db = point.y * radius;
			data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x, center.y + da, center.z + db ) ) );
			data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x + da, center.y, center.z + db ) ) );
			data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x + da, center.y + db, center.z ) ) );
			data.lineColors.Add( _self._color );
			data.lineColors.Add( _self._color );
			data.lineColors.Add( _self._color );
			data.lineIndices.Add( i++ ); data.lineIndices.Add( p == pLast ? i0 : (ushort) (i+2) );
			data.lineIndices.Add( i++ ); data.lineIndices.Add( p == pLast ? (ushort) (i0+1) : (ushort) (i+2) );
			data.lineIndices.Add( i++ ); data.lineIndices.Add( p == pLast ? (ushort) (i0+1) : (ushort) (i+2) );
		}
	}


	public static void DrawWireCircle( Vector3 center, float radius )
	{
		if( !_self ) Init();
		LayerData data = _self.GetOrCreateLayerData();
		if( _self._unitCircleLookup == null ) _self.CreateUnitCircleLookup();

		ushort i = (ushort) data.lineVertices.Count;
		ushort iFirst = i;
		ushort pLast = (ushort) (_self._unitCircleLookup.Length-1);
		for( ushort p = 0; p < _self._unitCircleLookup.Length; p++ ) {
			Vector2 point = _self._unitCircleLookup[ p ];
			float xOff = point.x * radius;
			float yOff = point.y * radius;
			data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x + xOff, center.y + yOff, center.z ) ) );
			data.lineColors.Add( _self._color );
			data.lineIndices.Add( i ); data.lineIndices.Add( p == pLast ? iFirst : ++i );
		}
	}


	public static void DrawFrustum( Vector3 center, float fov, float maxRange, float minRange, float aspect )
	{
		if( !_self ) Init();
		LayerData data = _self.GetOrCreateLayerData();

		float extents = Mathf.Tan( Mathf.Deg2Rad * fov * 0.5f );
		float extentsY = extents * minRange;
		float extentsX = extentsY * aspect;
		float z = center.z + minRange;
		ushort i = (ushort) data.lineVertices.Count;
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x - extentsX, center.y - extentsY, z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x + extentsX, center.y - extentsY, z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x + extentsX, center.y + extentsY, z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( center.x - extentsX, center.y + extentsY, z ) ) );
		extentsY = extents * maxRange;
		extentsX = extentsY * aspect;
		z = center.z + maxRange;
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( - extentsX, - extentsY, z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( + extentsX, - extentsY, z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( + extentsX, + extentsY, z ) ) );
		data.lineVertices.Add( _self._matrix.MultiplyPoint3x4( new Vector3( - extentsX, + extentsY, z ) ) );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineColors.Add( _self._color );
		data.lineIndices.Add( (ushort) (i+0) ); data.lineIndices.Add( (ushort) (i+1) );
		data.lineIndices.Add( (ushort) (i+1) ); data.lineIndices.Add( (ushort) (i+2) );
		data.lineIndices.Add( (ushort) (i+2) ); data.lineIndices.Add( (ushort) (i+3) );
		data.lineIndices.Add( (ushort) (i+3) ); data.lineIndices.Add( (ushort) (i+0) );
		data.lineIndices.Add( (ushort) (i+0) ); data.lineIndices.Add( (ushort) (i+1) );
		data.lineIndices.Add( (ushort) (i+1) ); data.lineIndices.Add( (ushort) (i+2) );
		data.lineIndices.Add( (ushort) (i+2) ); data.lineIndices.Add( (ushort) (i+3) );
		data.lineIndices.Add( (ushort) (i+3) ); data.lineIndices.Add( (ushort) (i+0) );
		data.lineIndices.Add( (ushort) (i+4) ); data.lineIndices.Add( (ushort) (i+5) );
		data.lineIndices.Add( (ushort) (i+5) ); data.lineIndices.Add( (ushort) (i+6) );
		data.lineIndices.Add( (ushort) (i+6) ); data.lineIndices.Add( (ushort) (i+7) );
		data.lineIndices.Add( (ushort) (i+7) ); data.lineIndices.Add( (ushort) (i+4) );
		data.lineIndices.Add( (ushort) (i+0) ); data.lineIndices.Add( (ushort) (i+4) );
		data.lineIndices.Add( (ushort) (i+1) ); data.lineIndices.Add( (ushort) (i+5) );
		data.lineIndices.Add( (ushort) (i+2) ); data.lineIndices.Add( (ushort) (i+6) );
		data.lineIndices.Add( (ushort) (i+3) ); data.lineIndices.Add( (ushort) (i+7) );
	}


	LayerData GetOrCreateLayerData()
	{
		LayerData data;
		if( _layerLookup.TryGetValue( _layer, out data ) ) return data;
		data = new LayerData();
		_layerLookup.Add( _layer, data );
		return data;
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