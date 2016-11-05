#include "UDPHandler.h"
#include <boost\thread.hpp>


UDPHandler::UDPHandler(LockfreeQueue& playingQueue, LockfreeQueue& recordingQueue, SampleBufferPool& pool)
	: m_PlayingQueue(playingQueue), m_RecordingQueue(recordingQueue), m_Pool(pool),
	m_Socket(m_IOService)
{
	
}

void UDPHandler::StartAsync(const std::string& targetIP, int port)
{
	stop = false;
	m_IOService.reset();

	m_Socket.open(udp::v4());
	m_Socket.bind(udp::endpoint(udp::v4(), port));
	m_Iterator = udp::resolver(m_IOService).resolve(udp::resolver::query(udp::v4(), targetIP.c_str(), std::to_string(port).c_str()));

	Send();
	Receive();
	
	boost::thread t(boost::bind(&boost::asio::io_service::run, &this->m_IOService));
}

void UDPHandler::StopAsync()
{
	stop = true;
	m_Socket.close();
	m_IOService.stop();
}

void UDPHandler::Send()
{
	while (!stop)
	{
		if (m_RecordingQueue.empty())
		{
			boost::this_thread::sleep(boost::posix_time::milliseconds(16));
			continue;
		}
		else
		{
			m_RecordingQueue.pop(m_SendBuffer);
			break;
		}
	}

	m_Socket.async_send_to(boost::asio::buffer(*m_SendBuffer, sizeof(SampleBuffer)), *m_Iterator,
		boost::bind(&UDPHandler::HandleSent, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred));
}

void UDPHandler::Receive()
{
	m_RecvBuffer = m_Pool.malloc();
	m_Socket.async_receive_from(boost::asio::buffer(*m_RecvBuffer, sizeof(SampleBuffer)), m_Endpoint,
		boost::bind(&UDPHandler::HandleReceived, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred));
}

void UDPHandler::HandleSent(const boost::system::error_code& error, size_t bytesSent)
{
	m_Pool.free(m_SendBuffer);
	
	if (!error || error == boost::asio::error::message_size)
	{
		Send();
	}
	else
	{
		LOG(error.message())
			Send();
	}
}

void UDPHandler::HandleReceived(const boost::system::error_code& error, size_t bytesReceive)
{
	m_PlayingQueue.push(m_RecvBuffer);

	if (!error || error == boost::asio::error::message_size)
	{
		Receive();
	}
	else
	{
		LOG(error.message())
			Receive();
	}
}
