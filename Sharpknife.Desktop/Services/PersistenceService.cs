﻿using Sharpknife.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Sharpknife.Desktop.Services
{
	/// <summary>
	/// Represents a service to manage persistence and serialization of objects.
	/// </summary>
	public class PersistenceService
	{
		private PersistenceService()
		{
			this.instances = new Dictionary<string, object>();

			this.Hook();
		}

		/// <summary>
		/// Gets an object from persistence with the specified type and name.
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

			if (!this.instances.ContainsKey(name))
			{
				this.instances[name] = this.Load<T>(name);
			}

			var instance = this.instances[name];
			var result = instance as T;

			if (result == null)
			{
				throw new InvalidOperationException("Invalid result.");
			}

			return result;
		}

		/// <summary>
		/// Syncs all currently loaded persistence objects by writing them to file.
		/// </summary>
		public void Sync()
		{
			foreach (var entry in this.instances)
			{
				this.Save(entry.Key, entry.Value);
			}
		}

		private T Load<T>(string name) where T : class, new()
		{
			var path = this.GetPath(name);
			var result = default(T);

			if (File.Exists(path))
			{
				var serializer = new XmlSerializer(typeof(T));

				using (var stream = File.Open(path, FileMode.Open))
				{
					result = serializer.Deserialize(stream) as T;
				}
			}

			if (result == null)
			{
				result = new T();
			}

			return result;
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
			{
				serializer.Serialize(stream, instance);
			}
		}

		private string GetPath(string name)
		{
			return Path.Combine(Assemblies.GetApplicationPath(), $"{name}.xml");
		}

		private void TrySync()
		{
			try
			{
				this.Sync();
			}
			catch
			{
				// Failed to save one or more files.
				// Perhaps throw the exception here?
			}
		}

		private void Hook()
		{
			Application.Current.Exit += (sender, eventArgs) => this.TrySync();
		}

		/// <summary>
		/// Gets the instance of the persistence service.
		/// </summary>
		public static PersistenceService Instance
		{
			get
			{
				return PersistenceService.instance;
			}
		}

		private static readonly PersistenceService instance = new PersistenceService();

		private Dictionary<string, object> instances;
	}
}