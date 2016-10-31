#include <AudioStreaming.h>
#include <iostream>

void printStringVector(const std::vector<std::string>& strings)
{
	for (std::vector<std::string>::const_iterator i = strings.begin(); i != strings.end(); ++i)
	{
		std::cout << *i << std::endl;
	}

	std::cout << std::endl;
}

int main()
{
	AudioStreamer audioStreamer;

	audioStreamer.Init();

	std::vector<std::string> inputDevices = audioStreamer.m_Audio.GetInputDevices();
	std::vector<std::string> outputDevices = audioStreamer.m_Audio.GetOutputDevices();

	std::cout << "InputDevices:" << std::endl;
	printStringVector(inputDevices);

	std::cout << "OutputDevices:" << std::endl;
	printStringVector(outputDevices);

	return 0;
}