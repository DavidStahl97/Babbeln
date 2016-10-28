#ifndef AUDIOSTREAMING_H
#define AUDIOSTREAMING_H

#include "Common.h"
#include "AudioHandler.h"
#include "UDPHandler.h"

class AudioStreamer
{
public:
	AudioStreamer();
	~AudioStreamer();
	void Start(const std::string& targetIP, int port);
	void Stop();
	void Init();

private:
	AudioHandler		m_Audio;
	UDPHandler			m_UDPClient;
	LockfreeQueue		m_PlayingQeue;
	LockfreeQueue		m_RecordingQueue;
	SampleBufferPool	m_Pool;
};



#endif 