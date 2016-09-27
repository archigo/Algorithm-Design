#include "Ball.hpp"

#include "SRE/SimpleRenderEngine.hpp"
#include "SRE/Mesh.hpp"
#include "SRE/Shader.hpp"
#include "glm/gtc/matrix_transform.hpp"

using namespace SRE;
using namespace glm;
using namespace SRE;

Ball::Ball() {
    mesh = Mesh::createSphere();
    shader = Shader::getUnlit();
    position = vec2(0,0);
    radius = 10;
}

Ball::~Ball() {
}

void Ball::draw() {
    shader->setVector("color", vec4(1));
    SimpleRenderEngine::instance->draw(mesh, getTransform(),shader);
}

void Ball::move(float deltaTimeInSeconds) {
    // Todo exercise 2
}

glm::mat4 Ball::getTransform() {
    // Todo exercise 1
	auto scaleMat = glm::scale(glm::mat4(1), vec3(radius, radius, 0.1f));
	auto translate = glm::translate(glm::mat4(1), vec3(position.x, position.y, 0));

	return translate*scaleMat;
}
