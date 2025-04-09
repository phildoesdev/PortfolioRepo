shader_type canvas_item;
uniform bool highlight = false;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
    if (highlight && COLOR.a == 1.0){
        COLOR.g = 0.3;
    }
    // COLOR = vec4(UV, 0.5, 1.0);
    //if (COLOR.a == 255){
        //COLOR = vec4(0.4, 0.6, 0.9, 1.0);
    //}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
