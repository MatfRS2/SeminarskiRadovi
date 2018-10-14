/**********************************************************************************************************************
	FocusOPEN Digital Asset Manager (TM) 
	(c) Daydream Interactive Limited 1995-2011, All Rights Reserved

	The name and trademarks of copyright holders may NOT be used in advertising or publicity pertaining to the software
	without specific, written prior permission. Title to copyright in this software and any associated documentation
	will at all times remain with copyright holders.

	Please refer to licences/focusopen.txt or http://www.digitalassetmanager.com for licensing information about this
	software. 
**********************************************************************************************************************/

using System;

namespace FocusOPEN.Data
{
	[Serializable]
	public class NullAssetLink : AssetLink
	{
		#region Singleton implementation

		private NullAssetLink()
		{
		}

		private static readonly NullAssetLink m_instance = new NullAssetLink();

		public static NullAssetLink Instance
		{
			get { return m_instance; }
		}

		#endregion

		#region Accessors
		
		/// <summary>
		/// Returns the AssetId of the AssetLink object.
		/// </summary>
		public override int AssetId
		{
			get { return 0; }
			set { ; }
		}

		/// <summary>
		/// Returns the LinkedAssetId of the AssetLink object.
		/// </summary>
		public override int LinkedAssetId
		{
			get { return 0; }
			set { ; }
		}
		#endregion
	
		public override bool IsNull
		{
			get { return true; }
		}
	}
}

