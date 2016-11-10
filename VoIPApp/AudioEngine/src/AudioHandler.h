#ifndef AUDIO_HANDLER_H
#define AUDIO_HANDLER_H

#include "Common.h"
#include <portaudio.h>
#include <string>
#include <vector>

static int StaticAudioCallback(const void *inputBuffer, void *outputBuffer,
	unsigned long framesPerBuffer,
	const PaStreamCallbackTimeInfo* timeInfo,
	PaStreamCallbackFlags statusFlags,
	void *userData);

enum DeviceType
{
	InputDevice,
	OutputDevice
};

class AudioHandler
{
public:
	AudioHandler(LockfreeQueue& playingQueue, LockfreeQueue& recordingQueue, SampleBufferPool& pool);
	~AudioHandler();

	void Init();
	VoIPError StartAsync();
	void StopAsync();
	const std::vector<std::string> GetInputDevices() const;
	const std::vector<std::string> GetOutputDevices() const;
	void SetInputDevice(const std::string& inputDevice);
	void SetOutputDevice(const std::string& outputDevice);

private:
	int AudioCallback(const void* inputBuffer, void* outputBuffer,
		unsigned long framesPerBuffer,
		const PaStreamCallbackTimeInfo* timeInfo,
		PaStreamCallbackFlags statusFlags);
	const std::vector<std::string> GetDevices(DeviceType type) const;
	PaDeviceIndex GetDeviceIndexByName(const std::string& deviceName);

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

