// HLSL port of a Palette Cycle shader made for BIRP that I found a while back (trust me it's way easier making one like this in shader code than Shader Graph)
// I wouldn't say this is the most efficient or best way to do it.
// You'd probably be better off using a screen shader for full palette swaps (like how Orbinaut Framework does it for example), but eh, I got lazy
// It would be nice if I could remember who made this shader 4 years ago...

void PaletteCycle_float(UnityTexture2D mainTex, UnityTexture2D colorMap, float2 colorMapUV, float2 colorMapSize, bool doLerp, float cycleSpeed, float threshold, float time, out float4 Out)
{
    uint maxPaletteLength = 64;

    uint colorMapWidth = colorMapSize.x, colorMapHeight = colorMapSize.y;
    float rowHeight = 1.0 / (float)colorMapHeight;
    uint cycleLength = colorMapHeight - 1;

    float cyclePositionRaw = fmod(time * cycleSpeed, cycleLength);
    uint cyclePosition = floor(cyclePositionRaw);
    uint cyclePositionNext = (cyclePosition) % cycleLength;
    float4 outputCol = tex2D(mainTex, colorMapUV);
    float4 colorResultNext;
    
    for (uint index = 0; index < maxPaletteLength; index++) {
        float4 colMapIndex = tex2D(colorMap, float2((float)index / colorMapWidth, 1));
        if (all(abs(int4(outputCol * 255).rgb - int4(colMapIndex * 255).rgb) <= threshold))
        {
            float4 colorResult = tex2D(colorMap, float2((float)index / colorMapWidth, 1 - rowHeight - (rowHeight * cyclePosition)));
            
            colorResult = float4(
                colorResult.r,
                colorResult.g,
                colorResult.b,
                outputCol.a
            );

            if (!doLerp)
            {
                Out = colorResult;
                return;
            }

            colorResultNext = tex2D(
                colorMap,
                float2(
                    (float)index / colorMapWidth,
                    1 - rowHeight - (rowHeight * cyclePositionNext)
                )
            );

            colorResultNext = float4(
                colorResultNext.r,
                colorResultNext.g,
                colorResultNext.b,
                outputCol.a
            );

            float lerpAmt = cyclePositionRaw - cyclePosition;

            outputCol = lerp(
                colorResult,
                colorResultNext,
                lerpAmt
            );
        }
    }
    Out = outputCol;
}
