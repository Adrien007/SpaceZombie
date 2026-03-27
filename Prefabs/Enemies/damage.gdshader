shader_type canvas_item;

uniform vec4 hit_color : source_color = vec4(0.80, 0.90, 1.0, 1.0);
uniform float hit_amount : hint_range(0.0, 1.0) = 0.0;

uniform vec4 attack_color : source_color = vec4(1.0, 0.149, 0.149, 1.0);
uniform float attack_amount : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
    
    vec3 color = tex.rgb;
     // Hit (bleu) par-dessus
    color = mix(color, hit_color.rgb, hit_amount);
    
    // Attaque (rouge)
    color = mix(color, attack_color.rgb, attack_amount);
    
    COLOR = vec4(color, tex.a) * COLOR;
    COLOR.rgb = color;
}
