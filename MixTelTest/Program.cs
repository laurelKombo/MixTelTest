using System.Diagnostics;
using System.Text;
using MixTelTest.data;

namespace MixTelTest;


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

    private static void GetClosestPositionIds(CoordinatePoint[] inputCoordinateList)
    {

        using var fs = new FileStream("data/VehiclePositions.dat", FileMode.Open, FileAccess.Read);

        var positions = new List<CoordinatePoint>();


        // Read the records into the array
        using var reader = new BinaryReader(fs);
        while (fs.Position < fs.Length)
        {
            var vehiclePositionId = reader.ReadInt32();
            var vehicleRegistration = reader.ReadAsciiString();
            var latitude = reader.ReadSingle();
            var longitude = reader.ReadSingle();
            var timestamp = reader.ReadInt64();

            positions.Add(new ()
            {
                Longitude = longitude,
                Latitude = latitude,
                PositionId = vehiclePositionId,
            });
        }

        GetPoints(inputCoordinateList,positions);
    }

   
    /// <summary>
    /// The goal of this method is to reduce the length of the loop by reducing the search range
    /// </summary>
    /// <param name="inputCoordinateList"></param>
    /// <param name="positions"></param>
    static void GetPoints(CoordinatePoint[] inputCoordinateList, List<CoordinatePoint> positions)
    {
        var centroid = new CoordinatePoint()
        {
            Longitude = positions.Average(p => p.Longitude),
            Latitude = positions.Average(p => p.Latitude)
        };

        positions = positions.OrderBy(o =>
        {
            o.Distance = GetDistanceFromTo(o.Longitude, o.Latitude, centroid.Longitude, centroid.Latitude);
            return o.Distance;
        }).ToList();

        // This variable will temporarily save distance between points as we compare them 
        var distances = new double[inputCoordinateList.Length];
        // this variable will save the closest vehicle position id
        var positionId = new int[inputCoordinateList.Length];

        // Loop through input coordinates
        for (var i = 0; i < inputCoordinateList.Length; i++)
        {
            var distanceFromCentroid = GetDistanceFromTo(inputCoordinateList[i].Longitude, inputCoordinateList[i].Latitude, centroid.Longitude, centroid.Latitude);


            //Get the maximum point of the range of search
            var rangeIndex = positions.FindIndex(f => f.Distance > distanceFromCentroid && ArePointsAligned(centroid, inputCoordinateList[i], f));

            // Loop through smallest range
            for (var y =0; y <= rangeIndex; y++)
            {
                var d = GetDistanceFromTo(inputCoordinateList[i].Longitude, inputCoordinateList[i].Latitude, positions[y].Longitude, positions[y].Latitude);

                if (distances[i] != 0 && !(distances[i] > d)) continue;
                distances[i] = d;
                positionId[i] = positions[y].PositionId;

            }

        }

        for (var i = 0; i < inputCoordinateList.Length; i++)
        {
            Console.WriteLine("position {0} is closer to Vehicle position ID: {1} ", i + 1, positionId[i]);
        }
    }

    /// <summary>
    /// This Method checks if all the points are aligned
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <param name="point3"></param>
    /// <returns></returns>

    static bool ArePointsAligned(CoordinatePoint point1, CoordinatePoint point2, CoordinatePoint point3)
    {
        double slope1 = Math.Round((point2.Latitude - point1.Latitude) / (point2.Longitude - point1.Longitude),3);
        double slope2 = Math.Round((point3.Latitude - point2.Latitude) / (point3.Longitude - point2.Longitude),3);
       
        return slope1 == slope2;
    }


    // calculate distance between two points
    private static double GetDistanceFromTo(double longitude, double latitude, double otherLongitude, double otherLatitude)
    {
        var dx = longitude - otherLongitude;
        var dy = latitude - otherLatitude;
        return Math.Sqrt(dx * dx + dy * dy);
    }


    private static CoordinatePoint[] GetInput()
    {
        return new[]
        {
            new CoordinatePoint() //1
            {

                Latitude =34.544909,
                Longitude = -102.100843
            },
            new CoordinatePoint()//2
            {

                Latitude =32.345544,
                Longitude = -99.123124
            },
            new CoordinatePoint()//3
            {

                Latitude =33.234235,
                Longitude = -100.214124

            },
            new CoordinatePoint()//4
            {

                Latitude =35.195739,
                Longitude = -95.348899
            },
            new CoordinatePoint()//5
            {

                Latitude =31.895839,
                Longitude = -97.789573
            },
            new CoordinatePoint()//6
            {

                Latitude =32.895839,
                Longitude = -101.789573
            },
            new CoordinatePoint()//7
            {

                Latitude =34.115839,
                Longitude = -100.225732
            },
            new CoordinatePoint()//8
            {

                Latitude =32.335839,
                Longitude = -99.992232
            },
            new CoordinatePoint()//9
            {

                Latitude =33.535339,
                Longitude = -94.792232
            },
            new CoordinatePoint()//10
            {

                Latitude =32.234235,
                Longitude = -100.222222
            }
        };



    }





}

