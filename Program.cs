using System;
using System.Collections.Generic;
using System.Globalization;


namespace PAMOCA
{
    class MainClass
    {
        static void Main(string[] args)
        {

            var motion = new Motion();
            String[] fileLines;
            fileLines = motion.importFile();

            /*   List<Joint> listJoints = new List<Joint>();
               listJoints = jointsFunction(fileLines);
               motion.setListJoints(listJoints); */


            List<Frame> listFrames = new List<Frame>();
            listFrames = framesFunction(fileLines);
            motion.setListFrames(listFrames);


            foreach (String line in fileLines)
            {
                if (line.Contains("Frames:"))
                {
                    String[] framesLine = line.Split('\t');

                    motion.setNbFrames(int.Parse(framesLine[1], CultureInfo.InvariantCulture.NumberFormat));
                }
                if (line.Contains("Frame Time:"))
                {
                    String[] framesLine = line.Split('\t');
                    motion.setFrameTime(float.Parse(framesLine[1], CultureInfo.InvariantCulture.NumberFormat));
                }
            }

            Console.WriteLine(motion);


        }

        static List<Joint> jointsFunction(String[] fileLines)
        {
            //Console.WriteLine("Hello World!");


            List<Joint> listOpenJoints = new List<Joint>();
            int countOffset = 0;
            int countLine = 0;
            int oppenedJoints = 0;
            int closedJoints = 0;


            foreach (String line in fileLines)
            {

                countLine = countLine + 1;
                // Console.WriteLine(test);

                if (line.Contains("JOINT"))
                {
                    oppenedJoints = oppenedJoints + 1;
                    var joint = new Joint();
                    joint.setJointName(line.TrimStart(' ').Split(' ')[1]);
                    listOpenJoints.Add(joint);

                }

                if (line.Contains("OFFSET"))
                {
                    // ignore first offset
                    countOffset = countOffset + 1;
                    if (countOffset == 1)
                    {
                        continue;
                    }
                    else
                    {
                        var joint = listOpenJoints[listOpenJoints.Count - 1];
                        float[] dataPoint = new float[3];
                        String[] words = line.TrimStart(' ').Split(' ');

                        dataPoint[0] = float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat);
                        dataPoint[1] = float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat);
                        dataPoint[2] = float.Parse(words[3], CultureInfo.InvariantCulture.NumberFormat);


                        joint.setDataPoint(dataPoint);
                    }

                }

                if (line.Contains("End Site"))
                {
                    var joint = new Joint();

                    joint.setJointName("End site");
                    listOpenJoints.Add(joint);

                }

                if (line.Contains("}"))
                {
                    closedJoints = closedJoints + 1;
                    if (listOpenJoints.Count > 1)
                    {
                        Console.WriteLine(countLine);

                        var joint1 = listOpenJoints[listOpenJoints.Count - 1];
                        var joint2 = listOpenJoints[listOpenJoints.Count - 2];
                        listOpenJoints.RemoveAt(listOpenJoints.Count - 1);
                        joint2.getListInnerJoints().Add(joint1);

                        Console.WriteLine(joint1.getJointName() + " inside " + joint2.getJointName());
                        Console.WriteLine("--------------------------");
                    }
                }
            }

            Console.WriteLine(listOpenJoints.Count);
            return listOpenJoints;
        }

        /*static List<Frame> framesFunction(String[] fileLines)
        {

            List<Frame> listFrames = new List<Frame>();
            Boolean framesFound = false;
            foreach (String line in fileLines)
            {  
                
                if (line.Contains("Frame Time:"))
                {                
                  
                   framesFound = true;
                    continue;
                }
                Console.WriteLine(framesFound);
                if (framesFound == true)
                {
       

                    var frame = new Frame();
                    //Frame frame = new Frame();

                    String[] lineParts = line.Split(' ');
         

                    List<float[]> positionRotation = new List<float[]>();
                    List<float[]> joints = new List<float[]>();
       
                    for (int i = 0; i < lineParts.Length-2; i++)
                    {
                        float[] listOffsets = new float[3];
                       
                        listOffsets[0] = float.Parse(lineParts[i], CultureInfo.InvariantCulture.NumberFormat);
                        listOffsets[1] = float.Parse(lineParts[i + 1], CultureInfo.InvariantCulture.NumberFormat);
                        listOffsets[2] = float.Parse(lineParts[i + 2], CultureInfo.InvariantCulture.NumberFormat);
                        joints.Add(listOffsets);

                        float[] listPositionRotation = new float[3];
                        listPositionRotation[0] = float.Parse(lineParts[i + 3], CultureInfo.InvariantCulture.NumberFormat);
                        listPositionRotation[1] = float.Parse(lineParts[i + 4], CultureInfo.InvariantCulture.NumberFormat);
                        listPositionRotation[2] = float.Parse(lineParts[i + 5], CultureInfo.InvariantCulture.NumberFormat);
                        positionRotation.Add(listPositionRotation);

                        i = i + 6;
                    }

                    frame.setJoints(joints);
                    frame.setPositionRotation(positionRotation);
                    listFrames.Add(frame);
                }

            }

            return listFrames;

        }  */

        static List<Frame> framesFunction(String[] fileLines)
        {

            List<Frame> listFrames = new List<Frame>();
            Boolean framesFound = false;
            foreach (String line in fileLines)
            {
                if (line.Contains("Frame Time:"))
                {
                    Console.WriteLine(line);
                    framesFound = true;
                    continue;
                }
                if (framesFound == true)
                {
                    List<Joint> jointsWithChannel = new List<Joint>();
                    jointsWithChannel = jointsWithChannels(line, fileLines);

                    var rootJoint = new Joint();
                    rootJoint.setJointName("root");
                    List<Joint> innerJointsOfRoot = new List<Joint>();
                    innerJointsOfRoot.Add(jointsWithChannel[0]);
                    innerJointsOfRoot.Add(jointsWithChannel[1]);
                    innerJointsOfRoot.Add(jointsWithChannel[2]);

                    rootJoint.setListInnerJoints(innerJointsOfRoot);

                    var frame = new Frame();
                    frame.setRootJoint(rootJoint);

                    listFrames.Add(frame);
                }

            }

            return listFrames;

        }

        static List<Joint> jointsWithChannels(String frameLine, String[] fileLines)
        {
            String[] lineParts = frameLine.Split(' ');
            int positionInFrame = 0;

            List<Joint> listOpenJoints = new List<Joint>();
            int countOffset = 0;
            int countLine = 0;
            int oppenedJoints = 0;
            int closedJoints = 0;

            foreach (String line in fileLines)
            {


                countLine = countLine + 1;
                // Console.WriteLine(test);

                if (line.Contains("JOINT"))
                {
                    oppenedJoints = oppenedJoints + 1;
                    var joint = new Joint();
                    joint.setJointName(line.TrimStart(' ').Split(' ')[1]);
                    listOpenJoints.Add(joint);

                }

                if (line.Contains("OFFSET"))
                {
                    // ignore first offset
                    countOffset = countOffset + 1;
                    if (countOffset == 1)
                    {
                        continue;
                    }
                    else
                    {
                        var joint = listOpenJoints[listOpenJoints.Count - 1];



                      


                        float[] dataPoint = new float[3];
                        String[] words = line.TrimStart(' ').Split(' ');
                        dataPoint[0] = float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat);
                        dataPoint[1] = float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat);
                        dataPoint[2] = float.Parse(words[3], CultureInfo.InvariantCulture.NumberFormat);
                        joint.setDataPoint(dataPoint);


                        List<float> channelsPoints = new List<float>();
                        channelsPoints.Add(float.Parse(lineParts[positionInFrame], CultureInfo.InvariantCulture.NumberFormat));
                        channelsPoints.Add(float.Parse(lineParts[positionInFrame + 1], CultureInfo.InvariantCulture.NumberFormat));
                        channelsPoints.Add(float.Parse(lineParts[positionInFrame + 2], CultureInfo.InvariantCulture.NumberFormat));
                        channelsPoints.Add(float.Parse(lineParts[positionInFrame + 3], CultureInfo.InvariantCulture.NumberFormat));
                        channelsPoints.Add(float.Parse(lineParts[positionInFrame + 4], CultureInfo.InvariantCulture.NumberFormat));
                        channelsPoints.Add(float.Parse(lineParts[positionInFrame + 5], CultureInfo.InvariantCulture.NumberFormat));
                        joint.setChannelsPoints(channelsPoints);
                    }

                }

                if (line.Contains("End Site"))
                {
                    var joint = new Joint();

                    joint.setJointName("End site");
                    listOpenJoints.Add(joint);

                }

                if (line.Contains("}"))
                {
                    closedJoints = closedJoints + 1;
                    if (listOpenJoints.Count > 1)
                    {
                        Console.WriteLine(countLine);

                        var joint1 = listOpenJoints[listOpenJoints.Count - 1];
                        var joint2 = listOpenJoints[listOpenJoints.Count - 2];
                        listOpenJoints.RemoveAt(listOpenJoints.Count - 1);
                        joint2.getListInnerJoints().Add(joint1);

                        Console.WriteLine(joint1.getJointName() + " inside " + joint2.getJointName());
                        Console.WriteLine("--------------------------");
                    }
                }
            }
            Console.WriteLine(listOpenJoints.Count);
            return listOpenJoints;
        }

    }
}
