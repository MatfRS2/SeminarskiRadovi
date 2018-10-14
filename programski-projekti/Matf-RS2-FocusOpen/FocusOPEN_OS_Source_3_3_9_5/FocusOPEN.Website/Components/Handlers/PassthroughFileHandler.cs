/**********************************************************************************************************************
	FocusOPEN Digital Asset Manager (TM) 
	(c) Daydream Interactive Limited 1995-2011, All Rights Reserved

	The name and trademarks of copyright holders may NOT be used in advertising or publicity pertaining to the software
	without specific, written prior permission. Title to copyright in this software and any associated documentation
	will at all times remain with copyright holders.

	Please refer to licences/focusopen.txt or http://www.digitalassetmanager.com for licensing information about this
	software. 
**********************************************************************************************************************/

using System.IO;
using System.Web;

namespace FocusOPEN.Website.Components.Handlers
{
	public class PassthroughFileHandler : BaseFileHandler
	{
		public override void ProcessRequest()
		{
			string file = Context.Request.AppRelativeCurrentExecutionFilePath;
			string path = Context.Server.MapPath(file);

			if (!File.Exists(path))
				throw new HttpException(404, "File not found");

			WriteFileToResponseStream(path);
		}
	}
}