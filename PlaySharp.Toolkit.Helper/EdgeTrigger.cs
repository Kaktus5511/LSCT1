using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;

namespace PlaySharp.Toolkit.Helper
{
	[SecuritySafeCritical]
	public class EdgeTrigger
	{
		private bool @value;

		public bool Value
		{
			[SecuritySafeCritical]
			get
			{
				return this.@value;
			}
			[SecuritySafeCritical]
			set
			{
				if (this.@value == value)
				{
					return;
				}
				if (value)
				{
					EventHandler eventHandler = this.Rising;
					if (eventHandler != null)
					{
						eventHandler(this, EventArgs.Empty);
					}
					else
					{
					}
					this.@value = true;
					EventHandler eventHandler1 = this.Risen;
					if (eventHandler1 != null)
					{
						eventHandler1(this, EventArgs.Empty);
					}
					else
					{
					}
				}
				if (!value)
				{
					EventHandler eventHandler2 = this.Falling;
					if (eventHandler2 != null)
					{
						eventHandler2(this, EventArgs.Empty);
					}
					else
					{
					}
					this.@value = false;
					EventHandler eventHandler3 = this.Fallen;
					if (eventHandler3 == null)
					{
						return;
					}
					eventHandler3(this, EventArgs.Empty);
				}
			}
		}

		public EdgeTrigger()
		{
		}

		public event EventHandler Fallen;

		public event EventHandler Falling;

		public event EventHandler Risen;

		public event EventHandler Rising;
	}
}