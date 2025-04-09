shader_type canvas_item;

uniform sampler2D noise_texture : repeat_enable;

void fragment() {
    vec4 color_out = vec4(1.0);
    float TimeSpeedR = 0.005;
    float TimeSpeedG = 0.007;
    float TimeSpeedB = 0.007;

    vec2 uv_r = vec2(UV.x + TIME*TimeSpeedR, UV.y);
    vec2 uv_g = vec2(UV.x - TIME*TimeSpeedG, UV.y);
    vec2 uv_b = vec2(UV.x, UV.y + TIME*TimeSpeedB);

    float noise_r = texture(noise_texture, uv_r).r;
    float noise_g = texture(noise_texture, uv_g).g;
    float noise_b = texture(noise_texture, uv_b).b;

    float new_alpha = noise_r * noise_g * noise_b;

    //COLOR.r = noise_r;
    //COLOR.g = noise_g;
    //COLOR.b = noise_b;
    color_out.r = clamp(sin(noise_r+TIME*TimeSpeedG),0.0,1.0);
    color_out.g = clamp(sin(noise_g+TIME*TimeSpeedG),0.0,1.0);
    color_out.b = clamp(cos(noise_b+TIME*TimeSpeedG),0.0,1.0);
    COLOR.rgb = color_out.rgb;
    COLOR.a *= clamp(new_alpha*2.0, 0.0, 1.0);


    //vec4 new_color = vec4(noise_r, noise_g, noise_b, new_alpha);

    //COLOR.rgb = texture(TEXTURE, UV).rgb;
    //COLOR.a *= new_alpha;

    //COLOR.rgb = new_color;
    //COLOR.a = texture(TEXTURE, newUV).a;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
