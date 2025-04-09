shader_type canvas_item;

uniform sampler2D gradient_texture;

void fragment() {
    // Turn it greyscale
    vec4 clr = texture(TEXTURE, UV);
    float avg = (clr.r + clr.g + clr.b)/3.0;
    COLOR = vec4(avg, avg, avg, clr.a);
    
    // So... We have the texture gradient that we've imported, and we can poll it using the texture(...) method
    // So, with that, we have a pixel that is some variation of green and we want to correlate that to a pixel (color) on our gradient texture...
    // ... we want to do this 'based on how grey' our thing is. 
    // Each pixel in a greyscale img has a value between (0.0, 1.0). We can use this as a percentage to get our position on the gradient
    vec4 gradient_color = texture(gradient_texture, vec2(avg, avg));
    gradient_color.a = clr.a;
    COLOR = gradient_color;
}
