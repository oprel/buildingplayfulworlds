//***************************************************
//
//  Author: Ben Hopkins
//  Copyright (C) 2016 kode80 LLC, 
//  all rights reserved
// 
//  Free to use for non-commercial purposes, 
//  see full license in project root:
//  PixelRenderNonCommercialLicense.html
//  
//  Commercial licenses available for purchase from:
//  http://kode80.com/
//
//***************************************************

using UnityEngine;
using System.Collections;

namespace kode80.PixelRender
{
	[ExecuteInEditMode]
	public class IrisWipeEffect : MonoBehaviour 
	{
		public Vector2 center;
		[Range( -0.5f, 1.0f)]
		public float position = 0.5f;
		public float animationSpeed;
		private Material _material;
		public float target;

		private float animTime =50f;
		private float currentAnimTime;

		void Start()
		{	
			target=1;
			if( Application.isPlaying)
			{
				position = -.1f;
			}
		}

	
		void Update()
		{
			if( target-position>.1f)
			{
				currentAnimTime+=Time.deltaTime;
				float t = currentAnimTime / animTime;
				t = Mathf.Sin(t * Mathf.PI * 0.5f);
				position = Mathf.Lerp(position,target,t);
			}else{
				currentAnimTime=0;
			}
		}


		void OnRenderImage( RenderTexture source, RenderTexture destination)
		{
			if( _material == null)
			{
				_material = new Material( Shader.Find( "Hidden/kode80/PixelRender/IrisWipe"));
				_material.hideFlags = HideFlags.HideAndDontSave;
			}

			float w = destination.width;
			float h = destination.height;
			Vector2 aspect = w > h ? new Vector2( 1.0f, h / w) : new Vector2( w / h, 1.0f);
			Vector2 nCenter = Vector2.Scale( center, aspect);
			float m = Mathf.Max( Mathf.Max( nCenter.x, aspect.x - nCenter.x),
								 Mathf.Max( nCenter.y, aspect.y - nCenter.y));
			float maxRadius = Mathf.Sqrt( m * m + m * m);

			_material.SetVector( "_Aspect", aspect);
			_material.SetVector( "_Center", nCenter);
			_material.SetFloat( "_Position", position);
			_material.SetFloat( "_MaxRadius", maxRadius);

			Graphics.Blit( source, destination, _material);
		}
	}
}
