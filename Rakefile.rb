require 'fileutils'
require 'rubygems'

@src_dir = '.'

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

  rm_rf "./Android-SDK-master"

  sh "curl -L https://github.com/Estimote/Android-SDK/archive/master.zip > Estimote_Android-SDK.zip"
  sh "unzip -o -q Estimote_Android-SDK.zip"


  destination_jar = "#{@src_dir}/Estimotes.Binding.Droid/Jars/estimote-sdk-preview.jar"
  rm_rf destination_jar if File.exist?(destination_jar)

  FileUtils.copy_file("./Android-SDK-master/EstimoteSDK/estimote-sdk-preview.jar", destination_jar)
  # cp "./Android-SDK-master/EstimoteSDK/estimote-sdk-preview.jar", destination_jar
  rm_rf "Estimote_Android-SDK.zip"
end

