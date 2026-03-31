shader_type canvas_item;

uniform float intensity = 2.0;
uniform vec4 glow_color : source_color = vec4(0.0, 1.0, 1.0, 1.0);

void fragment() {
    vec4 base = vec4(0.0, 1.0, 1.0, 1.0);

    vec3 glow = glow_color.rgb * intensity;

    vec3 final_color = base.rgb + glow * 0.5;

    COLOR = vec4(final_color, 1.0);
}