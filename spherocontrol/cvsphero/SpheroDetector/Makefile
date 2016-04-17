CC=g++
CFLAGS=`pkg-config --cflags --libs opencv`

OTHERS=Sphero.cpp Sphero.h
TARGET=SpheroDetector

$(TARGET): $(TARGET).cpp
	$(CC) $(CFLAGS) -o $(TARGET) $(TARGET).cpp $(OTHERS)