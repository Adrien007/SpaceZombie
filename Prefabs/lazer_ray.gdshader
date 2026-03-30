shader_type canvas_item;

void fragment()
{
    float dist = abs(UV.y - 0.5);

    float alpha = smoothstep(0.5, 0.0, dist);
    
    COLOR = vec4(1.0, 0.4, 0.0, alpha); 
}