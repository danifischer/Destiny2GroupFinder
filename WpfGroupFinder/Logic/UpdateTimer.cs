using System;
using System.Timers;

namespace WpfGroupFinder.Logic
{
	public class UpdateTimer : IUpdateTimer
	{
		private Timer _timer;

		public Action TimeoutAction { get; set; }

		public void StartTimer(int time)
		{
			_timer = new Timer(time);
			_timer.Elapsed += OnTimedEvent;
			_timer.AutoReset = true;
			_timer.Enabled = true;
		}

		public void StopTimer()
		{
			if (_timer == null) return;
			_timer.Stop();
			_timer.Dispose();
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			TimeoutAction();
		}
	}
}