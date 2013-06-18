require "rake/clean"

CLEAN.include "*.xam"
CLEAN.include "xpkg"
CLEAN.include "binding/*.a"
CLEAN.include "binding/*.zip"
CLEAN.include "binding/*.dll"
CLEAN.include "binding/bin"
CLEAN.include "binding/obj"

COMPONENT = "dropboxsync-1.0.8.xam"
MONOXBUILD = "/Library/Frameworks/Mono.framework/Commands/xbuild"

file "xpkg/xamarin-component.exe" do
	puts "* Downloading xamarin-component..."
	mkdir "xpkg"
	sh "curl -L https://components.xamarin.com/submit/xpkg > xpkg.zip"
	sh "unzip -o xpkg.zip -d xpkg"
	sh "rm xpkg.zip"
end

file "binding/DropBoxSync.iOS.dll" do
  sh "curl -L 'https://dl.dropboxusercontent.com/shz/vgu2o1xz6a22ook/gcbNbUrAbw/Dropbox.framework/Dropbox.framework?token_hash=AAH3iDUuocHKrHqmpyXOueLODnxucTI-3vXzKdm_KhD1pQ&top_level_offset=48&dl=1' > binding/DropBoxSync.zip"
  sh "unzip -p binding/DropBoxSync.zip 'Dropbox.framework/Dropbox' > binding/Dropbox.a"
  sh "#{MONOXBUILD} /p:Configuration=Release binding/DropBoxSync.iOS.csproj"
  sh "cp binding/bin/Release/DropBoxSync.iOS.dll binding/DropBoxSync.iOS.dll"
end

task :default => ["xpkg/xamarin-component.exe", "binding/DropBoxSync.iOS.dll"] do
	line = <<-END
	mono xpkg/xamarin-component.exe create #{COMPONENT} \
		--name="Dropbox Sync" \
		--summary="Give your app its own private Dropbox client and leave the syncing to Dropbox." \
		--publisher="Dropbox, Inc." \
		--website="https://www.dropbox.com/developers/sync" \
		--details="component/Details.md" \
		--license="component/License.md" \
		--getting-started="component/GettingStarted.md" \
		--icon="component/icons/dropboxsync_128x128.png" \
		--icon="component/icons/dropboxsync_512x512.png" \
		--library="ios":"binding/DropBoxSync.iOS.dll" \
		--sample="iOS Sample. Demonstrates Dropbox Awesomeness on iOS.":"samples/DropBoxSyncSampleMTD.sln"
		END
	puts "* Creating #{COMPONENT}..."
	puts line.strip.gsub "\t\t", "\\\n    "
	sh line, :verbose => false
	puts "* Created #{COMPONENT}"
end

task :prepare => "binding/DropBoxSync.iOS.dll" do
  puts "\n\n"
  puts "Binding project prepared, now you can open DropBoxSync.iOS.csproj and samples in Xamarin Studio"
end
