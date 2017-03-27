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

	for (int i = 0; i < 64; ++i)
	{
		m_Pool.push(new SampleBuffer());
	}
}

AudioStreamer::~AudioStreamer()
{
#if defined (Debug) | defined (_DEBUG)
	FreeConsole();
#endif

	SampleBuffer* sb;
	while (m_Pool.pop(sb))
	{
		delete sb;
	}
}

void AudioStreamer::Init()
{
	m_Audio.Init();
}

void AudioStreamer::StartAsync(const std::string& targetIP, int port)
{
	LOG("started audio stream")

	m_StartedSuccessfully = true;

	VoIPError error = m_Audio.StartAsync();

	if (error != VoIP_NoError)
	{
		LOG("failed starting audio")
		m_StartedSuccessfully = false;
		return;
	}

	m_UDPClient.StartAsync(targetIP, port);
}

void AudioStreamer::StopAsync()
{
	LOG("stopped audio stream")

	if (m_StartedSuccessfully)
	{
		m_Audio.StopAsync();
		m_UDPClient.StopAsync();
	}

	ClearQueues();
}

void AudioStreamer::ClearQueues()
{
	SampleBuffer* buffer;
	while (!m_PlayingQeue.empty())
	{
		m_PlayingQeue.pop(buffer);
		if (buffer != nullptr)
		{
			m_Pool.push(buffer);
		}
	}

	while (!m_RecordingQueue.empty())
	{
		m_RecordingQueue.pop(buffer);
		if (buffer != nullptr)
		{
			m_Pool.push(buffer);
		}
	}
}
