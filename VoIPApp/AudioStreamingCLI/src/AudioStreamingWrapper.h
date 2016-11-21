#pragma once

using namespace System;
using namespace System::Collections::Generic;

#include <vector>
#include <string>
#include <memory>
#include "SmartPointer.h"

class AudioStreamer;

namespace CPPWrapper {

	public ref class AudioStreamingService
	{
	public:
		AudioStreamingService();

		void Init();
		void StartAsync(String^ hostname, int port);
		void StopAsync();
		List<String^>^ GetInputDevice();
		List<String^>^ GetOutputDevice();
		void SetInputDevice(String^ inputDevice);
		void SetOutputDevice(String^ outputDevice);
		void SetVolumeGain(double gain);

	private:
		List<String^>^ ConvertStringVectorToList(const std::vector<std::string>& stringVector);

	private:
		 SmartPointer<AudioStreamer> m_AudioStreamer;
	};
}