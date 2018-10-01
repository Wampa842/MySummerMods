/*
Copyright (C) 2018 Wampa842

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using MSCLoader;

namespace TwentyFourClock
{
	public class ClockCommand : ConsoleCommand
	{
		public override string Name => "digiclock";
		public override string Help => "Digital clock debug command - type 'digiclock help' for details";

		private TwentyFourClock _mod;
		private DigitalClockBehaviour _clock;
		private readonly string _usage = "Usage: digiclock <command>\nwhere <command> is:\nhelp: display this messag\nalarm set <hour [minute]>: set the alarm to the specified time. If only one number is provided, the minute will default to zero.\nalarm unset: mute and disable the alarm.\nalarm show: show the current status of the alarm.\nshow time: print the time (hh:mm:ss).\nshow angle: print the rotations of the Suomi clock's hands in degrees.";

		public ClockCommand(TwentyFourClock mod, DigitalClockBehaviour clock)
		{
			this._mod = mod;
			this._clock = clock;
		}

		public override void Run(string[] args)
		{
			if (args.Length <= 0)
				return;
			switch (args[0].ToLowerInvariant())
			{
				case "help":
					ModConsole.Print(_usage);
					return;
				case "alarm":
					if(args.Length >= 2)
					{
						switch (args[1].ToLowerInvariant())
						{
							case "show":
								ModConsole.Print(string.Format("Alarm is {0} at {1:0}:{2:00}", _clock.AlarmEnabled ? "enabled" : "disabled", _clock.AlarmHour, _clock.AlarmMinute));
								return;
							case "set":
								if (args.Length >= 3 && int.TryParse(args[2], out int h))
								{
									h %= 24;
									int m = 0;
									if (args.Length >= 4)
										int.TryParse(args[3], out m);
									m %= 60;
									_clock.Arm(h, m);
									ModConsole.Print(string.Format("Alarm set to {0:0}:{1:00}", h, m));
								}
								else
									ModConsole.Print(_usage);
								return;
							case "unset":
								_clock.Disarm();
								ModConsole.Print("Alarm disabled");
								return;
							default:
								break;
						}
					}
					return;
				case "show":
					if(args.Length >= 2)
					{
						if(args[1].ToLowerInvariant() == "time")
							ModConsole.Print(string.Format("Current time is {0:0}:{1:00}:{2:00}", _clock.Clock.Hour24, _clock.Clock.Minute, _clock.Clock.Second));
						else if (args[1].ToLowerInvariant() == "angle")
							ModConsole.Print(string.Format("The angles of the clock hands are:\nHour: {0:000}°\nMinute: {1:000}°", _clock.Clock.AngleHour, _clock.Clock.AngleMinute));
					}
					return;
				default:
					return;
			}
		}
	}
}