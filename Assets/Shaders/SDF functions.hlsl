void RectangleSDF_float(float2 UV, float2 Size, out float Dist){
    
    float2 d =  abs(UV) - Size;
    Dist = length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

void InlineSDF_float(float Distance, float Thickness, out float Dist){
	
	Dist = max(Distance, -Distance - Thickness);
}