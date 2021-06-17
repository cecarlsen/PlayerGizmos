/*
	Copyright © Carl Emil Carlsen 2021
	http://cec.dk
*/

using System;
using UnityEngine;

namespace UnityExtensions
{
	[Serializable]
	public class Layer
	{
		[SerializeField] int _index = 0;

		public int index {
			get { return _index; }
			set {
				if( value < 0 || value > 32 ) return;
				_index = value;
			}
		}

		public int mask {
			get { return 1 << _index; }
		}

		public Layer( int index = 0 )
		{
			_index = index;
		}
	}
}