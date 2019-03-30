using System;

namespace WpfGroupFinder.Logic
{
	public interface IUpdateTimer
	{
		Action TimeoutAction { get; set; }

		void StartTimer(int time);
		void StopTimer();
	}
}