/*
 *  name:       Noise
 *  purpose:    Provide a library for generating noise
 *  details:	This follows a tutorial on creating noise:
 *				https://catlikecoding.com/unity/tutorials/noise/
 *				Use this noise api via Noise.noiseMethods[Value or Perlin][1-3](Vector3 point, float frequency)
 */
using UnityEngine;

// Delegate to hold noise generation methods
public delegate float NoiseMethod(Vector3 point, float frequency);

// Enum to be used for indices in the noiseMethods array
public enum NoiseMethodType {
	Value,
	Perlin
}

public static class Noise
{
	public static NoiseMethod[] valueMethods =
	{
		Value1D,
		Value2D,
		Value3D
	};

	public static NoiseMethod[] perlinMethods =
	{
		Perlin1D,
		Perlin2D,
		Perlin3D
	};

	public static NoiseMethod[][] noiseMethods =
	{
		valueMethods,
		perlinMethods
	};

	// Hash function
	static int[] hash = {
		151,160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140,
		36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234,
		75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237,
		149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
		27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105,
		92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73,
		209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
		164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
		147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
		28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
		101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224,
		232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144,
		12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214,
		31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150,
		254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66,
		215, 61, 156, 180
	};

	// Possible gradients the Perlin noise functions (all normalized)
	static int[] gradients1D = { -1, 1 };

	static Vector2[] gradients2D =
	{
		new Vector2(1f, 0f),
		new Vector2(-1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(0f, -1f),
		new Vector2(1f, 1f).normalized,
		new Vector2(1f, -1f).normalized,
		new Vector2(-1f, 1f).normalized,
		new Vector2(-1f, -1f).normalized
	};

	private static Vector3[] gradients3D = {
		new Vector3( 1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3( 1f,-1f, 0f),
		new Vector3(-1f,-1f, 0f),
		new Vector3( 1f, 0f, 1f),
		new Vector3(-1f, 0f, 1f),
		new Vector3( 1f, 0f,-1f),
		new Vector3(-1f, 0f,-1f),
		new Vector3( 0f, 1f, 1f),
		new Vector3( 0f,-1f, 1f),
		new Vector3( 0f, 1f,-1f),
		new Vector3( 0f,-1f,-1f),
		new Vector3( 1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3( 0f,-1f, 1f),
		new Vector3( 0f,-1f,-1f)
	};

	private const int gradientsMask3D = 15;

	// Size of the key space and the range of values output from the function
	const int hashMask = 255;

	// Size of the gradient space for Perlin Noise 1D function
	const int gradientsMask1D = 1;
	const int gradientsMask2D = 7;

	// Needed for the perlin noise generation methods
	static float sqrt2 = Mathf.Sqrt(2);

	// Create a fractal noice pattern using the noise generation method provided and the same
	// generation method at double the frequency and half the amplitude
	public static float Sum(NoiseMethod method, Vector3 point, float frequency, int octaves, float freqChange, float ampChange)
    {
		float sum = method(point, frequency);

		float amplitude = 1f;
		float total = 1f;

		for (int i = 1; i < octaves; i++)
		{
			frequency *= freqChange;
			amplitude *= ampChange;
			total += amplitude;
			point *= freqChange;
			sum += method(point, frequency) * amplitude;
		}

		return sum / total;
	}

	public static float Value1D(Vector3 point, float frequency)
    {
        // Number of lattice points per 1 unit in world space
        point *= frequency;

        // Convert world floating point value to an integer
        int i0 = Mathf.FloorToInt(point.x);

		// Distance from the left coordinate to the sample point
		float dt = point.x - i0;

        // Find coordinate using 8 bit mask. Lattice coordinates are 0 - 255, repeating
        i0 &= hashMask;

		// The next coordinate in the lattice
		int i1 = (i0 + 1) & hashMask;


		// Hash values of each of the two hash coordinates
        int h0 = hash[i0];
		int h1 = hash[i1];

		// Interpolate the hash values and normalize them so they span a range of 0 to 1
		return Mathf.Lerp(h0, h1, Smooth(dt)) * (1f / hashMask);
	}

	public static float Value2D(Vector3 point, float frequency)
    {
		// Number of lattice points per 1 unit in world space
		point *= frequency;

		// Convert world coordinates x and y components to integers
		int ix0 = Mathf.FloorToInt(point.x);
		int iy0 = Mathf.FloorToInt(point.y);

		// Find distance from current point to previous lattice point
		float dx = point.x - ix0;
		float dy = point.y - iy0;

		// Find coordinate using 8 bit mask. Lattice coordinates are 0 - 255, repeating
		ix0 &= hashMask;
		iy0 &= hashMask;

		// Coordinates of next lattice points
		int ix1 = (ix0 + 1) & hashMask;
		int iy1 = (iy0 + 1) & hashMask;

		// Determine the hashes for each coordinate; 2^n for each dimension; 
		int h0 = hash[ix0];
		int h1 = hash[ix1];
		int h00 = hash[(h0 + iy0) & hashMask];
		int h01 = hash[(h0 + iy1) & hashMask];
		int h10 = hash[(h1 + iy0) & hashMask];
		int h11 = hash[(h1 + iy1) & hashMask];

		// Interpolate the hash values and normalize them so they span a range of 0 to 1
		return Mathf.Lerp(Mathf.Lerp(h00, h10, Smooth(dx)), Mathf.Lerp(h01, h11, Smooth(dx)), Smooth(dy)) * (1f / hashMask);
	}

	public static float Value3D(Vector3 point, float frequency)
	{
		// Number of lattice points per 1 unit in world space
		point *= frequency;

		// Convert world floating point value to an integer
		int ix0 = Mathf.FloorToInt(point.x);
		int iy0 = Mathf.FloorToInt(point.y);
		int iz0 = Mathf.FloorToInt(point.z);

		// Find distance from current point to previous lattice point
		float dx = point.x - ix0;
		float dy = point.y - iy0;
		float dz = point.z - iz0;

		// Coordinates of next lattice points
		int ix1 = (ix0 + 1) & hashMask;
		int iy1 = (iy0 + 1) & hashMask;
		int iz1 = (iz0 + 1) & hashMask;

		// Find coordinate using 8 bit mask. Lattice coordinates are 0 - 255, repeating
		ix0 &= hashMask;
		iy0 &= hashMask;
		iz0 &= hashMask;

		// Calculate hash values for each of the coordinates on the lattice (in 3 dimensions)
		int h0 = hash[ix0];
		int h1 = hash[ix1];
		int h00 = hash[(h0 + iy0) & hashMask];
		int h01 = hash[(h0 + iy1) & hashMask];
		int h10 = hash[(h1 + iy0) & hashMask];
		int h11 = hash[(h1 + iy1) & hashMask];
		int h000 = hash[(h00 + iz0) & hashMask];
		int h001 = hash[(h00 + iz1) & hashMask];
		int h010 = hash[(h01 + iz0) & hashMask];
		int h011 = hash[(h01 + iz1) & hashMask];
		int h100 = hash[(h10 + iz0) & hashMask];
		int h101 = hash[(h10 + iz1) & hashMask];
		int h110 = hash[(h11 + iz0) & hashMask];
		int h111 = hash[(h11 + iz1) & hashMask];

		// Normalize hash values so they span a range of 0 to 1
		return Mathf.Lerp(
			Mathf.Lerp(
				Mathf.Lerp(h000, h100, Smooth(dx)),
				Mathf.Lerp(h010, h110, Smooth(dx)),
				Smooth(dy)
				),
			Mathf.Lerp(
				Mathf.Lerp(h001, h101, Smooth(dx)),
				Mathf.Lerp(h011, h111, Smooth(dx)),
				Smooth(dy)
				),
			Smooth(dz)
			) * (1f / hashMask);
	}

	public static float Perlin1D(Vector3 point, float frequency)
	{
		// Number of lattice points per 1 unit in world space
		point *= frequency;

		// Convert world floating point value to an integer
		int i0 = Mathf.FloorToInt(point.x);

		// Distance from the left coordinate to the sample point
		float dt0 = point.x - i0;

		// Distance from right coordinate to the sample point (negative number)
		float dt1 = dt0 - 1f;

		// Find coordinate using 8 bit mask. Lattice coordinates are 0 - 255, repeating
		i0 &= hashMask;

		// The next coordinate in the lattice
		int i1 = (i0 + 1) & hashMask;


		// Gradient values of each of the two lattice coordinates, determined by the hash function
		int g0 = gradients1D[hash[i0] & gradientsMask1D];
		int g1 = gradients1D[hash[i1] & gradientsMask1D];

		// Values of each gradient at the current position, like a 1D dot product
		float v0 = g0 * dt0;
		float v1 = g1 * dt1;

		// Interpolate the dot products and adjust so the result is in a range [0, 1]
		return Mathf.Lerp(v0, v1, Smooth(dt0)) * 2f;
	}

	public static float Perlin2D(Vector3 point, float frequency)
	{
		// Number of lattice points per 1 unit in world space
		point *= frequency;

		// Convert world coordinates x and y components to integers
		int ix0 = Mathf.FloorToInt(point.x);
		int iy0 = Mathf.FloorToInt(point.y);

		// Find distance from current point to previous and next lattice points
		float dx0 = point.x - ix0;
		float dy0 = point.y - iy0;
		float dx1 = dx0 - 1f;
		float dy1 = dy0 - 1f;

		// Find coordinate using 8 bit mask. Lattice coordinates are 0 - 255, repeating
		ix0 &= hashMask;
		iy0 &= hashMask;

		// Coordinates of next lattice points
		int ix1 = (ix0 + 1) & hashMask;
		int iy1 = (iy0 + 1) & hashMask;

		// Determine the hashes for each coordinate; 2^n for each dimension; 
		int h0 = hash[ix0];
		int h1 = hash[ix1];

		Vector2 g00 = gradients2D[hash[(h0 + iy0) & hashMask] & gradientsMask2D];
		Vector2 g01 = gradients2D[hash[(h0 + iy1) & hashMask] & gradientsMask2D];
		Vector2 g10 = gradients2D[hash[(h1 + iy0) & hashMask] & gradientsMask2D];
		Vector2 g11 = gradients2D[hash[(h1 + iy1) & hashMask] & gradientsMask2D];

		// Calculate dot products of each gradient and the 
		float v00 = DotProd2(g00, new Vector2(dx0, dy0));
		float v10 = DotProd2(g10, new Vector2(dx1, dy0));
		float v01 = DotProd2(g01, new Vector2(dx0, dy1));
		float v11 = DotProd2(g11, new Vector2(dx1, dy1));

		// Interpolate the dot products and ensure output is in range [0, 1]
		return Mathf.Lerp(Mathf.Lerp(v00, v10, Smooth(dx0)), Mathf.Lerp(v01, v11, Smooth(dx0)), Smooth(dy0)) * sqrt2; ;
	}

	public static float Perlin3D(Vector3 point, float frequency)
	{
		// Number of lattice points per 1 unit in world space
		point *= frequency;

		// Convert world floating point value to an integer
		int ix0 = Mathf.FloorToInt(point.x);
		int iy0 = Mathf.FloorToInt(point.y);
		int iz0 = Mathf.FloorToInt(point.z);

		// Find distance from current point to previous lattice point
		float dx0 = point.x - ix0;
		float dy0 = point.y - iy0;
		float dz0 = point.z - iz0;
		float dx1 = dx0 - 1f;
		float dy1 = dy0 - 1f;
		float dz1 = dz0 - 1f;

		// Coordinates of next lattice points
		int ix1 = (ix0 + 1) & hashMask;
		int iy1 = (iy0 + 1) & hashMask;
		int iz1 = (iz0 + 1) & hashMask;

		// Find coordinate using 8 bit mask. Lattice coordinates are 0 - 255, repeating
		ix0 &= hashMask;
		iy0 &= hashMask;
		iz0 &= hashMask;

		// Calculate hash values to be used in determining the gradients
		int h0 = hash[ix0];
		int h1 = hash[ix1];
		int h00 = hash[(h0 + iy0) & hashMask];
		int h01 = hash[(h0 + iy1) & hashMask];
		int h10 = hash[(h1 + iy0) & hashMask];
		int h11 = hash[(h1 + iy1) & hashMask];

		// Calculate the gradients
		Vector3 g000 = gradients3D[hash[(h00 + iz0) & hashMask] & gradientsMask3D];
		Vector3 g001 = gradients3D[hash[(h00 + iz1) & hashMask] & gradientsMask3D];
		Vector3 g010 = gradients3D[hash[(h01 + iz0) & hashMask] & gradientsMask3D];
		Vector3 g011 = gradients3D[hash[(h01 + iz1) & hashMask] & gradientsMask3D];
		Vector3 g100 = gradients3D[hash[(h10 + iz0) & hashMask] & gradientsMask3D];
		Vector3 g101 = gradients3D[hash[(h10 + iz1) & hashMask] & gradientsMask3D];
		Vector3 g110 = gradients3D[hash[(h11 + iz0) & hashMask] & gradientsMask3D];
		Vector3 g111 = gradients3D[hash[(h11 + iz1) & hashMask] & gradientsMask3D];

		// Find the dot product of the gradient and the current point
		float v000 = DotProd3(g000, new Vector3(dx0, dy0, dz0));
		float v100 = DotProd3(g100, new Vector3(dx1, dy0, dz0));
		float v010 = DotProd3(g010, new Vector3(dx0, dy1, dz0));
		float v110 = DotProd3(g110, new Vector3(dx1, dy1, dz0));
		float v001 = DotProd3(g001, new Vector3(dx0, dy0, dz1));
		float v101 = DotProd3(g101, new Vector3(dx1, dy0, dz1));
		float v011 = DotProd3(g011, new Vector3(dx0, dy1, dz1));
		float v111 = DotProd3(g111, new Vector3(dx1, dy1, dz1));


		// Interpolate dot products
		return Mathf.Lerp(
			Mathf.Lerp(Mathf.Lerp(v000, v100, Smooth(dx0)), Mathf.Lerp(v010, v110, Smooth(dx0)), Smooth(dy0)),
			Mathf.Lerp(Mathf.Lerp(v001, v101, Smooth(dx0)), Mathf.Lerp(v011, v111, Smooth(dx0)), Smooth(dy0)),
			Smooth(dz0));
	}

	static float Smooth(float t)
    {
		return t * t * t * (t * (6f * t - 15f) + 10f);
    }

	static float DotProd2(Vector2 u, Vector2 v)
    {
		return u.x * v.x + u.y * v.y;
    }

	static float DotProd3(Vector3 u, Vector3 v)
    {
		return u.x * v.x + u.y * v.y + u.z * v.z;
    }
}
