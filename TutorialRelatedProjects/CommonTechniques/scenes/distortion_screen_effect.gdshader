shader_type canvas_item;

uniform sampler2D screen_texture: hint_screen_texture, filter_nearest;


// This is 'how much distortion'
uniform float distortion_strength: hint_range(-1.0, 1.0) = 0.1f;

// This is essentially "amt to scroll perk TIME tick"
uniform vec2 offset1 = vec2(0.1);
uniform vec2 offset2 = vec2(0.2);
uniform sampler2D noise_tex1: repeat_enable;
uniform sampler2D noise_tex2: repeat_enable;


void fragment() {

    vec4 noise_color1 = texture(noise_tex1, UV+offset1*TIME);
    vec4 noise_color2 = texture(noise_tex2, UV+offset2*TIME);

    // This has a nice effect, almost like water
    vec4 water_color = mix(noise_color1, noise_color2, 0.5);  // This doesn't skew it towards 0.0 or 1.0, but instead softens things.. 0.5 is a soft mix of both, and anything closer to the min/max produces a softer version of w/e texture is at the end it is approaching. Cool effect. Cool amt of control. Could imagine fading something in interestingly this way.
    vec4 spooky_color = noise_color1 * noise_color2;
    vec4 spooky_color_2 = vec4(vec3(noise_color1.r * noise_color2.r), 1.0);  // This is how he is calculating this. It's the same result but it considers the alphas... in my 'spooky_color' I know all alphas are 1.0 so it doesnt matter. This would fix that, if it were an issue.
    vec4 skewed_white_color = noise_color1 / noise_color2;  // this is super interesting b/c mult skews it towards 1.0, and divide skews it towards 0.0

    // Overall... this method of layering noise and sampling it, or straight up displaying it is beauitufl. It's easy to imagine using these r values as alphas, or w/e.
    // Could make fire, water, smoke, etc. Seems cool. Seems like it could be the basis for a bunch of interesting things. Will have to try and experiement a lot.

    vec4 screen_distorted_color = texture(screen_texture, SCREEN_UV+(noise_color1.r*distortion_strength));
    vec4 output_color = screen_distorted_color;
    COLOR = output_color;

}
