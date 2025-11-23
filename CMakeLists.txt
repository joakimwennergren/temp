cmake_minimum_required(VERSION 4.0.0)
include(FetchContent)

set(CMAKE_POLICY_DEFAULT_CMP0077 NEW)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Declare project
project(EntropyApp VERSION 1.0.0 LANGUAGES CXX)

#macro(charp)
#        # Paths for Windows
#        set(MY_DLL_SOURCE "${PROJECT_SOURCE_DIR}/tmp-project/csharp/bin/Debug/net9.0/win-x64/CSharpLibrary.dll")
#        set(MY_DLL_DESTINATION "${PROJECT_BINARY_DIR}/CSharpLibrary.dll")
#
#       add_custom_target(publish_native_aot ALL
#                COMMAND dotnet publish -r win-x64 -c Debug
#                COMMAND ${CMAKE_COMMAND} -E copy_if_different "${MY_DLL_SOURCE}" "${MY_DLL_DESTINATION}"
#                WORKING_DIRECTORY ${PROJECT_SOURCE_DIR}/tmp-project/csharp
#               COMMENT "Publishing and copying CSharpLibrary.dll"
#        )

# Make sure it runs before linking
#add_dependencies(${PROJECT_NAME} publish_native_aot)
#endmacro()

macro(charp)
        set(MY_DYLIB_SOURCE "${PROJECT_SOURCE_DIR}/tmp-project/csharp/bin/Debug/net9.0/osx-arm64/native/CSharpLibrary.dylib")
        set(MY_DYLIB_DESTINATION "${PROJECT_BINARY_DIR}/CSharpLibrary.dylib")

        add_custom_target(publish_native_aot ALL
                COMMAND dotnet publish -r osx-arm64 -c Debug
                COMMAND ${CMAKE_COMMAND} -E copy "${MY_DYLIB_SOURCE}" "${MY_DYLIB_DESTINATION}"
                COMMAND install_name_tool -id "@rpath/CSharpLibrary.dylib" "${MY_DYLIB_DESTINATION}"
                WORKING_DIRECTORY ${PROJECT_SOURCE_DIR}/tmp-project/csharp
                COMMENT "Publishing, copying, and patching CSharpLibrary.dylib"
        )

        # Make sure it runs before linking
        add_dependencies(${PROJECT_NAME} publish_native_aot)
endmacro()

# Point to your .icns file
set(APP_ICON ${CMAKE_CURRENT_SOURCE_DIR}/static/icons/icon.icns)

# Tell CMake to put the .icns into the app’s Resources
set_source_files_properties(${APP_ICON} PROPERTIES
        MACOSX_PACKAGE_LOCATION "Resources"
)
set(BUILD_ENTROPY_AS_EXECUTABLE OFF CACHE BOOL "" FORCE)
add_subdirectory(../Entropy-Engine Entropy)

# Glob source files
file(GLOB_RECURSE SOURCES
        ${PROJECT_SOURCE_DIR}/src/*.cpp
)

if(MSVC)
        # Example: treat warnings as errors
        add_compile_options(/W4)

        # Enable C++ latest standard features (for MSVC 19+)
        add_compile_options(/std:c++17)
endif()

# IOS
set(CMAKE_XCODE_GENERATE_SCHEME TRUE)
set(CMAKE_XCODE_SCHEME_ENABLE_GPU_API_VALIDATION FALSE)
set(CMAKE_XCODE_SCHEME_ENABLE_GPU_FRAME_CAPTURE_MODE DISABLED)
set(CMAKE_FIND_ROOT_PATH_MODE_LIBRARY BOTH)
set(CMAKE_FIND_ROOT_PATH_MODE_INCLUDE BOTH)
set(CMAKE_XCODE_ATTRIBUTE_CODE_SIGNING_ALLOWED "YES")
set(CMAKE_XCODE_ATTRIBUTE_CODE_SIGN_STYLE "Automatic")
set(CMAKE_XCODE_ATTRIBUTE_DEVELOPMENT_TEAM "HC784924PR")
set(CMAKE_XCODE_ATTRIBUTE_CODE_SIGN_IDENTITY "Apple Development")
set(CMAKE_XCODE_ATTRIBUTE_TARGETED_DEVICE_FAMILY "1,2")

# Assume these are your precompiled SPIR-V files
set(SHADER_VERT ${CMAKE_BINARY_DIR}/2dshader_vert.spv)
set(SHADER_FRAG ${CMAKE_BINARY_DIR}/2dshader_frag.spv)

if(IOS)
        add_executable(${PROJECT_NAME} MACOSX_BUNDLE
                ${PROJECT_SOURCE_DIR}/ios/AppDelegate.h
                ${PROJECT_SOURCE_DIR}/ios/AppDelegate.mm
                ${PROJECT_SOURCE_DIR}/ios/SceneDelegate.h
                ${PROJECT_SOURCE_DIR}/ios/SceneDelegate.mm
                ${PROJECT_SOURCE_DIR}/ios/ViewController.h
                ${PROJECT_SOURCE_DIR}/ios/ViewController.mm
                ${PROJECT_SOURCE_DIR}/ios/main.mm
        )
        set(LINK_LIBS
                Entropy
                flecs::flecs_static
                GPUOpen::VulkanMemoryAllocator
                glm::glm
                quill
        )
else()
        add_executable(${PROJECT_NAME} #MACOSX_BUNDLE
                ${SOURCES}
        )
        set(LINK_LIBS
                Entropy
                #flecs::flecs_static
                #GPUOpen::VulkanMemoryAllocator
                #glm::glm
                #quill
                ${PROJECT_BINARY_DIR}/CSharpLibrary.dylib
        )
        #charp()
endif()

#target_sources(${PROJECT_NAME} PRIVATE ${APP_ICON})

# Set the bundle icon property so Info.plist references it
#set_target_properties(${PROJECT_NAME} PROPERTIES
#    MACOSX_BUNDLE TRUE
#   MACOSX_BUNDLE_ICON_FILE "icon.icns"  # name only, not a path
#)

set_target_properties(${PROJECT_NAME} PROPERTIES
        BUILD_RPATH "@executable_path"
        INSTALL_RPATH "@executable_path"
)

target_include_directories(${PROJECT_NAME} PRIVATE
        ${ENTROPY_INCLUDE_DIRS}
        include
)

target_link_libraries(${PROJECT_NAME} PUBLIC
        ${LINK_LIBS}
)


add_custom_command(
        TARGET ${PROJECT_NAME}
        POST_BUILD
        COMMAND ${CMAKE_SOURCE_DIR}/scripts/publish-native-aot.sh --project-source-dir ${CMAKE_SOURCE_DIR}/csharp_old --platform macos-arm64 --dll-name CSharpLibrary.dylib
        COMMAND install_name_tool -id "@rpath/CSharpLibrary.dylib" ${CMAKE_BINARY_DIR}/CSharpLibrary.dylib
        COMMAND install_name_tool -change "bin/Debug/net9.0/osx-arm64/native/CSharpLibrary.dylib" "@rpath/CSharpLibrary.dylib" ${CMAKE_BINARY_DIR}/EntropyApp
        VERBATIM
)

target_sources(${PROJECT_NAME} PRIVATE ${CMAKE_BINARY_DIR}/2dshader_vert.spv ${CMAKE_BINARY_DIR}/2dshader_frag.spv)

if(IOS)
        set_target_properties(${PROJECT_NAME} PROPERTIES
                MACOSX_BUNDLE_INFO_PLIST ${PROJECT_SOURCE_DIR}/ios/info.plist
        )

        set_target_properties(${PROJECT_NAME} PROPERTIES
                BUNDLE_IDENTIFIER se.databeams.svampjakt
                XCODE_ATTRIBUTE_PRODUCT_BUNDLE_IDENTIFIER "se.databeams.svampjakt"
                XCODE_ATTRIBUTE_CODE_SIGNING_ALLOWED "YES"
                XCODE_ATTRIBUTE_CLANG_ENABLE_MODULES "YES"
                XCODE_ATTRIBUTE_LD_RUNPATH_SEARCH_PATHS "@executable_path/Frameworks"

                # XCODE_ATTRIBUTE_CODE_SIGN_STYLE "Automatic" # already default value
                # XCODE_ATTRIBUTE_CODE_SIGN_IDENTITY "iPhone Developer"
                MACOSX_BUNDLE_SHORT_VERSION_STRING 1.0.0
                MACOSX_BUNDLE_BUNDLE_VERSION 1.0.0

                # XCODE_ATTRIBUTE_DEVELOPMENT_TEAM "XXX"
                # XCODE_EMBED_FRAMEWORKS "${Vulkan_MoltenVK_LIBRARY};${Vulkan_LIBRARIES};${Vulkan_Layer_VALIDATION}"
                XCODE_EMBED_FRAMEWORKS_CODE_SIGN_ON_COPY "YES"
                XCODE_EMBED_FRAMEWORKS_REMOVE_HEADERS_ON_COPY "YES"
                XCODE_ATTRIBUTE_SKIP_INSTALL NO
                XCODE_ATTRIBUTE_INSTALL_PATH "$(LOCAL_APPS_DIR)"
                XCODE_ATTRIBUTE_ASSETCATALOG_COMPILER_APPICON_NAME "AppIcon"
                XCODE_ATTRIBUTE_DEAD_CODE_STRIPPING NO
        )
else()
        # Make the dylib advertise its ID as @rpath/CSharpLibrary.dylib
        set_target_properties(${PROJECT_NAME} PROPERTIES
                BUILD_RPATH "@loader_path" # while running from build tree
                INSTALL_RPATH "@executable_path" # when installed next to the dylib
        )
endif()

file(COPY ${CMAKE_SOURCE_DIR}/assets DESTINATION ${CMAKE_BINARY_DIR})
