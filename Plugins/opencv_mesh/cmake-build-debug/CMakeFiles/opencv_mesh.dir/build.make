# CMAKE generated file: DO NOT EDIT!
# Generated by "MinGW Makefiles" Generator, CMake Version 3.16

# Delete rule output on recipe failure.
.DELETE_ON_ERROR:


#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:


# Remove some rules from gmake that .SUFFIXES does not remove.
SUFFIXES =

.SUFFIXES: .hpux_make_needs_suffix_list


# Suppress display of executed commands.
$(VERBOSE).SILENT:


# A target that is always out of date.
cmake_force:

.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

SHELL = cmd.exe

# The CMake executable.
CMAKE_COMMAND = "C:\Program Files\JetBrains\CLion 2020.1.2\bin\cmake\win\bin\cmake.exe"

# The command to remove a file.
RM = "C:\Program Files\JetBrains\CLion 2020.1.2\bin\cmake\win\bin\cmake.exe" -E remove -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\cmake-build-debug

# Include any dependencies generated for this target.
include CMakeFiles/opencv_mesh.dir/depend.make

# Include the progress variables for this target.
include CMakeFiles/opencv_mesh.dir/progress.make

# Include the compile flags for this target's objects.
include CMakeFiles/opencv_mesh.dir/flags.make

CMakeFiles/opencv_mesh.dir/library.cpp.obj: CMakeFiles/opencv_mesh.dir/flags.make
CMakeFiles/opencv_mesh.dir/library.cpp.obj: ../library.cpp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\cmake-build-debug\CMakeFiles --progress-num=$(CMAKE_PROGRESS_1) "Building CXX object CMakeFiles/opencv_mesh.dir/library.cpp.obj"
	C:\PROGRA~1\MINGW-~1\X86_64~1.0-P\mingw64\bin\G__~1.EXE  $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -o CMakeFiles\opencv_mesh.dir\library.cpp.obj -c D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\library.cpp

CMakeFiles/opencv_mesh.dir/library.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/opencv_mesh.dir/library.cpp.i"
	C:\PROGRA~1\MINGW-~1\X86_64~1.0-P\mingw64\bin\G__~1.EXE $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -E D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\library.cpp > CMakeFiles\opencv_mesh.dir\library.cpp.i

CMakeFiles/opencv_mesh.dir/library.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/opencv_mesh.dir/library.cpp.s"
	C:\PROGRA~1\MINGW-~1\X86_64~1.0-P\mingw64\bin\G__~1.EXE $(CXX_DEFINES) $(CXX_INCLUDES) $(CXX_FLAGS) -S D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\library.cpp -o CMakeFiles\opencv_mesh.dir\library.cpp.s

# Object files for target opencv_mesh
opencv_mesh_OBJECTS = \
"CMakeFiles/opencv_mesh.dir/library.cpp.obj"

# External object files for target opencv_mesh
opencv_mesh_EXTERNAL_OBJECTS =

libopencv_mesh.a: CMakeFiles/opencv_mesh.dir/library.cpp.obj
libopencv_mesh.a: CMakeFiles/opencv_mesh.dir/build.make
libopencv_mesh.a: CMakeFiles/opencv_mesh.dir/link.txt
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --bold --progress-dir=D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\cmake-build-debug\CMakeFiles --progress-num=$(CMAKE_PROGRESS_2) "Linking CXX static library libopencv_mesh.a"
	$(CMAKE_COMMAND) -P CMakeFiles\opencv_mesh.dir\cmake_clean_target.cmake
	$(CMAKE_COMMAND) -E cmake_link_script CMakeFiles\opencv_mesh.dir\link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
CMakeFiles/opencv_mesh.dir/build: libopencv_mesh.a

.PHONY : CMakeFiles/opencv_mesh.dir/build

CMakeFiles/opencv_mesh.dir/clean:
	$(CMAKE_COMMAND) -P CMakeFiles\opencv_mesh.dir\cmake_clean.cmake
.PHONY : CMakeFiles/opencv_mesh.dir/clean

CMakeFiles/opencv_mesh.dir/depend:
	$(CMAKE_COMMAND) -E cmake_depends "MinGW Makefiles" D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\cmake-build-debug D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\cmake-build-debug D:\UnityProjects\UnityPlayground\Plugins\opencv_mesh\cmake-build-debug\CMakeFiles\opencv_mesh.dir\DependInfo.cmake --color=$(COLOR)
.PHONY : CMakeFiles/opencv_mesh.dir/depend

