using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

[assembly: AssemblyTitle("NuGet Search")]
[assembly: AssemblyDescription("NuGet Search")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("CinLogic LLC")]
[assembly: AssemblyProduct("NuGet Search")]
[assembly: AssemblyCopyright("CinLogic LLC")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0")]

#if RELEASE
[assembly: Application(Debuggable=false)]
#else
[assembly: Application(Debuggable = true)]
#endif