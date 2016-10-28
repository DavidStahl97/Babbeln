#ifndef AUDIO_HANDLER_H
#define AUDIO_HANDLER_H

#include "Common.h"
#include <portaudio.h>

static int StaticAudioCallback(const void *inputBuffer, void *outputBuffer,
	unsigned long framesPerBuffer,
	const PaStreamCallbackTimeInfo* timeInfo,
	PaStreamCallbackFlags statusFlags,
	void *userData);

class AudioHandler
{
public:
	AudioHandler(LockfreeQueue& playingQueue, LockfreeQueue& recordingQueue, SampleBufferPool& pool);
	~AudioHandler();

	void Init();
	void Start();
	void Stop();

private:
	int AudioCallback(const void* inputBuffer, void* outputBuffer,
		unsigned long framesPerBuffer,
		const PaStreamCallbackTimeInfo* timeInfo,
		PaStreamCallbackFlags statusFlags);

private:
	PaStreamParameters	m_InputParamters;
	PaStreamParameters	m_OutputParameters;
	PaStream*			m_AudioStream;

	LockfreeQueue&		m_RecordingQueue;
	LockfreeQueue&		m_PlayingQueue;
	SampleBufferPool&   m_Pool;

	friend static int StaticAudioCallback(const void *inputBuffer, void *outputBuffer,
		unsigned long framesPerBuffer,
		const PaStreamCallbackTimeInfo* timeInfo,
		PaStreamCallbackFlags statusFlags,
		void *userData);
};

#endif

