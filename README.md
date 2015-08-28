# Estimotes.Xamarin.Android

This is a Java Library binding for the [Estimotes SDK for Android](https://github.com/Estimote/Android-SDK).

Many thanks to James Montemagno for updating this to 0.8.7 of the Android SDK for Estimotes.

There are three directories of interest:

* `component` - Holds the necessary files to build the Xamarin component.
* `samples` - Holds the sample project for the Estimote binding.
* `src` - Holds the binding project for the binding.

The Xamarin Component is a shell component that depends on the existence of the NuGet package.

## Installation

There are two ways to add this binding to your Xamarin.Android project. The first way and preferred way is to [use the NuGet package](https://www.nuget.org/packages/Estimotes-Xamarin/) :

    PM> Install-Package Estimotes-Xamarin

The other way is to use the Xamarin Component store.


# Building

A `cake.build` file is provided and can be called with the following targets:

On Mac:
```bash
sh build.sh              # Builds everything
sh build.sh -t=libs      # Builds the bindings library
sh build.sh -t=clean     # Removes all build artifacts, nupkg's and .xam's
sh build.sh -t=nuget     # Builds a .nupkg
sh build.sh -t=component # Builds a .xam component file
sh build.sh -t=samples   # Builds the sample project(s)
sh build.sh -t=expunge   # Cleans out the component cache for this component
```

On Windows:
```
.\build.ps1                     # Builds everything
.\build.ps1 -Target "libs"      # Builds the bindings library
.\build.ps1 -Target "clean"     # Removes all build artifacts, nupkg's and .xam's
.\build.ps1 -Target "nuget"     # Builds a .nupkg
.\build.ps1 -Target "component" # Builds a .xam component file
.\build.ps1 -Target "samples"   # Builds the sample project(s)
.\build.ps1 -Target "expunge"   # Cleans out the component cache for this component
```

When a new version of the Estimote Android SDK is released, you can update the `build.cake` file's version to match the new version available on Maven.  Running `cake t=all` will clean and redownload the newest sdk version.

