/*
	based on:

	* This source code is a product of Sun Microsystems, Inc. and is provided
	* for unrestricted use.  Users may copy or modify this source code without
	* charge.
	*
	* SUN SOURCE CODE IS PROVIDED AS IS WITH NO WARRANTIES OF ANY KIND INCLUDING
	* THE WARRANTIES OF DESIGN, MERCHANTIBILITY AND FITNESS FOR A PARTICULAR
	* PURPOSE, OR ARISING FROM A COURSE OF DEALING, USAGE OR TRADE PRACTICE.
	*
	* Sun source code is provided with no support and without any obligation on
	* the part of Sun Microsystems, Inc. to assist in its use, correction,
	* modification or enhancement.
	*
	* SUN MICROSYSTEMS, INC. SHALL HAVE NO LIABILITY WITH RESPECT TO THE
	* INFRINGEMENT OF COPYRIGHTS, TRADE SECRETS OR ANY PATENTS BY THIS SOFTWARE
	* OR ANY PART THEREOF.
	*
	* In no event will Sun Microsystems, Inc. be liable for any lost revenue
	* or profits or other special, indirect and consequential damages, even if
	* Sun has been advised of the possibility of such damages.
	*
	* Sun Microsystems, Inc.
	* 2550 Garcia Avenue
	* Mountain View, California  94043
	
*/

#ifndef G711CODEC_H
#define G711CODEC_H

#include "Common.h"

class G711Codec
{
public:
	G711Codec() = delete;

	static void Encode(const SampleBuffer& pcmValueBuffer, CompressedSampleBuffer& alawBuffer);
	static void Decode(const CompressedSampleBuffer& alawBuffer, SampleBuffer& pcmValueBuffer);

private:
	static CompressedSample PcmValueToAlaw(Sample pcmValue);
	static Sample AlawToPcmValue(CompressedSample alaw);
	static Sample Search(Sample val, Sample* table, Sample size);

private:
	static Sample seg_aend[8];
	static CompressedSample _u2a[128];
	static CompressedSample _a2u[128];
};

#endif