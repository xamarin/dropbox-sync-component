using System;
using MonoTouch.ObjCRuntime;

[assembly: LinkWith ("Dropbox.a", LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator, ForceLoad = true, LinkerFlags = "-lc++", Frameworks = "CFNetwork Security SystemConfiguration QuartzCore")]
