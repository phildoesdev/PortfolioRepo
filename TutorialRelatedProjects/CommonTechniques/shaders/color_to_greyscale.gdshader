shader_type canvas_item;

void fragment() {
    // Turning something greyscale
    // Black... vec4(0.0, 0.0, 0.0, 1.0)
    // White... vec4(1.0, 1.0, 1.0, 1.0)

    // Get the original color, take the avg of the RGB to get a value to set all to, then set the color to this, w/ the original alpha
    vec4 clr = texture(TEXTURE, UV);    // the og color
    float avg = (clr.r + clr.g + clr.b)/3.0;

    COLOR = vec4(avg, avg, avg, clr.a);
}