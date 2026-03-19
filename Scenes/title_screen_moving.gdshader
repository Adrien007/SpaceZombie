shader_type canvas_item;

uniform float speed = 0.1;
uniform float scroll_duration = 3.0;
uniform float pause_duration = 0.5;

// Ease in-out
float ease_inout(float t) {
    return t * t * (3.0 - 2.0 * t); // smoothstep
}

void vertex() {
    float cycle = scroll_duration * 2.0 + pause_duration * 2.0;
    float t = mod(TIME, cycle);

    float offset = 0.0;
    float max_offset = scroll_duration * speed;

    if (t < scroll_duration) {
        float progress = t / scroll_duration;
        offset = ease_inout(progress) * max_offset;

    } else if (t < scroll_duration + pause_duration) {
        // Pause
        offset = max_offset;

    } else if (t < scroll_duration * 2.0 + pause_duration) {
        float t2 = t - (scroll_duration + pause_duration);
        float progress = t2 / scroll_duration;
        offset = (1.0 - ease_inout(progress)) * max_offset;

    } else {
        // Pause
        offset = 0.0;
    }

    UV.x -= offset;
}