#include "AudioStreaming.h"
#include <boost\thread\thread.hpp>

int main(int argc, char* argv[])
{
	if (argc < 3)
	{
		std::cout << "Argumente: host port" << std::endl;
		return 0;
	}

	LOG(argv[1]);
	LOG(atoi(argv[2]));

	AudioStreamer streamer;
	streamer.Init();
	streamer.StartAsync(argv[1], atoi(argv[2]));

	system("pause");

	streamer.StopAsync();

	return 0;
}