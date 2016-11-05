#include "G711Codec.h"

#define	SIGN_BIT	(0x80)		/* Sign bit for a A-law byte. */
#define	QUANT_MASK	(0xf)		/* Quantization field mask. */
#define	NSEGS		(8)		/* Number of A-law segments. */
#define	SEG_SHIFT	(4)		/* Left shift for segment number. */
#define	SEG_MASK	(0x70)		/* Segment field mask. */

Sample G711Codec::seg_aend[8] = { 0x1F, 0x3F, 0x7F, 0xFF,
0x1FF, 0x3FF, 0x7FF, 0xFFF };

CompressedSample G711Codec::_u2a[128] = {			
	1,	1,	2,	2,	3,	3,	4,	4,
	5,	5,	6,	6,	7,	7,	8,	8,
	9,	10,	11,	12,	13,	14,	15,	16,
	17,	18,	19,	20,	21,	22,	23,	24,
	25,	27,	29,	31,	33,	34,	35,	36,
	37,	38,	39,	40,	41,	42,	43,	44,
	46,	48,	49,	50,	51,	52,	53,	54,
	55,	56,	57,	58,	59,	60,	61,	62,
	64,	65,	66,	67,	68,	69,	70,	71,
	72,	73,	74,	75,	76,	77,	78,	79,
	80,	82,	83,	84,	85,	86,	87,	88,
	89,	90,	91,	92,	93,	94,	95,	96,
	97,	98,	99,	100,	101,	102,	103,	104,
	105,	106,	107,	108,	109,	110,	111,	112,
	113,	114,	115,	116,	117,	118,	119,	120,
	121,	122,	123,	124,	125,	126,	127,	128 };

CompressedSample G711Codec::_a2u[128] = {			
	1,	3,	5,	7,	9,	11,	13,	15,
	16,	17,	18,	19,	20,	21,	22,	23,
	24,	25,	26,	27,	28,	29,	30,	31,
	32,	32,	33,	33,	34,	34,	35,	35,
	36,	37,	38,	39,	40,	41,	42,	43,
	44,	45,	46,	47,	48,	48,	49,	49,
	50,	51,	52,	53,	54,	55,	56,	57,
	58,	59,	60,	61,	62,	63,	64,	64,
	65,	66,	67,	68,	69,	70,	71,	72,
	73,	74,	75,	76,	77,	78,	79,	80,
	80,	81,	82,	83,	84,	85,	86,	87,
	88,	89,	90,	91,	92,	93,	94,	95,
	96,	97,	98,	99,	100,	101,	102,	103,
	104,	105,	106,	107,	108,	109,	110,	111,
	112,	113,	114,	115,	116,	117,	118,	119,
	120,	121,	122,	123,	124,	125,	126,	127 };

Sample G711Codec::Search(Sample val, Sample* table, Sample size) 
{
	for (Sample i = 0; i < size; i++) 
	{
		if (val <= *table++)
		{
			return (i);
		}
	}
	return (size);
}

CompressedSample G711Codec::PcmValueToAlaw(Sample pcmValue)
{
	Sample mask;
	Sample seg;
	CompressedSample aval;

	pcmValue = pcmValue >> 3;

	if (pcmValue >= 0) 
	{
		mask = 0xD5;		/* sign (7th) bit = 1 */
	}
	else 
	{
		mask = 0x55;		/* sign bit = 0 */
		pcmValue = -pcmValue - 1;
	}

	/* Convert the scaled magnitude to segment number. */
	seg = Search(pcmValue, seg_aend, 8);

	/* Combine the sign, segment, and quantization bits. */

	if (seg >= 8)		/* out of range, return maximum value. */
	{
		return (CompressedSample)(0x7F ^ mask);
	}
	else 
	{
		aval = (CompressedSample)seg << SEG_SHIFT;
		if (seg < 2)
			aval |= (pcmValue >> 1) & QUANT_MASK;
		else
			aval |= (pcmValue >> seg) & QUANT_MASK;
		return (aval ^ mask);
	}
}

Sample G711Codec::AlawToPcmValue(CompressedSample alaw)
{
	Sample t;
	Sample seg;

	alaw ^= 0x55;

	t = (alaw & QUANT_MASK) << 4;
	seg = ((unsigned)alaw & SEG_MASK) >> SEG_SHIFT;
	switch (seg) {
	case 0:
		t += 8;
		break;
	case 1:
		t += 0x108;
		break;
	default:
		t += 0x108;
		t <<= seg - 1;
	}
	return ((alaw & SIGN_BIT) ? t : -t);
}

void G711Codec::Encode(const SampleBuffer& pcmValueBuffer, CompressedSampleBuffer& compressedBuffer)
{
	for (int i = 0; i < FRAMES_PER_BUFFER; ++i)
	{
		compressedBuffer[i] = PcmValueToAlaw(pcmValueBuffer[i]);
	}
}

void G711Codec::Decode(const CompressedSampleBuffer& compressedBuffer, SampleBuffer& pcmValueBuffer)
{
	for (int i = 0; i < FRAMES_PER_BUFFER; ++i)
	{
		pcmValueBuffer[i] = AlawToPcmValue(compressedBuffer[i]);
	}
}