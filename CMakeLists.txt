cmake_minimum_required(VERSION 3.28.1)
include(FetchContent)

set(CMAKE_POLICY_DEFAULT_CMP0077 NEW)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Declare project
project(EntropyApp VERSION 1.0.0 LANGUAGES CXX)

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

        # Declare how to build the output file
        #add_custom_command(
        #        OUTPUT ${MY_DYLIB_DESTINATION}
        #       COMMAND dotnet publish -r osx-arm64 -c Debug
        #        COMMAND ${CMAKE_COMMAND} -E copy "${MY_DYLIB_SOURCE}" "${MY_DYLIB_DESTINATION}"
        #        COMMAND install_name_tool -id "@rpath/MyNativeAotLibrary.dylib" "${MY_DYLIB_DESTINATION}"
        #        WORKING_DIRECTORY ${PROJECT_SOURCE_DIR}/MyNativeAotLibrary
        #        COMMENT "Publishing, copying, and patching MyNativeAotLibrary.dylib"
        #)

        # Create a target that produces the .dylib file
        #add_custom_target(publish_native_aot DEPENDS ${MY_DYLIB_DESTINATION})

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

# Create the library 
add_executable(${PROJECT_NAME} #MACOSX_BUNDLE
        ${SOURCES}
)

charp()

target_sources(${PROJECT_NAME} PRIVATE ${APP_ICON})

# Set the bundle icon property so Info.plist references it
#set_target_properties(${PROJECT_NAME} PROPERTIES
#    MACOSX_BUNDLE TRUE
#   MACOSX_BUNDLE_ICON_FILE "icon.icns"  # name only, not a path
#)

target_include_directories(${PROJECT_NAME} PRIVATE
        ${ENTROPY_INCLUDE_DIRS}
        include
)

target_link_libraries(${PROJECT_NAME} PRIVATE
        Entropy
        flecs::flecs_static
        GPUOpen::VulkanMemoryAllocator
        glm::glm
        quill
        ${PROJECT_BINARY_DIR}/CSharpLibrary.dylib
)

file(COPY ${CMAKE_SOURCE_DIR}/tmp-project/assets DESTINATION ${CMAKE_BINARY_DIR})
