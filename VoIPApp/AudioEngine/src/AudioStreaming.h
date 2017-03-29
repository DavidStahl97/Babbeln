#ifndef AUDIOSTREAMING_H
#define AUDIOSTREAMING_H

#include "Common.h"
#include "AudioHandler.h"
#include "NetworkHandler.h"
#include <vector>

class AudioStreamer
{
public:
	AudioStreamer();
	~AudioStreamer();
	void StartAsync(const std::string& targetIP, int port);
	void StopAsync();
	void Init();

private:
	void ClearQueues();

public:
	AudioHandler		m_Audio;
	NetworkHandler		m_UDPClient;

private:
	LockfreeQueue		m_PlayingQeue;
	LockfreeQueue		m_RecordingQueue;
	SampleBufferPool	m_Pool;

	bool m_StartedSuccessfully;
};



#endif 