﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpknife.Utilities
{
	/// <summary>
	/// A collection of common file methods.
	/// </summary>
	public static class Files
	{
		/// <summary>
		/// Returns a unique file name for the specified path, ensuring that no other file exists.
		/// </summary>
		/// <param name="path">the path</param>
		/// <returns>the result</returns>
		public static string GetUniquePath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			var directory = Path.GetDirectoryName(path);
			var name = Path.GetFileNameWithoutExtension(path);
			var extension = Path.GetExtension(path);

			var result = path;
			var count = 0;

			while (File.Exists(result) || count <= 0)
			{
				count++;
				result = Path.Combine(directory, string.Format("{0}{1}{2}", name, count, extension));
			}

			return result;
		}

		/// <summary>
		/// Returns the friendly size of the specified size.
		/// </summary>
		/// <param name="size">the size</param>
		/// <returns>the result</returns>
		public static string GetSize(long size)
		{
			var sizes = new string[] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" };
			var order = 0;

			while (size >= 1024 && order + 1 < sizes.Length)
			{
				order++;
				size /= 1024;
			}

			var suffix = sizes[order];
			var result = string.Format("{0:0} {1}", size, suffix);

			return result;
		}

		/// <summary>
		/// Returns a description of the specified extension.
		/// </summary>
		/// <param name="extension">the extension</param>
		/// <returns>the description</returns>
		public static string GetDescription(string extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			extension = extension.ToUpper();
			extension = extension.Replace(".", string.Empty);

			switch (extension)
			{
				case "PNG":
				case "JPG":
				case "JPEG":
				case "GIF":
				{
					return "Image File";
				}
				case "MP3":
				case "WAV":
				{
					return "Audio File";
				}
				case "MPG":
				case "MP4":
				case "WEBM":
				case "AVI":
				{
					return "Video File";
				}
				case "RTF":
				case "TXT":
				{
					return "Text Document";
				}
				case "CS":
				case "VB":
				case "C":
				case "CPP":
				case "D":
				case "M":
				case "H":
				case "JS":
				case "PY":
				case "LUA":
				{
					return "Code Document";
				}
				case "HTM":
				case "HTML":
				{
					return "HTML Document";
				}
				case "XML":
				{
					return "XML Document";
				}
				default:
				{
					return string.Format("{0} File", extension);
				}
			}
		}
	}
}
