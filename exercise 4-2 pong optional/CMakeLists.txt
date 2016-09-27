cmake_minimum_required(VERSION 3.6)
project(Ex4_2_Pong_MathEx)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++14")
set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} "${CMAKE_SOURCE_DIR}/cmake")

set(SOURCE_FILES src/main.cpp src/Pong.cpp src/Pong.hpp src/Box.cpp src/Box.hpp src/Ball.cpp src/Ball.hpp)
add_executable(Pong_MathEx ${SOURCE_FILES})


find_package(SDL2 REQUIRED)
include_directories(${SDL2_INCLUDE_DIR})

#########################################################
# FIND OPENGL
#########################################################
IF(APPLE)
    INCLUDE_DIRECTORIES ( /System/Library/Frameworks )
    FIND_LIBRARY(OpenGL_LIBRARY OpenGL )
    MARK_AS_ADVANCED (OpenGL_LIBRARY)
    SET(EXTRA_LIBS ${OpenGL_LIBRARY})
ELSE (APPLE)
    find_package(OpenGL REQUIRED)
    include_directories(${OPENGL_INCLUDE_DIR})
    IF(UNIX)
        find_package(GLEW REQUIRED)
    ELSE(UNIX)
        find_package(glew REQUIRED)
    ENDIF(UNIX)
    include_directories(${GLEW_INCLUDE_DIR})
    SET(EXTRA_LIBS ${OPENGL_LIBRARY} ${GLEW_LIBRARY})
ENDIF (APPLE)

find_package(SDL2_IMAGE REQUIRED)
include_directories(${SDL2_IMAGE_INCLUDE_DIRS})
include_directories(.)

# GLM -- on Windows: depends on env var: GLM_ROOT_DIR
find_package (glm REQUIRED)

IF(UNIX AND NOT APPLE)
    IF( glm_FOUND )
        INCLUDE_DIRECTORIES( ${GLM_INCLUDE_DIR} )
    ELSE( glm_FOUND )
        MESSAGE( STATUS "Failed to find GLM ----------------------")
    ENDIF( glm_FOUND )
ELSE(UNIX AND NOT APPLE)
    IF( GLM_FOUND )
        INCLUDE_DIRECTORIES( ${GLM_INCLUDE_DIR} )
    ELSE( GLM_FOUND )
        MESSAGE( STATUS "Failed to find GLM ----------------------")
    ENDIF( GLM_FOUND )
ENDIF(UNIX AND NOT APPLE)

find_package(SRE REQUIRED)
if (SRE_FOUND)
    include_directories(${SRE_INCLUDE_DIR})
ELSE (SRE_FOUND)
    MESSAGE( STATUS "Failed to find Simple Render Engine")
endif(SRE_FOUND)

target_link_libraries(Pong_MathEx ${EXTRA_LIBS} ${SDL2_LIBRARY} ${SDL2_IMAGE_LIBRARIES} ${SRE_LIBRARY})

IF (WIN32)
    SET(SDL2_dll "" CACHE STRING "Where the SDL2.dll can be found")
    SET(SDL2_image_dll "" CACHE STRING "Where the SDL2_image.dll can be found")
    SET(libjpeg-9_dll "" CACHE STRING "Where the libjpeg-9.dll can be found")
    SET(libpng16-16_dll "" CACHE STRING "Where the libpng16-16.dll can be found")
    SET(libtiff-5_dll "" CACHE STRING "Where the libtiff-5.dll can be found")
    SET(libwebp-4_dll "" CACHE STRING "Where the libwebp-4.dll can be found")
    SET(zlib1_dll "" CACHE STRING "Where the zlib1.dll can be found")
    SET(DLLFileList ${libjpeg-9_dll} ${libpng16-16_dll} ${SDL2_dll} ${SDL2_image_dll} ${zlib1_dll} ${libtiff-5_dll} ${libwebp-4_dll})
    file(COPY ${DLLFileList} DESTINATION Debug)
    file(COPY ${DLLFileList} DESTINATION Release)
ENDIF(WIN32)
