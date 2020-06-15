
#define PI 3.14159
#define TAU 6.28318

float4x4 rotationMatrix(float3 axis, float angle) {
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    return float4x4(oc*axis.x*axis.x + c, oc*axis.x*axis.y - axis.z*s, oc*axis.z*axis.x + axis.y*s, 0.0,
                oc*axis.x*axis.y + axis.z*s, oc*axis.y*axis.y + c, oc*axis.y*axis.z - axis.x*s, 0.0,
                oc*axis.z*axis.x - axis.y*s, oc*axis.y*axis.z + axis.x*s, oc*axis.z*axis.z + c, 0.0,
                0.0, 0.0, 0.0, 1.0);
}

void rotation (in out float2 p, float a) {
  float c=cos(a),s=sin(a);
  p = mul(float2x2(c,-s,s,c), p);
}

// http://stackoverflow.com/questions/12964279/whats-the-origin-of-this-glsl-rand-one-liner
float random(float2 co)
{
  return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
}

// hash based 3d value noise
// function taken from https://www.shadertoy.com/view/XslGRr
// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// ported from GLSL to HLSL
float hash( float n )
{
  return frac(sin(n)*43758.5453);
}

float noise( float3 x )
{
  // The noise function returns a value in the range -1.0f -> 1.0f
  float3 p = floor(x);
  float3 f = frac(x);
  f       = f*f*(3.0-2.0*f);
  float n = p.x + p.y*57.0 + 113.0*p.z;
  return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
  lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
}

// 3D Noise based on map
float noise3D (float3 x, sampler2D map) {
  float3 i = floor(x);
  float3 f = frac(x);
  f = f*f*(3.0-2.0*f);
  float2 uv = i.xy + float2(37,17)*i.z;
  float2 rgA = tex2Dlod( map, float4((uv+float2(0,0))/255.0, 0, 0 )).yx;
  float2 rgB = tex2Dlod( map, float4((uv+float2(1,0))/255.0, 0, 0 )).yx;
  float2 rgC = tex2Dlod( map, float4((uv+float2(0,1))/255.0, 0, 0 )).yx;
  float2 rgD = tex2Dlod( map, float4((uv+float2(1,1))/255.0, 0, 0 )).yx;
  float2 rg = lerp( lerp( rgA, rgB, f.x ), lerp( rgC, rgD, f.x ), f.y );
  return lerp( rg.x, rg.y, f.z );
}

// Sam Hocevar
float3 rgb2hsv(float3 c)
{
  float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
  float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
  float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

  float d = q.x - min(q.w, q.y);
  float e = 1.0e-10;
  return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Sam Hocevar
float3 hsv2rgb(float3 c)
{
  float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
  return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
