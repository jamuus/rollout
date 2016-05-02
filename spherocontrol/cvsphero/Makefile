CC=g++
CFLAGS=`pkg-config --cflags --libs opencv`

OTHERS=Sphero.cpp Sphero.h
TARGET=SpheroDetector


all: $(TARGET)

$(TARGET): Sphero.o
	$(CC) $(CFLAGS) $(TARGET).cpp Sphero.o -o $(TARGET)

Sphero.o: Sphero.cpp
	$(CC) -c Sphero.cpp

clean:
	rm *.o $(TARGET)