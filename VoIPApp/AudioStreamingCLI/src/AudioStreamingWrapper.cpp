#include <AudioStreaming.h>
#include <msclr\marshal_cppstd.h>
#include "AudioStreamingWrapper.h"
#include <string>

namespace CPPWrapper
{
	AudioStreamingService::AudioStreamingService()
		: m_AudioStreamer(new AudioStreamer())
	{				
	}

	void AudioStreamingService::Init()
	{
		m_AudioStreamer->Init();
	}

	void AudioStreamingService::StartAsync(System::String^ hostname, int port)
	{
		const std::string& nativeString = msclr::interop::marshal_as<std::string>(hostname);
		m_AudioStreamer->StartAsync(nativeString, port);
	}

	void AudioStreamingService::StopAsync()
	{
		m_AudioStreamer->StopAsync();
	}

	List<String^>^ AudioStreamingService::GetInputDevice()
	{
		const std::vector<std::string>& devices = m_AudioStreamer->m_Audio.GetInputDevices();
		return ConvertStringVectorToList(devices);
	}

	List<String^>^ AudioStreamingService::GetOutputDevice()
	{
		const std::vector<std::string>& devices = m_AudioStreamer->m_Audio.GetOutputDevices();
		return ConvertStringVectorToList(devices);
	}

	void AudioStreamingService::SetInputDevice(String^ inputDevice)
	{
		const std::string& nativeString = msclr::interop::marshal_as<std::string>(inputDevice);
		m_AudioStreamer->m_Audio.SetInputDevice(nativeString);
	}

	void AudioStreamingService::SetOutputDevice(String^ outputDevice)
	{
		const std::string& nativeString = msclr::interop::marshal_as<std::string>(outputDevice);
		m_AudioStreamer->m_Audio.SetOutputDevice(nativeString);
	}

	List<String^>^ AudioStreamingService::ConvertStringVectorToList(const std::vector<std::string>& nativeStrings)
	{
		List<String^>^ managedStrings = gcnew List<System::String^>();
		if (managedStrings != nullptr)
		{
			for (std::vector<std::string>::const_iterator i = nativeStrings.begin(); i != nativeStrings.end(); ++i)
			{
				const std::string& nativeString = *i;
				managedStrings->Add(msclr::interop::marshal_as<String^>(nativeString));
			}
		}

		return managedStrings;
	}

	void AudioStreamingService::SetVolumeGain(double gain)
	{
		m_AudioStreamer->m_Audio.SetVolumeGain(gain);
	}
}
