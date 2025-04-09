shader_type canvas_item;    // The mode we picked when creating the shader - canvas_item equates to '2d shader' where as spcial shader_type would be '3d shader'

uniform float my_float;
uniform float my_float2: hint_range(1.0, 10.0) = 5.0;
uniform vec4 my_color: source_color = vec4(0.0, 0.0, 0.0, 1.0);


void vertex()
{
    if ((VERTEX.y < 0.0 && VERTEX.x < 0.0) || (VERTEX.y > 0.0 && VERTEX.x > 0.0))
    {
        float speed = 0.75;
        // VERTEX += vec2(-100.0, -100.0);
        VERTEX += vec2(cos(TIME*my_float)*100.0, sin(TIME)*100.0);
    }
    // VERTEX += vec2(TIME*40.0, TIME*40.0);
    // VERTEX += vec2(cos(TIME)*100.0, sin(TIME)*100.0);


}

void fragment() {
    // This forces the sin or cos to be b/w 0.0 and 1.0. Seems useful.
    float cur_sin = ((sin(TIME)+1.0)*0.5);
    float cur_cos = ((cos(TIME)+1.0)*0.5);
	// Called for every pixel the material is visible on.
    // COLOR = vec4(sin(TIME), cos(TIME), cos(TIME), 1.0);
    // COLOR = vec4(0.0, 0.0, 0.0, 1.0);

    vec4 output_color = vec4(1.0, 1.0, 0.0, 0.7);

    vec4 color1 = vec4(1.0, 0.0, 0.0, 1.0); // red
    vec4 color2 = vec4(0.0, 1.0, 0.0, 1.0); // green
    vec4 mixed_color = mix(color1, color2, cur_sin);
    vec4 texture_color = texture(TEXTURE, UV*1.0);
    vec4 gradient_color = vec4(UV.x*cur_sin, UV.y*cur_cos, UV.x*cur_cos, 1.0);


    // COLOR = texture(TEXTURE, UV);
    // COLOR = gradient_color*texture_color;
    // COLOR = mix(texture(TEXTURE, UV), my_color, 5.0);;
    // COLOR = texture(TEXTURE, UV)+my_color;
    COLOR = texture(TEXTURE, UV)*my_color;
}
