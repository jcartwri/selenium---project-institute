using System;

namespace WindowsFormsApp1
{
	internal class StartThread
	{
		private Func<object> p;

		public StartThread(Func<object> p)
		{
			this.p = p;
		}
	}
}