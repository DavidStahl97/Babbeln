#include <AudioStreaming.h>
#include <msclr\marshal_cppstd.h>
#include "AudioStreamingWrapper.h"
#include <string>

namespace CPPWrapper
{
	AudioStreamingService::AudioStreamingService()
	{		
		m_AudioStreamer = new AudioStreamer();
	}

	void AudioStreamingService::Init()
	{
		m_AudioStreamer->Init();
	}

	void AudioStreamingService::Start(System::String^ hostname, int port)
	{
		std::string nativeString = msclr::interop::marshal_as<std::string>(hostname);
		m_AudioStreamer->Start(nativeString, port);
	}

	void AudioStreamingService::Stop()
	{
		m_AudioStreamer->Stop();
	}
}
