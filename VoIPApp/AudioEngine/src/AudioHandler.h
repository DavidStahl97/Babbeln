#ifndef AUDIO_HANDLER_H
#define AUDIO_HANDLER_H

#include "Common.h"
#include <portaudio.h>
#include <string>
#include <vector>
#include <atomic>

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
	void SetInputVolumeGain(double gain);
	void SetOutputVolumeGain(double gain);

private:
	int AudioCallback(const void* inputBuffer, void* outputBuffer,
		unsigned long framesPerBuffer,
		const PaStreamCallbackTimeInfo* timeInfo,
		PaStreamCallbackFlags statusFlags);
	const std::vector<std::string> GetDevices(DeviceType type) const;
	PaDeviceIndex GetDeviceIndexByName(const std::string& deviceName);
	bool IsDeviceValid(const std::string& deviceName, const std::vector<std::string>& deviceNames) const;
	void AmplifySamples(SampleBuffer& sampleBuffer, double gain);

private:
	PaStreamParameters	m_InputParamters;
	PaStreamParameters	m_OutputParameters;
	PaStream*			m_AudioStream;

	LockfreeQueue&		m_RecordingQueue;
	LockfreeQueue&		m_PlayingQueue;
	SampleBufferPool&   m_Pool;

	std::atomic<double> m_InputGain;
	std::atomic<double> m_OutputGain;

	friend static int StaticAudioCallback(const void *inputBuffer, void *outputBuffer,
		unsigned long framesPerBuffer,
		const PaStreamCallbackTimeInfo* timeInfo,
		PaStreamCallbackFlags statusFlags,
		void *userData);
};

#endif

