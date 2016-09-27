#include <SRE/Shader.hpp>
#include "Pong.hpp"
#include "SDL.h"
#include "glm/gtc/matrix_transform.hpp"
#include "SRE/Debug.hpp"
#include "SRE/Text.hpp"
#include "SRE/Texture.hpp"
#include <iostream>
#include <string>

using namespace glm;

Pong::Pong(SDL_Window * win,int width, int height) {
    this->win = win;
    this->width = width;
    this->height = height;
    wPressed = false;
    pPressed = false;
    sPressed = false;
    lPressed = false;

    backgroundColor = vec4{0,0,0,1};

    leftPaddle.position = vec2(paddleOffsetX,height/2);
    leftPaddle.scale = vec2(10,paddleHeight);
    rightPaddle.position = vec2(width-paddleOffsetX,height/2);
    rightPaddle.scale = vec2(lineWidth,paddleHeight);

    topBar.position = vec2(width/2,height-barOffsetY);
    topBar.scale = vec2(width/2,lineWidth);
    bottomBar.position = vec2(width/2,barOffsetY);
    bottomBar.scale = vec2(width/2,lineWidth);

    resetBall(true);
}

Pong::~Pong() {
}

void Pong::startGameLoop() {
    // delta time from http://gamedev.stackexchange.com/a/110831
    Uint64 NOW = SDL_GetPerformanceCounter();
    Uint64 LAST = 0;
    float deltaTimeSec = 0;
    auto sre = SimpleRenderEngine::instance;
    while (true){
        LAST = NOW;
        NOW = SDL_GetPerformanceCounter();

        deltaTimeSec = glm::clamp(((NOW - LAST) / (float)SDL_GetPerformanceFrequency() ),0.0001f,1.0f);

        sre->clearScreen(backgroundColor);
        sre->getCamera()->setWindowCoordinates();

        // move paddles
        handleKeyInput();

        float paddleSpeed = 200.0f;
        if (wPressed){
            movePaddle(&leftPaddle, paddleSpeed*deltaTimeSec);
        }
        if (sPressed){
            movePaddle(&leftPaddle, -paddleSpeed*deltaTimeSec);
        }
        if (pPressed){
            movePaddle(&rightPaddle, paddleSpeed*deltaTimeSec);
        }
        if (lPressed){
            movePaddle(&rightPaddle, -paddleSpeed*deltaTimeSec);
        }

        handleCollision(&leftPaddle);
        handleCollision(&rightPaddle);
        handleCollision(&topBar);
        handleCollision(&bottomBar);

        // move ball
        ball.move(deltaTimeSec);

        std::string score = std::to_string(leftScore)+"   "+std::to_string(rightScore);
        SDL_SetWindowTitle(win,score.c_str());

        handleOutOfBounds();

        leftPaddle.draw();
        rightPaddle.draw();
        topBar.draw();
        bottomBar.draw();
        ball.draw();

        sre->swapWindow();
        SDL_Delay(16);
    }
}


void Pong::handleKeyInput() {
    SDL_Event event;
    while( SDL_PollEvent( &event ) ) {
        switch (event.type) {
            case SDL_KEYDOWN:
                switch (event.key.keysym.sym) {
                    case SDLK_w:
                        wPressed = true;
                        break;
                    case SDLK_s:
                        sPressed = true;
                        break;
                    case SDLK_p:
                        pPressed = true;
                        break;
                    case SDLK_l:
                        lPressed = true;
                        break;
                }
                break;
            case SDL_KEYUP:
                switch (event.key.keysym.sym) {
                    case SDLK_w:
                        wPressed = false;
                        break;
                    case SDLK_s:
                        sPressed = false;
                        break;
                    case SDLK_p:
                        pPressed = false;
                        break;
                    case SDLK_l:
                        lPressed = false;
                        break;
                }
                break;
        }
    }
}

void Pong::movePaddle(Box *paddle, float yDelta) {
    // Todo: Exercise 2

	//glm::clamp()

	paddle->position.y += yDelta;
}

void Pong::resetBall(bool right) {
    ball.velocity = vec2(right?100:-100, 180);
    ball.position = vec2(width/2,height/2);
}

bool Pong::handleCollision(Box *paddle) {
    for (int i=0;i<4;i++){
        auto e = paddle->getEdge(i);

        if (handleCollision(e)){
            return true;
        }
    }
    return false;
}

bool Pong::handleCollision(Edge2D edge) {
    // Todo: Exercise 3
    // If the angle between edge.normal and ball->velocity is less than 90 degrees, then assume no collision
    // test for collision between edge and this->ball. If collision is detected, then reflect the velocity on the ball
    // using edge.normal.


    return false; // should return true when collision happens
}

void Pong::handleOutOfBounds() {
    // Todo: Exercise 3
}

