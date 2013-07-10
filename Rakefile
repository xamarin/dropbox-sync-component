require "rake/clean"

CLEAN.include "*.xam"
CLEAN.include "xpkg"
CLEAN.include "source/binding/*.a"
CLEAN.include "source/binding/*.zip"
CLEAN.include "source/binding/*.dll"
CLEAN.include "source/binding/bin"
CLEAN.include "source/binding/obj"
CLEAN.include "source/samples/DropBoxSyncSampleMTD/bin"
CLEAN.include "source/samples/DropBoxSyncSampleMTD/obj"

COMPONENT = "dropboxsync-1.9.xam"
MONOXBUILD = "/Library/Frameworks/Mono.framework/Commands/xbuild"

file "xpkg/xamarin-component.exe" do
	puts "* Downloading xamarin-component..."
	mkdir "xpkg"
	sh "curl -L https://components.xamarin.com/submit/xpkg > xpkg.zip"
	sh "unzip -o xpkg.zip -d xpkg"
	sh "rm xpkg.zip"
end

file "source/binding/DropBoxSync.iOS.dll" do
  sh "curl -L 'http://dl.dropboxusercontent.com/s/qvmbr43p94iszak/dropbox-ios-sync-sdk-2.0-b1.zip' > source/binding/dropbox-ios-sync-sdk-2.0-b1.zip"
  sh "unzip -p source/binding/dropbox-ios-sync-sdk-2.0-b1.zip 'dropbox-ios-sync-sdk-2.0-b1/Dropbox.framework/Dropbox' > source/binding/Dropbox.a"
  sh "#{MONOXBUILD} /p:Configuration=Release source/binding/DropBoxSync.iOS.csproj"
  sh "cp source/binding/bin/Release/DropBoxSync.iOS.dll source/binding/DropBoxSync.iOS.dll"
end

task :default => ["xpkg/xamarin-component.exe", "source/binding/DropBoxSync.iOS.dll"] do
	line = <<-END
	mono xpkg/xamarin-component.exe package
	END
	puts "* Creating #{COMPONENT}..."
	puts line.strip.gsub "\t\t", "\\\n    "
	sh line, :verbose => false
	puts "* Created #{COMPONENT}"
end

task :prepare => "source/binding/DropBoxSync.iOS.dll" do
  puts "\n\n"
  puts "Binding project prepared, now you can open DropBoxSync.iOS.csproj and samples in Xamarin Studio"
end
