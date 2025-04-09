shader_type canvas_item;

uniform sampler2D screen_texture: hint_screen_texture;

void vertex() {
}

void fragment() {
    // So... we have the screen texture from godot w/ the uniform above, now we want to pull the color from it
    vec4 screen_color = texture(screen_texture, SCREEN_UV);
    float rgbavg = (screen_color.a + screen_color.g + screen_color.b) / 3.0;
    vec4 new_sc = vec4(rgbavg, rgbavg, rgbavg, screen_color.a);
    COLOR = new_sc;
}
