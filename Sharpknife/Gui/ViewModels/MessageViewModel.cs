﻿using Sharpknife.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sharpknife.Gui.ViewModels
{
	internal class MessageViewModel : Observable
	{
		public MessageViewModel(string title, string message)
		{
			this.title = title;
			this.message = message;
		}

		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.SetNotify(ref this.title, value);
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.SetNotify(ref this.message, value);
			}
		}

		public Command CloseCommand
		{
			get
			{
				return new Command(() => WindowCenter.Instance.CloseCurrentWindow());
			}
		}

		private string title;
		private string message;
	}
}