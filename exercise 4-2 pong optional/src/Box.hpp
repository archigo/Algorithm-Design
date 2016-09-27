#pragma once

#include "SRE/Mesh.hpp"
#include "SRE/SimpleRenderEngine.hpp"
#include "glm/glm.hpp"

#include "glm/gtc/matrix_transform.hpp"

using namespace SRE;
using namespace glm;

struct Edge2D{
    glm::vec2 from;
    glm::vec2 to;
    glm::vec2 normal;
};

class Box {
public:
    Box();
    virtual ~Box();

    void draw();

    // get the 2D edge, with the edge normal (perpendicular to the edge)
    Edge2D getEdge(int index);

    glm::vec2 position;
    glm::vec2 scale;
private:
    glm::mat4 getTransform();
    SRE::Shader* shader;
    SRE::Mesh *mesh;
};


