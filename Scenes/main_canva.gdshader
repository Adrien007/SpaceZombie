shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform float intensity = 1.5;
uniform float threshold = 0.6;

void fragment() {
    vec4 col = texture(SCREEN_TEXTURE, SCREEN_UV);

    float brightness = max(col.r, max(col.g, col.b));

    if (brightness > threshold) {
        COLOR = col * intensity;
    } else {
        COLOR = col;
    }
}