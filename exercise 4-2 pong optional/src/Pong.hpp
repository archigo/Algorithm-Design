#pragma once

#include "SRE/SimpleRenderEngine.hpp"
#include "glm/glm.hpp"
#include "Box.hpp"
#include "Ball.hpp"
#include "SDL.h"

class Pong {
public:
    Pong(SDL_Window * win, int width, int height);
    virtual ~Pong();
    void startGameLoop();
private:
    int width;
    int height;
    const int paddleHeight = 50;
    const int lineWidth = 10;
    const int paddleOffsetX = 50;
    const int barOffsetY = lineWidth*2;
    bool wPressed;
    bool pPressed;
    bool sPressed;
    bool lPressed;

    int leftScore;
    int rightScore;
    void movePaddle(Box* paddle, float yDelta);
    void handleKeyInput();
    bool handleCollision(Edge2D edge);
    bool handleCollision(Box* paddle);
    void resetBall(bool right);
    Ball ball;
    Box leftPaddle;
    Box rightPaddle;
    Box topBar;
    Box bottomBar;
    glm::vec4 backgroundColor;
    SDL_Window * win;

    void handleOutOfBounds();
};

