# Estimotes.Xamarin.Android

This is a Java Library binding for the [Estimotes SDK for Android](https://github.com/Estimote/Android-SDK).

Many thanks to James Montemagno for updating this to 0.4.3 of the Android SDK for Estimotes.

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

A Rakefile is provided to assist with some aspects of creating the component.  Run `rake -T` to see a list of options:

    $ rake -T
    rake build_component  # Build the .xam component file
    rake clean            # Removes build artifacts
    rake compile_binding  # Compiles the project and updates the appropriate locations with the new .DLL
    rake compile_sample   # Compiles the sample project
    rake update_jars      # Will download the latest JAR from Github


When a new version of the Estimote Android SDK is release, the proper order of activity would be:

1. Update the JAR file with `rake update_jars`
2. Recompile the binding with `rake compile_binding`.
3. Open the sample project, and run it to make sure that the sample application still works.
4. Edit `src/Estimotes-Xamarin.nuspec`, and update the version and release notes element.
4. Create the NuGet package: `nuget pack ./src/Estimotes-Xamarin.nuspec`
5. Upload the NuGet package to Nuget.Org.
6. Edit the `component/component.yaml` and update the version and Android package.
7. Build the component using `rake build_component`.
8. If necessary, delete old versions of the component from your computer using `sh expunge_component.sh`.
9. Install this new component using `mono component/xamarin-component/xamarin-component.exe <NAME_OF_XAM>`.
10. Test out the component by creating a new Xamarin.Android project and adding the component to it.
11. Open the sample project that is included with the component and make sure that it works.
12. Submit the component to the Xamarin component store.

**Note:** There is a bash script `expunge_component.sh`, which will delete the component from your local OS X computer.

# What About iOS?

Greg Shackles has a [binding for Xamarin.iOS](https://github.com/gshackles/Estimote-iOS-Xamarin).