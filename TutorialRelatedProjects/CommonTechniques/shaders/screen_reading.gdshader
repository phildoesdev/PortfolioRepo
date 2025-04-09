shader_type canvas_item;

uniform sampler2D screen_texture: hint_screen_texture, filter_nearest;

void vertex() {
}

void fragment() {
    // So... we have the screen texture from godot w/ the uniform above, now we want to pull the color from it
    vec4 screen_color = texture(screen_texture, UV);
    COLOR = screen_color;
}
