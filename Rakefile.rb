require 'fileutils'
require 'rubygems'

@src_dir = '/Users/tom/work/xamarin/code/Estimotes-Xamarin/src'

task :default => [:compile_binding]

desc "Removes build artifacts"
task :clean do
  directories_to_delete = [
      "Android-SDK-master",
      "#{@src_dir}/Estimotes.Droid/bin",
      "#{@src_dir}/Estimotes.Droid/obj",
      "#{@src_dir}/Estimotes.Binding.Droid/bin",
      "#{@src_dir}/Estimotes.Binding.Droid/obj",
  ]

  directories_to_delete.each { |x|
    rm_rf x
  }
end

desc "Compiles the project."
task :compile_binding => [:clean] do
  `xbuild #{@src_dir}/Estimotes.Binding.Droid/Xamarin.Estimotes.Android.csproj /p:Configuration=Release`
end

desc "Will download the latest JAR from Github."
task :update_jars do
  sh "curl -L https://github.com/Estimote/Android-SDK/archive/master.zip > Estimote_Android-SDK.zip"
  sh "unzip -o -q Estimote_Android-SDK.zip"
  rm_rf "Estimote_Android-SDK.zip"
  rm_rf "#{@src_dir}/Estimotes.Binding.Droid/Jars/estimote-sdk-preview.jar"
  cp "Android-SDK-master/EstimoteSDK/estimote-sdk-preview.jar", "#{@src_dir}/Estimotes.Binding.Droid/Jars/estimote-sdk-preview.jar"
end

