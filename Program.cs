using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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
              motion.setListJoints(listJoints);*/

            Joint rootJoint = new Joint();
            rootJoint = jointsFunction(fileLines);
            motion.setRootJoint(rootJoint);

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
                    //motion.setFrameTime(0.1f);
                }
            }

            Console.WriteLine(motion);
            export(motion);




        }

        static Joint jointsFunction(String[] fileLines)
        {
            //Console.WriteLine("Hello World!");
            var rootJoint = new Joint();
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
                        List<float> dataPoint = new List<float>();

                        String[] words = line.TrimStart(' ').Split(' ');


                        dataPoint.Add(float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[3], CultureInfo.InvariantCulture.NumberFormat));

                        rootJoint.setDataPoint(dataPoint);
                        rootJoint.setJointName("Hips");
                        continue;
                    }
                    else
                    {
                        var joint = listOpenJoints[listOpenJoints.Count - 1];

                        List<float> dataPoint = new List<float>();
                        String[] words = line.TrimStart(' ').Split(' ');


                        dataPoint.Add(float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[3], CultureInfo.InvariantCulture.NumberFormat));

                        joint.setDataPoint(dataPoint);
                    }

                }

                if (line.Contains("End Site"))
                {
                    var joint = new Joint();

                    joint.setJointName("End Site");
                    listOpenJoints.Add(joint);

                }

                if (line.Contains("}"))
                {
                    closedJoints = closedJoints + 1;
                    //  Console.WriteLine("line[0] == @" + line[0]+"@");
                    //  Console.WriteLine("line[2] == @" + line[2]+"@");
                    //   if (line[2] != '}' & line[0] != '}')
                    //   if (listOpenJoints.Count > 1)
                    if (line.Length > 3)
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

                /*        if (line.Contains("}"))
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

                        */


            }

            Console.WriteLine(listOpenJoints.Count);





            rootJoint.setListInnerJoints(listOpenJoints);
            return rootJoint;
        }

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

                    var rootJoint = jointsWithChannel[0];
                    //rootJoint.setJointName("root");
                    List<Joint> innerJointsOfRoot = new List<Joint>();
                    for (int i = 1; i < jointsWithChannel.Count; i++)
                    {
                        innerJointsOfRoot.Add(jointsWithChannel[i]);
                    }



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



                if (line.Contains("OFFSET"))
                {
                    // ignore first offset
                    countOffset = countOffset + 1;
                    if (countOffset == 1)
                    {
                        oppenedJoints = oppenedJoints + 1;
                        var joint = new Joint();
                        joint.setJointName("Hips");

                        List<float> dataPoint = new List<float>();
                        List<float> channelsPoints = new List<float>();
                        String[] words = line.TrimStart(' ').Split(' ');

                        dataPoint.Add(float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[3], CultureInfo.InvariantCulture.NumberFormat));
                        joint.setDataPoint(dataPoint);


                        listOpenJoints.Add(joint);

                    }
                    else
                    {
                        var joint = listOpenJoints[listOpenJoints.Count - 1];

                        List<float> dataPoint = new List<float>();
                        List<float> channelsPoints = new List<float>();
                        String[] words = line.TrimStart(' ').Split(' ');

                        dataPoint.Add(float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat));
                        dataPoint.Add(float.Parse(words[3], CultureInfo.InvariantCulture.NumberFormat));
                        joint.setDataPoint(dataPoint);



                    }

                }


                if (line.Contains("CHANNELS"))
                {
                    List<float> channelsPoints = new List<float>();
                    var joint = listOpenJoints[listOpenJoints.Count - 1];
                    channelsPoints.Add(float.Parse(lineParts[positionInFrame], CultureInfo.InvariantCulture.NumberFormat));
                    channelsPoints.Add(float.Parse(lineParts[positionInFrame + 1], CultureInfo.InvariantCulture.NumberFormat));
                    channelsPoints.Add(float.Parse(lineParts[positionInFrame + 2], CultureInfo.InvariantCulture.NumberFormat));
                    channelsPoints.Add(float.Parse(lineParts[positionInFrame + 3], CultureInfo.InvariantCulture.NumberFormat));
                    channelsPoints.Add(float.Parse(lineParts[positionInFrame + 4], CultureInfo.InvariantCulture.NumberFormat));
                    channelsPoints.Add(float.Parse(lineParts[positionInFrame + 5], CultureInfo.InvariantCulture.NumberFormat));
                    Console.WriteLine("***************************");
                    Console.WriteLine(lineParts[positionInFrame]);
                    Console.WriteLine(lineParts[positionInFrame] + 1);
                    Console.WriteLine(lineParts[positionInFrame + 2]);
                    Console.WriteLine(lineParts[positionInFrame + 3]);
                    Console.WriteLine(lineParts[positionInFrame + 4]);
                    Console.WriteLine(lineParts[positionInFrame + 5]);
                    Console.WriteLine("***************************");
                    joint.setChannelsPoints(channelsPoints);
                    positionInFrame = positionInFrame + 6;
                }

                if (line.Contains("JOINT"))
                {
                    oppenedJoints = oppenedJoints + 1;
                    var joint = new Joint();
                    joint.setJointName(line.TrimStart(' ').Split(' ')[1]);
                    listOpenJoints.Add(joint);
                }



                if (line.Contains("End Site"))
                {
                    var joint = new Joint();

                    joint.setJointName("End Site");
                    listOpenJoints.Add(joint);

                }

                if (line.Contains("}"))
                {
                    closedJoints = closedJoints + 1;
                    if (line.Length > 3)
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

        static void export(Motion objMotion)
        {
            Console.WriteLine("----------------------------- exporting ----------------------- ");

            string path = @"C:\Users\nazli\Documents\GitHub\PAMOCA\exports\file.bvh";
            if (File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("HIERARCHY");
                    /*      sw.WriteLine("Root " + objMotion.getRootJoint().getJointName());
                          sw.WriteLine("{");
                          sw.WriteLine("OFFSET " + objMotion.getRootJoint().getDataPoint()[0] + " " + objMotion.getRootJoint().getDataPoint()[1] + " " +
                                       objMotion.getRootJoint().getDataPoint()[2]); */
                    List<Joint> jointsList = new List<Joint>();
                    List<int> nbChilds = new List<int>();
                    recur(objMotion.getRootJoint(), jointsList, nbChilds);

                    List<int> levels = defineLevels(nbChilds);


                    for (int i = 0; i < levels.Count; i++)
                    {
                        int level = levels[i];

                        //  sw.WriteLine(" i = " + i);
                        String spaces = makeSpaces(level);
                        Joint currentJoin = jointsList[i];
                        if (i == 0)
                        {
                            sw.WriteLine("ROOT " + currentJoin.getJointName());
                            sw.WriteLine("{");
                            sw.WriteLine(spaces + "  OFFSET " + currentJoin.getDataPoint()[0].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower() + " " + currentJoin.getDataPoint()[1].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower() + " " +
                                         currentJoin.getDataPoint()[2].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower());
                            sw.WriteLine(spaces + "  CHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation");


                        }
                        else
                        {
                            if (currentJoin.getJointName() != "End Site")
                            {

                                sw.WriteLine(spaces + "JOINT " + currentJoin.getJointName());
                                sw.WriteLine(spaces + "{");
                                sw.WriteLine(spaces + "  OFFSET " + currentJoin.getDataPoint()[0].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower() + " " + currentJoin.getDataPoint()[1].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower() + " " +
                                             currentJoin.getDataPoint()[2].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower());
                                sw.WriteLine(spaces + "  CHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation");
                            }
                            else
                            {
                                // sw.WriteLine(spaces + "CHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation");
                                sw.WriteLine(spaces + currentJoin.getJointName());
                                sw.WriteLine(spaces + "{");
                                sw.WriteLine(spaces + "  OFFSET " + currentJoin.getDataPoint()[0].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower() + " " + currentJoin.getDataPoint()[1].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower() + " " +
                                             currentJoin.getDataPoint()[2].ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower());
                                sw.WriteLine(spaces + "}");
                                if (i != levels.Count - 1)
                                {
                                    int nbClosers = level - levels[i + 1];
                                    for (int k = 0; k < nbClosers; k++)
                                    {
                                        spaces = makeSpaces(level - k - 1);
                                        sw.WriteLine(spaces + "}");
                                    }

                                    //  sw.WriteLine("closings here = " + nbClosers);
                                }
                                else
                                {
                                    for (int k = 0; k < level; k++)
                                    {
                                        spaces = makeSpaces(level - k - 1);
                                        sw.WriteLine(spaces + "}");
                                    }
                                }


                            }

                        }

                    }

                    sw.WriteLine("MOTION");
                    sw.WriteLine("Frames:	" + objMotion.getNbFrames());
                    sw.WriteLine("Frame Time: " + objMotion.getFrameTime().ToString(System.Globalization.CultureInfo.InvariantCulture));

                    for (int i = 0; i < objMotion.getListFrames().Count; i++)
                    {
                        String frame = "";
                        List<Joint> jointsListByFrame = new List<Joint>();
                        recur(objMotion.getListFrames()[i].getRootJoint(), jointsListByFrame, nbChilds);
                        for (int j = 0; j < jointsListByFrame.Count - 1; j++)
                        {
                            if (jointsListByFrame[j].getJointName() != "End Site")
                            {
                                for (int k = 0; k < 6; k++)
                                {
                                    Console.WriteLine(" i =  " + i + " j = " + j + " k = " + k);
                                    Console.WriteLine(" i =  " + i + " j = " + j + " k = " + k);
                                    Console.WriteLine(jointsListByFrame[j].getChannelsPoints()[k].ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    frame = frame + jointsListByFrame[j].getChannelsPoints()[k].ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";

                                }
                            }



                        }
                        sw.WriteLine(frame);
                        //objMotion.getListFrames()[0].getRootJoint().getListInnerJoints()[0].getChannelsPoints

                    }


                }
            }

            // Open the file to read from.
            /*   using (StreamReader sr = File.OpenText(path))
               {
                   string s = "";
                   while ((s = sr.ReadLine()) != null)
                   {
                       Console.WriteLine(s);
                   }
               } */
        }

        static void recur(Joint joint, List<Joint> jointsList, List<int> nbChilds)
        {
            // Console.WriteLine(joint.getJointName());
            jointsList.Add(joint);
            nbChilds.Add(joint.getListInnerJoints().Count);

            for (int i = 0; i < joint.getListInnerJoints().Count; i++)
            {
                // Console.WriteLine(joint.getListInnerJoints()[i].getJointName());
                recur(joint.getListInnerJoints()[i], jointsList, nbChilds);
            }
        }

        static String makeSpaces(int nb)
        {
            String spaces = "";
            for (int i = 0; i < nb; i++)
            {
                spaces = spaces + "  ";
            }

            return spaces;
        }

        static List<int> defineLevels(List<int> nbChilds)
        {
            List<int> levels = new List<int>();
            List<int> nbCounted = new List<int>();
            levels.Add(0);
            nbCounted.Add(0);
            for (int i = 1; i < nbChilds.Count; i++)
            {
                int j = i - 1;
                while (nbCounted[j] >= nbChilds[j])
                {
                    j--;

                }

                nbCounted[j] = nbCounted[j] + 1;
                levels.Add(levels[j] + 1);
                nbCounted.Add(0);


            }
            return levels;
        }
    }
}