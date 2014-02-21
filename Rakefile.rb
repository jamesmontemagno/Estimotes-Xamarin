require 'fileutils'
require 'rubygems'

@root_dir = '/Users/tom/work/xamarin/code/Estimotes-Xamarin'

task :default => [:clean, :build_binding]

desc "Removes build artifacts"
task :clean do
  directories_to_delete = [
      "#{@root_dir}/Estimotes.Droid/bin",
      "#{@root_dir}/Estimotes.Droid/obj",
      "#{@root_dir}/Estimotes.Binding.Droid/bin",
      "#{@root_dir}/Estimotes.Binding.Droid/obj",
  ]

  directories_to_delete.each { |x|
    rm_rf x
  }
end

desc "Compiles the project."
task :build_binding do 
  `xbuild ./Estimotes.Binding.Droid/Estimotes.Droid.Binding.csproj /p:Configuration=Release`
end





