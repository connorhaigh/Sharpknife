﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Sharpknife.Utilities;

namespace Sharpknife.Desktop.Services
{
	/// <summary>
	/// Represents a service to manage persistence and serialization of objects.
	/// </summary>
	public sealed class PersistenceService
	{
		private PersistenceService()
		{
			this.instances = new Dictionary<string, object>();

			this.Hook();
		}

		/// <summary>
		/// Returns an object from persistence with the specified type and name.
		/// </summary>
		/// <typeparam name="T">the type</typeparam>
		/// <param name="name">the name</param>
		/// <returns>the object</returns>
		public T Get<T>(string name) where T : class, new()
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (this.IsDesigner())
			{
				// Return a new object every time in designer mode.

				return new T();
			}

			if (!this.instances.ContainsKey(name))
			{
				this.instances[name] = this.Load<T>(name);
			}

			var result = this.instances[name] as T;

			if (result == null)
			{
				throw new InvalidOperationException("Failed to cast instance.");
			}

			return result;
		}

		/// <summary>
		/// Returns an object from persistence with the specified type, using its name.
		/// </summary>
		/// <typeparam name="T">the type</typeparam>
		/// <returns>the object</returns>
		public T Get<T>() where T : class, new()
		{
			return this.Get<T>(typeof(T).Name);
		}

		/// <summary>
		/// Syncs all currently loaded persistence to file.
		/// </summary>
		public void Sync()
		{
			if (this.IsDesigner())
			{
				// Don't save in designer mode.

				return;
			}

			foreach (var entry in this.instances)
			{
				this.Save(entry.Key, entry.Value);
			}
		}

		private T Load<T>(string name) where T : class, new()
		{
			var path = this.GetPath(name);

			if (File.Exists(path))
			{
				var serializer = new XmlSerializer(typeof(T));

				using (var stream = File.Open(path, FileMode.Open))
				{
					var result = serializer.Deserialize(stream) as T;

					if (result == null)
					{
						throw new InvalidOperationException("Failed to load instance.");
					}

					return result;
				}
			}
			else
			{
				return new T();
			}
		}

		private void Save(string name, object instance)
		{
			var path = this.GetPath(name);
			var directory = Path.GetDirectoryName(path);

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var serializer = new XmlSerializer(instance.GetType());

			using (var stream = File.Open(path, FileMode.Create))
			using (var writer = XmlWriter.Create(stream, PersistenceService.settings))
			{
				serializer.Serialize(writer, instance);
			}
		}

		private string GetPath(string name)
		{
			return Path.Combine(Assemblies.GetApplicationPath(), $"{name}.xml");
		}

		private bool IsDesigner()
		{
			using (var process = Process.GetCurrentProcess())
			{
				var name = process.ProcessName.ToLower();

				if (name == "devenv" || name == "xdesproc")
				{
					return true;
				}
			}

			return false;
		}

		private void Hook()
		{
			Application.Current.Exit += (sender, args) =>
			{
				try
				{
					this.Sync();
				}
				catch
				{

				}
			};
		}

		/// <summary>
		/// Gets the instance of the persistence service.
		/// </summary>
		public static PersistenceService Instance => PersistenceService.instance;

		private static readonly PersistenceService instance = new PersistenceService();

		private static readonly XmlWriterSettings settings = new XmlWriterSettings()
		{
			Indent = true,
			IndentChars = "\t",
			OmitXmlDeclaration = true
		};

		private Dictionary<string, object> instances;
	}
}
