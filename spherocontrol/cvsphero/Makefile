CC=g++
CFLAGS=`pkg-config --cflags --libs opencv`

TARGET=SpheroDetector

all: $(TARGET)

$(TARGET): Sphero.o SpheroDetector.cpp
	$(CC) $(CFLAGS) $(TARGET).cpp Sphero.o -o $(TARGET)

Sphero.o: Sphero.cpp
	$(CC) -c Sphero.cpp

clean:
	rm *.o $(TARGET)