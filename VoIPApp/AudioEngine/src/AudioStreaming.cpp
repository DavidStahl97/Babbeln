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

void AudioStreamer::StartAsync(const std::string& targetIP, int port)
{
	LOG("started audio stream")

	m_Audio.StartAsync();
	m_UDPClient.StartAsync(targetIP, port);
}

void AudioStreamer::StopAsync()
{
	LOG("stopped audio stream")

	m_Audio.StopAsync();
	m_UDPClient.StopAsync();
}
