#pragma once

using namespace System;

class AudioStreamer;

namespace CPPWrapper {

	public ref class AudioStreamingService
	{
	public:
		AudioStreamingService();

		void Init();
		void Start(System::String^ hostname, int port);
		void Stop();

	private:
		AudioStreamer* m_AudioStreamer;
	};
}