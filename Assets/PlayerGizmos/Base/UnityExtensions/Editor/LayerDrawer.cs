/*
	Copyright © Carl Emil Carlsen 2021
	http://cec.dk
*/

using UnityEngine;
using UnityEditor;

namespace UnityExtensions
{
	[CustomPropertyDrawer( typeof( Layer ) )]
	public class LayerDrawer : PropertyDrawer
	{
		public override void OnGUI( Rect _position, SerializedProperty _property, GUIContent _label )
		{
			EditorGUI.BeginProperty( _position, GUIContent.none, _property );
			SerializedProperty layerIndex = _property.FindPropertyRelative( "_index" );
			_position = EditorGUI.PrefixLabel( _position, GUIUtility.GetControlID( FocusType.Passive ), _label );
			if( layerIndex != null ) {
				layerIndex.intValue = EditorGUI.LayerField( _position, layerIndex.intValue );
			}
			EditorGUI.EndProperty();
		}
	}
}