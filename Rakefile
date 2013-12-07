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

COMPONENT = "dropboxsync-1.0.8.xam"
MONOXBUILD = "/Library/Frameworks/Mono.framework/Commands/xbuild"

file "xpkg/xamarin-component.exe" do
	puts "* Downloading xamarin-component..."
	mkdir "xpkg"
	sh "curl -L https://components.xamarin.com/submit/xpkg > xpkg.zip"
	sh "unzip -o -q xpkg.zip -d xpkg"
	sh "rm xpkg.zip"
end

file "source/binding/DropBoxSync.iOS.dll" do
  sh "curl -L 'https://dl.dropboxusercontent.com/shz/vgu2o1xz6a22ook/gcbNbUrAbw/Dropbox.framework/Dropbox.framework?token_hash=AAH3iDUuocHKrHqmpyXOueLODnxucTI-3vXzKdm_KhD1pQ&top_level_offset=48&dl=1' > source/binding/DropBoxSync.zip"
  sh "unzip -p source/binding/DropBoxSync.zip 'Dropbox.framework/Dropbox' > source/binding/Dropbox.a"
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
