# Distributed under the OSI-approved BSD 3-Clause License.  See accompanying
# file LICENSE.rst or https://cmake.org/licensing for details.

cmake_minimum_required(VERSION ${CMAKE_VERSION}) # this file comes with cmake

# If CMAKE_DISABLE_SOURCE_CHANGES is set to true and the source directory is an
# existing directory in our source tree, calling file(MAKE_DIRECTORY) on it
# would cause a fatal error, even though it would be a no-op.
if(NOT EXISTS "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-src")
  file(MAKE_DIRECTORY "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-src")
endif()
file(MAKE_DIRECTORY
  "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-build"
  "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-subbuild/quill-populate-prefix"
  "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-subbuild/quill-populate-prefix/tmp"
  "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-subbuild/quill-populate-prefix/src/quill-populate-stamp"
  "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-subbuild/quill-populate-prefix/src"
  "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-subbuild/quill-populate-prefix/src/quill-populate-stamp"
)

set(configSubDirs Debug)
foreach(subDir IN LISTS configSubDirs)
    file(MAKE_DIRECTORY "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-subbuild/quill-populate-prefix/src/quill-populate-stamp/${subDir}")
endforeach()
if(cfgdir)
  file(MAKE_DIRECTORY "/Users/joakimwennergren/Projects/Entropy-Application/build-ios/_deps/quill-subbuild/quill-populate-prefix/src/quill-populate-stamp${cfgdir}") # cfgdir has leading slash
endif()
