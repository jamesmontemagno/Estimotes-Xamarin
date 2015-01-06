require 'fileutils'
require 'rubygems'

@src_dir = Rake.application.original_dir + "/src"
@samples_dir = Rake.application.original_dir + "/samples"
@component_dir = Rake.application.original_dir + "/component"
@xbuild = '/usr/bin/xbuild'

file "xamarin-component/xamarin-component.exe" do
  xamarin_component_dir = "#{@component_dir}/xamarin-component"
  puts "* Downloading xamarin-component to #{xamarin_component_dir}..."

  rm_rf xamarin_component_dir if File.exists?(xamarin_component_dir)

	mkdir "#{xamarin_component_dir}"
	sh "curl -L https://components.xamarin.com/submit/xpkg > #{xamarin_component_dir}/xamarin-component.zip"
	sh "unzip -o -q #{xamarin_component_dir}/xamarin-component.zip -d #{xamarin_component_dir}"
	rm_rf "#{xamarin_component_dir}/xamarin-component.zip"
end

task :default => [:compile_binding]

desc "Removes build artifacts"
task :clean do
  directories_to_delete = [
      "#{@src_dir}/Estimotes.Binding.Droid/bin",
      "#{@src_dir}/Estimotes.Binding.Droid/obj",
      "#{@samples_dir}/Estimotes.Droid/bin",
      "#{@samples_dir}/Estimotes.Droid/obj",
      "#{@component_dir}/xamarin-component",
			"#{@component_dir}/samples/Estimotes.Droid/bin",
			"#{@component_dir}/samples/Estimotes.Droid/obj"
  ]

  directories_to_delete.each { |x|
    rm_rf x
  }
end

desc "Build the .xam component file."
task :build_component => ["xamarin-component/xamarin-component.exe"] do
  system("mono #{@component_dir}/xamarin-component/xamarin-component.exe package #{@component_dir}")
end

desc "Compiles the project and updates the appropriate locations with the new .DLL"
task :compile_binding => [:clean] do

  Dir.chdir(@src_dir) do
    system("#{@xbuild} /verbosity:diagnostic /p:Configuration=Release #{@src_dir}/Estimotes.Binding.Droid/Xamarin.Estimotes.Android.csproj")
  end

  # Update the DLL for the sample & the component.
	binding_dll = "#{@src_dir}/Estimotes.Binding.Droid/bin/Release/Estimotes.Xamarin.Android.dll"
	cp binding_dll, "#{@samples_dir}/Estimotes.Droid/lib/Estimotes.Xamarin.Android.dll"
	cp binding_dll, "#{@component_dir}/bin/Estimotes.Xamarin.Android.dll"
end

desc "Will download the latest JAR from Github."
task :update_jars do
  Dir.chdir(@src_dir) do
    rm_rf "./Android-SDK-master" if Dir.exist?("./Android-SDK-master")

    sh "curl -L https://github.com/Estimote/Android-SDK/archive/master.zip > Estimote_Android-SDK.zip"
    sh "unzip -o -q Estimote_Android-SDK.zip"

    destination_jar = "#{@src_dir}/Estimotes.Binding.Droid/Jars/estimote-sdk-preview.jar"
    rm_rf destination_jar if File.exist?(destination_jar)

    FileUtils.copy_file("./Android-SDK-master/EstimoteSDK/estimote-sdk-preview.jar", destination_jar)
    rm_rf "Estimote_Android-SDK.zip"
  end
end

desc "Compiles the sample project."
task :compile_sample do
  Dir.chdir(@samples_dir) do
    system("#{@xbuild} /verbosity:diagnostic /t:Package /p:Configuration=Debug /p:AndroidUseSharedRuntime=false /p:EmbedAssembliesIntoApk=true ./Estimotes.Droid/Estimotes.Droid.csproj")
  end
end
