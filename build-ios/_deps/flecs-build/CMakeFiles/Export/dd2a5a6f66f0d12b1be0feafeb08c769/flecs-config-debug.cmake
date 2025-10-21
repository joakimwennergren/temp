#----------------------------------------------------------------
# Generated CMake target import file for configuration "Debug".
#----------------------------------------------------------------

# Commands may need to know the format version.
set(CMAKE_IMPORT_FILE_VERSION 1)

# Import target "flecs::flecs_static" for configuration "Debug"
set_property(TARGET flecs::flecs_static APPEND PROPERTY IMPORTED_CONFIGURATIONS DEBUG)
set_target_properties(flecs::flecs_static PROPERTIES
  IMPORTED_LINK_INTERFACE_LANGUAGES_DEBUG "C"
  IMPORTED_LOCATION_DEBUG "${_IMPORT_PREFIX}/lib/libflecs_static.a"
  )

list(APPEND _cmake_import_check_targets flecs::flecs_static )
list(APPEND _cmake_import_check_files_for_flecs::flecs_static "${_IMPORT_PREFIX}/lib/libflecs_static.a" )

# Commands beyond this point should not need to know the version.
set(CMAKE_IMPORT_FILE_VERSION)
