#include <AudioStreaming.h>
#include <boost\thread\thread.hpp>

int main()
{
	AudioStreamer streamer;
	streamer.Init();
	streamer.StartAsync("127.0.0.1", 10000);

	boost::posix_time::time_duration duration = boost::posix_time::seconds(10);
	boost::this_thread::sleep(duration);

	streamer.StopAsync();

	return 1;
}