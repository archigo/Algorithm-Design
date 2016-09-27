//
// Created by Morten Nobel-Jørgensen on 20/09/16.
//

#include "Box.hpp"
#include "SRE/Shader.hpp"
#include "glm/gtc/matrix_transform.hpp"

using namespace SRE;
using namespace glm;

Box::Box() {
    mesh = Mesh::createCube();
    shader = Shader::getUnlit();
    position = vec2(0,0);
    scale = vec2(1,1);
}

Box::~Box() {
    delete mesh;
}

void Box::draw() {
    shader->setVector("color", vec4(1));
    SimpleRenderEngine::instance->draw(mesh, getTransform(),shader);
}

glm::mat4 Box::getTransform() {
    // Todo exercise 1

	auto scaleMat = glm::scale(glm::mat4(1), vec3(scale.x, scale.y, 0.1f));
	auto translate = glm::translate(glm::mat4(1), vec3(position.x, position.y, 0));

    return translate*scaleMat;
}

Edge2D Box::getEdge(int index) {
    Edge2D res;
    glm::vec4 vertices[] = {
            glm::vec4(-1,-1,0,1),
            glm::vec4(1,-1,0,1),
            glm::vec4(1,1,0,1),
            glm::vec4(-1,1,0,1),
    };
    res.from = vec2(getTransform() * vertices[index]);
    res.to = vec2(getTransform() * vertices[(index+1)%4]);
    vec2 norm = normalize(res.to - res.from);
    res.normal = vec2(norm.y,-norm.x);

    return res;
}
