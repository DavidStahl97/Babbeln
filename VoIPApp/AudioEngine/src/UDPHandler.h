#ifndef UDP_HANDLER_H
#define UDP_HANDLER_H

#include <boost\asio.hpp>
#include <string>
#include <atomic>
#include "Common.h"

class UDPHandler
{
public:
	typedef boost::asio::ip::udp udp;

	UDPHandler(LockfreeQueue& playingQueue, LockfreeQueue& recordingQueue, SampleBufferPool& pool);

	void StartAsync(const std::string& targetIP, int port);
	void StopAsync();

private:
	void Send();
	void Receive();
	void HandleSent(const boost::system::error_code& error, size_t bytesSent);
	void HandleReceived(const boost::system::error_code& error, size_t bytesReceived);

private:
	LockfreeQueue&				m_RecordingQueue;
	LockfreeQueue&				m_PlayingQueue;
	SampleBufferPool&			m_Pool;
	CompressedSampleBufferPool  m_CompressedPool;
	CompressedSampleBuffer*		m_SendBuffer;
	CompressedSampleBuffer*		m_RecvBuffer;

	boost::asio::io_service m_IOService;
	udp::socket				m_Socket;
	udp::endpoint			m_Endpoint;
	udp::resolver::iterator m_Iterator;

	std::atomic<bool> stop;
};


#endif