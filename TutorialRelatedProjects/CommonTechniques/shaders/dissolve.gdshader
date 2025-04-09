shader_type canvas_item;

// The first step is to create a 'noise texture', which is just a built-in type of texture
uniform sampler2D noise_tex;
// We also define 'dissolve percentage', which defines the threshold (from 0.0 to 1.0) at which we no longer show the original pixel...
uniform float dissolve_pct: hint_range(0.0, 1.0) = 0.0f;

void fragment() {
    // So... after creating the noise texture we want to sample it. It is a field w/ randomly generated values in the range [0.0, 1.0]
    //  We want to use that field to determine if we are showing a pixel or not on our original sprite
    vec4 original_color = texture(TEXTURE, UV);
    vec4 final_color = original_color;
    vec4 noise = texture(noise_tex, UV);

    float test = ((sin(TIME)+1.0)*0.5);

    // Now, we defined dissolve_pct, it is the threshold for which we will no longer show a pixel in the original sprite
    //  r,g,b are all the same, so we can compare any.
    // This setup allows us to slowly increase the 'dissolve_pct' in code, or w/e, which, and as this pct goes up, more random pixels from the noise mask match, and more and more of the img disappears
    // Could do this w/ a uniform in code, or with sin/cos and TIME
    // Specifically, in the tutorial, we are using an animation player to progress dissolve_pct from 0.0 to 1.0.. I implemented this, b/c it's better, more control and always starts at 0.0 . Depending on the use-case, either could be interesting
    if(dissolve_pct >= noise.r){
        final_color.a = 0.0;
    }

    COLOR = final_color;
}