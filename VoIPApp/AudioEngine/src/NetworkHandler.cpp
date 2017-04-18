#include "NetworkHandler.h"
#include "G711Codec.h"

NetworkHandler::NetworkHandler(LockfreeQueue& playingQueue, LockfreeQueue& recordingQueue, SampleBufferPool& pool)
	: m_PlayingQueue(playingQueue), m_RecordingQueue(recordingQueue), m_Pool(pool),
	m_Socket(m_IOService), stop(false)
{
	
}

void NetworkHandler::StartAsync(const std::string& targetIP, int port)
{
	stop = false;
	m_IOService.reset();

	m_Socket.open(udp::v4());
	m_Socket.bind(udp::endpoint(udp::v4(), port));
	m_Iterator = udp::resolver(m_IOService).resolve(udp::resolver::query(udp::v4(), targetIP.c_str(), std::to_string(port).c_str()));
	
	
	m_Socket.async_send_to(boost::asio::buffer(m_SendBuffer, sizeof(CompressedSampleBuffer)), *m_Iterator,
		boost::bind(&NetworkHandler::HandleSent, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred));

	Receive();

	for (size_t i = 0; i < m_HandlerThreads.size(); ++i)
	{
		m_HandlerThreads[i].reset(new boost::thread(boost::bind(&boost::asio::io_service::run, &this->m_IOService)));
	}
}

void NetworkHandler::StopAsync()
{
	stop.store(true, std::memory_order_release);
	m_Socket.close();
	m_IOService.stop();
	
	for (size_t i = 0; i < m_HandlerThreads.size(); ++i)
	{
		m_HandlerThreads[i]->join();
	}
}

void NetworkHandler::Send()
{
	SampleBuffer* pcmBuffer = nullptr;

	//polling is actually better for performance than mutex locks
	while (!stop.load(std::memory_order_acquire))
	{
		if (m_RecordingQueue.pop(pcmBuffer))
		{
			break;
		}
		else
		{
			boost::this_thread::sleep(boost::posix_time::milliseconds(1));
		}
	}

	if (!stop.load(std::memory_order_acquire))
	{
		G711Codec::Encode(*pcmBuffer, m_SendBuffer);
		m_Pool.push(pcmBuffer);

		m_Socket.async_send_to(boost::asio::buffer(m_SendBuffer, sizeof(CompressedSampleBuffer)), *m_Iterator,
			boost::bind(&NetworkHandler::HandleSent, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}
}

void NetworkHandler::Receive()
{
	if (!stop.load(std::memory_order_acquire))
	{
		m_Socket.async_receive_from(boost::asio::buffer(m_RecvBuffer, sizeof(CompressedSampleBuffer)), m_Endpoint,
			boost::bind(&NetworkHandler::HandleReceived, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}
}

void NetworkHandler::HandleSent(const boost::system::error_code& error, size_t bytesSent)
{
	if (!error || error == boost::asio::error::message_size)
	{
		Send();
	}
	else
	{
		//TO-DO!!!!!!!!!!!!!!!
		LOG(error.message())
			Send();
	}
}

void NetworkHandler::HandleReceived(const boost::system::error_code& error, size_t bytesReceive)
{
	SampleBuffer* pcmBuffer;
	if (m_Pool.pop(pcmBuffer))
	{
		G711Codec::Decode(m_RecvBuffer, *pcmBuffer);
		m_PlayingQueue.push(pcmBuffer);
		
	}

	if (!error || error == boost::asio::error::message_size)
	{
		Receive();
	}
	else
	{
		//TO-DO!!!!!!!!!!!!!!!
		LOG(error.message())
			Receive();
	}
}
