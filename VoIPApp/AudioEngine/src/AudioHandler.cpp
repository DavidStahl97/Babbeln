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
	: m_PlayingQueue(playingQueue), m_RecordingQueue(recordingQueue), m_Pool(pool), m_InputGain(1.0), m_OutputGain(1.0)
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
		LOG("No default output device found");
		return;
	}
	m_OutputParameters.channelCount = NUMBER_OF_CHANNELS;                     /* stereo output */
	m_OutputParameters.sampleFormat = PA_SAMPLE_TYPE;
	m_OutputParameters.suggestedLatency = Pa_GetDeviceInfo(m_OutputParameters.device)->defaultLowOutputLatency;
	m_OutputParameters.hostApiSpecificStreamInfo = NULL;

	m_InputParamters.device = Pa_GetDefaultInputDevice(); /* default input device */
	if (m_InputParamters.device == paNoDevice) {
		LOG("No default input device found");
		return;
	}
	m_InputParamters.channelCount = NUMBER_OF_CHANNELS;                    /* stereo input */
	m_InputParamters.sampleFormat = PA_SAMPLE_TYPE;
	m_InputParamters.suggestedLatency = Pa_GetDeviceInfo(m_InputParamters.device)->defaultLowInputLatency;
	m_InputParamters.hostApiSpecificStreamInfo = NULL;
}

VoIPError AudioHandler::StartAsync()
{
	if (m_OutputParameters.device == paNoDevice)
	{
		return VoIP_NoOutputDevice;
	}

	if (m_InputParamters.device == paNoDevice)
	{
		return VoIP_NoInputDevice;
	}

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
		return -1;
	}

	err = Pa_StartStream(m_AudioStream);
	if (err != paNoError)
	{
		return -1;
	}

	return VoIP_NoError;
}

void AudioHandler::StopAsync()
{
	PaError err = Pa_AbortStream(m_AudioStream);
}

int AudioHandler::AudioCallback(const void* inputBuffer, void* outputBuffer,
	unsigned long framesPerBuffer,
	const PaStreamCallbackTimeInfo* timeInfo,
	PaStreamCallbackFlags statusFlags)
{

	//get the recorded samples and push it to the queue
	SampleBuffer* recordBuffer; 
	if (m_Pool.pop(recordBuffer))
	{
		const Sample* recordReadPointer = (const Sample*)inputBuffer;
		Sample* recordWritePointer = recordBuffer->begin();
		for (int i = 0; i < FRAMES_PER_BUFFER; ++i)
		{
			*recordWritePointer++ = *recordReadPointer++;
		}

		AmplifySamples(*recordBuffer, m_InputGain.load(std::memory_order_acquire));
		m_RecordingQueue.push(recordBuffer);
	}

	//play the samples from the playing queue
	SampleBuffer* playBuffer;
	Sample* playWritePointer = (Sample*)outputBuffer;
	if (m_PlayingQueue.pop(playBuffer))
	{		
		AmplifySamples(*playBuffer, m_OutputGain.load(std::memory_order_acquire));
		Sample* playReadPointer = playBuffer->begin();
		for (size_t i = 0; i < FRAMES_PER_BUFFER; ++i)
		{
			*playWritePointer++ = *playReadPointer++;
		}

		m_Pool.push(playBuffer);
	}
	else
	{
		for (size_t i = 0; i < FRAMES_PER_BUFFER; ++i)
		{
			*playWritePointer++ = SAMPLE_SILENCE;
		}
	}

	return paContinue;
}

void AudioHandler::AmplifySamples(SampleBuffer& sampleBuffer, double gain)
{
	if (gain > 1.01 || gain < 0.99)
	{
		for (SampleBuffer::iterator it = sampleBuffer.begin(); it != sampleBuffer.end(); it++)
		{
			double dSample = static_cast<double>(*it) * gain;

			if (dSample > std::numeric_limits<Sample>::max())
			{
				dSample = std::numeric_limits<Sample>::max();
			}
			else if (dSample < std::numeric_limits<Sample>::min())
			{
				dSample = std::numeric_limits<Sample>::min();
			}

			*it = static_cast<Sample>(dSample);
		}
	}
}

const std::vector<std::string> AudioHandler::GetInputDevices() const
{
	return GetDevices(InputDevice);
}

const std::vector<std::string> AudioHandler::GetOutputDevices() const
{
	return GetDevices(OutputDevice);
}

void AudioHandler::SetInputDevice(const std::string& inputDevice)
{
	PaDeviceIndex i = GetDeviceIndexByName(inputDevice);
	if (i != -1)
	{
		m_InputParamters.device = i;
	}
}

void AudioHandler::SetOutputDevice(const std::string& outputDevice)
{
	PaDeviceIndex i = GetDeviceIndexByName(outputDevice);
	if (i != -1)
	{
		m_OutputParameters.device = i;
	}
}

PaDeviceIndex AudioHandler::GetDeviceIndexByName(const std::string& deviceName)
{
	int numDevices = Pa_GetDeviceCount();
	const PaDeviceInfo* deviceInfo;

	for (int i = 0; i < numDevices; ++i)
	{
		deviceInfo = Pa_GetDeviceInfo(i);
		if (deviceName.compare(deviceInfo->name) == 0)
		{
			return i;
		}
	}

	return -1;
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
				if (IsDeviceValid(deviceInfo->name, deviceNames))
				{
					deviceNames.push_back(deviceInfo->name);
				}
			}
			break;

		case OutputDevice:
			if (deviceInfo->maxOutputChannels > 0)
			{
				if (IsDeviceValid(deviceInfo->name, deviceNames))
				{
					deviceNames.push_back(deviceInfo->name);
				}
			}
			break;

		default:
			break;
		}
	}

	return deviceNames;
}

//workaround for bug in portaudio, where devices will be listed multiple times with cutted name
bool AudioHandler::IsDeviceValid(const std::string& testName, const std::vector<std::string>& deviceNames) const
{
	for (const std::string& name : deviceNames)
	{
		if (name.find(testName) != std::string::npos)
		{
			return false;
		}
	}

	return true;
}

void AudioHandler::SetInputVolumeGain(double gain)
{
	m_InputGain.store(gain, std::memory_order_release);
}

void AudioHandler::SetOutputVolumeGain(double gain)
{
	m_OutputGain.store(gain, std::memory_order_release);
}


