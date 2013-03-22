require "rake/clean"

CLEAN.include "*.xam"
CLEAN.include "xpkg"
CLEAN.include "binding/*.a"
CLEAN.include "binding/*.zip"
CLEAN.include "binding/*.dll"
CLEAN.include "binding/bin"
CLEAN.include "binding/obj"

COMPONENT = "dropboxsync-1.0.5.xam"
MONOXBUILD = "/Library/Frameworks/Mono.framework/Commands/xbuild"

file "xpkg/xpkg.exe" do
	puts "* Downloading xpkg..."
	mkdir "xpkg"
	sh "curl -L https://components.xamarin.com/submit/xpkg > xpkg.zip"
	sh "unzip -o xpkg.zip -d xpkg"
	sh "rm xpkg.zip"
end

file "binding/DropBoxSync.iOS.dll" do
  sh "curl -L 'https://dl.dropbox.com/shz/wiltmm1qyos5mqx/WydqpgZj3e/Dropbox.framework/Dropbox.framework?token_hash=AAEabVT0u2EiA5BRnen6AMKk34y8lU1NYkSoMekHB3TOpg&top_level_offset=48&dl=1' > binding/DropBoxSync.zip"
  sh "unzip -p binding/DropBoxSync.zip 'Dropbox.framework/Dropbox' > binding/Dropbox.a"
  sh "#{MONOXBUILD} /p:Configuration=Release binding/DropBoxSync.iOS.csproj"
  sh "cp binding/bin/Release/DropBoxSync.iOS.dll binding/DropBoxSync.iOS.dll"
end

task :default => ["xpkg/xpkg.exe", "binding/DropBoxSync.iOS.dll"] do
	line = <<-END
	mono xpkg/xpkg.exe create #{COMPONENT} \
		--name="Dropbox Sync" \
		--summary="Give your apps their own private Dropbox client." \
		--publisher="Dropbox, Inc." \
		--website="https://www.dropbox.com/developers/sync" \
		--details="Details.md" \
		--license="License.md" \
		--getting-started="GettingStarted.md" \
		--icon="icons/dropboxsync_128x128.png" \
		--icon="icons/dropboxsync_512x512.png" \
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
