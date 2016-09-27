#include <iostream>
#include <string>
#include <fstream>
#include <sstream>

#define SDL_MAIN_HANDLED
#include "SDL.h"
#include "SRE/SimpleRenderEngine.hpp"
#include "Pong.hpp"

using namespace std;

int main(int argc, char** argv) {
    SDL_Window *window;                    // Declare a pointer

    SDL_Init(SDL_INIT_VIDEO);              // Initialize SDL2

    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 1);

    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

    int width = 640;
    int height = 480;
    // Create an application window with the following settings:
    window = SDL_CreateWindow(
            "An SDL2 window",                  // window title
            SDL_WINDOWPOS_UNDEFINED,           // initial x position
            SDL_WINDOWPOS_UNDEFINED,           // initial y position
            width,                               // width, in pixels
            height,                               // height, in pixels
            SDL_WINDOW_OPENGL                  // flags - see below
    );
    SDL_SetWindowTitle(window,"Hello world");
    // Check that the window was successfully made
    if (window == NULL) {
        // In the event that the window could not be made...
        printf("Could not create window: %s\n", SDL_GetError());
        return 1;
    }

    SRE::SimpleRenderEngine r{window};

    // Use windows coordinates in this example (lower left corner is (0,0), upper right corner is (width, height)
    r.getCamera()->setWindowCoordinates();

    Pong p(window, width, height);
    p.startGameLoop();

    // Close and destroy the window
    SDL_DestroyWindow(window);

    // Clean up
    SDL_Quit();

    return 0;
}