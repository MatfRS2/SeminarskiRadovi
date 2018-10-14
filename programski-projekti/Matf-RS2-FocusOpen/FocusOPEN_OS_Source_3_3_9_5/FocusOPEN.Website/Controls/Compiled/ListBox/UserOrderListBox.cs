/**********************************************************************************************************************
	FocusOPEN Digital Asset Manager (TM) 
	(c) Daydream Interactive Limited 1995-2011, All Rights Reserved

	The name and trademarks of copyright holders may NOT be used in advertising or publicity pertaining to the software
	without specific, written prior permission. Title to copyright in this software and any associated documentation
	will at all times remain with copyright holders.

	Please refer to licences/focusopen.txt or http://www.digitalassetmanager.com for licensing information about this
	software. 
**********************************************************************************************************************/

using System.Collections.Generic;
using Daydream.Data;
using FocusOPEN.Data;
using FocusOPEN.Website.Components;

namespace FocusOPEN.Website.Controls
{
	public class UserOrderListBox : AbstractListBox
	{
		#region AbstractListBox Implementation

		public override object GetDataSource()
		{
			EntityList<Order> orders = ContextInfo.UserOrderManager.Orders;
			Dictionary<int, string> data = new Dictionary<int, string>();

			foreach (Order order in orders)
			{
				string displayText = string.Format("Ref {0} Date: {1}", order.OrderId, order.OrderDate.ToString(Global.DateFormat));
				data.Add(order.OrderId.GetValueOrDefault(), displayText);
			}

			return data;
		}

		public override string GetDataTextField()
		{
			return "Value";
		}

		public override string GetDataValueField()
		{
			return "Key";
		}

		#endregion
	}
}