#include "AudioStreaming.h"

AudioStreamer::AudioStreamer()
	: m_Audio(m_PlayingQeue, m_RecordingQueue, m_Pool),
	m_UDPClient(m_PlayingQeue, m_RecordingQueue, m_Pool)
{
#if defined (Debug) | defined (_DEBUG)
	AllocConsole();
	AttachConsole(GetCurrentProcessId());
	freopen("CONOUT$", "w", stdout);

	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
}

AudioStreamer::~AudioStreamer()
{
#if defined (Debug) | defined (_DEBUG)
	FreeConsole();
#endif
}

void AudioStreamer::Init()
{
	m_Audio.Init();
}

void AudioStreamer::Start(const std::string& targetIP, int port)
{
	std::cout << "started audio stream" << std::endl;
	m_Audio.Start();
	m_UDPClient.Start(targetIP, port);
}

void AudioStreamer::Stop()
{
	std::cout << "stopped audio stream" << std::endl;

	m_UDPClient.Stop();
	m_Audio.Stop();

	SampleBuffer* buffer;
	while (!m_PlayingQeue.empty())
	{
		m_PlayingQeue.pop(buffer);
		if (buffer != nullptr)
		{
			m_Pool.free(buffer);
		}
	}

	while (!m_RecordingQueue.empty())
	{
		m_RecordingQueue.pop(buffer);
		if (buffer != nullptr)
		{
			m_Pool.free(buffer);
		}
	}
}
