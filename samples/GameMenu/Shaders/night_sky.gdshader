shader_type canvas_item;
render_mode unshaded;

const float LAYER_COUNT = 8.0;

mat2 rotation(float angle) {
	float s = sin(angle);
	float c = cos(angle);
	return mat2(vec2(c, -s), vec2(s, c));
}

float sdStar(vec2 uv, float size) {
	return size / pow(length(uv), 1.5);
}

float rand(vec2 p) {
	return fract(sin(dot(p, vec2(12.9898, 4.1414))) * 43758.5453);
}

float sdStarSmoothed(vec2 uv, float starSize) {
	float result = sdStar(uv, starSize);
	result *= smoothstep(0.0, 0.1, result); // reduce light spread
	result *= smoothstep(0.0, 0.01, length(uv)); // blur a bit to avoid small stars flickering
	return result;
}

vec3 starLayer(vec2 uv) {
	vec2 id = floor(uv);
	uv = fract(uv) - 0.5;

	vec3 col = vec3(0.0);

	for (int y = -1; y <= 1; ++y) {
		for (int x = -1; x <= 1; ++x) {
			vec2 offset = vec2(float(x), float(y));
			float randomPos = rand(id + offset);
			float size = fract(randomPos * 342.487);
			float flareStrength = smoothstep(0.7, 1.0, size) * fract(randomPos * 861.4) * 0.25;
			float star = sdStarSmoothed(uv - offset - vec2(randomPos - 0.5, fract(randomPos * 17.0) - 0.5), 1.0 / 512.0);

			vec3 starColor = sin(vec3(0.1, 0.0, 0.9) * fract(randomPos * 7943.45) * PI * 1.4) * 0.5 + 0.5;
			starColor += 0.5;

			star *= sin(TIME + randomPos * PI * 2.0) * 0.5 + 0.7;

			col += star * size * starColor;
		}
	}

	return col;
}

void fragment() {
	vec2 uv = (FRAGCOORD.xy - 0.5 / SCREEN_PIXEL_SIZE) * SCREEN_PIXEL_SIZE.y;
	float time = TIME * 0.075;

	vec3 col = vec3(0.0);

	for (float i = 0.1; i <= 1.0; i += 1.0 / LAYER_COUNT) {
		float depth = fract(i + time);
		float scale = mix(20.0, 4.0, depth);
		float fade = depth * smoothstep(1., .9, depth);
		col += starLayer(uv * scale + i * 423.4) * fade;
	}

	COLOR = vec4(col, 1.0);
}
