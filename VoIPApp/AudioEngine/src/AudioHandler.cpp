#include "AudioHandler.h"

static int StaticAudioCallback(const void *inputBuffer, void *outputBuffer,
	unsigned long framesPerBuffer,
	const PaStreamCallbackTimeInfo* timeInfo,
	PaStreamCallbackFlags statusFlags,
	void *userData)
{
	return ((AudioHandler*)userData)
		->AudioCallback(inputBuffer, outputBuffer, framesPerBuffer, timeInfo, statusFlags);
}

AudioHandler::AudioHandler(LockfreeQueue& playingQueue, LockfreeQueue& recordingQueue, SampleBufferPool& pool)
	: m_PlayingQueue(playingQueue), m_RecordingQueue(recordingQueue), m_Pool(pool)
{

}

AudioHandler::~AudioHandler()
{
	Pa_Terminate();
}

void AudioHandler::Init()
{
	PaError err = Pa_Initialize();

	m_OutputParameters.device = Pa_GetDefaultOutputDevice(); /* default output device */
	if (m_OutputParameters.device == paNoDevice) {
		std::cout << "No default output device found" << std::endl;
		return;
	}
	m_OutputParameters.channelCount = NUMBER_OF_CHANNELS;                     /* stereo output */
	m_OutputParameters.sampleFormat = PA_SAMPLE_TYPE;
	m_OutputParameters.suggestedLatency = Pa_GetDeviceInfo(m_OutputParameters.device)->defaultLowOutputLatency;
	m_OutputParameters.hostApiSpecificStreamInfo = NULL;

	m_InputParamters.device = Pa_GetDefaultInputDevice(); /* default input device */
	if (m_InputParamters.device == paNoDevice) {
		std::cout << "No default input device found" << std::endl;
		return;
	}
	m_InputParamters.channelCount = NUMBER_OF_CHANNELS;                    /* stereo input */
	m_InputParamters.sampleFormat = PA_SAMPLE_TYPE;
	m_InputParamters.suggestedLatency = Pa_GetDeviceInfo(m_InputParamters.device)->defaultLowInputLatency;
	m_InputParamters.hostApiSpecificStreamInfo = NULL;
}

void AudioHandler::Start()
{
	/* Record some audio. -------------------------------------------- */
	PaError err = Pa_OpenStream(
		&m_AudioStream,
		&m_InputParamters,
		&m_OutputParameters,               
		SAMPLE_RATE,
		FRAMES_PER_BUFFER,
		paClipOff,      /* we won't output out of range samples so don't bother clipping them */
		StaticAudioCallback,
		this);
	if (err != paNoError)
	{
		//error handling
		return;
	}

	err = Pa_StartStream(m_AudioStream);
	if (err != paNoError)
	{
		return;
	}
}

void AudioHandler::Stop()
{
	PaError err = Pa_AbortStream(m_AudioStream);
	err = Pa_CloseStream(m_AudioStream);
}

int AudioHandler::AudioCallback(const void* inputBuffer, void* outputBuffer,
	unsigned long framesPerBuffer,
	const PaStreamCallbackTimeInfo* timeInfo,
	PaStreamCallbackFlags statusFlags)
{
	SampleBuffer* playBuffer = nullptr;
	Sample* playWritePointer = (Sample*)outputBuffer;
	Sample* playReadPointer = nullptr;

	SampleBuffer* recordBuffer = m_Pool.malloc();
	Sample* recordWritePointer = recordBuffer->begin();
	const Sample* recordReadPointer = (const Sample*)inputBuffer;

	int i;

	if (!m_PlayingQueue.empty())
	{
		m_PlayingQueue.pop(playBuffer);
		playReadPointer = playBuffer->begin();
	}

	for (i = 0; i < FRAMES_PER_BUFFER; ++i)
	{
		if (playReadPointer != nullptr)
		{
			*playWritePointer++ = *playReadPointer++;
		}
		else
		{
			*playWritePointer++ = SAMPLE_SILENCE;
		}

		*recordWritePointer++ = *recordReadPointer++;
	}

	if(playBuffer != nullptr)
	{
		m_Pool.free(playBuffer);
	}

	m_RecordingQueue.push(recordBuffer);

	return paContinue;
}

const std::vector<std::string> AudioHandler::GetInputDevices() const
{
	return GetDevices(InputDevice);
}

const std::vector<std::string> AudioHandler::GetOutputDevices() const
{
	return GetDevices(OutputDevice);
}

const std::vector<std::string> AudioHandler::GetDevices(DeviceType type) const
{
	int numDevices = Pa_GetDeviceCount();
	const PaDeviceInfo* deviceInfo;
	std::vector<std::string> deviceNames;

	if (numDevices < 0)
	{
		return std::vector<std::string>();
	}

	for (int i = 0; i < numDevices; i++)
	{
		deviceInfo = Pa_GetDeviceInfo(i);
		switch (type)
		{
		case InputDevice:
			if (deviceInfo->maxInputChannels > 0)
			{
				deviceNames.push_back(deviceInfo->name);
			}
			break;

		case OutputDevice:
			if (deviceInfo->maxOutputChannels > 0)
			{
				deviceNames.push_back(deviceInfo->name);
			}
			break;

		default:
			break;
		}
	}

	return deviceNames;
}


