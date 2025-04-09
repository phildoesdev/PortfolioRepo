shader_type canvas_item;

// percentage values that will be set externally
uniform float direct_dmg_percentage = 0.00;
uniform float phys_dmg_percentage = 0.00;
uniform float fire_dmg_percentage = 0.00;
uniform float cold_dmg_percentage = 0.00;
uniform float poison_dmg_percentage = 0.00;
uniform float energy_dmg_percentage = 0.00;

void fragment() {
    // Add all input percentages up so we can average them and make them out of 1.0 (so the bar is always full)
    float total_percentage = direct_dmg_percentage + phys_dmg_percentage + fire_dmg_percentage + cold_dmg_percentage + poison_dmg_percentage + energy_dmg_percentage;

    // Make each percentage out of the total so the bar is always full
    float direct_dmg_percentage_mod = direct_dmg_percentage/total_percentage;
    float phys_dmg_percentage_mod = phys_dmg_percentage/total_percentage;
    float fire_dmg_percentage_mod = fire_dmg_percentage/total_percentage;
    float cold_dmg_percentage_mod = cold_dmg_percentage/total_percentage;
    float poison_dmg_percentage_mod = poison_dmg_percentage/total_percentage;
    float energy_dmg_percentage_mod = energy_dmg_percentage/total_percentage;

    // Colors for different dmg types that we want to display on our bar
    vec4 direct_dmg_color = vec4(0.85, 0.44, 0.03, 1.0);
    vec4 phys_dmg_color = vec4(0.06, 0.31, 0.44, 1.0);
    vec4 fire_dmg_color = vec4(0.56, 0.03, 0.03, 1.0);
    vec4 cold_dmg_color = vec4(0.72, 0.72, 0.75, 1.0);
    vec4 poison_dmg_color = vec4(0.0, 0.63, 0.0, 1.0);
    vec4 energy_dmg_color = vec4(0.41, 0.29, 0.52, 1.0);

    // an intensity offset that should give some cool glowy-type effects
    float color_intensity_shifter = 0.0;

    // Take the greyscale intensity of each color
    float direct_dmg_color_intensity = (0.30 * direct_dmg_color.r + 0.59*direct_dmg_color.g + 0.11*direct_dmg_color.b + color_intensity_shifter);
    float phys_dmg_color_intensity = 0.30 * phys_dmg_color.r + 0.59*phys_dmg_color.g + 0.11*phys_dmg_color.b + color_intensity_shifter;
    float fire_dmg_color_intensity = (0.30 * fire_dmg_color.r + 0.59*fire_dmg_color.g + 0.11*fire_dmg_color.b + color_intensity_shifter)*1.1;   // Mult by 1.2 to give a less saturated red
    float cold_dmg_color_intensity = (0.30 * cold_dmg_color.r + 0.59*cold_dmg_color.g + 0.11*cold_dmg_color.b + color_intensity_shifter)/3.0;   // Dividing by 3.0 is req'd to turn the greyscale more white for 'cold'
    float poison_dmg_color_intensity = 0.30 * poison_dmg_color.r + 0.59*poison_dmg_color.g + 0.11*poison_dmg_color.b + color_intensity_shifter;
    float energy_dmg_color_intensity = (0.30 * energy_dmg_color.r + 0.59*energy_dmg_color.g + 0.11*energy_dmg_color.b + color_intensity_shifter)/1.5; // Dividing by 1.5 to lighten it up

    // If it's greyscale, and we have something to draw, draw it
    if (COLOR.r == COLOR.g && COLOR.r == COLOR.b && total_percentage > 0.0)
    {
        float current_percentage = UV.x;
        if (current_percentage <= direct_dmg_percentage_mod)
        {
            // DIRECT DMG
            COLOR = (direct_dmg_color*COLOR/(direct_dmg_color_intensity));
        }else if(current_percentage <= (direct_dmg_percentage_mod+phys_dmg_percentage_mod))
        {
            // PHYS DMG
            COLOR = (phys_dmg_color*COLOR/(phys_dmg_color_intensity));
        }else if(current_percentage <= (direct_dmg_percentage_mod+phys_dmg_percentage_mod+fire_dmg_percentage_mod))
        {
            // FIRE DMG
            COLOR = (fire_dmg_color*COLOR/(fire_dmg_color_intensity));
        }else if(current_percentage <= (direct_dmg_percentage_mod+phys_dmg_percentage_mod+fire_dmg_percentage_mod+cold_dmg_percentage_mod))
        {
            // COLD DMG
            COLOR = (cold_dmg_color*COLOR/(cold_dmg_color_intensity));
        }else if(current_percentage <= (direct_dmg_percentage_mod+phys_dmg_percentage_mod+fire_dmg_percentage_mod+cold_dmg_percentage_mod+poison_dmg_percentage_mod))
        {
            // POISON DMG
            COLOR = (poison_dmg_color*COLOR/(poison_dmg_color_intensity));
        }else if(current_percentage <= (direct_dmg_percentage_mod+phys_dmg_percentage_mod+fire_dmg_percentage_mod+cold_dmg_percentage_mod+poison_dmg_percentage_mod+energy_dmg_percentage_mod))
        {
            // ENERGY DMG
            COLOR = (energy_dmg_color*COLOR/(energy_dmg_color_intensity));
        }
    }
}
