shader_type canvas_item;

uniform sampler2D mask_tex;

void fragment() {
    vec4 mask_color = texture(mask_tex, UV);
    vec4 orig_color = texture(TEXTURE, UV);

    if (orig_color.a > 0.0)
    {
        // Setting the alpha to the r value (r==b==g) gives a gradient, fade effect..
        // ... this works nice for the fuzzy mask. I could see how you could get into trouble overriding any alpha, though.
        // ... Regardless, it is easy to see how this might be interesting. It is the exact kind of mask they use for lighting effects
        // if mask_color.r == 0.0 { COLOR.a = 0.0; }  // this is another approach to setting it, avoiding gradients
        COLOR.a = mask_color.r;
    }
    // COLOR = texture(mask_tex, UV);
}
