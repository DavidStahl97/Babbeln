#include "G711Codec.h"

#define	SIGN_BIT	(0x80)		// 1000 0000
#define	QUANT_MASK	(0xf)		// 0000 1111
#define	SEG_MASK	(0x70)		// 0111 0000
#define	SEG_SHIFT	(4)			

Sample G711Codec::seg_aend[8] = { 0x1F, 0x3F, 0x7F, 0xFF,
0x1FF, 0x3FF, 0x7FF, 0xFFF };

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
		mask = 0xD5;		//Sign-Bit auf 1
	}
	else 
	{
		mask = 0x55;		//Sign-Bit auf 0
		pcmValue = -pcmValue - 1;
	}

	// suchen des Segmentindexes
	seg = Search(pcmValue, seg_aend, 8);

	
	if (seg >= 8)		
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