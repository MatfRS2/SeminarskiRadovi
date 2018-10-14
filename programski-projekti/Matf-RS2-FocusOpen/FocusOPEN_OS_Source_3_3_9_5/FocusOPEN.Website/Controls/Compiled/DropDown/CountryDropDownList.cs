/**********************************************************************************************************************
	FocusOPEN Digital Asset Manager (TM) 
	(c) Daydream Interactive Limited 1995-2011, All Rights Reserved

	The name and trademarks of copyright holders may NOT be used in advertising or publicity pertaining to the software
	without specific, written prior permission. Title to copyright in this software and any associated documentation
	will at all times remain with copyright holders.

	Please refer to licences/focusopen.txt or http://www.digitalassetmanager.com for licensing information about this
	software. 
**********************************************************************************************************************/

using FocusOPEN.Data;

namespace FocusOPEN.Website.Controls
{
	public class CountryDropDownList : AbstractDropDownList
	{
		#region AbstractDropDownList Implementation

		public override object GetDataSource()
		{
			return CountryCache.Instance.GetList();
		}

		public override string GetDataTextField()
		{
			return Country.Columns.Name.ToString();
		}

		public override string GetDataValueField()
		{
			return Country.Columns.CountryId.ToString();
		}

		#endregion
	}
}