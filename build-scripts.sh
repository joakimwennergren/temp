cd tmp-project/csharp
dotnet publish -r ios-arm64
cd ../..
install_name_tool -rpath @executable_path @executable_path/Frameworks tmp-project/csharp/bin/Release/net9.0/ios-arm64/publish/CSharpLibrary.dylib
install_name_tool -id @rpath/CSharpLibrary.framework/CSharpLibrary tmp-project/csharp/bin/Release/net9.0/ios-arm64/publish/CSharpLibrary.dylib
lipo -create tmp-project/csharp/bin/Release/net9.0/ios-arm64/publish/CSharpLibrary.dylib -output CSharpLibrary.framework/CSharpLibrary 