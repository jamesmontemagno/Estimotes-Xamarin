
var ESTIMOTES_VERSION = "0.8.7";

var ESTIMOTES_SDK_URL = "http://search.maven.org/remotecontent?filepath=com/estimote/sdk/" + ESTIMOTES_VERSION + "/sdk-" + ESTIMOTES_VERSION + ".aar";
var ESTIMOTES_DOC_URL = "http://search.maven.org/remotecontent?filepath=com/estimote/sdk/" + ESTIMOTES_VERSION + "/sdk-" + ESTIMOTES_VERSION + "-javadoc.jar";

var target = Argument ("target", "Default");

Task ("externals")
	.WithCriteria (!DirectoryExists ("./externals/javadocs/") && !FileExists ("./externals/estimotes.aar"))
	.Does (() => 
{
	CreateDirectory ("./externals/");

	DownloadFile (ESTIMOTES_SDK_URL, "./externals/estimotes.aar");
	DownloadFile (ESTIMOTES_DOC_URL, "./externals/javadocs.jar");

	Unzip ("./externals/javadocs.jar", "./externals/javadocs/");
});

Task ("libs").IsDependentOn ("externals").Does (() => 
{
	NuGetRestore ("./src/Estimotes.Binding.Droid/Estimotes-Binding.sln");
	DotNetBuild ("./src/Estimotes.Binding.Droid/Estimotes-Binding.sln", c => c.Configuration = "Release");

	CreateDirectory ("./output/");
	CopyFiles ("./src/Estimotes.Binding.Droid/**/*.dll", "./output/");
});

Task ("samples").IsDependentOn ("libs").Does (() => 
{
	NuGetRestore ("./samples/Estimotes.Droid/Estimotes-Example.sln");
	DotNetBuild ("./samples/Estimotes.Droid/Estimotes-Example.sln");
});

Task ("nuget").IsDependentOn ("libs").Does (() => 
{
	// NuGet messes up path on mac, so let's add ./ in front twice
	var basePath = IsRunningOnUnix () ? "././nuget" : "./nuget";

	NuGetPack ("./nuget/Estimotes-Xamarin.nuspec", new NuGetPackSettings { 
		Verbosity = NuGetVerbosity.Detailed,
		OutputDirectory = "./output",		
		BasePath = basePath,
	});				
});

Task ("component").IsDependentOn ("nuget").Does (() => 
{
	if (!FileExists ("./externals/xamarin-component.exe")) {
		DownloadFile ("https://components.xamarin.com/submit/xpkg", "./externals/xpkg.zip");
		Unzip ("./externals/xpkg.zip", "./externals/");
	}

	StartProcess (MakeAbsolute (new FilePath ("./externals/xamarin-component.exe")), new ProcessSettings {
		Arguments = "package ./component/"
	});

	MoveFiles ("./component/*.xam", "./output/");
});

Task ("clean").Does (() => 
{
	if (DirectoryExists ("./externals"))
		DeleteDirectory ("./externals", true);
	if (DirectoryExists ("./output"))
		DeleteDirectory ("./output", true);

	CleanDirectories ("./**/bin");
	CleanDirectories ("./**/obj");	
});

Task ("expunge").Does (() => {
	CleanDirectories ("~/Library/Caches/Xamarin/ComponentInfo/estimotesdk*.*");
	CleanDirectories ("~/Library/Caches/Xamarin/Components/estimotesdk*.xam");
});

Task ("Default").IsDependentOn ("clean").IsDependentOn ("component").Does (() => { });
Task ("all").IsDependentOn ("Default").Does (() => { });

RunTarget (target);
