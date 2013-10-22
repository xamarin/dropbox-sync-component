require "rake/clean"

CLEAN.include "*.xam"
CLEAN.include "xpkg"
CLEAN.include "source/binding/iOS/*.a"
CLEAN.include "source/binding/iOS/*.zip"
CLEAN.include "source/binding/iOS/*.dll"
CLEAN.include "source/binding/iOS/bin"
CLEAN.include "source/binding/iOS/obj"
CLEAN.include "source/samples/DropBoxSyncSampleMTD/DropBoxSyncSampleMTD/bin"
CLEAN.include "source/samples/DropBoxSyncSampleMTD/DropBoxSyncSampleMTD/obj"
CLEAN.include "source/samples/MonkeyBox/MonkeyBox/bin"
CLEAN.include "source/samples/MonkeyBox/MonkeyBox/obj"
CLEAN.include "source/binding/Android/*.zip"
CLEAN.include "source/binding/Android/*.dll"
CLEAN.include "source/binding/Android/Jars/*.jar"
CLEAN.include "source/binding/Android/bin"
CLEAN.include "source/binding/Android/obj"
CLEAN.include "source/binding/Android/Jars/armeabi"
CLEAN.include "source/binding/Android/Jars/armeabi-v7a"
CLEAN.include "source/binding/Android/Jars/mips"
CLEAN.include "source/binding/Android/Jars/x86"

VERSION="2.0.2"
COMPONENT = "dropboxsync-#{VERSION}.xam"
MONOXBUILD = "/Library/Frameworks/Mono.framework/Commands/xbuild"

file "xpkg/xamarin-component.exe" do
	puts "* Downloading xamarin-component..."
	mkdir "xpkg"
	sh "curl -L https://components.xamarin.com/submit/xpkg > xpkg.zip"
	sh "unzip -o xpkg.zip -d xpkg"
	sh "rm xpkg.zip"
end

file "source/binding/iOS/DropBoxSync.iOS.dll" do
  sh "curl -L 'https://dl.dropboxusercontent.com/s/bq1764wzfhlfeqt/dropbox-ios-sync-sdk-2.0.2.zip' > source/binding/iOS/dropbox-ios-sync-sdk-#{VERSION}.zip"
  sh "unzip -p source/binding/iOS/dropbox-ios-sync-sdk-#{VERSION}.zip 'dropbox-ios-sync-sdk-#{VERSION}/Dropbox.framework/Dropbox' > source/binding/iOS/Dropbox.a"
  sh "#{MONOXBUILD} /p:Configuration=Release source/binding/iOS/DropBoxSync.iOS.csproj"
  sh "cp source/binding/iOS/bin/Release/DropBoxSync.iOS.dll source/binding/iOS/DropBoxSync.iOS.dll"
end

file "source/binding/Android/DropboxSync.Android.dll" do
  sh "curl -L 'https://dl.dropboxusercontent.com/s/zsr4dns0dk69uym/dropbox-android-sync-sdk-2.0.2.zip' > source/binding/Android/dropbox-android-sync-sdk-#{VERSION}.zip"
  sh "mkdir -p source/binding/Android/Jars/armeabi/"
  sh "mkdir -p source/binding/Android/Jars/armeabi-v7a/"
  sh "mkdir -p source/binding/Android/Jars/mips/"
  sh "mkdir -p source/binding/Android/Jars/x86/"
  sh "unzip -p source/binding/Android/dropbox-android-sync-sdk-#{VERSION}.zip 'dropbox-android-sync-sdk-#{VERSION}/libs/armeabi/libDropboxSync.so' > source/binding/Android/Jars/armeabi/libDropboxSync.so"
  sh "unzip -p source/binding/Android/dropbox-android-sync-sdk-#{VERSION}.zip 'dropbox-android-sync-sdk-#{VERSION}/libs/armeabi/libDropboxSync.so' > source/binding/Android/Jars/armeabi-v7a/libDropboxSync.so"
  sh "unzip -p source/binding/Android/dropbox-android-sync-sdk-#{VERSION}.zip 'dropbox-android-sync-sdk-#{VERSION}/libs/mips/libDropboxSync.so' > source/binding/Android/Jars/mips/libDropboxSync.so"
  sh "unzip -p source/binding/Android/dropbox-android-sync-sdk-#{VERSION}.zip 'dropbox-android-sync-sdk-#{VERSION}/libs/x86/libDropboxSync.so' > source/binding/Android/Jars/x86/libDropboxSync.so"
  sh "unzip -p source/binding/Android/dropbox-android-sync-sdk-#{VERSION}.zip 'dropbox-android-sync-sdk-#{VERSION}/libs/dropbox-sync-sdk-android.jar' > source/binding/Android/Jars/dropbox-sync-sdk-android.jar"
  sh "#{MONOXBUILD} /p:Configuration=Release source/binding/Android/DropboxSync.Android.csproj"
  sh "cp source/binding/Android/bin/Release/DropboxSync.Android.dll source/binding/Android/DropboxSync.Android.dll"
end

task :default => ["xpkg/xamarin-component.exe", "source/binding/iOS/DropBoxSync.iOS.dll", "source/binding/Android/DropboxSync.Android.dll"] do
	line = <<-END
	mono xpkg/xamarin-component.exe package
	END
	puts "* Creating #{COMPONENT}..."
	puts line.strip.gsub "\t\t", "\\\n    "
	sh line, :verbose => false
	puts "* Created #{COMPONENT}"
end

task :prepare => ["source/binding/iOS/DropBoxSync.iOS.dll", "source/binding/Android/DropboxSync.Android.dll"] do
  puts "\n\n"
  puts "Binding projects prepared, now you can open projects without any missing files errors"
end
