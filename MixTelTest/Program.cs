using System.Diagnostics;
using System.Text;

namespace MixTelTest;

internal class InputPosition
{
    public double Latitude ;
    public double Longitude;
}

internal static class BinaryReaderExtension
{
    // Help converting null terminated string (ASCII) to regular sting
    public static string ReadAsciiString(this BinaryReader reader)
    {
        var result = new StringBuilder();
        while (true)
        {
            var b = reader.ReadByte();
            if (0 == b)
                break;
            result.Append((char)b);
        }
        return result.ToString();
    }

}

internal class Program
{
    private static void Main(string[] args)
    {
        var watch = new Stopwatch();

        watch.Start();

        var inputCoordinateList = GetInput();
        GetClosestPositionIds(inputCoordinateList);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        Console.WriteLine("GetClosestPositionIds Elapsed time: {0} ms", elapsedMs);
    }


    private static void GetClosestPositionIds(InputPosition[] inputCoordinateList)
    {
        // This variable will temporarily save distance between points as we compare them 
        var distances = new double[inputCoordinateList.Length];
        // this variable will save the closest vehicle position id
        var positionId = new int[inputCoordinateList.Length];
        // Open the file for reading
        using var fs = new FileStream("data/VehiclePositions.dat", FileMode.Open, FileAccess.Read);
        // Read the records into the array
        using var reader = new BinaryReader(fs);
        while (fs.Position < fs.Length)
        {

            var vehiclePositionId = reader.ReadInt32();
            var vehicleRegistration = reader.ReadAsciiString();
            var latitude = reader.ReadSingle();
            var longitude = reader.ReadSingle();
            var timestamp = reader.ReadInt64();


            //Here we are comparing the list of selected point again the current point that is being read from the VehiclePositions.dat file
            for (var i = 0; i < inputCoordinateList.Length; i++)
            {
                var d = GetDistanceFromTo(inputCoordinateList[i].Longitude, inputCoordinateList[i].Latitude, longitude, latitude);

                if (distances[i] != 0 && !(distances[i] > d)) continue;
                distances[i] = d;
                positionId[i] = vehiclePositionId;
            }

        }

        // Display The result
        for (var i = 0; i < inputCoordinateList.Length; i++)
        {
            Console.WriteLine("position {0} is closer to Vehicle position ID: {1}", i+1, positionId[i]);
        }
    }


    // calculate distance between two points
    private static double GetDistanceFromTo(double longitude, double latitude, double otherLongitude, double otherLatitude)
    {
        var dx = longitude - otherLongitude;
        var dy = latitude - otherLatitude;
        return Math.Sqrt(dx * dx + dy * dy);
    }


    private static InputPosition[] GetInput()
    {
        return new[]
        {
            new InputPosition() //1
            {

                Latitude =34.544909,
                Longitude = -102.100843
            },
            new InputPosition()//2
            {

                Latitude =32.345544,
                Longitude = -99.123124
            },
            new InputPosition()//3
            {

                Latitude =33.234235,
                Longitude = -100.214124

            },
            new InputPosition()//4
            {

                Latitude =35.195739,
                Longitude = -95.348899
            },
            new InputPosition()//5
            {

                Latitude =31.895839,
                Longitude = -97.789573
            },
            new InputPosition()//6
            {

                Latitude =32.895839,
                Longitude = -101.789573
            },
            new InputPosition()//7
            {

                Latitude =34.115839,
                Longitude = -100.225732
            },
            new InputPosition()//8
            {

                Latitude =32.335839,
                Longitude = -99.992232
            },
            new InputPosition()//9
            {

                Latitude =33.535339,
                Longitude = -94.792232
            },
            new InputPosition()//10
            {

                Latitude =32.234235,
                Longitude = -100.222222
            }
        };



    }





}

