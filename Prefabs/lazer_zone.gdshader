shader_type canvas_item;

uniform float time_scale : hint_range(1.0, 8.0) = 4.5;
uniform float crackle_amount : hint_range(0.5, 4.0) = 2.2;
uniform float brightness : hint_range(0.5, 3.0) = 1.7;
uniform vec4 electric_color : source_color = vec4(0.571, 0.05, 0.05, 1.0);
uniform float alpha : hint_range(0.0, 1.0) = .0;

void fragment() {
    vec2 uv = SCREEN_UV * 12.0; // higher = more detail
    
    // Layered noise for jagged electricity
    float n1 = fract(sin(dot(uv, vec2(12.9898, 78.233)) + TIME * time_scale) * 43758.5453);
    float n2 = fract(sin(dot(uv * 2.3, vec2(12.9898, 78.233)) + TIME * time_scale * 1.7) * 43758.5453);
    
    float electricity = max(n1, n2);
    electricity = pow(electricity, 3.5) * crackle_amount;
    
    // Add some wavy arcs
    electricity += sin(uv.y * 25.0 - TIME * 12.0) * 0.2;
    
    float alphaResult = electric_color.a * (electricity * 2.5 + 0.25) * alpha;
    
    COLOR = vec4(electric_color.rgb * (electricity * brightness + 0.6), alphaResult);
}