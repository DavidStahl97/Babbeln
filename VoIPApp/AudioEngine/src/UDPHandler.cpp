#include "UDPHandler.h"
#include "G711Codec.h"

UDPHandler::UDPHandler(LockfreeQueue& playingQueue, LockfreeQueue& recordingQueue, SampleBufferPool& pool)
	: m_PlayingQueue(playingQueue), m_RecordingQueue(recordingQueue), m_Pool(pool),
	m_Socket(m_IOService), stop(false)
{
	
}

void UDPHandler::StartAsync(const std::string& targetIP, int port)
{
	stop = false;
	m_IOService.reset();

	m_Socket.open(udp::v4());
	m_Socket.bind(udp::endpoint(udp::v4(), port));
	m_Iterator = udp::resolver(m_IOService).resolve(udp::resolver::query(udp::v4(), targetIP.c_str(), std::to_string(port).c_str()));
	
	
	m_Socket.async_send_to(boost::asio::buffer(m_SendBuffer, sizeof(CompressedSampleBuffer)), *m_Iterator,
		boost::bind(&UDPHandler::HandleSent, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred));

	m_Socket.async_receive_from(boost::asio::buffer(m_RecvBuffer, sizeof(CompressedSampleBuffer)), m_Endpoint,
		boost::bind(&UDPHandler::HandleReceived, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred));

	//TODO: run ioservice in two threads so receive handle does not get blocked when send handle waits for a new buffer
	m_WorkerThread.reset(new boost::thread(boost::bind(&boost::asio::io_service::run, &this->m_IOService)));
}

void UDPHandler::StopAsync()
{
	stop = true;
	m_Socket.close();
	m_IOService.stop();
	m_WorkerThread->join();
}

void UDPHandler::Send()
{
	SampleBuffer* pcmBuffer = nullptr;

	//TODO: lock consumer with mutex to avoid polling
	while (!stop)
	{
		if (m_RecordingQueue.pop(pcmBuffer))
		{
			break;
		}
		else
		{
			boost::this_thread::sleep(boost::posix_time::milliseconds(16));
		}
	}

	if (pcmBuffer != nullptr || !stop)
	{
		G711Codec::Encode(*pcmBuffer, m_SendBuffer);
		m_Pool.push(pcmBuffer);

		m_Socket.async_send_to(boost::asio::buffer(m_SendBuffer, sizeof(CompressedSampleBuffer)), *m_Iterator,
			boost::bind(&UDPHandler::HandleSent, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}
}

void UDPHandler::Receive()
{
	if (!stop)
	{
		m_Socket.async_receive_from(boost::asio::buffer(m_RecvBuffer, sizeof(CompressedSampleBuffer)), m_Endpoint,
			boost::bind(&UDPHandler::HandleReceived, this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}
}

void UDPHandler::HandleSent(const boost::system::error_code& error, size_t bytesSent)
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

void UDPHandler::HandleReceived(const boost::system::error_code& error, size_t bytesReceive)
{
	SampleBuffer* pcmBuffer;
	if (m_Pool.pop(pcmBuffer))
	{
		G711Codec::Decode(m_RecvBuffer, *pcmBuffer);

		if (m_PlayingQueue.write_available())
		{
			m_PlayingQueue.push(pcmBuffer);
		}
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
