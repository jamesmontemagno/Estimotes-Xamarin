require 'fileutils'
require 'rubygems'

@src_dir = '/Users/tom/work/xamarin/code/Estimotes-Xamarin/src'
@component_dir = '/Users/tom/work/xamarin/code/Estimotes-Xamarin/component'

task :default => [:clean, :build_binding]

desc "Removes build artifacts"
task :clean do
  directories_to_delete = [
      "Android-SDK-master",
      "#{@src_dir}/Estimotes.Droid/bin",
      "#{@src_dir}/Estimotes.Droid/obj",
      "#{@src_dir}/Estimotes.Binding.Droid/bin",
      "#{@src_dir}/Estimotes.Binding.Droid/obj",
      "#{@component_dir}/bin/EstimotesBinding.dll"
  ]

  directories_to_delete.each { |x|
    rm_rf x
  }
end

desc "Compiles the project."
task :build_binding do 
  `xbuild #{@src_dir}/Estimotes.Binding.Droid/Estimotes.Droid.Binding.csproj /p:Configuration=Release`
  cp "#{@src_dir}/Estimotes.Binding.Droid/bin/Release/EstimotesBinding.dll", "#{@component_dir}/bin/EstimotesBinding.dll"
end

task :update_jars do
  sh "curl -L https://github.com/Estimote/Android-SDK/archive/master.zip > Estimote_Android-SDK.zip"
  sh "unzip -o -q Estimote_Android-SDK.zip"
  rm_rf "Estimote_Android-SDK.zip"
  rm_rf "#{@src_dir}/Estimotes.Binding.Droid/Jars/estimote-sdk-preview.jar"
  cp "Android-SDK-master/EstimoteSDK/estimote-sdk-preview.jar", "#{@src_dir}/Estimotes.Binding.Droid/Jars/estimote-sdk-preview.jar"
end 