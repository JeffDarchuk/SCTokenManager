using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace TokenManager.Handlers
{

	public abstract class BaseHttpHandler : IHttpHandler, IRouteHandler, IReadOnlySessionState
	{
		private readonly ConcurrentDictionary<string, string> _resourceCache = new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, byte[]> _imageCache = new ConcurrentDictionary<string, byte[]>();

		public virtual bool IsReusable => true;

		/// <summary>
		/// processes http request
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(new HttpContextWrapper(context));
		}

		/// <summary>
		/// sets up the context for specific content
		/// </summary>
		/// <param name="context"></param>
		/// <param name="message"></param>
		/// <param name="contentType"></param>
		/// <param name="status"></param>
		/// <param name="endResponse"></param>
		protected void ReturnResponse(HttpContextBase context, string message, string contentType = "text/plain", HttpStatusCode status = HttpStatusCode.OK, bool endResponse = false)
		{
			if (!string.IsNullOrWhiteSpace(message)) context.Response.Write(message);
			context.Response.StatusCode = (int)status;
			context.Response.ContentType = contentType;
			if (endResponse) context.Response.End();
		}

		/// <summary>
		/// return not found response
		/// </summary>
		/// <param name="context"></param>
		/// <param name="message"></param>
		protected void NotFound(HttpContextBase context, string message = null)
		{
			ReturnResponse(context, message, status: HttpStatusCode.NotFound);
		}

		/// <summary>
		/// returns an error response
		/// </summary>
		/// <param name="context"></param>
		/// <param name="e"></param>
		/// <param name="message"></param>
		protected void Error(HttpContextBase context, Exception e, string message = null)
		{
			message = (string.IsNullOrWhiteSpace(message) ? e.ToString() : message + "\r\n" + e);
			ReturnResponse(context, message, status: HttpStatusCode.InternalServerError);
		}

		/// <summary>
		/// return specific file resource stored in the binary
		/// </summary>
		/// <param name="context"></param>
		/// <param name="file"></param>
		/// <param name="contentType"></param>
		protected void ReturnResource(HttpContextBase context, string file, string contentType)
		{
			ReturnResponse(context, GetResource(file), contentType);
		}

		/// <summary>
		/// return image resource from the binary
		/// </summary>
		/// <param name="context"></param>
		/// <param name="file"></param>
		/// <param name="imageFormat"></param>
		/// <param name="contentType"></param>
		protected void ReturnImage(HttpContextBase context, string file, ImageFormat imageFormat, string contentType)
		{
			var buffer = GetImage(file, imageFormat);
			context.Response.ContentType = "image/png";
			context.Response.BinaryWrite(buffer);
			context.Response.Flush();
		}

		/// <summary>
		/// return json resource
		/// </summary>
		/// <param name="context"></param>
		/// <param name="o"></param>
		protected void ReturnJson(HttpContextBase context, object o)
		{
			var json = o == null ? string.Empty : JsonNetWrapper.SerializeObject(o);
			context.Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
			context.Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
			context.Response.AppendHeader("Expires", "0"); // Proxies.
			ReturnResponse(context, json, "application/json");
		}

		/// <summary>
		/// extracts the resource out of the binary
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public string GetResource(string filename)
		{
			filename = filename.ToLowerInvariant();
			string result;
			if (_resourceCache.TryGetValue(filename, out result)) return result;
			using (var stream = typeof(TokenManagerHandler).Assembly.GetManifestResourceStream("TokenManager.Resources." + filename))
			{
				if (stream != null)
				{
					using (var reader = new StreamReader(stream))
					{
						result = reader.ReadToEnd();
					}
				}
			}

			_resourceCache[filename] = result;
			return result;
		}

		/// <summary>
		/// returns image resource
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="imageFormat"></param>
		/// <returns></returns>
		private byte[] GetImage(string filename, ImageFormat imageFormat)
		{
			filename = filename.ToLowerInvariant();
			byte[] result;
			if (_imageCache.TryGetValue(filename, out result)) return result;
			using (var stream = typeof(TokenManagerHandler).Assembly.GetManifestResourceStream("TokenManager.Resources." + filename))
			{
				if (stream != null)
				{
					using (var ms = new MemoryStream())
					{
						var bmp = new Bitmap(stream);
						bmp.Save(ms, imageFormat);
						result = ms.ToArray();
					}
				}
			}

			_imageCache[filename] = result;
			return result;
		}

		/// <summary>
		/// process the request
		/// </summary>
		/// <param name="context"></param>
		public abstract void ProcessRequest(HttpContextBase context);

		public IHttpHandler GetHttpHandler(RequestContext requestContext) { return this; }
	}
}
