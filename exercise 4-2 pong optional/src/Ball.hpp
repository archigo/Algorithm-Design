#pragma once

#include <glm/glm.hpp>

#include "SRE/Mesh.hpp"
#include "SRE/Shader.hpp"

class Ball {
public:
    Ball();
    virtual ~Ball();
    void draw();

    void move(float deltaTimeInSeconds);

    glm::vec2 position;
    float radius;

    glm::vec2 velocity; // pixels per seconds
private:
    glm::mat4 getTransform();
    SRE::Shader* shader;
    SRE::Mesh *mesh;
};


