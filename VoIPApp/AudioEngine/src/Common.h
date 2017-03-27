#ifndef COMMON_H
#define COMMON_H

#define SAMPLE_RATE 8000
#define NUMBER_OF_CHANNELS 1
#define PA_SAMPLE_TYPE paInt16
#define FRAMES_PER_BUFFER 128
#define SAMPLE_SILENCE 0

#define LOG(x) {std::cout << x << std::endl;}


#include <boost\lockfree\spsc_queue.hpp>
#include <boost\lockfree\stack.hpp>
#include <boost\array.hpp>
#include <cstdint>
#include <iostream>


typedef short Sample;
typedef unsigned char CompressedSample;

typedef boost::array<Sample, FRAMES_PER_BUFFER> SampleBuffer;

typedef boost::array<CompressedSample, FRAMES_PER_BUFFER> CompressedSampleBuffer;

typedef boost::lockfree::spsc_queue<SampleBuffer*, boost::lockfree::capacity<64>> LockfreeQueue;

typedef boost::lockfree::stack<SampleBuffer*, boost::lockfree::capacity<64>> SampleBufferPool;

typedef int VoIPError;
enum VoIPErrorCode
{
	VoIP_NoError = 0,

	VoIP_NoInputDevice = -1000,
	VoIP_NoOutputDevice
};

#endif
