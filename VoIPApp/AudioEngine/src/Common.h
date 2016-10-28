#ifndef COMMON_H
#define COMMON_H

#define FRAMES_PER_BUFFER 128
#define SAMPLE_RATE 8000
#define NUMBER_OF_CHANNELS 1
#define PA_SAMPLE_TYPE paFloat32
#define SAMPLE_SILENCE 0.0f

#include <boost\array.hpp>
#include <boost\lockfree\spsc_queue.hpp>
#include <boost\pool\object_pool.hpp>

typedef float Sample;
typedef boost::array<Sample, FRAMES_PER_BUFFER> SampleBuffer;
typedef boost::lockfree::spsc_queue<SampleBuffer*, boost::lockfree::capacity<64>> LockfreeQueue;
typedef boost::object_pool<SampleBuffer> SampleBufferPool;

#endif
