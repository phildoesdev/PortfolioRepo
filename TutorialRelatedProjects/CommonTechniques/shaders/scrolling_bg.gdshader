shader_type canvas_item;

uniform vec2 tiling = vec2(10.0, 10.0);
uniform vec2 offset = vec2(0.4, 0.4);

void vertex() {
	// tile img
    // animate using offset to look like it is scrolling
    UV = UV*tiling + offset*TIME;
}
