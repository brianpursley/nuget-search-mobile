using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.
[assembly: AssemblyTitle ("NuGet Search")]
[assembly: AssemblyDescription ("NuGet Search")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("CinLogic LLC")]
[assembly: AssemblyProduct ("NuGet Search")]
[assembly: AssemblyCopyright ("CinLogic LLC")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]
// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.
[assembly: AssemblyVersion ("1.0.0")]
// The following attributes are used to specify the signing key for the assembly, 
// if desired. See the Mono documentation for more information about signing.
//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]

#if RELEASE
[assembly: Application(Debuggable=false)]
#else
[assembly: Application(Debuggable=true)]
#endif