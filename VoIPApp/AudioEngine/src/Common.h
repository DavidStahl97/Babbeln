#ifndef COMMON_H
#define COMMON_H

#define FRAMES_PER_BUFFER 128
#define SAMPLE_RATE 8000
#define NUMBER_OF_CHANNELS 1
#define PA_SAMPLE_TYPE paInt16;
#define COMPRESSED_SAMPLE_TYPE paInt8
#define SAMPLE_SILENCE 0.0f

#if defined(DEUBG) | defined (_DEBUG)
#define LOG(x) {std::cout << x << std::endl;}
#else
#define LOG(x)
#endif

#include <boost\lockfree\spsc_queue.hpp>
#include <boost\array.hpp>
#include <boost\pool\object_pool.hpp>
#include <cstdint>

typedef short Sample;
typedef unsigned char CompressedSample;
typedef boost::array<Sample, FRAMES_PER_BUFFER> SampleBuffer;
typedef boost::array<CompressedSample, FRAMES_PER_BUFFER> CompressedSampleBuffer;
typedef boost::lockfree::spsc_queue<SampleBuffer*, boost::lockfree::capacity<64>> LockfreeQueue;
typedef boost::object_pool<SampleBuffer> SampleBufferPool;
typedef boost::object_pool<CompressedSampleBuffer> CompressedSampleBufferPool;

#endif
