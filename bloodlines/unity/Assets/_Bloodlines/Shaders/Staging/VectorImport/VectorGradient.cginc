float2 UnpackFloat2(fixed4 c)
{
    return float2(c.r * 255 + c.g, c.b * 255 + c.a);
}

float2 RayUnitCircleFirstHit(float2 rayStart, float2 rayDir)
{
    float tca = dot(-rayStart, rayDir);
    float d2 = dot(rayStart, rayStart) - tca * tca;
    float thc = sqrt(1.0f - d2);
    float t0 = tca - thc;
    float t1 = tca + thc;
    float t = min(t0, t1);
    if (t < 0.0f)
    {
        t = max(t0, t1);
    }

    return rayStart + rayDir * t;
}

float RadialAddress(float2 uv, float2 focus)
{
    uv = (uv - float2(0.5f, 0.5f)) * 2.0f;
    float2 pointOnPerimeter = RayUnitCircleFirstHit(focus, normalize(uv - focus));
    float2 diff = pointOnPerimeter - focus;
    if (abs(diff.x) > 0.0001f)
    {
        return (uv.x - focus.x) / diff.x;
    }

    if (abs(diff.y) > 0.0001f)
    {
        return (uv.y - focus.y) / diff.y;
    }

    return 0.0f;
}

fixed4 EvaluateGradient(float settingIndex, float2 uv, sampler2D atlas, float2 texelSize)
{
    float2 settingUV = float2(0.5f, settingIndex + 0.5f) * texelSize;
    fixed4 gradientSettings = tex2D(atlas, settingUV);
    if (gradientSettings.x > 0.0f)
    {
        float2 focus = (gradientSettings.zw - float2(0.5f, 0.5f)) * 2.0f;
        uv = float2(RadialAddress(uv, focus), 0.0);
    }

    int addressing = round(gradientSettings.y * 255);
    uv.x = (addressing == 0) ? fmod(uv.x, 1.0f) : uv.x;
    uv.x = (addressing == 1) ? max(min(uv.x, 1.0f), 0.0f) : uv.x;
    float wrap = fmod(uv.x, 2.0f);
    uv.x = (addressing == 2) ? (wrap > 1.0f ? 1.0f - fmod(wrap, 1.0f) : wrap) : uv.x;

    float2 nextUV = float2(texelSize.x, 0);
    float2 pos = (UnpackFloat2(tex2D(atlas, settingUV + nextUV) * 255) + float2(0.5f, 0.5f)) * texelSize;
    float2 size = UnpackFloat2(tex2D(atlas, settingUV + nextUV * 2) * 255) * texelSize;
    uv = uv * size + pos;

    return tex2D(atlas, uv);
}
