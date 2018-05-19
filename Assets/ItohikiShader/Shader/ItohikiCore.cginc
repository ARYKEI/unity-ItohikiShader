float _Distance;
float _Amount;
float _Catenary;
float4x4 _DelayedMat;  
float _DelayEnable;   

void ItohikiVert(inout appdata_full v)
{
	float center = saturate(1-abs(v.texcoord.y - 0.5)*2);

	// Adjust the thickness
	_Amount = 1-_Amount;
	float radius = length(v.vertex.xy);	
	v.vertex.xyz -= v.normal * radius * saturate(_Amount * center + _Amount);

	// Transform vertex to worldspace
	float3 wpos = mul(unity_ObjectToWorld,v.vertex).xyz;
	float3 wposDelayed = mul(_DelayedMat,v.vertex).xyz;

	// Delay vertices near the center
	if(_DelayEnable > 0)
		wpos = lerp(wpos,wposDelayed,center);

	// Calculate the catenary curve
	float x = (v.texcoord.y - 0.5) * _Distance;
	float a = _Catenary;
	wpos.y += cosh(x/a);
	wpos.y -= cosh((0.5*_Distance)/a);	// Adjust so that both sides are zero in height

	// Return vertex to objectspace
	v.vertex.xyz = mul(unity_WorldToObject,float4(wpos,1)).xyz;
}