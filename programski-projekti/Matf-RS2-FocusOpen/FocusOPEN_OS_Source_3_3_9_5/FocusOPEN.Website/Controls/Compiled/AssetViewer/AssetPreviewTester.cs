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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.UI;
using FocusOPEN.Business;
using FocusOPEN.Data;
using FocusOPEN.Shared;
using FocusOPEN.Website.Components;

namespace FocusOPEN.Website.Controls
{
	public class AssetPreviewTester : Control
	{
		#region Constructor

        public AssetPreviewTester()
		{
			Asset = Asset.Empty;
			UseMarginCssClass = true;
		}

		#endregion

		#region Accessors

		public Asset Asset { get; set; }

		public bool UseMarginCssClass { get; set; }

        public int PluginId { get; set; }

        public ContextType ContextType { get; set; }

		#endregion

		#region ViewState Methods

		protected override object SaveViewState()
		{
			object[] viewStates = new object[4];

			object baseViewState = base.SaveViewState();

			viewStates[0] = baseViewState;
			viewStates[1] = Asset;
            viewStates[2] = PluginId;
            viewStates[3] = ContextType;

			return (viewStates);
		}

		protected override void LoadViewState(object savedState)
		{
			if (savedState != null)
			{
				object[] state = (object[]) savedState;

				if (state[0] != null)
					base.LoadViewState(state[0]);

				if (state[1] != null)
					Asset = (Asset) state[1];

                if (state[2] != null)
                    PluginId = (int)state[2];

                if (state[3] != null)
                    ContextType = (ContextType)state[3];
			}
		}

		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.ClientScript.IsClientScriptIncludeRegistered("FlowPlayer"))
				Page.ClientScript.RegisterClientScriptInclude("FlowPlayer", ResolveUrl("~/FlowPlayer/flowplayer-3.2.4.min.js"));
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (Asset.IsNull)
			{
				// No asset

                WriteImage(writer, "~/Images/Asset/Previews/Unavailable.gif");
				return;
			}

			if (!EntitySecurityManager.CanUserViewAssetPreview(SessionInfo.Current.User, Asset))
			{
				// Access denied

                WriteImage(writer, "~/Images/Asset/Previews/Unavailable.gif");
				return;
			}

			// The asset has not been marked as processed, so show a 'processing' image placeholder
			// instead, even if the filepath is not empty.  This is because the file may exist, but
			// may still have data being written to it by another process, so we don't have any
			// guarantee that it's safe to display to the user.
			if (!Asset.IsProcessed)
			{
				WriteImage(writer, "~/Images/Asset/Previews/Processing.gif");
				return;
			}

			// Get asset File Info
			AssetPreviewInfo info = new AssetPreviewInfo(Asset);

			// If we don't have an asset preview file. Show a placeholder image instead.
			if (!info.FileExists)
			{
				WriteImage(writer, "~/Images/Asset/Previews/Unavailable.gif");
				return;
			}

			// Setup variables to be replaced
			string fileExtension = StringUtils.GetFileExtension(info.FilePath);
			string assetPreviewUrl = AssetFileUrlHelper.GetPreviewUrl(Asset.AssetId);
			string relativePreviewUrl = ResolveUrl(assetPreviewUrl);
			string absolutePreviewUrl = SiteUtils.GetWebsiteUrl(assetPreviewUrl);
			string playerId = ClientID + "_Preview";
			string rtmpStreamingServer = ConfigurationManager.AppSettings.GetValue("RTMPStreamingServer");

			// If the streaming server is enabled, then hack the file extension to get the correct markup
			if (fileExtension == "flv" && !StringUtils.IsBlank(rtmpStreamingServer))
				fileExtension += "-streaming";

            //gets the plugin to be tested
            Plugin plugin = ContextInfo.PluginManager.GetAnyPluginById(PluginId);

            if (!plugin.IsNull && plugin.HasPluginFile)
            {
                //ensure asset is of the right file type or force preview set
                if (!plugin.PluginFile.FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase) && !plugin.PluginFile.ForcePreviewFormat)
                {
                    writer.Write(string.Format("[Plugin does not support file extension: {0}]", fileExtension));
                    return;
                }
            }
            else
            {
                writer.Write(string.Format("[Invalid plugin: {0}]", plugin.Name));
                return;
            }
            
            //get the plugins relative URL
            string relativePluginUrl = ResolveUrl(plugin.RelativePath);

			// Get the HTML
            string markup = ContextInfo.PluginManager.GetMarkup(plugin, ContextType);

			// Otherwise, ensure we have markup
			if (string.IsNullOrEmpty(markup))
			{
				writer.Write(string.Format("[Unable to preview asset: {0}]", Asset.AssetId));
				return;
			}

			// Replace params
			markup = markup.Replace("[ASSET-PREVIEW-URL]", assetPreviewUrl);
			markup = markup.Replace("[RELATIVE-PREVIEW-URL]", relativePreviewUrl);
			markup = markup.Replace("[ABSOLUTE-PREVIEW-URL]", absolutePreviewUrl);
			markup = markup.Replace("[FOCUSOPEN-PREVIEW-FILENAME]", Path.GetFileName(info.FilePath));
			markup = markup.Replace("[PLAYER-ID]", playerId);
			markup = markup.Replace("[ASSET-ID]", Asset.AssetId.ToString());
			markup = markup.Replace("[ASSET-REFERENCE]", info.Asset.FileReference);
			markup = markup.Replace("[STREAMING-SERVER-URL]", rtmpStreamingServer);
            markup = markup.Replace("[USER-ID]", SessionInfo.Current.User.UserId.GetValueOrDefault().ToString());
			markup = markup.Replace("[PREVIEW-FILE-EXTENSION]",fileExtension);
            markup = markup.Replace("[ASSET-TYPE-ID]",Asset.AssetTypeId.ToString());
            markup = markup.Replace("[ASSET-TYPE-NAME]",Asset.AssetType.Name);
            markup = markup.Replace("[PREVIEW-AVAILABLE]",(info.PreviewAvailable?"true":"false"));
            markup = markup.Replace("[PLUGIN-URL]",relativePluginUrl);
            markup = markup.Replace("~/", WebUtils.GetApplicationPath());

			// Write the HTML to the page
			writer.Write(markup);
		}

		#region Helper Methods

		private void WriteImage(TextWriter writer, string url)
		{
			string cssClass = EntitySecurityManager.IsAssetRestricted(SessionInfo.Current.User, Asset) ? "ImgThbOutline0" : "ImgThbOutline1";

			if (UseMarginCssClass)
				cssClass += " DetailsThbMrg";

			string html = string.Format("<img src=\"{0}\" class=\"{1}\" />", ResolveUrl(url), cssClass);
			writer.Write(html);
		}

		#endregion
	}
}